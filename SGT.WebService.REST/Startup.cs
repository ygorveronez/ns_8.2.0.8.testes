using Dominio.Interfaces.Database;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using Repositorio.Global.Contexto;
using Servicos.Database;
using Servicos.Http;
using SGT.WebService.REST.Middleware;
using SGT.WebService.REST.Utils;
using System;
using System.Reflection;

namespace SGT.WebService.REST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Infrastructure.Services.Logging.Logger.Configure(new Servicos.Logging.ServicosLogger());
            HttpClientRegistration.RegisterClients();
            services.AddControllers();
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Multisoftware API", Version = "v1" });

                c.AddSecurityDefinition("Token", new OpenApiSecurityScheme()
                {
                    Description = "Autoriza��o via token.",
                    Name = "Token",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Scheme = "http,https",
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Token"
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Token",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Token"
                            },
                         },
                         new string[] {}
                     }
                });

                c.DocumentFilter<ExibicaoMetodoFilter>();

                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //string xmlFilenameDominio = "Dominio.xml";
                //string caminhoXmlDominio = Environment.CurrentDirectory.Replace("SGT.WebService.REST", "Dominio\\");
                c.IncludeXmlComments(Utilidades.IO.FileStorageService.Storage.Combine(AppContext.BaseDirectory, xmlFilename));
                //c.IncludeXmlComments(Utilidades.IO.FileStorageService.Storage.Combine(caminhoXmlDominio, xmlFilenameDominio));
                c.CustomSchemaIds(x => x.FullName);
                c.SchemaFilter<AutoRestSchemaFilter>();
            });
            services.AddMvc().AddXmlSerializerFormatters();

            services.AddHttpContextAccessor();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            ConfigureMongoDb(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseConfiguracaoSwagger();
            app.UseSwagger(options => { });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", "Multisoftware API"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCustomHttpLogging();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureMongoDb(IServiceCollection services)
        {
            services.AddSingleton<ITenantService, TenantService>();

            services
                .AdicionarMongoDb()
                .AdicionarRepositoriosMongoDb();

            ConfigureHangfire(services);

            services.AddScoped<Dominio.Interfaces.ProcessadorTarefas.IAdicionarRequestAssincrono, Servicos.ProcessadorTarefas.AdicionarRequestAssincrono>();
        }

        private void ConfigureHangfire(IServiceCollection services)
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

                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
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

            // NÃO adicionar AddHangfireServer() - apenas cliente para enfileirar jobs
        }

        private class HangfireMongoConfig
        {
            public MongoClient MongoClient { get; set; }
            public string DatabaseName { get; set; }
        }
    }
}
