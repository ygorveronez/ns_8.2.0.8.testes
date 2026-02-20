using Dominio.Entidades.ProcessadorTarefas.Configurator;
using Dominio.Interfaces.Database;
using Dominio.Interfaces.Repositorios;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Security.Authentication;

namespace Repositorio.Global.Contexto
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AdicionarMongoDb(this IServiceCollection services)
        {
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            services.AddSingleton<MongoClient>(sp =>
            {
                var config = sp.GetRequiredService<ITenantService>().ObterMongoDbConfiguracao();
                var settings = MongoClientSettings.FromUrl(config.ToMongoUrl());

                if (config.UseTls)
                    settings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = SslProtocols.Tls12
                    };

                return new MongoClient(settings);
            });

            return services;
        }

        public static IServiceCollection AdicionarRepositoriosMongoDb(this IServiceCollection services)
        {
            services.AddScoped<Dominio.Interfaces.Repositorios.ProcessadorTarefas.ITarefaIntegracao, ProcessadorTarefas.TarefaIntegracao>();

            services.AddScoped<Dominio.Interfaces.Repositorios.ProcessadorTarefas.IProcessamentoTarefaRepository, ProcessadorTarefas.ProcessamentoTarefaRepository>();
            services.AddScoped<Dominio.Interfaces.Repositorios.ProcessadorTarefas.IRequestDocumentoRepository, ProcessadorTarefas.RequestDocumentoRepository>();
            services.AddScoped<Dominio.Interfaces.Repositorios.ProcessadorTarefas.IRequestSubtarefaRepository, ProcessadorTarefas.RequestSubtarefaRepository>();

            new ProcessamentoTarefaConfigurator().RegisterClassMap();
            new EtapaInfoConfigurator().RegisterClassMap();
            new RequestDocumentoConfigurator().RegisterClassMap();
            new RequestSubtarefaConfigurator().RegisterClassMap();
            new IntegracaoConfigurator().RegisterClassMap();
            new TarefaIntegracaoConfigurator().RegisterClassMap();
            new ArquivoIntegracaoConfigurator().RegisterClassMap();

            return services;
        }
    }
}
