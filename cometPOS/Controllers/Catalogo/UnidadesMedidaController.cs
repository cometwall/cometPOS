using cometPOS.Data;
using cometPOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cometPOS.DTOs.Catalogo;
using cometPOS.Models;
using Microsoft.EntityFrameworkCore;

namespace cometPOS.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UnidadesMedidaController : ControllerBase
    {
        private readonly AppDbContext _context;

        private IQueryable<UnidadMedida> UnidadesMiEmpresa() =>
            _context.UnidadesMedida.Where(u => u.EmpresaId == EmpresaId);

        public UnidadesMedidaController(AppDbContext context)
        {
            _context = context;
        }

        private int EmpresaId => User.EmpresaId() ?? 0;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnidadMedidaDto>>> ObtenerUnidades()
        {
            var unidades = await UnidadesMiEmpresa()
                .Where(u => u.EmpresaId == EmpresaId && u.Activa)
                .Select(u => new UnidadMedidaDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Abreviatura = u.Abreviatura,
                    ClaveSat = u.ClaveSat,
                    PermiteDecimales = u.PermiteDecimales,
                    Activa = u.Activa,
                    FechaCreacion = u.FechaCreacion
                }).ToListAsync();

            return Ok(unidades);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Crear")]
        public async Task<ActionResult<UnidadMedidaDto>> CrearUnidad(CrearUnidadMedidaDto dto)
        {
            if (await _context.UnidadesMedida.AnyAsync(u => u.Abreviatura == dto.Abreviatura && u.EmpresaId == EmpresaId))
                return Conflict("Ya existe una unidad de medida con esa abreviatura.");
            var unidad = new UnidadMedida
            {
                Nombre = dto.Nombre,
                Abreviatura = dto.Abreviatura,
                ClaveSat = dto.ClaveSat,
                PermiteDecimales = dto.PermiteDecimales,
                Activa = true,
                FechaCreacion = DateTime.UtcNow,
                EmpresaId = EmpresaId
            };
            _context.UnidadesMedida.Add(unidad);
            await _context.SaveChangesAsync();

            var resultado = new UnidadMedidaDto
            {
                Id = unidad.Id,
                Nombre = unidad.Nombre,
                Abreviatura = unidad.Abreviatura,
                ClaveSat = unidad.ClaveSat,
                PermiteDecimales = unidad.PermiteDecimales,
                Activa = unidad.Activa,
                FechaCreacion = unidad.FechaCreacion
            };
            return CreatedAtAction(nameof(ObtenerUnidades), null, resultado);
        }

    }
}
