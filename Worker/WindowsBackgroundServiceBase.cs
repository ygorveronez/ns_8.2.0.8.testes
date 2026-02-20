using Dominio.Excecoes.Embarcador;
using Utilidades.Extensions;

namespace SGT.Hangfire.Threads
{
    public class WindowsBackgroundServiceBase : BackgroundService
    {
        public readonly ILogger _logger;
        public readonly IConfiguration _configuration;

        public WindowsBackgroundServiceBase(ILogger<WindowsBackgroundService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
