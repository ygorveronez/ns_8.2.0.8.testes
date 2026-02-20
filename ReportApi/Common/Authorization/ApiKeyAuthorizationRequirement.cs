using System;
using System.Linq;
using System.Web.Mvc;

namespace ReportApi.Common.Authorization
{
    public class ApiKeyAuthorizationRequirement : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var headers = httpContext.Request.Headers;
            if (headers.AllKeys.Contains("x-apikey"))
            {
                var key = headers.Get("x-apikey");
                //TODO: Ideal e colocar por cliente configurado via banco na base adminmultisoftware
                // Apos isso deve passar p o request qual cliente 'e para poder conectar na base correta
                if (key == "9f8d29af-f0b5-41ac-81f0-a3fae7a05801")
                {
                    return true;
                }
            }
            return false;
        }



        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult("Você não tem permissão para acessar este recurso.");
        }
    }
}
