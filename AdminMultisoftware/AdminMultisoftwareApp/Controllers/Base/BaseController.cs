using AdminMultisoftware.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers
{
    public class BaseController : Controller
    {
        #region Atributos

        private AdminMultisoftware.Dominio.ObjetosDeValor.Auditoria.Auditado _auditado;
        private AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario _usuario;

        #endregion Atributos

        #region Propriedades

        public AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario Usuario
        {
            get
            {
                AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
                AdminMultisoftware.Repositorio.Pessoas.Usuario repUsuario = new AdminMultisoftware.Repositorio.Pessoas.Usuario(unitOfWork);
                if (this._usuario == null && Session["IdUsuario"] != null && Session["GrupoRedmine"] == null)
                {

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario usuario = repUsuario.BuscarPorCodigo((int)Session["IdUsuario"]);

                    this._usuario = usuario;
                }
                else
                {
                    this._usuario = repUsuario.BuscarPrimeiro();
                    if ((string)Session["NomeUsuario"] != null) 
                        this._usuario.Nome = (string)Session["NomeUsuario"];
                }

                return this._usuario;
            }
            set
            {
                this._usuario = value;
            }
        }

        public AdminMultisoftware.Dominio.ObjetosDeValor.Auditoria.Auditado Auditado
        {
            get
            {
                if (this._auditado == null)
                {
                    this._auditado = new AdminMultisoftware.Dominio.ObjetosDeValor.Auditoria.Auditado
                    {
                        TipoAuditado = AdminMultisoftware.Dominio.Enumeradores.TipoAuditadoAdmin.Usuario,
                        Usuario = Usuario,
                        Texto = ""
                    };
                }

                return _auditado;
            }
        }

        #endregion
        #region Métodos

        public FileStreamResult Arquivo(System.IO.Stream fileStream, string contentType, string fileDownloadName)
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

        public bool ExcessaoPorPossuirDependeciasNoBanco(Exception ex)
        {
            if (ex.Message == "O registro possui dependências e não pode ser excluido.")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public JsonResult Json<T>(T dados, bool sucesso, string erro, string[] campos, long totalRegistros)
        {
            API.Retorno<T> Ret = new API.Retorno<T>(dados);
            Ret.Sucesso = sucesso;
            Ret.Erro = erro;
            Ret.Campos = campos;
            Ret.TotalRegistros = totalRegistros;
            API.JsonPResult retorno = new API.JsonPResult();
            retorno.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            retorno.Data = Ret;
            return retorno;
        }
        #endregion
    }
}