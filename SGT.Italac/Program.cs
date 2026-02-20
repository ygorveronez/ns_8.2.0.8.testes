using Servicos.Http;
using SGT.Italac;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "SGT.Italac";
});
HttpClientRegistration.RegisterClients();
builder.Services.AddSingleton<ItalacService>();
builder.Services.AddHostedService<WindowsBackgroundService>();

IHost host = builder.Build();

host.Run();