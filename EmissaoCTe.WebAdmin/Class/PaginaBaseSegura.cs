using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.UI.HtmlControls;

namespace EmissaoCTe.WebAdmin
{
    public class PaginaBaseSegura : PaginaBase
    {
        public PaginaBaseSegura()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        #region Manipuladores de Eventos

        override protected void OnInit(EventArgs e)
        {
            if (!this.ValidarAcesso())
                Response.Redirect("NaoAutorizado.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RenderizarMenu();
        }

        #endregion

        #region Propriedades

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

        protected Dominio.Entidades.Usuario Usuario
        {
            get
            {
                return BuscarUsuario();
            }
        }

        protected Dominio.Entidades.Empresa EmpresaUsuario
        {
            get
            {
                Dominio.Entidades.Usuario usuario = BuscarUsuario();
                return usuario.Empresa;
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Usuario BuscarUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCodigo(this.IdUsuario);

            if (usuario == null)
            {
                FormsAuthentication.SignOut();
                Response.Redirect("Logon.aspx");
            }

            return usuario;
        }

        public List<Dominio.ObjetosDeValor.PaginaUsuario> BuscarPaginasUsuario()
        {
            //string key = string.Concat("CTeUserPages_", this.IdUsuario);
            //List<Dominio.Entidades.PaginaUsuario> paginasUsuario = (List<Dominio.Entidades.PaginaUsuario>)Cache.Get(key);
            //if (paginasUsuario == null || paginasUsuario.Count <= 0)
            //{
            //    Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(Conexao.StringConexao);
            //    paginasUsuario = repPaginaUsuario.BuscarPorUsuario(this.IdUsuario);
            //    Cache.Add(key, paginasUsuario, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            //}
            //return paginasUsuario;
            string key = string.Concat("CTeUserPages_", this.IdUsuario);

            List<Dominio.ObjetosDeValor.PaginaUsuario> paginasUsuario = null;

            object cachePaginas = Cache.Get(key);

            if (cachePaginas != null && cachePaginas.GetType() == typeof(List<Dominio.ObjetosDeValor.PaginaUsuario>))
                paginasUsuario = (List<Dominio.ObjetosDeValor.PaginaUsuario>)cachePaginas;
            else
                Cache.Remove(key);

            if (paginasUsuario == null || paginasUsuario.Count <= 0)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);

                List<Dominio.Entidades.PaginaUsuario> paginas = repPaginaUsuario.BuscarPorUsuario(this.IdUsuario);

                paginasUsuario = ObterObjetosPaginas(paginas);

                Cache.Add(key, paginasUsuario, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
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

        private bool ValidarAcesso()
        {
            string pagina = Request.FilePath.Split('/').LastOrDefault().ToLower();
            if (pagina.Equals("default.aspx") ||
                pagina.Equals("naoautorizado.aspx") ||
                pagina.Equals("ajuda.aspx"))
            {
                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.PaginaUsuario permissao = (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals(pagina) select obj).FirstOrDefault();
                if (permissao != null)
                    if (permissao.PermissaoDeAcesso == "A")
                        return true;
                return false;
            }
        }

        private void RenderizarMenu()
        {
            HtmlGenericControl ulPrincipal = (HtmlGenericControl)this.Page.Master.FindControl("navigation");
            HtmlGenericControl liFilho = new HtmlGenericControl("li");
            liFilho.Style.Add("padding-top", "0");
            liFilho.Style.Add("padding-bottom", "0");
            liFilho.Style.Add("float", "left");
            liFilho.Style.Add("clear", "none");
            HtmlAnchor a = new HtmlAnchor();
            a.InnerText = "Home";
            a.HRef = "Default.aspx";
            liFilho.Controls.Add(a);
            ulPrincipal.Controls.Add(liFilho);
            var paginasUsuario = (from obj in this.BuscarPaginasUsuario() where obj.PermissaoDeAcesso == "A" && obj.Pagina.MostraNoMenu orderby obj.Pagina.Menu ascending, obj.Pagina.Descricao select obj).ToList();
            List<string> menus = (from obj in paginasUsuario select obj.Pagina.Menu).Distinct().ToList();
            foreach (string menu in menus)
            {
                var paginas = (from obj in paginasUsuario where obj.Pagina.Menu == menu select obj).ToList();
                if (string.IsNullOrWhiteSpace(menu))
                {
                    foreach (Dominio.ObjetosDeValor.PaginaUsuario pagina in paginas)
                    {
                        liFilho = new HtmlGenericControl("li");
                        liFilho.Style.Add("padding-top", "0");
                        liFilho.Style.Add("padding-bottom", "0");
                        liFilho.Style.Add("float", "left");
                        liFilho.Style.Add("clear", "none");
                        a = new HtmlAnchor();
                        a.InnerText = pagina.Pagina.Descricao;
                        a.HRef = pagina.Pagina.Formulario;
                        liFilho.Controls.Add(a);
                        ulPrincipal.Controls.Add(liFilho);
                    }
                }
                else
                {
                    liFilho = new HtmlGenericControl("li");
                    liFilho.Style.Add("padding-top", "0");
                    liFilho.Style.Add("padding-bottom", "0");
                    liFilho.Style.Add("float", "left");
                    liFilho.Style.Add("clear", "none");
                    a = new HtmlAnchor();
                    a.Attributes.Add("class", "sf-with-ul");
                    a.InnerText = menu;
                    a.HRef = "#";
                    liFilho.Controls.Add(a);
                    HtmlGenericControl ulFilho = new HtmlGenericControl("ul");
                    foreach (Dominio.ObjetosDeValor.PaginaUsuario pagina in paginas)
                    {
                        HtmlGenericControl liFilho2 = new HtmlGenericControl("li");
                        a = new HtmlAnchor();
                        a.InnerText = pagina.Pagina.Descricao;
                        a.HRef = pagina.Pagina.Formulario;
                        liFilho2.Controls.Add(a);
                        ulFilho.Controls.Add(liFilho2);
                    }
                    liFilho.Controls.Add(ulFilho);
                    ulPrincipal.Controls.Add(liFilho);
                }
            }
        }

        #endregion

    }
}