using Monitoramento;
using Servicos.Http;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
HttpClientRegistration.RegisterClients();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Monitoramento";
});

builder.Services.AddSingleton<MonitoramentoService>();
builder.Services.AddHostedService<WindowsBackgroundService>();
builder.Services.AddHostedService<ConfiguracaoRefreshService>();

IHost host = builder.Build();
host.Run();