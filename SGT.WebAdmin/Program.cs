using Hangfire;
using Hangfire.SqlServer;
using Servicos.Http;
using Servicos.SecretManagement;
using SGT.BackgroundWorkers.Utils;
using SGT.WebAdmin;
using SGT.WebAdmin.Controllers;
using SGT.WebAdmin.Extensions;
using SGT.WebAdmin.Filter;
using SGT.WebAdmin.Handlers;
using SGT.WebAdmin.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.ConfigureCulture();

Localization.Service.JSON.CreateJSResourceFile(System.IO.Path.Combine(Servicos.FS.GetPath(builder.Environment.ContentRootPath), "ViewsScripts"));

// Add services to the container.
Infrastructure.Services.Logging.Logger.Configure(new Servicos.Logging.ServicosLogger());
HttpClientRegistration.RegisterClients();
Startup startup = new Startup(builder.Configuration);
startup.Configuration(builder.Services);

builder.Services.AddSingleton<ISecretManager, AzureKeyVaultSecretManager>();
builder.Services.AddSingleton<Conexao>();
builder.Services.AddSingleton<CompanySettings>();
builder.Services.AddSingleton<IDistributedLock, DistributedLock>();

var hangfireWorkerConnectionStringn = Servicos.Database.ConnectionString.Instance.GetHangfireWorkerConnectionString();

if (hangfireWorkerConnectionStringn is not null)
{
    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(hangfireWorkerConnectionStringn, new SqlServerStorageOptions
        {
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),

        }));
}

// NÃO adicionar UseHangfireServer(), assim o Hangfire não executará jobs.

WebApplication app = builder.Build();

app.UseResponseCompression();



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (hangfireWorkerConnectionStringn is not null)
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });
}


app.RegisterRoutes();

app.UseWebOptimizer();

app.UseMiddleware<RequestInterceptorMiddleware>();

Servicos.Providers.ServiceProviderContext.ServiceProvider = app.Services;

app.Run();