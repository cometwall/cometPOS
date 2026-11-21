using cometPOS.Data;
using cometPOS.Models;
using cometPOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cometPOS.DTOs.Inventario;
using Microsoft.EntityFrameworkCore;

namespace cometPOS.Controllers.Inventario
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SucursalProductoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SucursalProductoController(AppDbContext context)
        {
            _context = context;
        }

        private int EmpresaId => User.EmpresaId() ?? 0;

        private IQueryable<SucursalProducto> SucursalMiProductoEmpresa() =>
            _context.SucursalProductos.Where(p => p.Sucursal.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SucursalProductoDto>>> ObtenerProductosSucursal()
        {
            var productosSucursal = await SucursalMiProductoEmpresa()
                .Where(p => p.Sucursal.Activa && p.Activo)
                .Select(p => new SucursalProductoDto
                {
                    VarianteId = p.VarianteId,
                    SucursalId = p.SucursalId,
                    PrecioVenta = p.PrecioVenta,
                    StockMinimo = p.StockMinimo,
                    StockMaximo = p.StockMaximo ?? 0,
                    PuntoReorden = p.PuntoReorden ?? 0,
                    Ubicacion = p.Ubicacion,
                    Cantidad = p.Inventario != null ? p.Inventario.Cantidad : 0
                }).ToListAsync();

            return Ok(productosSucursal);
        }

        [HttpGet("{sucursalId}/{varianteId}")]
        public async Task<ActionResult<SucursalProductoDto>> ObtenerProductoSucursal(
            int sucursalId,
            int varianteId)
        {
            var productoSucursal = await SucursalMiProductoEmpresa()
                .Where(p => p.SucursalId == sucursalId &&
                            p.VarianteId == varianteId &&
                            p.Sucursal.Activa &&
                            p.Activo)
                .Select(p => new SucursalProductoDto
                {
                    VarianteId = p.VarianteId,
                    SucursalId = p.SucursalId,
                    PrecioVenta = p.PrecioVenta,
                    StockMinimo = p.StockMinimo,
                    StockMaximo = p.StockMaximo ?? 0,
                    PuntoReorden = p.PuntoReorden ?? 0,
                    Ubicacion = p.Ubicacion,
                    Cantidad = p.Inventario != null ? p.Inventario.Cantidad : 0
                }).FirstOrDefaultAsync();

            if (productoSucursal == null)
                return NotFound("No se encontro el producto en esta sucursal");

            return Ok(productoSucursal);
        }

        [HttpPost("Crear")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<SucursalProductoDto>> CrearProductoSucursal(AsignarProductoSucursalDto dto)
        {
            var sucursal = await _context.Sucursales
                .FirstOrDefaultAsync(s =>
                s.Id == dto.SucursalId &&
                s.EmpresaId == EmpresaId &&
                s.Activa);

            if (sucursal == null)
                return BadRequest("La sucursal no existe o no pertenece a tu empresa");

            var variante = await _context.ProductoVariantes.FirstOrDefaultAsync(v => v.Id == dto.VarianteId && v.EmpresaId == EmpresaId);

            if (variante == null)
                return NotFound("La variante del producto no existe");

            var existe = await _context.SucursalProductos.AnyAsync(p =>
               p.SucursalId == dto.SucursalId &&
               p.VarianteId == dto.VarianteId);

            if (existe)
                return Conflict("Este producto ya esta asignado a esta sucursal.");

            var productoSucursal = new SucursalProducto
            {
                SucursalId = dto.SucursalId,
                VarianteId = dto.VarianteId,
                PrecioVenta = dto.PrecioVenta,
                StockMinimo = dto.StockMinimo,
                StockMaximo = dto.StockMaximo,
                PuntoReorden = dto.PuntoReorden,
                Ubicacion = dto.Ubicacion,
                Activo = true
            };

            _context.SucursalProductos.Add(productoSucursal);
            _context.InventarioSucursal.Add(new InventarioSucursal
            {
                SucursalId = dto.SucursalId,
                VarianteId = dto.VarianteId,
                Cantidad = 0
            });

            await _context.SaveChangesAsync();

            var respuesta = new SucursalProductoDto
            {
                SucursalId = productoSucursal.SucursalId,
                VarianteId = productoSucursal.VarianteId,
                PrecioVenta = productoSucursal.PrecioVenta,
                StockMinimo = productoSucursal.StockMinimo,
                StockMaximo = productoSucursal.StockMaximo ?? 0,
                PuntoReorden = productoSucursal.PuntoReorden ?? 0,
                Ubicacion = productoSucursal.Ubicacion,
                Cantidad = 0
            };

            return CreatedAtAction(
                nameof(ObtenerProductoSucursal),
                new
                {
                    sucursalId = respuesta.SucursalId,
                    varianteId = respuesta.VarianteId
                },
                respuesta
            );
        }
    }
}
