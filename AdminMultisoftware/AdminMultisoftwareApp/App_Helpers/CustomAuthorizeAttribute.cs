using AdminMultisoftwareApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        #region Atributos Privados

        private readonly string[] _acoesLiberadasParaPermissaoSomenteLeitura;
        private readonly string[] _caminhosFormulariosPermitidos;

        #endregion

        #region Construtores 

        public CustomAuthorizeAttribute(params string[] caminhosFormulariosPermitidos) : this(acoesLiberadasParaPermissaoSomenteLeitura: new string[] { }, caminhosFormulariosPermitidos: caminhosFormulariosPermitidos) { }

        public CustomAuthorizeAttribute(string[] acoesLiberadasParaPermissaoSomenteLeitura, params string[] caminhosFormulariosPermitidos)
        {
            _acoesLiberadasParaPermissaoSomenteLeitura = acoesLiberadasParaPermissaoSomenteLeitura;
            _caminhosFormulariosPermitidos = caminhosFormulariosPermitidos;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
#if DEBUG
                System.Web.HttpContext.Current.Response.Headers.Add("SomenteLeitura", "false");
                return true;
#endif

                if (_caminhosFormulariosPermitidos.Any(o => o == "Home"))
                {
                    HttpContext.Current.Response.Headers.Add("SomenteLeitura", "false");
                    return true;
                }

                if (httpContext.User == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated || httpContext.Session == null || httpContext.Session["IdUsuario"] == null)
                {
                    return false;
                }

                HttpContext.Current.Response.Headers.Add("SomenteLeitura", "false");
                return true;

            }
            catch (Exception excecao)
            {
                throw;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string urlRedirect = "";

            //if ((this._allowedroles.Length > 0) && !string.IsNullOrWhiteSpace(this._allowedroles[0]))
            //  urlRedirect = this._allowedroles[0];

            filterContext.Result = new JsonpResult()
            {
                Authorized = false,
                Success = false,
                Msg = "Acesso negado! Sua sessão expirou ou você não possui permissão para este recurso!",
                RedirectURL = urlRedirect,
                ContentType = "application/x-javascript",
                ContentEncoding = System.Text.Encoding.UTF8
            };
        }

        #endregion
    }
}