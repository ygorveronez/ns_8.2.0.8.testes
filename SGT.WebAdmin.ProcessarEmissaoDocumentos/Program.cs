using Servicos.Http;
using SGT.WebAdmin.ProcessarEmissaoDocumentos;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Processar Emissão de Documentos";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();

HttpClientRegistration.RegisterClients();
await host.RunAsync();