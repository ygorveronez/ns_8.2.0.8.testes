using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.API
{
    public class JsonPResult : JsonResult
    {
        public string Callback;

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(ContentType) ? /*"application/json"*/"" : ContentType;

            if (Callback == null)
                Callback = context.HttpContext.Request.QueryString["callback"];

            if (Data != null)
            {
                string retorno = JsonConvert.SerializeObject(Data);

                response.Write(Callback + "(" + retorno + ");");
            }
        }
    }
}