using Hangfire;
using Hangfire.SqlServer;
using Servicos.Http;
using Servicos.SecretManagement;
using SGT.Hangfire.Threads;
using SGT.Hangfire.Threads.Jobs;

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "SGT.Hangfire.Threads";
    });

    HttpClientRegistration.RegisterClients();
    builder.Services.AddHangfireServer();

    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(Servicos.Database.ConnectionString.Instance.GetHangfireWorkerConnectionString(), new SqlServerStorageOptions
        {
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        }));

    var adminMultisoftwareConectionString = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");

    GlobalConfiguration.Configuration
        .UseActivator(new HangfireActivator(builder.Services.BuildServiceProvider()));

    GlobalJobFilters.Filters.Add(new JobFinalizadoFilterAttribute());
    builder.Services.AddSingleton<IHostedService, JobsInitializerService>();

    builder.Services.AddHostedService<WindowsBackgroundService>();

    IHost host = builder.Build();

    host.Run();
}
catch (Exception ex)
{
    Servicos.Log.TratarErro(ex);
}