using Microsoft.AspNetCore.Builder;
using System;

namespace SGT.WebService.REST.Middleware
{
    public static class ProcessarRequisicaoExtensao
    {
        public static IApplicationBuilder UseCustomHttpLogging(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.UseMiddleware<ProcessarRequisicao>();
            return builder;
        }
    }
}
