using System;
using System.Web;
using System.Web.Mvc;
using EmissaoCTe.API.Models;
using System.IO;

namespace EmissaoCTe.API
{
    public class ApiSemAutenticacaoController : Controller
    {
        public ApiSemAutenticacaoController()
        {
        }

        public JsonResult Json(Exception erro)
        {
            Retorno<string> Ret = new Retorno<string>();
            Ret.Sucesso = false;
            Ret.Erro = erro.Message;
            JsonPResult retorno = new JsonPResult();
            retorno.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            retorno.Data = Ret;
            return retorno;
        }

        public JsonResult Json(SessaoExpiradaException erro)
        {
            Retorno<string> Ret = new Retorno<string>();
            Ret.Sucesso = false;
            Ret.Erro = erro.Message;
            Ret.SessaoExpirada = erro.SessaoExpirada;
            JsonPResult retorno = new JsonPResult();
            retorno.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            retorno.Data = Ret;
            return retorno;
        }

        public JsonResult Json<T>(T dados, bool sucesso)
        {
            Retorno<T> Ret = new Retorno<T>();
            Ret.Sucesso = sucesso;
            Ret.Objeto = dados;
            JsonPResult retorno = new JsonPResult();
            retorno.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            retorno.Data = Ret;
            return retorno;
        }

        public JsonResult Json<T>(T dados, bool sucesso, string erro)
        {
            Retorno<T> Ret = new Retorno<T>();
            Ret.Sucesso = sucesso;
            Ret.Erro = erro;
            Ret.Objeto = dados;
            JsonPResult retorno = new JsonPResult();
            retorno.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            retorno.Data = Ret;
            return retorno;
        }

        public JsonResult Json<T>(T dados, bool sucesso, string erro, string[] campos, long totalRegistros)
        {
            Retorno<T> Ret = new Retorno<T>(dados);
            Ret.Sucesso = sucesso;
            Ret.Erro = erro;
            Ret.Campos = campos;
            Ret.TotalRegistros = totalRegistros;
            JsonPResult retorno = new JsonPResult();
            retorno.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            retorno.Data = Ret;
            return retorno;
        }

        public FileStreamResult Arquivo(Stream fileStream, string contentType, string fileDownloadName)
        {
            this.SetarCookieFileDownload();

            return File(fileStream, contentType, fileDownloadName);
        }

        public FileContentResult Arquivo(byte[] bytes, string contentType, string fileDownloadName)
        {
            this.SetarCookieFileDownload();

            return File(bytes, contentType, fileDownloadName);
        }

        private void SetarCookieFileDownload()
        {
            Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
        }
    }
}
