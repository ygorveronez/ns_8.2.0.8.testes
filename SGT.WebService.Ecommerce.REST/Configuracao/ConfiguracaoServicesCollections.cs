using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

namespace SGT.WebService.Ecommerce.REST.Configuracao
{
    public static class ConfiguracaoServicesCollections
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce", Version = "v1" });

                c.AddSecurityDefinition("X-API-KEY", new OpenApiSecurityScheme()
                {
                    Description = "Autorização via token.",
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Scheme = "X-API-KEY",
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "X-API-KEY"
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "X-API-KEY",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-API-KEY"
                            },
                         },
                         new string[] {}
                     }
                });
            });

            return services;
        }

        public static IServiceCollection AddApiToken(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "X-API-KEY";
                options.DefaultChallengeScheme = "X-API-KEY";
            }).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("X-API-KEY", options => { });

            return services;
        }

        public static IServiceCollection AddFiltros(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            return services;
        }       
    }
}