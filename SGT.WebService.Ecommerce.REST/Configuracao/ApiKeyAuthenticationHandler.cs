using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SGT.WebService.Ecommerce.REST.Configuracao
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected readonly IConfiguration _configuration;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
            : base(options, logger, encoder, clock)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                string? apiKey = Request.Headers["X-API-KEY"].FirstOrDefault();

                if (string.IsNullOrEmpty(apiKey))
                    return AuthenticateResult.Fail("Token inválido.");

                bool tokenValido = ValidarToken();

                if (!tokenValido)
                    return AuthenticateResult.Fail("Token inválido.");

                List<Claim> claims = new List<Claim>
                {
                    new Claim("Token", apiKey)
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Token inválido.");
            }
        }

        protected bool ValidarToken()
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Base.Conexao.StringConexao(Request, _memoryCache, _configuration, _webHostEnvironment), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            Dominio.Entidades.WebService.Integradora integradora;

            string token = Request.Headers["X-API-KEY"].FirstOrDefault() ?? "";

            Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork);
            integradora = repIntegradora.BuscarPorToken(token);

            if (integradora?.Ativo ?? false)
                return true;

            Servicos.Log.TratarErro("Token " + token + " inválido.");
            return false;
        }

    }
}
