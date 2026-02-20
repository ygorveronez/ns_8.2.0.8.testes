using Dominio.Interfaces.Database;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HangfireBasicAuthenticationFilter;
using MongoDB.Driver;
using Repositorio.Global.Contexto;
using Servicos.Database;
using Servicos.ProcessadorTarefas;
using SGT.ProcessadorTarefas.Configuracao;

namespace SGT.ProcessadorTarefas
{
    public class Startup
    {
        #region Propriedades Privadas

        private IConfiguration _configuration { get; }

        #endregion Propriedades Privadas

        #region Construtores

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ConfigureServices(IServiceCollection services)
        {
            ConnectionFactory.ConfigureFileStorage(_configuration);
            ConfigureMongoDb(services);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
            });

            Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo repositorioMongo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo(new Repositorio.UnitOfWork(ConnectionFactory.StringConexao(_configuration)));
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo configuracaoMongo = repositorioMongo.BuscarConfiguracaoPadrao();

#if DEBUG
            configuracaoMongo.Banco = "gruposc-teste";
#endif
            MongoClient mongoClient = new MongoClient(configuracaoMongo.MongoUrl.ToMongoUrl());

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseMongoStorage(mongoClient, configuracaoMongo.Banco, new MongoStorageOptions
                      {
                          MigrationOptions = new MongoMigrationOptions
                          {
                              MigrationStrategy = new MigrateMongoMigrationStrategy(),
                              BackupStrategy = new CollectionMongoBackupStrategy()
                          },
                          Prefix = "hangfire.mongo",
                          CheckConnection = false,
                          CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                      });
            });

            List<TipoEtapaTarefaExtensions.ConfigFila> filas = TipoEtapaTarefaExtensions.ObterFilasComWorkers();
            string hostname = Environment.MachineName;

            foreach (var fila in filas)
            {
                services.AddHangfireServer(options =>
                {
                    options.ServerName = $"Hangfire_{hostname}_{fila.Nome}";
                    options.Queues = new[] { fila.Nome };
                    options.WorkerCount = fila.WorkerCount;
                });
            }

            services.AddScoped<OrquestradorTarefas>();
        }

        public void Configure(WebApplication app)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo repositorioMongo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo(new Repositorio.UnitOfWork(ConnectionFactory.StringConexao(_configuration)));

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo configuracaoMongo = repositorioMongo.BuscarConfiguracaoPadrao();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "Hangfire Dashboard",
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = configuracaoMongo.UsuarioHangfire,
                        Pass = configuracaoMongo.SenhaHangfire,
                    }
                }
            });

            app.MapHangfireDashboard();

            using (var scope = app.Services.CreateScope())
            {
                var repoTarefa = scope.ServiceProvider.GetRequiredService<IProcessamentoTarefaRepository>();
                var repoRequest = scope.ServiceProvider.GetRequiredService<IRequestDocumentoRepository>();
                var repoSubtarefa = scope.ServiceProvider.GetRequiredService<IRequestSubtarefaRepository>();

                repoTarefa.CriarIndicesAsync().Wait();
                repoRequest.CriarIndicesAsync().Wait();
                repoSubtarefa.CriarIndicesAsync().Wait();
            }

            app.Run();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void ConfigureMongoDb(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton<ITenantService, TenantService>();

            AdicionarEtapas(services);
            AdicionarEstrategias(services);

            services
                .AdicionarMongoDb()
                .AdicionarRepositoriosMongoDb();
        }

        private void AdicionarEtapas(IServiceCollection services)
        {
            List<Type> etapaTypes = typeof(EtapaState).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(EtapaState).IsAssignableFrom(t))
                .ToList();

            foreach (Type etapaType in etapaTypes)
                services.AddScoped(etapaType);

            services.AddScoped<EtapaProcessor>();
            services.AddScoped<EtapaStateFactory>();
        }

        private void AdicionarEstrategias(IServiceCollection services)
        {
            List<Type> strategyTypes = typeof(EtapaState).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                        (typeof(IStrategyRetornoIntegracao).IsAssignableFrom(t) || typeof(IStrategyQuebraRequest).IsAssignableFrom(t))
                ).ToList();

            foreach (Type strategyType in strategyTypes)
                services.AddScoped(strategyType);

            services.AddScoped<TarefaStrategyFactory>();
        }

        #endregion Métodos Privados
    }
}