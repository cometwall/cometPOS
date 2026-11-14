using cometPOS.Data;
using cometPOS.DTOs.Catalogo;
using cometPOS.Models;
using cometPOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cometPOS.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        private IQueryable<Categoria> CategoriasMiEmpresa() =>
            _context.Categorias.Where(c => c.EmpresaId == EmpresaId);

        private int EmpresaId => User.EmpresaId() ?? 0;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> ObtenerCategorias()
        {
            var categorias = await CategoriasMiEmpresa()
                .Where(u => u.EmpresaId == EmpresaId && u.Activa)
                .Select(u => new CategoriaDto
                {
                    Id = u.Id,
                    EmpresaId = u.EmpresaId,
                    CategoriaPadreId = u.CategoriaPadreId,
                    Nombre = u.Nombre
                }).ToListAsync();

            return Ok(categorias);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("Crear")]
        public async Task<ActionResult<CategoriaDto>> CrearCategoria(CrearCategoriaDto dto)
        {
            if (await _context.Categorias.AnyAsync(c => c.Nombre == dto.Nombre && c.EmpresaId == EmpresaId))
                return Conflict("Ya existe una categoría con ese nombre.");

            if (dto.CategoriaPadreId.HasValue &&
                !await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaPadreId && c.EmpresaId == EmpresaId))
                return BadRequest("La categoría padre especificada no existe o no pertenece a la empresa.");

            var categoria = new Categoria
            {
                EmpresaId = EmpresaId,
                CategoriaPadreId = dto.CategoriaPadreId,
                Nombre = dto.Nombre,
                Activa = true
            };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            var resultado = new CategoriaDto
            {
                Id = categoria.Id,
                EmpresaId = categoria.EmpresaId,
                CategoriaPadreId = categoria.CategoriaPadreId,
                Nombre = categoria.Nombre
            };
            return CreatedAtAction(nameof(ObtenerCategorias), null, resultado);
        }
    }
}
