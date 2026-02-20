using EmissaoCTe.WebAdmin.Class;
using System;
using System.Web;

namespace EmissaoCTe.WebAdmin
{
    public partial class LoginInterno : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno configuracaoSSOInterno = 
                    SSOAuthenticationHelper.ValidarConfiguracaoSSO();

                if (configuracaoSSOInterno == null || !configuracaoSSOInterno.Ativo)
                {
                    Response.Redirect("~/Logon.aspx?errorMessage=" + HttpUtility.UrlEncode("SSO não configurado ou inativo."), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                string urlRedirecionamento = SSOAuthenticationHelper.IniciarFluxoSSO(configuracaoSSOInterno, Request);

                if (string.IsNullOrWhiteSpace(urlRedirecionamento))
                {
                    Response.Redirect("~/Logon.aspx?errorMessage=" + HttpUtility.UrlEncode("Erro ao iniciar fluxo SSO."), false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Response.Redirect(urlRedirecionamento, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Response.Redirect("~/Logon.aspx?errorMessage=" + HttpUtility.UrlEncode("Erro ao processar login SSO."), false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}

