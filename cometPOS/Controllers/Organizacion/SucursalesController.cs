using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.Models;
using cometPOS.DTOs;
using Microsoft.AspNetCore.Authorization;
using cometPOS.Services;

namespace cometPOS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SucursalesController : ControllerBase
{
    private readonly AppDbContext _context;

    private int EmpresaId => User.EmpresaId() ?? 0;

    private IQueryable<Sucursal> SucursalesDeMiEmpresa() =>
        _context.Sucursales.Where(s => s.EmpresaId == EmpresaId);

    private static SucursalDto ASucursalDto(Sucursal s) => new()
    {
        Id = s.Id,
        Nombre = s.Nombre,
        Codigo = s.Codigo,
        Direccion = s.Direccion,
        Telefono = s.Telefono,
        Activa = s.Activa,
        FechaCreacion = s.FechaCreacion
    };

    public SucursalesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SucursalDto>>> ObtenerSucursales() 
    {
        var sucursales = await SucursalesDeMiEmpresa().ToListAsync();

        return Ok(sucursales.Select(ASucursalDto));
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<SucursalDto>> ObtenerSucursal(int id)
    {
        var sucursal = await SucursalesDeMiEmpresa().Where(s => s.Id == id).Select(s => new SucursalDto { 
            Id = s.Id, Nombre = s.Nombre, Codigo = s.Codigo, Direccion = s.Direccion,
            Telefono = s.Telefono, Activa = s.Activa, FechaCreacion = s.FechaCreacion
        }).FirstOrDefaultAsync();
        if (sucursal == null)
            return NotFound();

        return Ok(sucursal);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("Crear")]
    public async Task<ActionResult<SucursalDto>> CrearSucursal(CrearSucursalDto dto)
    {
        var codigo = dto.Codigo.Trim().ToUpperInvariant();
        if (await _context.Sucursales.AnyAsync(s => s.Codigo == dto.Codigo))
            return Conflict($"Ya existe una sucursal con el codigo '{dto.Codigo}'.");
        var sucursal = new Sucursal
        {
            Nombre = dto.Nombre,
            Codigo = codigo,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            EmpresaId = EmpresaId,
            Activa = true
        };
        _context.Sucursales.Add(sucursal);
        await _context.SaveChangesAsync();

        var resultado = new SucursalDto
        {
            Id = sucursal.Id,
            Nombre = sucursal.Nombre!,
            Codigo = sucursal.Codigo!,
            Direccion = sucursal.Direccion,
            Telefono = sucursal.Telefono,
            Activa = sucursal.Activa,
            FechaCreacion = sucursal.FechaCreacion
        };

        return CreatedAtAction(
            nameof(ObtenerSucursal),
            new { id = sucursal.Id },
            resultado);
    }

    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPut("actualizar/{id}")]
    public async Task<IActionResult> ActualizarSucursal(int id, ActualizarSucursalDto dto)
    {
        var esSuperAdmin = User.EsSuperAdmin();

        if (!esSuperAdmin && User.SucursalId() != id)
            return Forbid();

        var sucursal = await SucursalesDeMiEmpresa().FirstOrDefaultAsync(s => s.Id == id);

        if (sucursal == null)
            return NotFound();

        sucursal.Nombre = dto.Nombre;
        sucursal.Direccion = dto.Direccion;
        sucursal.Telefono = dto.Telefono;
        if (esSuperAdmin)
            sucursal.Activa = dto.Activa;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("eliminar/{id}")]
    public async Task<IActionResult> DesactivarSucursal(int id)
    {
        var sucursal = await SucursalesDeMiEmpresa().FirstOrDefaultAsync(s => s.Id == id);

        if (sucursal == null)
            return NotFound();

        sucursal.Activa = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("Inactivas")]
    public async Task<ActionResult<IEnumerable<SucursalDto>>> ObtenerSucursales([FromQuery] bool incluirInactivas = false)
    {
        var query = SucursalesDeMiEmpresa();

        if (!incluirInactivas)
            query = query.Where(s => s.Activa);

        return Ok((await query.ToListAsync()).Select(ASucursalDto));
    }

}
