using AdminMultisoftware.Dominio.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace SGT.WebAdmin.Controllers.Login
{
    public class LoginInternoController : SignController
    {
        #region Construtores

        public LoginInternoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAnonymous]
        public async Task<IActionResult> Index(string errorMessage = "")
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            using AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repositorioClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repositorioClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
            SGT.BackgroundWorkers.MSMQ.GetInstance().QueueItem(unitOfWork, clienteURLAcesso.Cliente.Codigo, _conexao.StringConexao, _conexao.AdminStringConexao, clienteURLAcesso.TipoServicoMultisoftware);

            EnsureLoggedOut();

            if (clienteURLAcesso.TipoServicoMultisoftware == TipoServicoMultisoftware.Fornecedor)
                ViewBag.ClasseCorBotoes = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LayoutPersonalizadoFornecedor;
            else if (
                (clienteURLAcesso.TipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe) ||
                (clienteURLAcesso.TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador) ||
                (clienteURLAcesso.TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            )
                ViewBag.GTAG = "G-W9PLBNR31T";

            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno repositorioConfiguracaoSSOInterno = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno = repositorioConfiguracaoSSOInterno.BuscarConfiguracaoPadrao();

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                if ((clienteURLAcesso.TipoServicoMultisoftware != TipoServicoMultisoftware.MultiEmbarcador) && (clienteURLAcesso.TipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS))
                    return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode("Não é possível acessar neste ambiente utilizando autenticação SSO.")}");

                if ((configuracaoSSOInterno == null) || !configuracaoSSOInterno.Ativo)
                    return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode("Não existe uma configuração de SSO ativa para acessar neste ambiente.")}");

                return RedirectExternalSignIn(configuracaoSSOInterno);
            }
            else
                ViewBag.ErroLoginSSO = errorMessage;

            ViewBag.DisplaySSO = configuracaoSSOInterno?.Display ?? "Conta nstech";

            return View("~/Views/Login/IndexInterno.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            base.SignOut();

            return Redirect("/#Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalSignIn()
        {
            using AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repositorioClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repositorioClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

            if ((clienteURLAcesso.TipoServicoMultisoftware != TipoServicoMultisoftware.MultiEmbarcador) && (clienteURLAcesso.TipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS))
                return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode("Não é possível acessar neste ambiente utilizando autenticação SSO.")}");

            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno repositorioConfiguracaoSSOInterno = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno = repositorioConfiguracaoSSOInterno.BuscarConfiguracaoPadrao();

            if ((configuracaoSSOInterno == null) || !configuracaoSSOInterno.Ativo)
                return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode("Não existe uma configuração de SSO ativa para acessar neste ambiente.")}");

            return RedirectExternalSignIn(configuracaoSSOInterno);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Saml2(string returnUrl)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno repositorioConfiguracaoSSOInterno = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSOInterno(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno = repositorioConfiguracaoSSOInterno.BuscarConfiguracaoPadrao();

            if ((configuracaoSSOInterno == null) || !configuracaoSSOInterno.Ativo)
                return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode("Não existe uma configuração de SSO ativa para acessar neste ambiente.")}");

            App_Start.Response samlResponse = new App_Start.Response();
            string extensao = System.IO.Path.GetExtension(configuracaoSSOInterno.CaminhoArquivoCertificado).ToLower();

            if (extensao.Equals(".xml"))
                samlResponse.SetMetadataXml(Utilidades.IO.FileStorageService.Storage.ReadAllText(configuracaoSSOInterno.CaminhoArquivoCertificado));
            else
                samlResponse.SetCertificateStr(Utilidades.IO.FileStorageService.Storage.ReadAllText(configuracaoSSOInterno.CaminhoArquivoCertificado));

            List<string> listValues = new List<string>();

            foreach (var key in Request.Form.Keys)
                listValues.Add($"{key} = {Request.Form[key]}");

            Servicos.Log.TratarErro(string.Join(Environment.NewLine, listValues), "saml_interno");

            samlResponse.LoadXmlFromBase64(Request.Form["SAMLResponse"]);

            string erroValidacaoSSO = string.Empty;

            if (!samlResponse.IsValid(ref erroValidacaoSSO))
            {
                if (string.IsNullOrWhiteSpace(erroValidacaoSSO))
                    erroValidacaoSSO = Localization.Resources.Login.Login.NaoFoiPossivelValidarUsuarioAutenticacaoSAML;

                return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode(erroValidacaoSSO)}");   
            }

            try
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorUsuarioAtendimento();

                if (usuario != null)
                {
                    string nome = samlResponse.GetFirstName();
                    string sobrenome = samlResponse.GetLastName();
                    string nomeCompleto = $"{nome} {sobrenome}";
                    string email = samlResponse.GetEmail();
                    List<string> grupos = samlResponse.GetCustomAttributes("http://schemas.microsoft.com/ws/2008/06/identity/claims/groups");

                    if (string.IsNullOrWhiteSpace(email))
                        email = samlResponse.GetCustomAttribute("email");

                    Dominio.ObjetosDeValor.UsuarioInterno usuarioInterno = new Dominio.ObjetosDeValor.UsuarioInterno
                    {
                        Nome = nomeCompleto,
                        Email = email,
                        Administrador = grupos.Contains(configuracaoSSOInterno.IdentificadorGrupoAdministrador)
                    };

                    base.SignIn(usuario, loginSSO: true, usuarioInterno: usuarioInterno);

                    return Redirect("/#Home");
                }

                return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode(Localization.Resources.Login.Login.AutenticacaoSAMLValidaPoremOcorreuErroIdentificar)}");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Redirect($"/LoginInterno?errorMessage={WebUtility.UrlEncode(Localization.Resources.Login.Login.OcorreuFalhaAcessarSistemaFavorTentarNovamente)}");
            }
        }

        public void ExternalSignOut()
        {
            base.SignOutExternal();
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (User.Identity.IsAuthenticated)
                Logout();
        }

        private IActionResult RedirectExternalSignIn(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno)
        {
            string samlEndpoint = string.Format(configuracaoSSOInterno.UrlAutenticacao, configuracaoSSOInterno.ClientId);

            App_Start.AuthRequest request = new App_Start.AuthRequest(
                configuracaoSSOInterno.UrlDominio,
                $"{configuracaoSSOInterno.UrlDominio}LoginInterno/Saml2"
            );

            return Redirect(request.GetRedirectUrl(samlEndpoint));
        }

        #endregion Métodos Privados
    }
}