using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Middleware
{
    public class ConfiguracaoSwaggerMiddleware
    {
        private readonly RequestDelegate _next;
        protected readonly IConfiguration _configuration;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        private static int _maxPoolSize = 600;

        public ConfiguracaoSwaggerMiddleware(RequestDelegate next, IConfiguration configuration, IMemoryCache memoryCache, IWebHostEnvironment webHostEnvironment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _configuration = configuration;
            _memoryCache = memoryCache;
            _webHostEnvironment = webHostEnvironment;
            _maxPoolSize = _configuration.GetValue<int>("MaxPoolSize");
        }

        public async Task InvokeAsync(HttpContext context)
        {

            Repositorio.UnitOfWork unitOfWork = null;
            try
            {
                if (!context.Request.Path.StartsWithSegments("/swagger"))
                {
                    await _next(context);

                    return;
                }

                if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    await RetornoNaoAutorizado(context);

                    return;
                }

                string authHeaderValue = authHeader.ToString();
                if (!authHeaderValue.StartsWith("Basic "))
                {
                    await RetornoNaoAutorizado(context);

                    return;
                }

                string encodedCredentials = authHeaderValue.Substring("Basic ".Length).Trim();
                string decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));

                string[] credentials = decodedCredentials.Split(':');
                if (credentials.Length != 2)
                {
                    await RetornoNaoAutorizado(context);

                    return;
                }

                unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(context.Request, _memoryCache, _configuration, _webHostEnvironment,_maxPoolSize), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

                var username = credentials[0];
                var password = credentials[1];

                var repository = new Repositorio.Embarcador.Swagger.ConfiguracaoSwagger(unitOfWork);


                if (!await repository.ValidarCredenciaisAsync(username, password))
                {
                    await RetornoNaoAutorizado(context);

                    return;
                }

                await _next(context);
            }
            catch (Exception ex) { 
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                if (unitOfWork != null)
                   await unitOfWork.DisposeAsync();
            }
        }

        private async Task  RetornoNaoAutorizado(HttpContext httpContext)
        {
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\"";
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync("Unauthorized: Invalid username or password.");
        }
    }
}
