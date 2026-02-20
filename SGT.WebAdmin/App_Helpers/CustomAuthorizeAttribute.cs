using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SGT.WebAdmin.Controllers;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace SGT.WebAdmin
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _acoesLiberadasParaPermissaoSomenteLeitura;
        private readonly string[] _caminhosFormulariosPermitidos;
        private Conexao _conexao;

        public CustomAuthorizeAttribute(params string[] caminhosFormulariosPermitidos) : this(acoesLiberadasParaPermissaoSomenteLeitura: [], caminhosFormulariosPermitidos: caminhosFormulariosPermitidos) { }

        public CustomAuthorizeAttribute(string[] acoesLiberadasParaPermissaoSomenteLeitura, params string[] caminhosFormulariosPermitidos)
        {
            _acoesLiberadasParaPermissaoSomenteLeitura = acoesLiberadasParaPermissaoSomenteLeitura;
            _caminhosFormulariosPermitidos = caminhosFormulariosPermitidos;
        }

        public CustomAuthorizeAttribute(string caminhoFormulariosPermitido)
        {
            _caminhosFormulariosPermitidos = new[] { caminhoFormulariosPermitido };
        }

        private void GravarLogAcesso(string descricao, HttpContext httpContext)
        {
            try
            {
                var mensagem = $"descrição: {descricao} usuário: {(httpContext?.User.GetCodigoUsuario() ?? "")} url: {httpContext?.Request?.Path}";
                Servicos.Log.TratarErro(mensagem, "LogAcesso.txt");
            }
            catch
            {
                // ignored
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
#if DEBUG
            return;
#endif

            HttpContext httpContext = context.HttpContext;

            if (_conexao == null)
                _conexao = httpContext.RequestServices.GetRequiredService<Conexao>();

            if (IsHomePageRequest())
            {
                SetSomenteLeituraHeader(httpContext, false);
                return;
            }

            if (IsUserNotAuthenticated(httpContext))
            {
                if (IsAnonymousPageRequest())
                {
                    SetSomenteLeituraHeader(httpContext, false);
                    return;
                }

                LogAndDenyAccess("Usuário não definido", httpContext, context);
                return;
            }

            using var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            var usuario = GetUsuario(httpContext, unitOfWork);

            if (IsPasswordChangeRequired(usuario))
            {
                if (IsRequiredPasswordChangePageRequest())
                {
                    SetSomenteLeituraHeader(httpContext, false);
                    return;
                }
                else
                {
                    LogAndDenyAccess("Alteração de senha", httpContext, context);
                    return;
                }
            }

            if (IsAccessPolicyViolated(usuario, httpContext, unitOfWork))
            {
                LogAndDenyAccess("Politica de senha", httpContext, context);
                return;
            }

            if (IsConfigurationPageRequest() && usuario.UsuarioAdministrador)
            {
                SetSomenteLeituraHeader(httpContext, false);
                return;
            }

            if (IsMultisoftwareUserAndPage(usuario))
            {
                SetSomenteLeituraHeader(httpContext, false);
                return;
            }

            var formulariosEmCache = GetFormulariosEmCache();

            if (IsUserAdminWithSingleFormAccess(usuario) && IsBIReportAccessAllowed(httpContext, formulariosEmCache))
            {
                SetSomenteLeituraHeader(httpContext, false);
                return;
            }

            var (authorized, funcionarioFormulario) = IsUserAuthorizedToAccessForm(usuario, formulariosEmCache, context);
            if (authorized)
            {
                SetSomenteLeituraHeader(httpContext, funcionarioFormulario?.SomenteLeitura ?? false);
                return;
            }

            LogAndDenyAccess("Formulário sem liberação", httpContext, context);
        }

        private string[] GetCaminhoFormulariosPermitidosSanitized(AuthorizationFilterContext context, ref bool validarAcoesLiberadasParaPermissaoSomenteLeitura)
        {
            string[] caminhosFormulariosPermitidos = new string[_caminhosFormulariosPermitidos.Length];
            string nomeAcao = context.RouteData.Values["action"]?.ToString();

            for (int i = 0; i < this._caminhosFormulariosPermitidos.Length; i++)
            {
                string caminhoFormularioPermitido = this._caminhosFormulariosPermitidos[i];

                if (caminhoFormularioPermitido == "BusinessIntelligence/BI/Report")
                {
                    string queryString = context.HttpContext.Request.QueryString.ToString();

                    if (!string.IsNullOrWhiteSpace(queryString))
                        caminhoFormularioPermitido = $"{caminhoFormularioPermitido}{queryString}";
                }

                string[] partesCaminhoFormularioPermitido = caminhoFormularioPermitido.Split('/');
                string nomeAcaoCaminhoFormularioPermitido = partesCaminhoFormularioPermitido[partesCaminhoFormularioPermitido.Length - 1];

                if (nomeAcao == nomeAcaoCaminhoFormularioPermitido)
                    validarAcoesLiberadasParaPermissaoSomenteLeitura = false;

                caminhosFormulariosPermitidos[i] = caminhoFormularioPermitido;
            }

            return caminhosFormulariosPermitidos;
        }

        private bool IsHomePageRequest()
        {
            return Array.Exists(_caminhosFormulariosPermitidos, o => o == "Home");
        }

        private bool IsAnonymousPageRequest()
        {
            return Array.Exists(_caminhosFormulariosPermitidos, o => o == "Cargas/ImpressaoLoteCarga");
        }

        private bool IsRequiredPasswordChangePageRequest()
        {
            return Array.Exists(_caminhosFormulariosPermitidos, o => o == "Login/AtualizacaoSenhaObrigatoria");
        }

        private bool IsUserNotAuthenticated(HttpContext httpContext)
        {
            int? codigoUsuario = httpContext.User.Claims.FirstOrDefault(o => o.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value.ToInt();

            return httpContext.User == null || !httpContext.User.Identity.IsAuthenticated || !codigoUsuario.HasValue || codigoUsuario.Value == 0;
        }

        private void LogAndDenyAccess(string logMessage, HttpContext httpContext, AuthorizationFilterContext context)
        {
            GravarLogAcesso(logMessage, httpContext);
            context.Result = new UnauthorizedResult();
        }

        private void SetSomenteLeituraHeader(HttpContext httpContext, bool isSomenteLeitura)
        {
            httpContext.Response.Headers.Append("SomenteLeitura", isSomenteLeitura.ToString().ToLower());
        }

        private Dominio.Entidades.Usuario GetUsuario(HttpContext httpContext, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            int? codigoUsuario = httpContext.User.Claims.FirstOrDefault(o => o.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value.ToInt();

            if (!codigoUsuario.HasValue || codigoUsuario.Value == 0)
                return null;

            return repositorioUsuario.BuscarPorCodigo(codigoUsuario.Value);
        }

        private bool IsPasswordChangeRequired(Dominio.Entidades.Usuario usuario)
        {
            return usuario is { AlterarSenhaAcesso: true, UsuarioMultisoftware: false, UsuarioAtendimento: false, UsuarioCallCenter: false };
        }

        private bool IsAccessPolicyViolated(Dominio.Entidades.Usuario usuario, HttpContext httpContext, Repositorio.UnitOfWork unitOfWork)
        {
            if (usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Embarcador || usuario.UsuarioMultisoftware || usuario.UsuarioAtendimento || usuario.UsuarioCallCenter)
            {
                return false;
            }

            var repositorioPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
            var politicaSenha = repositorioPoliticaSenha.BuscarPoliticaPadrao();

            if (politicaSenha == null)
            {
                return false;
            }

            string guidUser = httpContext.User.Claims.FirstOrDefault(o => o.Type == "GuidUser")?.Value;

            if (politicaSenha.NaoPermitirAcessosSimultaneos && guidUser != usuario.Session)
            {
                return true;
            }

            string loginSSO = httpContext.User.Claims.FirstOrDefault(o => o.Type == "LoginSSO")?.Value;
            bool pularVerificacaoPrazo = loginSSO.ToBool();

            return !pularVerificacaoPrazo && politicaSenha.PrazoExpiraSenha > 0 &&
                   (!usuario.DataUltimaAlteracaoSenhaObrigatoria.HasValue || usuario.DataUltimaAlteracaoSenhaObrigatoria.Value.AddDays(politicaSenha.PrazoExpiraSenha) < DateTime.Now);
        }

        private bool IsConfigurationPageRequest()
        {
            return _caminhosFormulariosPermitidos.Any(caminhoFormulario =>
                caminhoFormulario.ToLower() == "configuracoes/configuracao" ||
                caminhoFormulario.ToLower() == "configuracoes/liberacaointegracao" ||
                caminhoFormulario.ToLower() == "configuracoes/configuracaoemissordocumento" ||
                caminhoFormulario.ToLower() == "configuracoes/configuracaoorquestradorfila" ||
                caminhoFormulario.ToLower() == "configuracoes/controlethread"
            );
        }

        private bool IsUserAdminWithSingleFormAccess(Dominio.Entidades.Usuario usuario)
        {
            return usuario.UsuarioAdministrador && _caminhosFormulariosPermitidos.Length == 1;
        }

        private bool IsMultisoftwareUserAndPage(Dominio.Entidades.Usuario usuario)
        {
            List<string> telasUsuariosMulti = new List<string>() { "configuracoes/execucaocomandos", "configuracoes/script" };
            if (_caminhosFormulariosPermitidos.Any(o => telasUsuariosMulti.Contains(o.ToLower())) && (usuario.UsuarioMultisoftware || usuario.UsuarioAtendimento || usuario.UsuarioCallCenter))
            {
                return true;
            }

            return false;
        }

        private bool IsBIReportAccessAllowed(HttpContext httpContext, List<WebAdmin.Controllers.CacheFormulario> formulariosEmCache)
        {
            string caminhoFormularioPermitido = _caminhosFormulariosPermitidos[0];
            if (caminhoFormularioPermitido == "BusinessIntelligence/BI/Report")
            {
                string queryString = httpContext.Request.QueryString.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(queryString))
                {
                    caminhoFormularioPermitido = $"{caminhoFormularioPermitido}{queryString}";
                }

                if (formulariosEmCache.Any(o => o.CaminhoFormulario == caminhoFormularioPermitido))
                {
                    return true;
                }

                if (caminhoFormularioPermitido.Contains("UtilizaMenu=1"))
                {
                    return true;
                }
            }

            return false;
        }

        private List<WebAdmin.Controllers.CacheFormulario> GetFormulariosEmCache()
        {
            var controllerModulo = new WebAdmin.Controllers.Modulos(_conexao);
            var formulariosEmCache = controllerModulo.RetornarFormulariosEmCache();

            if (formulariosEmCache.Count <= 0)
            {
                formulariosEmCache = controllerModulo.RetornarFormulariosEmCache(true);
            }

            return formulariosEmCache;
        }

        private bool IsAllowAuthenticateEndpoint(AuthorizationFilterContext context)
        {
            return context.ActionDescriptor.EndpointMetadata.Any(o => o is AllowAuthenticateAttribute);
        }

        private bool IsAllowAnonymousEndpoint(AuthorizationFilterContext context)
        {
            return context.ActionDescriptor.EndpointMetadata.Any(o => o is AllowAnonymousAttribute);
        }

        private (bool authorized, Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario) IsUserAuthorizedToAccessForm(Dominio.Entidades.Usuario usuario, List<WebAdmin.Controllers.CacheFormulario> formulariosEmCache, AuthorizationFilterContext context)
        {
            string nomeAcao = context.RouteData.Values["action"]?.ToString();

            if (IsAllowAnonymousEndpoint(context) ||
                IsAllowAuthenticateEndpoint(context) ||
                _caminhosFormulariosPermitidos.Length == 0 ||
                (usuario.UsuarioAdministrador && formulariosEmCache.Exists(o => _caminhosFormulariosPermitidos.Any(cfp => o.CaminhoFormulario == cfp))))
            {
                return (true, null);
            }

            bool validarAcoesLiberadasParaPermissaoSomenteLeitura = true;
            string[] caminhosFormulariosPermitidos = GetCaminhoFormulariosPermitidosSanitized(context, ref validarAcoesLiberadasParaPermissaoSomenteLeitura);

            List<WebAdmin.Controllers.CacheFormulario> listaCacheFormulario = (from o in formulariosEmCache where caminhosFormulariosPermitidos.Contains(o.CaminhoFormulario) select o).ToList();

            for (int i = 0; i < listaCacheFormulario.Count; i++)
            {
                CacheFormulario cacheFormulario = listaCacheFormulario[i];
                bool moduloLiberado = usuario.ModulosLiberados.Contains(cacheFormulario.CacheModulo.CodigoModulo);

                if (!moduloLiberado)
                {
                    var controllerModulo = new WebAdmin.Controllers.Modulos(_conexao);
                    moduloLiberado = controllerModulo.VerificarModulosPaiLiberadoRecursivamente(cacheFormulario.CacheModulo, usuario.ModulosLiberados.ToList());
                }

                if (moduloLiberado)
                {
                    return (true, null);
                }
            }

            if (IsUserAdminWithSingleFormAccess(usuario) && (_acoesLiberadasParaPermissaoSomenteLeitura?.Contains(nomeAcao.ToLowerInvariant()) ?? false))
            {
                return (true, null);
            }

            Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario formularioFuncionario = (
                from o in usuario.FormulariosLiberados
                where listaCacheFormulario.Exists(c => c.CodigoFormulario == o.CodigoFormulario)
                orderby o.SomenteLeitura ascending
                select o
            ).FirstOrDefault();

            string[] acoesLiberadas = ["pesquisa", "buscarporcodigo", "buscardadosrelatorio", "gerarrelatorio"];

            if (formularioFuncionario == null && usuario.UsuarioAdministrador)
            {
                if (acoesLiberadas.Contains(nomeAcao.ToLowerInvariant()))
                {
                    return (true, null);
                }

                if (_acoesLiberadasParaPermissaoSomenteLeitura?.Contains(nomeAcao.ToLowerInvariant()) ?? false)
                {
                    return (true, null);
                }
            }

            if (formularioFuncionario != null)
            {
                if (!formularioFuncionario.SomenteLeitura || !validarAcoesLiberadasParaPermissaoSomenteLeitura)
                    return (true, formularioFuncionario);

                if (acoesLiberadas.Contains(nomeAcao.ToLowerInvariant()))
                    return (true, formularioFuncionario);

                if (IsActionSomenteLeitura(nomeAcao))
                    return (true, formularioFuncionario);
            }

            return (false, null);
        }

        private bool IsActionSomenteLeitura(string nomeAcao)
        {
            return _acoesLiberadasParaPermissaoSomenteLeitura.Any(acao => acao.Equals(nomeAcao, StringComparison.OrdinalIgnoreCase));
        }
    }
}