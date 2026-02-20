using Servicos.Http;
using SGT.MercadoLivre;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Integração com o Mercado Livre";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();
HttpClientRegistration.RegisterClients();
await host.RunAsync();