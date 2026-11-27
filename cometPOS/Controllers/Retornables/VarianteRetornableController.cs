using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.Models;
using cometPOS.DTOs;
using Microsoft.AspNetCore.Authorization;
using cometPOS.Services;
using cometPOS.DTOs.Retornables;
using System.Runtime.InteropServices;

namespace cometPOS.Controllers.Retornables
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VarianteRetornableController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VarianteRetornableController(AppDbContext context)
        {
            _context = context;
        }

        private int EmpresaId => User.EmpresaId() ?? 0;

        private IQueryable<VarianteRetornable> varianteRetornablesDeMiEmpresa() =>
            _context.VarianteRetornables
                .Where(vr => vr.Retornable.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VarianteRetornableDto>>> ObtenerVarianteRetornables()
        {
            var varianteRetornables = await varianteRetornablesDeMiEmpresa()
                .Select(vr => new VarianteRetornableDto
                {
                    VarianteId = vr.VarianteId,
                    RetornableId = vr.RetornableId,
                    RetornableCodigo = vr.Retornable.Codigo,
                    RetornableNombre = vr.Retornable.Nombre,
                    CantidadPorUnidad = vr.CantidadPorUnidad
                }).ToListAsync();
            return Ok(varianteRetornables);
        }

        [HttpGet("{varianteId}/{retornableId}")]
        public async Task<ActionResult<VarianteRetornableDto>> ObtenerVarianteRetornable(int varianteId, int retornableId) 
        {
            var varianteRetornable = await varianteRetornablesDeMiEmpresa()
                .Where(vr => vr.VarianteId == varianteId && vr.RetornableId == retornableId)
                .Select(vr => new VarianteRetornableDto 
                { 
                    VarianteId = vr.VarianteId,
                    RetornableId = vr.RetornableId,
                    RetornableCodigo = vr.Retornable.Codigo,
                    RetornableNombre = vr.Retornable.Nombre,
                    CantidadPorUnidad = vr.CantidadPorUnidad
                }).FirstOrDefaultAsync();
            if (varianteRetornable == null)
                return NotFound("La asignacion no existe o no pertenece a tu empresa");
            return Ok(varianteRetornable);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Crear")]
        public async Task<ActionResult<VarianteRetornableDto>> CrearVarianteRetornable(AsignarVarianteRetornableDto dto)
        {
            if( await _context.VarianteRetornables.AnyAsync(vr => vr.VarianteId == dto.VarianteId && vr.RetornableId == dto.RetornableId))
                return Conflict("Esta variante ya tiene asignado ese retornable.");

            var variante = await _context.ProductoVariantes
                .FirstOrDefaultAsync(v => v.Id == dto.VarianteId && v.EmpresaId == EmpresaId);

            if (variante == null)
                return BadRequest("La variante no existe o no pertenece a tu empresa");

            var retornable = await _context.Retornables
                .FirstOrDefaultAsync(r => r.Id == dto.RetornableId && r.EmpresaId == EmpresaId);

            if (retornable == null)
                return BadRequest("El retornable no existe o no pertenece a tu empresa");

            var varianteRetornable = new VarianteRetornable 
            {
                VarianteId = dto.VarianteId,
                RetornableId = dto.RetornableId,
                CantidadPorUnidad = dto.CantidadPorUnidad
            };

            _context.Add(varianteRetornable);
            await _context.SaveChangesAsync();

            var resultado = new VarianteRetornableDto
            {
                VarianteId = varianteRetornable.VarianteId,
                RetornableId = varianteRetornable.RetornableId,
                RetornableCodigo = retornable.Codigo,
                RetornableNombre = retornable.Nombre,
                CantidadPorUnidad = varianteRetornable.CantidadPorUnidad
            };

            return CreatedAtAction(nameof(ObtenerVarianteRetornable), new { varianteId = resultado.VarianteId, retornableId = resultado.RetornableId }, resultado);
        }
    }
}
