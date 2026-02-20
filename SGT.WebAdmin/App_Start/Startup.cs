using Dominio.Interfaces.Database;
using Microsoft.AspNetCore.Http.Features;
using Repositorio.Global.Contexto;
using Servicos.Database;
using MediatR;
using SGT.WebAdmin.Extensions;

namespace SGT.WebAdmin
{
    public partial class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configuration(IServiceCollection services)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.ServicePointManager.SecurityProtocol;
            ConfigureMetricsForJobs();
            ConfigureAuth(services);
            ConfigureMongoDb(services);
            services.AddMediatRConfiguration();
            ConfigureSSo(services);
            ConfigureForm(services);
            BundleConfig.RegisterBundles(services);

            Environment.SetEnvironmentVariable("threadsIniciadas", "false");
        }

        /// <summary>
        /// Initialize the services responsible for send metrics of Threads (BackgroundWorkers).
        /// </summary>        
        private void ConfigureMetricsForJobs()
        {
            if (bool.TryParse(_configuration["StatsD:Enabled"], out var enabled) && enabled)
            {
                var host = _configuration["StatsD:Host"] ?? "localhost";
                var port = int.Parse(_configuration["StatsD:Port"] ?? "8125");
                var prefix = _configuration["StatsD:Prefix"] ?? "sgt.webadmin";

                JobMetricsHelper.Initialize(host, port, prefix);
            }
        }

        private void ConfigureForm(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
                options.KeyLengthLimit = int.MaxValue;
            });
        }

        private void ConfigureMongoDb(IServiceCollection services)
        {
            services.AddSingleton<ITenantService, TenantService>();

            services
                .AdicionarMongoDb()
                .AdicionarRepositoriosMongoDb();
        }
    }
}