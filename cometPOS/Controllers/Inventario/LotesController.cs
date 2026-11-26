using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cometPOS.Models;
using cometPOS.DTOs;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.Services;
using cometPOS.DTOs.Inventario;

namespace cometPOS.Controllers.Inventario
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private int EmpresaId => User.EmpresaId() ?? 0;
        public LotesController(AppDbContext context) 
        {
            _context = context;
        }

        private IQueryable<ProductoLote> LoteEmpresa() =>
            _context.ProductoLotes.Where(l => l.Variante.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoteDto>>> ObtenerLotes() 
        {
            var lotes = await LoteEmpresa()
                .Select(l => new LoteDto
                {
                    Id = l.Id,
                    VarianteId = l.VarianteId,
                    NumeroLote = l.NumeroLote,
                    FechaFabricacion = l.FechaFabricacion,
                    FechaCaducidad = l.FechaCaducidad,
                    Activo = l.Activo,
                    FechaCreacion = l.FechaCreacion
                }).ToListAsync();

            return Ok(lotes);
        }

        [HttpPost]
        [Authorize(Roles ="SuperAdmin,Admin")]
        public async Task<ActionResult<LoteDto>> CrearLote(CrearLoteDto loteDto)
        {
            if (await _context.ProductoLotes.AnyAsync(l => l.VarianteId == loteDto.VarianteId && l.NumeroLote == loteDto.NumeroLote))
                return Conflict("Ya existe un lote con el mismo número de lote para esta variante");

            var variante = await _context.ProductoVariantes.Include(v => v.Producto)
                .FirstOrDefaultAsync(v => v.Id == loteDto.VarianteId && v.EmpresaId == EmpresaId);

            if (variante == null)
                return BadRequest("La variante no existe o no pertenece a tu empresa");

            if (variante.Producto.TipoSeguimiento != "LOTE")
                return BadRequest("Este producto no esta configurado para seguimiento por lote");

            var lote = new ProductoLote
            {
                VarianteId = loteDto.VarianteId,
                NumeroLote = loteDto.NumeroLote,
                FechaCaducidad = loteDto.FechaCaducidad,
                FechaFabricacion = loteDto.FechaFabricacion
            };

            _context.ProductoLotes.Add(lote);
            await _context.SaveChangesAsync();

            var resultDto = new LoteDto
            {
                Id = lote.Id,
                VarianteId = lote.VarianteId,
                NumeroLote = lote.NumeroLote,
                FechaFabricacion = lote.FechaFabricacion,
                FechaCaducidad = lote.FechaCaducidad,
                Activo = lote.Activo,
                FechaCreacion = lote.FechaCreacion
            };

            return CreatedAtAction(nameof(ObtenerLotes), null, resultDto);
        }
    }
}
