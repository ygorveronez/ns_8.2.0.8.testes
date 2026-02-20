using Newtonsoft.Json;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGTAdmin.Controllers
{
	public class JsonpResult : IActionResult
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

		public Task ExecuteResultAsync(ActionContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			HttpResponse response = context.HttpContext.Response;
			response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

			if (ContentEncoding != null)
			{
				response.Headers["Content-Encoding"] = ContentEncoding.WebName;
			}

			HttpRequest request = context.HttpContext.Request;
			var callback = request.Query["callback"].ToString();

			callback = System.Net.WebUtility.HtmlEncode(callback);

			var json = JsonConvert.SerializeObject(this);
			var responseText = !string.IsNullOrWhiteSpace(callback) ? $"{callback}({json});" : json;

			if (ContentEncoding != null)
				return response.WriteAsync(responseText, ContentEncoding);
			else
				return response.WriteAsync(responseText);
		}

		//public void ExecuteResult(ControllerContext context)
		//{
		//	if (context == null) throw new ArgumentNullException("context");

		//	HttpResponse response = context.HttpContext.Response;
		//	//response.ContentType = "application/json";

		//	//response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
		//	//response.ContentEncoding 
		//	if (ContentEncoding != null) response.ContentEncoding = ContentEncoding;

		//	HttpRequest request = context.HttpContext.Request;

		//	var callback = request.Params("callback") ?? "";

		//	callback = HttpUtility.HtmlEncode(callback);

		//	response.Write(string.Format(!string.IsNullOrWhiteSpace(callback) ? "{0}({1});" : "{0}{1}", callback, JsonConvert.SerializeObject(this)));
		//}

		#endregion
	}
}
