using EmissaoCTe.WebAdmin.Class;
using System;
using System.Web;
using System.Web.Security;

namespace EmissaoCTe.WebAdmin
{
    public partial class LoginSSOCallback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar se é um POST com dados SAML
            if (Request.HttpMethod != "POST" || string.IsNullOrWhiteSpace(Request.Form["SAMLResponse"]))
            {
                // Se não é POST com SAML, redireciona para o login
                Response.Redirect("~/Logon.aspx?errorMessage=" + HttpUtility.UrlEncode("Acesso inválido à página de callback SSO."), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            ProcessarRespostaSAML();
        }

        private void ProcessarRespostaSAML()
        {
            try
            {
                string samlResponse = Request.Form["SAMLResponse"];

                if (string.IsNullOrWhiteSpace(samlResponse))
                {
                    RedirecionarComErro("Resposta SAML não encontrada.");
                    return;
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno = 
                    SSOAuthenticationHelper.ValidarConfiguracaoSSO();

                if (configuracaoSSOInterno == null || !configuracaoSSOInterno.Ativo)
                {
                    RedirecionarComErro("Configuração SSO não está ativa.");
                    return;
                }

                DadosUsuarioSAML dadosUsuario = SSOAuthenticationHelper.ProcessarRespostaSAML(samlResponse, configuracaoSSOInterno);

                if (!dadosUsuario.Sucesso)
                {
                    RedirecionarComErro($"Erro ao validar SAML: {dadosUsuario.MensagemErro}");
                    return;
                }

                ResultadoAutenticacaoSSO resultadoAutenticacao = 
                    SSOAuthenticationHelper.AutenticarUsuarioSSO(dadosUsuario.Email, Session);

                if (!resultadoAutenticacao.Sucesso)
                {
                    RedirecionarComErro($"Erro de autenticação: {resultadoAutenticacao.MensagemErro}");
                    return;
                }

                SSOAuthenticationHelper.SetarPermissoesUsuario(resultadoAutenticacao.Usuario, Cache);

                FormsAuthentication.RedirectFromLoginPage(resultadoAutenticacao.Usuario.Nome, false);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                string mensagemDetalhada = $"Falha no processamento SSO: {ex.Message}";
                if (ex.InnerException != null)
                {
                    mensagemDetalhada += $" | Detalhes: {ex.InnerException.Message}";
                }
                RedirecionarComErro(mensagemDetalhada);
            }
        }

        private void RedirecionarComErro(string mensagemErro)
        {
            Response.Redirect("~/Logon.aspx?errorMessage=" + HttpUtility.UrlEncode(mensagemErro), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}

