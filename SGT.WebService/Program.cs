using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Dominio.Interfaces.Database;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;
using Repositorio.Global.Contexto;
using Servicos.Database;
using Servicos.Http;
using SGT.WebService;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
HttpClientRegistration.RegisterClients();

builder.Services.AddServiceModelServices().AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.WebHost.UseIISIntegration();
builder.Services.Configure<IISOptions>(options =>
{
    options.ForwardClientCertificate = false;
});

builder.Services.AddSingleton<ITenantService, TenantService>();

builder.Services
    .AdicionarMongoDb()
    .AdicionarRepositoriosMongoDb();

ConfigureHangfire(builder.Services);

builder.Services.AddScoped<Dominio.Interfaces.ProcessadorTarefas.IAdicionarRequestAssincrono, Servicos.ProcessadorTarefas.AdicionarRequestAssincrono>();

var serviceTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract &&
                t.GetInterfaces().Any(i => i.GetCustomAttribute<ServiceContractAttribute>() != null)
    );

foreach (var serviceType in serviceTypes)
{
    builder.Services.AddTransient(serviceType);
}

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.RegisterWcfServices();
app.MapControllers();
app.MapGet("/", () => "SGT Web Service");
app.Run();

static void ConfigureHangfire(IServiceCollection services)
{
    services.AddSingleton<HangfireMongoConfig>(sp =>
    {
        var tenantService = sp.GetRequiredService<ITenantService>();
        var mongoConfig = tenantService.ObterMongoDbConfiguracao();
        
        if (mongoConfig == null)
            throw new InvalidOperationException("Configuração MongoDB não encontrada");

        var mongoClient = new MongoClient(mongoConfig.ToMongoUrl());
        var databaseName = mongoConfig.DatabaseName;

#if DEBUG
        databaseName = "gruposc-teste";
#endif

        return new HangfireMongoConfig
        {
            MongoClient = mongoClient,
            DatabaseName = databaseName
        };
    });

    services.AddHangfire((sp, config) =>
    {
        var hangfireConfig = sp.GetRequiredService<HangfireMongoConfig>();

        config.SetDataCompatibilityLevel(Hangfire.CompatibilityLevel.Version_170)
              .UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
              .UseMongoStorage(hangfireConfig.MongoClient, hangfireConfig.DatabaseName, new MongoStorageOptions
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
}

class HangfireMongoConfig
{
    public MongoClient MongoClient { get; set; }
    public string DatabaseName { get; set; }
}