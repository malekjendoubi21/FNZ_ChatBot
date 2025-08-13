using FNZ_ChatBot.Services;

namespace FNZ_ChatBot.Services
{
    public class CleanupBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CleanupBackgroundService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Nettoyage toutes les heures

        public CleanupBackgroundService(IServiceProvider serviceProvider, ILogger<CleanupBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service de nettoyage des codes de vérification démarré");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var verificationCodeService = scope.ServiceProvider.GetRequiredService<IVerificationCodeService>();
                    
                    await verificationCodeService.CleanupExpiredCodesAsync();
                    
                    _logger.LogDebug("Nettoyage des codes expirés effectué");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du nettoyage des codes expirés");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Service de nettoyage des codes de vérification arrêté");
        }
    }
}