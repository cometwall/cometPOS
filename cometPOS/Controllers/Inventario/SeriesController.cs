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
    public class SeriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private int EmpresaId => User.EmpresaId() ?? 0;
        public SeriesController(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<ProductoSerie> SerieEmpresa() =>
            _context.ProductoSeries.Where(s => s.Variante.EmpresaId == EmpresaId);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SerieDto>>> ObtenerSeries()
        {
            var series = await SerieEmpresa()
                .Select(s => new SerieDto
                {
                    Id = s.Id,
                    VarianteId = s.VarianteId,
                    NumeroSerie = s.NumeroSerie,
                    LoteId = s.LoteId,
                    SucursalId = s.SucursalId,
                    Estado = s.Estado,
                    FechaCreacion = s.FechaCreacion
                }).ToListAsync();

            return Ok(series);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<SerieDto>> CrearSerie(CrearSerieDto serieDto)
        {
            if (await _context.ProductoSeries.AnyAsync(s => s.VarianteId == serieDto.VarianteId && s.NumeroSerie == serieDto.NumeroSerie))
                return Conflict("Ya existe una serie con el mismo número de serie");

            var variante = await _context.ProductoVariantes.Include(v => v.Producto)
                .FirstOrDefaultAsync(v => v.Id == serieDto.VarianteId && v.EmpresaId == EmpresaId);

            if (variante == null)
                return BadRequest("La variante especificada no existe o no pertenece a la empresa");

            if (variante.Producto.TipoSeguimiento != "SERIE")
                return BadRequest("El producto asociado a la variante no tiene seguimiento por serie");

            if (serieDto.LoteId.HasValue &&
                !await _context.ProductoLotes.AnyAsync( l =>
                    l.Id == serieDto.LoteId && l.VarianteId == serieDto.VarianteId))
            {
                return BadRequest("El lote no existe o no corresponde a esta variante");
            }

            var serie = new ProductoSerie
            {
                VarianteId = serieDto.VarianteId,
                NumeroSerie = serieDto.NumeroSerie,
                LoteId = serieDto.LoteId,
                SucursalId = User.SucursalId(),
                FechaCreacion = DateTime.UtcNow
            };

            _context.ProductoSeries.Add(serie);
            await _context.SaveChangesAsync();

            var resultDto = new SerieDto
            {
                Id = serie.Id,
                VarianteId = serie.VarianteId,
                NumeroSerie = serie.NumeroSerie,
                LoteId = serie.LoteId,
                SucursalId = serie.SucursalId,
                Estado = serie.Estado,
                FechaCreacion = serie.FechaCreacion
            };

            return CreatedAtAction(nameof(ObtenerSeries), null, resultDto);
        }
    }
}
