using Servicos.Http;
using SGT.WebAdmin.ProcessarCalculoDeFrete;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "SGT WebAdmin Processar Calculo De Frete";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();

HttpClientRegistration.RegisterClients();
await host.RunAsync();