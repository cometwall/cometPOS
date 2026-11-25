using cometPOS.Data;
using cometPOS.DTOs.Inventario;
using cometPOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cometPOS.Models;
using Microsoft.EntityFrameworkCore;

namespace cometPOS.Controllers.Inventario
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private int EmpresaId => User.EmpresaId() ?? 0;
        public MovimientosInventarioController(AppDbContext context) 
        {
            _context = context;
        }
        private IQueryable<MovimientoInventario> MovimientosMiEmpresa() =>
            _context.MovimientosInventario.Where(mbox => mbox.Sucursal.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoInventarioDto>>> ObtenerMovimientos([FromQuery] int? sucursalId, [FromQuery] int? varianteId)
        {
            var movimientos = await MovimientosMiEmpresa()
                .Where(mbox => (!sucursalId.HasValue || mbox.SucursalId == sucursalId.Value) &&
                               (!varianteId.HasValue || mbox.VarianteId == varianteId.Value))
                .Select(mbox => new MovimientoInventarioDto
                {
                    Id = mbox.Id,
                    SucursalId = mbox.SucursalId,
                    VarianteId = mbox.VarianteId,
                    Tipo = mbox.Tipo,
                    Cantidad = mbox.Cantidad,
                    CostoUnitario = mbox.CostoUnitario,
                    Notas = mbox.Notas,
                    FechaMovimiento = mbox.FechaMovimiento
                }).ToListAsync();
            return Ok(movimientos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoInventarioDto>> ObtenerMovimiento(long id) 
        {
            var movimiento = await MovimientosMiEmpresa()
                .Where(mbox => mbox.Id == id && mbox.Sucursal.EmpresaId == EmpresaId)
                .Select(mbox => new MovimientoInventarioDto
                {
                    Id = mbox.Id,
                    SucursalId = mbox.SucursalId,
                    VarianteId = mbox.VarianteId,
                    Tipo = mbox.Tipo,
                    Cantidad = mbox.Cantidad,
                    CostoUnitario = mbox.CostoUnitario,
                    Notas = mbox.Notas,
                    FechaMovimiento = mbox.FechaMovimiento
                }).FirstOrDefaultAsync();

            if (movimiento == null)
                return NotFound("El movimiento no existe o no pertenece a tu empresa");

            return Ok(movimiento);

        }

        [HttpPost("Crear")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<MovimientoInventarioDto>> RegistroMovimiento(RegistrarMovimientoDto dto)
        {
            if (dto.Tipo != "Entrada" && dto.Tipo != "Salida")
                return BadRequest("El tipo de movimiento debe ser 'Entrada' o 'Salida'");

            if (dto.Cantidad < 0)
                return BadRequest("La cantidad debe ser mayor a 0");

            var inventarioSucursal = await _context.InventarioSucursal
                .Include(i => i.SucursalProducto)
                .FirstOrDefaultAsync(i =>
                    i.SucursalProducto.SucursalId == dto.SucursalId &&
                    i.SucursalProducto.VarianteId == dto.VarianteId &&
                    i.SucursalProducto.Sucursal.EmpresaId == EmpresaId
                );

            if (inventarioSucursal == null)
                return BadRequest("El producto no esta asignado a esa sucursal");

            if (dto.Tipo == "Entrada")
            {
                inventarioSucursal.Cantidad += dto.Cantidad;
            }
            else
            {
                if (inventarioSucursal.Cantidad < dto.Cantidad)
                    return BadRequest("No hay suficiente inventario para realizar esta salida.");
                inventarioSucursal.Cantidad -= dto.Cantidad;
            }

            var movimiento = new MovimientoInventario
            {
                SucursalId = dto.SucursalId,
                VarianteId = dto.VarianteId,

                Tipo = dto.Tipo,
                Cantidad = dto.Cantidad,
                CostoUnitario = dto.CostoUnitario,
                Notas = dto.Notas,
                FechaMovimiento = DateTime.UtcNow
            };

            _context.MovimientosInventario.Add(movimiento);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerMovimientos), null, new MovimientoInventarioDto { 
                Id = movimiento.Id,
                SucursalId = movimiento.SucursalId,
                VarianteId = movimiento.VarianteId,
                Tipo = movimiento.Tipo,
                Cantidad = movimiento.Cantidad,
                CostoUnitario = movimiento.CostoUnitario,
                Notas = movimiento.Notas,
                FechaMovimiento = movimiento.FechaMovimiento,
                CantidadResultante = inventarioSucursal.Cantidad
            });

        }
    }
}
