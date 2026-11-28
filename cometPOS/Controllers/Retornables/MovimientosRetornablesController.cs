using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.Models;
using cometPOS.DTOs;
using Microsoft.AspNetCore.Authorization;
using cometPOS.Services;
using cometPOS.DTOs.Retornables;
using System.Security.Claims;

namespace cometPOS.Controllers.Retornables
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovimientosRetornablesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MovimientosRetornablesController(AppDbContext context)
        {
            _context = context;
        }
        private int EmpresaId => User.EmpresaId() ?? 0;

        private int? UsuarioId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        private IQueryable<MovimientoRetornable> MovimientosDeMiEmpresa() =>
            _context.MovimientosRetornables
                .Where(m => m.Retornable.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoRetornableDto>>> ObtenerListaMovimientos()
        {
            var movimientos = await MovimientosDeMiEmpresa()
                .Select(m => new MovimientoRetornableDto
                {
                    Id = m.Id,
                    SucursalId = m.SucursalId,
                    SucursalNombre = m.Sucursal.Nombre,
                    RetornableId = m.RetornableId,
                    RetornableNombre = m.Retornable.Nombre,
                    Tipo = m.Tipo,
                    Cantidad = m.Cantidad,
                    ValorDeposito = m.ValorDeposito,
                    ReferenciaTipo = m.ReferenciaTipo,
                    ReferenciaId = m.ReferenciaId,
                    UsuarioId = m.UsuarioId,
                    UsuarioNombre = m.Usuario != null ? m.Usuario.Nombre :null,
                    Notas = m.Notas,
                    Fecha = m.FechaMovimiento
                })
                .ToListAsync();

            return Ok(movimientos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoRetornableDto>> ObtenerMovimiento(long id)
        {
            var movimiento = await MovimientosDeMiEmpresa()
                .Where(m => m.Id == id)
                .Select(m => new MovimientoRetornableDto
                {
                    Id = m.Id,
                    SucursalId = m.SucursalId,
                    SucursalNombre = m.Sucursal.Nombre,
                    RetornableId = m.RetornableId,
                    RetornableNombre = m.Retornable.Nombre,
                    Tipo = m.Tipo,
                    Cantidad = m.Cantidad,
                    ValorDeposito = m.ValorDeposito,
                    ReferenciaTipo = m.ReferenciaTipo,
                    ReferenciaId = m.ReferenciaId,
                    UsuarioId = m.UsuarioId,
                    UsuarioNombre = m.Usuario != null ? m.Usuario.Nombre : null,
                    Notas = m.Notas,
                    Fecha = m.FechaMovimiento
                }).FirstOrDefaultAsync();

            if (movimiento == null)
                return NotFound("El movimiento no existe o no pertenece a tu empresa");

            return Ok(movimiento);
        }

        [HttpGet("Inventario")]
        public async Task<ActionResult<IEnumerable<InventarioRetornableDto>>> ObtenerInventarioRetornable()
        {
            var inventario = await _context.InventarioRetornables
                .Where(i => i.Retornable.EmpresaId == EmpresaId)
                .Select(i => new InventarioRetornableDto
                {
                    SucursalId = i.SucursalId,
                    RetornableId = i.RetornableId,
                    RetornableCodigo = i.Retornable.Codigo,
                    RetornableNombre = i.Retornable.Nombre,
                    CantidadVacios = i.CantidadVacios,
                    FechaActualizacion = i.FechaActualizacion
                }).ToListAsync();

            return Ok(inventario);
        }

        [HttpPost("Crear")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<MovimientoRetornableDto>> RegistrarMovimiento(RegistrarMovimientoRetornableDto dto)
        {
            if (dto.Tipo != "Entrada" && dto.Tipo != "Salida")
                return BadRequest("El tipo de movimiento debe ser 'Entrada' o 'Salida'");

            var sucursal = await _context.Sucursales.FindAsync(dto.SucursalId);
            if (sucursal == null || sucursal.EmpresaId != EmpresaId)
                return BadRequest("Sucursal no válida.");

            var retornable = await _context.Retornables.FindAsync(dto.RetornableId);
            if (retornable == null || retornable.EmpresaId != EmpresaId)
                return BadRequest("Retornable no válido.");

            var inventario = await _context.InventarioRetornables
                .FirstOrDefaultAsync(i => i.SucursalId == dto.SucursalId && i.RetornableId == dto.RetornableId);

            if (inventario == null)
            {
                inventario = new InventarioRetornable
                {
                    SucursalId = dto.SucursalId,
                    RetornableId = dto.RetornableId,
                    CantidadVacios = 0
                };
                _context.InventarioRetornables.Add(inventario);
            }

            if (dto.Tipo == "Entrada")
            {
                inventario.CantidadVacios += dto.Cantidad;
            }
            else
            {
                if (inventario.CantidadVacios < dto.Cantidad)
                    return BadRequest("No hay suficientes envases vacíos para esta salida.");
                inventario.CantidadVacios -= dto.Cantidad;
            }

            inventario.FechaActualizacion = DateTime.UtcNow;

            var movimiento = new MovimientoRetornable
            {
                SucursalId = dto.SucursalId,
                RetornableId = dto.RetornableId,
                Tipo = dto.Tipo,
                Cantidad = dto.Cantidad,
                ValorDeposito = dto.ValorDeposito ?? 0,
                UsuarioId = UsuarioId,
                Notas = dto.Notas,
                FechaMovimiento = DateTime.UtcNow
            };
            _context.MovimientosRetornables.Add(movimiento);
            await _context.SaveChangesAsync();

            var resultado = new MovimientoRetornableDto
            {
                Id = movimiento.Id,
                SucursalId = movimiento.SucursalId,
                SucursalNombre = sucursal.Nombre,
                RetornableId = movimiento.RetornableId,
                RetornableNombre = retornable.Nombre,
                Tipo = movimiento.Tipo,
                Cantidad = movimiento.Cantidad,
                ValorDeposito = movimiento.ValorDeposito,
                ReferenciaTipo = movimiento.ReferenciaTipo,
                ReferenciaId = movimiento.ReferenciaId,
                UsuarioId = movimiento.UsuarioId,
                UsuarioNombre = null,
                Notas = movimiento.Notas,
                Fecha = movimiento.FechaMovimiento
            };
            return CreatedAtAction(nameof(ObtenerMovimiento), new { id = resultado.Id }, resultado);
        }
    }
}
