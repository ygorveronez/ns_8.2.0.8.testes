using Servicos.Http;
using SGT.ConsultaDestinadosSefaz;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Consulta documentos destinados Sefaz";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();

HttpClientRegistration.RegisterClients();
await host.RunAsync();