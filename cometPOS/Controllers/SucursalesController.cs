using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.Models;
using cometPOS.DTOs;

namespace cometPOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SucursalesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SucursalesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sucursal>>> ObtenerSucursales() 
    {
        var sucursales = await _context.Sucursales.ToListAsync();

        return Ok(sucursales);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Sucursal>> ObtenerSucursal(int id)
    {
        var sucursal = await _context.Sucursales.Select(s => new SucursalDto { 
            Id = s.Id, Nombre = s.Nombre, Codigo = s.Codigo, Direccion = s.Direccion,
            Telefono = s.Telefono, Activa = s.Activa, FechaCreacion = s.FechaCreacion
        }).ToListAsync();
        if (sucursal == null)
            return NotFound();

        return Ok(sucursal);
    }

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

    [HttpPut("actualizar/{id}")]
    public async Task<IActionResult> ActualizarSucursal(int id, ActualizarSucursalDto dto)
    {
        var sucursal = await _context.Sucursales.FindAsync(id);

        if (sucursal == null)
            return NotFound();

        sucursal.Nombre = dto.Nombre;
        sucursal.Direccion = dto.Direccion;
        sucursal.Telefono = dto.Telefono;
        sucursal.Activa = dto.Activa;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("eliminar/{id}")]
    public async Task<IActionResult> DesactivarSucursal(int id)
    {
        var sucursal = await _context.Sucursales.FindAsync(id);

        if (sucursal == null)
            return NotFound();

        sucursal.Activa = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("Inactivas")]
    public async Task<ActionResult<IEnumerable<Sucursal>>> ObtenerSucursales([FromQuery] bool incluirInactivas = false)
    {
        var query = _context.Sucursales.AsQueryable();

        if (!incluirInactivas)
            query = query.Where(s => s.Activa);

        return Ok(await query.ToListAsync());
    }

}
