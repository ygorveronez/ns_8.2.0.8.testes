using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers
{
    public class JsonpResult : ActionResult
    {
        #region Propriedades

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        public bool Success { get; set; }

        public bool Authorized { get; set; }

        public string RedirectURL { get; set; }

        public string Msg { get; set; }

        public int QuantidadeRegistros { get; set; }

        #endregion

        #region Construtores

        public JsonpResult()
        {
        }

        public JsonpResult(object data)
        {
            this.Data = data;
            this.Success = true;
            this.Authorized = true;
        }

        public JsonpResult(object data, int quantidadeRegistros)
        {
            this.Data = data;
            this.Success = true;
            this.QuantidadeRegistros = quantidadeRegistros;
            this.Authorized = true;
        }

        public JsonpResult(bool status, string mensagem)
        {
            this.Success = status;
            this.Msg = mensagem;
            this.Authorized = true;
        }

        public JsonpResult(object data, bool status, string mensagem)
        {
            this.Data = data;
            this.Success = status;
            this.Msg = mensagem;
            this.Authorized = true;
        }

        public JsonpResult(object data, bool status, string mensagem, string contentType)
        {
            this.Data = data;
            this.Success = status;
            this.Msg = mensagem;
            this.ContentType = contentType;
            this.Authorized = true;
        }

        #endregion

        #region Métodos

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            HttpRequestBase request = context.HttpContext.Request;

            var callback = request.Params["callback"] ?? "";

            response.Write(string.Format(!string.IsNullOrWhiteSpace(callback) ? "{0}({1});" : "{0}{1}", callback, JsonConvert.SerializeObject(this)));
        }

        #endregion
    }
}