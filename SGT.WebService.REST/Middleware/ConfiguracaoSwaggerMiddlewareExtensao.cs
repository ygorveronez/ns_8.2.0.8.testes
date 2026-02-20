using Microsoft.AspNetCore.Builder;

namespace SGT.WebService.REST.Middleware
{
    public static class ConfiguracaoSwaggerMiddlewareExtensao
    {
        public static IApplicationBuilder UseConfiguracaoSwagger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ConfiguracaoSwaggerMiddleware>();
        }
    }
}
