using MediatR;
using SGT.WebAdmin.Notifications;
using SGT.WebAdmin.Handlers;
using SGT.WebAdmin.Services.Interfaces;
using SGT.WebAdmin.Services;

namespace SGT.WebAdmin.Extensions
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
        {
            // Adicionar MediatR com assembly atual
            services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            });

            // Registrar serviços
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}