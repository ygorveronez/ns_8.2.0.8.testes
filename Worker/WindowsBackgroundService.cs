namespace SGT.Hangfire.Threads
{
    public class WindowsBackgroundService : WindowsBackgroundServiceBase
    {
        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(logger, configuration) { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");

                //Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);

                //_hangfireManagement.SetarExecucaoThreads(Cliente, StringConexao, stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //_monitoramentoService.FinalizarThreadsMonitoramentos();
            //_monitoramentoService.FinalizarThreadsIntegracoes();

            Servicos.Log.TratarErro("Serviço Monitoramento Parado.");

            return base.StopAsync(cancellationToken);
        }
    }
}
