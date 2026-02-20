using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using ReportApi.Common;
using ReportApi.Interfaces;
using ReportApi.options;
using ReportApi.ReportService;
using ReportApi.Storage;
using ReportApi.Worker;
using Servicos.Embarcador.Relatorios;

namespace ReportApi;

public class DependencyInjection
{
    public static IServiceProvider Configure()
    {
        var host = Host.CreateApplicationBuilder();
        
        host.Logging.AddSimpleConsole(i => i.ColorBehavior = LoggerColorBehavior.Disabled);

        ConfigureServices(host.Services);
        Task.Run(() => host.Build().RunAsync());

        return host.Services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersAsServices(typeof(MvcApplication).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => typeof(IController).IsAssignableFrom(t)
                        || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));

        services.AddScoped<Repositorio.UnitOfWork>(sp => new Repositorio.UnitOfWork(ConnectionFactory.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova));
        services.AddScoped<RelatorioReportService>();
        services.AddScoped<IStorage, FileSystemStorage>();
        services.AddScoped<BemReportService>();

        services.AddScoped<IOptions<DatabaseOptions>>(sp => Options.Create(new DatabaseOptions(ConnectionFactory.StringConexao, 10)));

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio() { QuantidadeRelatoriosParalelo = 10 };
        services.AddScoped<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio>(sp => configuracaoRelatorio);
        services.AddScoped<RelatorioFactory>();
        services.AddScoped<ReportServiceBase>();
        services.AddScoped<Servicos.Embarcador.Relatorios.RelatorioReportService>();

        services.AddHostedService<ConsultarRelatoriosPendentesWorker>();
        services.AddHostedService<GerarAutomatizacoesPendentesWorker>();

        var typesOfReportFound = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(t => typeof(IReport).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        foreach (var type in typesOfReportFound)
        {
            services.AddScoped(type);
        }


        services.AddSingleton<TypeFinder>();
    }
}



public class DefaultDependencyResolver : IDependencyResolver
{
    private readonly IServiceProvider _services;

    public DefaultDependencyResolver(IServiceProvider services)
    {
        _services = services;
    }


    public object GetService(System.Type serviceType)
    {
        return _services.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return _services.GetServices(serviceType);
    }
}

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddControllersAsServices(this IServiceCollection services,
        IEnumerable<Type> controllerTypes)
    {
        foreach (var type in controllerTypes)
        {
            services.AddTransient(type);
        }
        return services;
    }
}