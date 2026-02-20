using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using EmissaoCTe.API.Models;
using System.IO;

namespace EmissaoCTe.API
{
    [Controllers.Compress]
    public class ApiController : Controller
    {
        public ApiController()
        {
        }

        protected int IdUsuario
        {
            get
            {
                if (Session["IdUsuario"] != null)
                    return (int)Session["IdUsuario"];
                else
                    return 0;
            }
        }

        protected int IdUsuarioAdministrativo
        {
            get
            {
                if (Session["IdUsuarioAdmin"] != null)
                    return (int)Session["IdUsuarioAdmin"];
                else
                    return 0;
            }
        }

        private Dominio.Entidades.Usuario _UsuarioLogado;
        private Dominio.Entidades.Usuario BuscarUsuario()
        {
            if (this._UsuarioLogado == null)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Controllers.Conexao.StringConexao);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                this._UsuarioLogado = repositorioUsuario.BuscarPorCodigoFetchEmpresaConfiguracao(this.IdUsuario);//BuscarPorCodigo(this.IdUsuario);
            }

            if (this._UsuarioLogado == null)
                throw new SessaoExpiradaException("Sessão expirada.");

            return this._UsuarioLogado;
        }

        private Dominio.Entidades.Usuario _UsuarioAdministradorLogado;
        private Dominio.Entidades.Usuario BuscarUsuarioAdministrativo()
        {
            if (this._UsuarioAdministradorLogado == null)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Controllers.Conexao.StringConexao);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                this._UsuarioAdministradorLogado = repositorioUsuario.BuscarPorCodigo(this.IdUsuarioAdministrativo);
            }

            return this._UsuarioAdministradorLogado;
        }

        public List<Dominio.ObjetosDeValor.PaginaUsuario> BuscarPaginasUsuario()
        {
            string key = string.Concat("CTeUserPages_", this.IdUsuario);

            List<Dominio.ObjetosDeValor.PaginaUsuario> paginasUsuario = null;

            object cachePaginas = HttpContext.Cache.Get(key);

            if (cachePaginas != null && cachePaginas.GetType() == typeof(List<Dominio.ObjetosDeValor.PaginaUsuario>))
                paginasUsuario = (List<Dominio.ObjetosDeValor.PaginaUsuario>)cachePaginas;
            else
                HttpContext.Cache.Remove(key);

            if (paginasUsuario == null || paginasUsuario.Count <= 0)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Controllers.Conexao.StringConexao);
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);

                List<Dominio.Entidades.PaginaUsuario> paginas = repPaginaUsuario.BuscarPorUsuario(this.IdUsuario);

                paginasUsuario = ObterObjetosPaginas(paginas);

                HttpContext.Cache.Add(key, paginasUsuario, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }
            return paginasUsuario;
        }

        private List<Dominio.ObjetosDeValor.PaginaUsuario> ObterObjetosPaginas(List<Dominio.Entidades.PaginaUsuario> paginas)
        {
            List<Dominio.ObjetosDeValor.PaginaUsuario> paginasObj = new List<Dominio.ObjetosDeValor.PaginaUsuario>();

            foreach (Dominio.Entidades.PaginaUsuario paginaUsuario in paginas)
            {
                paginasObj.Add(new Dominio.ObjetosDeValor.PaginaUsuario()
                {
                    Pagina = new Dominio.ObjetosDeValor.Pagina()
                    {
                        Descricao = paginaUsuario.Pagina.Descricao,
                        Formulario = paginaUsuario.Pagina.Formulario,
                        Icone = paginaUsuario.Pagina.Icone,
                        Menu = paginaUsuario.Pagina.Menu,
                        MenuApp = ObterObjetoMenu(paginaUsuario.Pagina.MenuApp),
                        MostraNoMenu = paginaUsuario.Pagina.MostraNoMenu,
                        Status = paginaUsuario.Pagina.Status
                    },
                    PermissaoDeAcesso = paginaUsuario.PermissaoDeAcesso,
                    PermissaoDeAlteracao = paginaUsuario.PermissaoDeAlteracao,
                    PermissaoDeDelecao = paginaUsuario.PermissaoDeDelecao,
                    PermissaoDeInclusao = paginaUsuario.PermissaoDeInclusao
                });
            }

            return paginasObj;
        }

        private Dominio.ObjetosDeValor.MenuApp ObterObjetoMenu(Dominio.Entidades.Menu menu)
        {
            if (menu != null)
            {
                Dominio.ObjetosDeValor.MenuApp menuObj = new Dominio.ObjetosDeValor.MenuApp()
                {
                    Codigo = menu.Codigo,
                    Descricao = menu.Descricao,
                    MenuPai = this.ObterObjetoMenu(menu.MenuPai)
                };

                return menuObj;
            }

            return null;
        }


        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _Auditado;
        public Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado
        {
            get
            {
                if (this._Auditado == null)
                {
                    this._Auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                        Usuario = UsuarioAdministrativo ?? Usuario,
                        Empresa = EmpresaUsuario,
                        Texto = ""
                    };
                }
                return _Auditado;
            }
        }

        protected Dominio.Entidades.Usuario Usuario
        {
            get
            {
                if (_UsuarioLogado == null)
                    _UsuarioLogado = BuscarUsuario();

                return _UsuarioLogado;
            }
        }

        private Dominio.Entidades.Usuario _usuarioAdministrativo;
        protected Dominio.Entidades.Usuario UsuarioAdministrativo
        {
            get
            {
               if (_usuarioAdministrativo == null)
                    _usuarioAdministrativo = BuscarUsuarioAdministrativo();

                return _usuarioAdministrativo;
            }
        }

        private Dominio.Entidades.Empresa _empresaUsuario;
        protected Dominio.Entidades.Empresa EmpresaUsuario
        {
            get
            {
                if (_empresaUsuario == null)
                    _empresaUsuario = this.BuscarUsuario().Empresa;

                return _empresaUsuario;
            }
        }

        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (this.Session["IdUsuario"] == null || !this.User.Identity.IsAuthenticated)
            {
                filterContext.Result = Json(new Models.SessaoExpiradaException("Sessão Expirada"));
            }
            base.OnAuthorization(filterContext);
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
