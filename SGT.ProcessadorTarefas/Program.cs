using Servicos.Http;

namespace SGT.ProcessadorTarefas;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        HttpClientRegistration.RegisterClients();
        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app);
    }
}