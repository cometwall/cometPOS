using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cometPOS.Models;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.DTOs.Catalogo;
using cometPOS.Services;

namespace cometPOS.Controllers.Catalogo
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        private int EmpresaId => User.EmpresaId() ?? 0;

        private IQueryable<Producto> ProductosMiEmpresa() =>
            _context.Productos.Where(p => p.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> ObtenerProductos() 
        {
            var productos = await ProductosMiEmpresa()
                .Where(p => p.Activo)
                .Select(p => new ProductoDto
                {
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    CategoriaId = p.CategoriaId,
                    MarcaId = p.MarcaId,
                    Sku = p.Variantes
                    .Where(v => v.Activa && v.EsDefault)
                    .Select(v => v.Sku)
                    .FirstOrDefault(),

                    PrecioVenta = p.Variantes
                    .Where(v => v.Activa && v.EsDefault)
                    .Select(v => v.PrecioVenta)
                    .FirstOrDefault()
                })
                .ToListAsync();
            return Ok(productos);
        }

        [HttpPost("Crear")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<ProductoDto>> CrearProducto(CrearProductoDto dto)
        { 
            if (!await _context.UnidadesMedida.AnyAsync(p => p.Id == dto.UnidadBaseId && p.EmpresaId == EmpresaId))
                return BadRequest("La unidad base no existe o no pertenece a tu empresa");

            if (await _context.ProductoVariantes.AnyAsync(p => p.Sku == dto.Sku && p.EmpresaId == EmpresaId))
                return Conflict("Ya existe un producto con ese SKU.");

            if (dto.CategoriaId.HasValue &&
                !await _context.Categorias.AnyAsync(c =>
                    c.Id == dto.CategoriaId &&
                    c.EmpresaId == EmpresaId))
            {
                return BadRequest("La categoría no existe o no pertenece a tu empresa");
            }

            if (dto.MarcaId.HasValue &&
                !await _context.Marcas.AnyAsync(m =>
                    m.Id == dto.MarcaId &&
                    m.EmpresaId == EmpresaId))
            {
                return BadRequest("La marca no existe o no pertenece a tu empresa");
            }

            var producto = new Producto
            {
                EmpresaId = EmpresaId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                CategoriaId = dto.CategoriaId,
                MarcaId = dto.MarcaId,
                UnidadBaseId = dto.UnidadBaseId
            };
            var productoVariante = new ProductoVariante
            {
                EmpresaId = EmpresaId,
                Sku = dto.Sku,
                PrecioVenta = dto.PrecioVenta,
                Activa = true,
                EsDefault = true,
                Producto = producto
            };

            _context.Productos.Add(producto);
            _context.ProductoVariantes.Add(productoVariante);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerProductos), null, new ProductoDto 
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CategoriaId = producto.CategoriaId,
                MarcaId = producto.MarcaId,
                Sku = productoVariante.Sku,
                PrecioVenta = productoVariante.PrecioVenta
            });
        }
    }
}
