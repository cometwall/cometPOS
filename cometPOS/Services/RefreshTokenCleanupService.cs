using cometPOS.Data;
using Microsoft.EntityFrameworkCore;

namespace cometPOS.Services
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<RefreshTokenCleanupService> _logger;

        public RefreshTokenCleanupService(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<RefreshTokenCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var horas = _config.GetValue("RefreshTokenCleanup:IntervaloHoras", 6);
            using var timer = new PeriodicTimer(TimeSpan.FromHours(horas));

            do
            {
                try
                {
                    await LimpiarAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al limpiar refresh tokens.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task LimpiarAsync(CancellationToken ct)
        {
            var dias = _config.GetValue("RefreshTokenCleanup:RetencionDias", 7);
            var limite = DateTime.UtcNow.AddDays(-dias);

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var borrados = await context.RefreshTokens
                .Where(r => r.FechaExpiracion < limite
                         || (r.FechaRenovacion != null && r.FechaRenovacion < limite))
                .ExecuteDeleteAsync(ct);
            _logger.LogInformation("Limpieza de refresh tokens: {Cantidad} eliminados.", borrados);
        }
    }
}
