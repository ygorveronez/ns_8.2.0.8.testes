using Servicos.Http;
using SGT.Intercab;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Integração com o Intercab";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();
HttpClientRegistration.RegisterClients();
await host.RunAsync();