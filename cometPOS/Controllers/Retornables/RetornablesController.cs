using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.Models;
using cometPOS.DTOs;
using Microsoft.AspNetCore.Authorization;
using cometPOS.Services;
using cometPOS.DTOs.Retornables;

namespace cometPOS.Controllers.Retornables
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RetornablesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RetornablesController(AppDbContext context) 
        {
            _context = context;
        }

        private int EmpresaId => User.EmpresaId() ?? 0;

        private IQueryable<Retornable> RetornablesDeMiEmpresa() =>
            _context.Retornables.Where(r => r.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RetornableDto>>> ObtenerRetornables() 
        {
            var retornable = await RetornablesDeMiEmpresa()
                .Where(r => r.Activo)
                .Select(r => new RetornableDto
                {
                    Id = r.Id,
                    EmpresaId = r.EmpresaId,
                    Codigo = r.Codigo,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    ValorDeposito = r.ValorDeposito,
                    Costo = r.Costo,
                    EsPropio = r.EsPropio,
                    Activo = r.Activo,
                    FechaCreacion = r.FechaCreacion
                }).ToListAsync();

            return Ok(retornable);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Crear")]
        public async Task<ActionResult<RetornableDto>> CrearRetornable(CrearRetornableDto dto)
        {
            if (await _context.Retornables.AnyAsync(r => r.Codigo == dto.Codigo && r.EmpresaId == EmpresaId))
                return Conflict("Ya existe un retornable con ese código.");

            var retornable = new Retornable
            {
                EmpresaId = EmpresaId,
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                ValorDeposito = dto.ValorDeposito,
                Costo = dto.Costo,
                EsPropio = dto.EsPropio,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Retornables.Add(retornable);
            await _context.SaveChangesAsync();

            var resultado = new RetornableDto
            {
                Id = retornable.Id,
                EmpresaId = retornable.EmpresaId,
                Codigo = retornable.Codigo,
                Nombre = retornable.Nombre,
                Descripcion = retornable.Descripcion,
                ValorDeposito = retornable.ValorDeposito,
                Costo = retornable.Costo,
                EsPropio = retornable.EsPropio,
                Activo = retornable.Activo,
                FechaCreacion = retornable.FechaCreacion
            };

            return CreatedAtAction(nameof(ObtenerRetornables), null, resultado);
        }


    }
}
