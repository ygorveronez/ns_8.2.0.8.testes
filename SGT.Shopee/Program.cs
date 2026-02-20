using Servicos.Http;
using SGT.Shopee;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WindowsBackgroundService>();

var host = builder.Build();
HttpClientRegistration.RegisterClients();
host.Run();
