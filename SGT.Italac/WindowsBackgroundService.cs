namespace SGT.Italac
{
    public class WindowsBackgroundService : WindowsBackgroundServiceBase
    {
        public WindowsBackgroundService(ItalacService italacService, ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(italacService, logger, configuration) { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
                        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");

                        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                        _italacService.Iniciar(Cliente, StringConexao);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Servicos.Log.TratarErro("Tharead Cancelada");
            }
        }
    }
}