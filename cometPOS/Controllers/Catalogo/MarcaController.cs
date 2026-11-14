using cometPOS.Data;
using cometPOS.DTOs.Catalogo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cometPOS.Models;
using cometPOS.Services;
using Microsoft.EntityFrameworkCore;

namespace cometPOS.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MarcaController : ControllerBase
    {
        private readonly AppDbContext _context;

        private IQueryable<Marca> MarcasMiEmpresa() =>
            _context.Marcas.Where(u => u.EmpresaId == EmpresaId);

        public MarcaController(AppDbContext context)
        {
            _context = context;
        }

        private int EmpresaId => User.EmpresaId() ?? 0;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarcaDto>>> ObtenerMarcas()
        {
            var marcas = await MarcasMiEmpresa()
                .Where(u => u.EmpresaId == EmpresaId && u.Activa)
                .Select(u => new MarcaDto
                {
                    Id = u.Id,
                    EmpresaId = u.EmpresaId,
                    Nombre = u.Nombre
                }).ToListAsync();
            return Ok(marcas);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Crear")]
        public async Task<ActionResult<MarcaDto>> CrearMarca(CrearMarcaDto dto)
        {
            if (await _context.Marcas.AnyAsync(u => u.Nombre == dto.Nombre && u.EmpresaId == EmpresaId))
                return Conflict("Ya existe una marca con ese nombre.");
            var marca = new Marca
            {
                EmpresaId = EmpresaId,
                Nombre = dto.Nombre,
                Activa = true
            };

            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            var resultado = new MarcaDto
            {
                Id = marca.Id,
                EmpresaId = marca.EmpresaId,
                Nombre = marca.Nombre,
            };
            return CreatedAtAction(nameof(ObtenerMarcas), null, resultado);
        }
    }
}
