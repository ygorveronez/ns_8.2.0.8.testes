using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace EmissaoCTe.WebApp
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
            this.RenderizarMenu2();
            this.SetarDadosEmpresa();
            this.SetarNomeUsuario();
            //this.SetarNomeEmpresa();
            //this.SetarEmailEmpresa();
            //this.SetarSkypeEmpresa();
            //this.SetarTelefoneEmpresa();
            //this.SetarSiteEmpresa();
        }

        #endregion

        #region Propriedades

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

        protected int IdUsuarioAdmin
        {
            get
            {
                if (Session["IdUsuarioAdmin"] != null)
                    return (int)Session["IdUsuarioAdmin"];
                else
                    return 0;
            }
        }

        protected bool ReloadPages
        {
            get
            {
                if (Session["ReloadPages"] != null)
                    return (bool)Session["ReloadPages"];
                else
                    return false;
            }
        }

        private Dominio.Entidades.Usuario _Usuario;
        protected Dominio.Entidades.Usuario Usuario
        {
            get
            {
                if (this._Usuario == null)
                    this._Usuario = ObterUsuarioPorCodigo(this.IdUsuario);

                return this._Usuario;
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
                {
                    Dominio.Entidades.Usuario usuario = BuscarUsuario();
                    _empresaUsuario = usuario.Empresa;
                }
                return _empresaUsuario;
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Usuario BuscarUsuario()
        {
            Dominio.Entidades.Usuario usuario = ObterUsuarioPorCodigo(this.IdUsuario);
            if (usuario == null)
            {
                FormsAuthentication.SignOut();
                Response.Redirect("Logon.aspx");
            }
            return usuario;
        }

        private Dominio.Entidades.Usuario _UsuarioAdministradorLogado;
        
        private Dominio.Entidades.Usuario BuscarUsuarioAdministrativo()
        {
            if (this._UsuarioAdministradorLogado == null)
                this._UsuarioAdministradorLogado = ObterUsuarioPorCodigo(this.IdUsuarioAdmin);

            return this._UsuarioAdministradorLogado;
        }

        public List<Dominio.ObjetosDeValor.PaginaUsuario> BuscarPaginasUsuario()
        {
            string key = string.Concat("CTeUserPages_", this.IdUsuario);

            List<Dominio.ObjetosDeValor.PaginaUsuario> paginasUsuario = null;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(this.IdUsuario);

            object cachePaginas = Cache.Get(key);

            if (cachePaginas != null && cachePaginas.GetType() == typeof(List<Dominio.ObjetosDeValor.PaginaUsuario>) && !ReloadPages)
                paginasUsuario = (List<Dominio.ObjetosDeValor.PaginaUsuario>)cachePaginas;
            else
            {
                Session["ReloadPages"] = null;
                Cache.Remove(key);
            }

            if (paginasUsuario == null || paginasUsuario.Count <= 0)
            {
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);

                List<Dominio.Entidades.PaginaUsuario> paginas = new List<Dominio.Entidades.PaginaUsuario>();
                
                if (usuario != null && usuario.AlterarSenhaAcesso)
                {
                    paginas.Add(repPaginaUsuario.BuscarPorUsuarioEFormulario(usuario.Codigo, "AlterarSenha.aspx"));
                }
                else if (usuario != null && usuario.Empresa.StatusFinanceiro == "B")
                {
                    paginas.Add(repPaginaUsuario.BuscarPorUsuarioEFormulario(usuario.Codigo, "AvisoEmpresaBloqueioFinanceiro.aspx"));
                }
                else if (usuario != null && IdUsuarioAdmin == 0 && Servicos.Usuario.ObrigatorioTermos(usuario, unitOfWork))
                {
                    paginas.Add(repPaginaUsuario.BuscarPorUsuarioEFormulario(usuario.Codigo, "TermosDeUso.aspx"));
                }
                else
                    paginas = repPaginaUsuario.BuscarPorUsuario(this.IdUsuario);

                paginasUsuario = ObterObjetosPaginas(paginas);

                Cache.Add(key, paginasUsuario, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }

            return paginasUsuario;
        }

        private Dominio.Entidades.Usuario ObterUsuarioPorCodigo(int codigoUsuario)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

            return usuario;
        }

        private List<Dominio.ObjetosDeValor.PaginaUsuario> ObterObjetosPaginas(List<Dominio.Entidades.PaginaUsuario> paginas)
        {
            List<Dominio.ObjetosDeValor.PaginaUsuario> paginasObj = new List<Dominio.ObjetosDeValor.PaginaUsuario>();

            bool naoExibirBaixaDeDuplicatasAReceber = EmpresaUsuario?.Configuracao?.GeraDuplicatasAutomaticamente == Dominio.Enumeradores.OpcaoSimNao.Sim;

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
                    PermissaoDeAcesso = paginaUsuario.Pagina.Formulario == "BaixaDeDuplicatasAReceber.aspx" && naoExibirBaixaDeDuplicatasAReceber ? "I" : paginaUsuario.PermissaoDeAcesso,
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
                pagina.Equals("ajuda.aspx") ||
                pagina.Equals("termosdeuso.aspx") ||
                pagina.Equals("avisoEmpresabloqueiofinanceiro.aspx"))
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

        private Dominio.ObjetosDeValor.MenuApp ObterMenuRaiz(Dominio.ObjetosDeValor.MenuApp menu)
        {
            if (menu == null)
                return null;

            if (menu.MenuPai == null)
                return menu;

            return this.ObterMenuRaiz(menu.MenuPai);
        }

        private List<Dominio.ObjetosDeValor.Menu> MontarMenu(Dominio.ObjetosDeValor.MenuApp menu, ref List<Dominio.ObjetosDeValor.Pagina> paginas)
        {
            var paginasMenu = menu == null ? (from obj in paginas where obj.MenuApp == null select obj) : (from obj in paginas where obj.MenuApp.Codigo == menu.Codigo select obj);

            var menuMontado = (from obj in paginasMenu select new Dominio.ObjetosDeValor.Menu() { Descricao = obj.Descricao, Formulario = obj.Formulario, Icone = obj.Icone, CodigoMenu = menu != null ? menu.Codigo : 0, SubMenu = false, Itens = null }).ToList();

            if (menu != null)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
                Repositorio.Menu repMenu = new Repositorio.Menu(unitOfWork);

                List<Dominio.Entidades.Menu> subMenus = repMenu.BuscarSubMenus(menu.Codigo);

                if (subMenus.Count > 0)
                {
                    foreach (Dominio.Entidades.Menu subMenu in subMenus)
                    {
                        if ((from obj in paginas where obj.MenuApp.Codigo == subMenu.Codigo select obj).Any())
                        {
                            menuMontado.Add(new Dominio.ObjetosDeValor.Menu() { Descricao = subMenu.Descricao, Formulario = string.Empty, Icone = string.Empty, CodigoMenu = subMenu.Codigo, SubMenu = true, Itens = this.MontarMenu(new Dominio.ObjetosDeValor.MenuApp() { Codigo = subMenu.Codigo, Descricao = subMenu.Descricao }, ref paginas) });
                        }
                    }
                }
            }

            return menuMontado.OrderBy(o => o.Descricao).ToList();
        }

        private void RenderizarMenu(Dominio.ObjetosDeValor.Menu menu, ref HtmlGenericControl elemento)
        {
            HtmlGenericControl liPrincipal = null;
            HtmlAnchor a = null;
            HtmlGenericControl b = null;
            HtmlGenericControl ulMenu = null;

            if (menu.CodigoMenu > 0)
            {
                liPrincipal = new HtmlGenericControl("li");

                a = new HtmlAnchor();
                a.InnerText = menu.Descricao;
                a.HRef = "#";

                b = new HtmlGenericControl("b");
                b.Attributes.Add("class", "caret");
                a.Controls.Add(b);
                liPrincipal.Controls.Add(a);

                ulMenu = new HtmlGenericControl("ul");
                ulMenu.Attributes.Add("class", "dropdown-menu");
            }

            foreach (Dominio.ObjetosDeValor.Menu item in menu.Itens)
            {
                if (!item.SubMenu)
                {
                    HtmlGenericControl liFilho = new HtmlGenericControl("li");
                    HtmlAnchor aPagina = new HtmlAnchor();

                    aPagina.InnerText = item.Descricao;
                    aPagina.HRef = item.Formulario;

                    liFilho.Controls.Add(aPagina);

                    if (menu.CodigoMenu > 0)
                        ulMenu.Controls.Add(liFilho);
                    else
                        elemento.Controls.Add(liFilho);
                }
                else
                {
                    this.RenderizarMenu(item, ref ulMenu);
                }
            }

            if (menu.CodigoMenu > 0)
            {
                liPrincipal.Controls.Add(ulMenu);

                elemento.Controls.Add(liPrincipal);
            }
        }

        private void RenderizarMenu2()
        {
            HtmlGenericControl ulPrincipal = (HtmlGenericControl)this.Page.Master.FindControl("navigation");

            var paginasUsuario = (from obj in this.BuscarPaginasUsuario() where obj.PermissaoDeAcesso == "A" && obj.Pagina.MostraNoMenu orderby obj.Pagina.MenuApp.Descricao, obj.Pagina.Descricao select obj.Pagina).ToList();

            List<Dominio.ObjetosDeValor.MenuApp> menus = (from obj in paginasUsuario select obj.MenuApp).Distinct().ToList();

            List<Dominio.ObjetosDeValor.MenuApp> raizes = (from obj in menus select this.ObterMenuRaiz(obj)).Distinct().OrderBy(o => o.Descricao).ToList();

            foreach (Dominio.ObjetosDeValor.MenuApp raiz in raizes)
            {
                Dominio.ObjetosDeValor.Menu menu = new Dominio.ObjetosDeValor.Menu();

                if (raiz != null)
                {
                    menu.CodigoMenu = raiz.Codigo;
                    menu.Descricao = raiz.Descricao;
                }

                menu.Itens = this.MontarMenu(raiz, ref paginasUsuario);

                this.RenderizarMenu(menu, ref ulPrincipal);
            }
        }

        private void SetarDadosEmpresa()
        {
            HtmlGenericControl spnDescricaoEmpresa = (HtmlGenericControl)this.Page.Master.FindControl("spnDescricaoEmpresa");
            spnDescricaoEmpresa.InnerText = this.EmpresaUsuario != null ? string.Concat(Utilidades.String.Left(this.EmpresaUsuario.RazaoSocial, 50), " - ", this.EmpresaUsuario.CNPJ_Formatado, " (", this.EmpresaUsuario.DescricaoEmissao, ")") : string.Empty;

            HtmlAnchor ancEmailEmpresa = (HtmlAnchor)this.Page.Master.FindControl("ancEmailEmpresa");
            ancEmailEmpresa.HRef = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? string.Concat("mailto:", this.EmpresaUsuario.EmpresaPai.Email) : string.Empty;
            ancEmailEmpresa.InnerText = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Email : string.Empty;

            HtmlGenericControl lblTelefoneEmpresa = (HtmlGenericControl)this.Page.Master.FindControl("lblTelefoneEmpresa");
            lblTelefoneEmpresa.InnerText = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? string.Concat(this.EmpresaUsuario.EmpresaPai.Telefone, " | ", this.EmpresaUsuario.EmpresaPai.Fax) : string.Empty;

            HtmlAnchor ancSkypeEmpresa = (HtmlAnchor)this.Page.Master.FindControl("ancSkypeEmpresa");
            ancSkypeEmpresa.HRef = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? string.Concat("skype:", this.EmpresaUsuario.EmpresaPai.Skype, "?chat") : string.Empty;
            ancSkypeEmpresa.InnerText = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Skype : string.Empty;

            HtmlAnchor ancSiteEmpresa = (HtmlAnchor)this.Page.Master.FindControl("ancSiteEmpresa");
            ancSiteEmpresa.HRef = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.URLSistema != null ? this.EmpresaUsuario.EmpresaPai.URLSistema : "http://www.multicte.com.br/";

            HtmlGenericControl version = (HtmlGenericControl)this.Page.Master.FindControl("lblVersao");
            version.InnerText = SetarVersao();

        }

        private void SetarNomeEmpresa()
        {
            HtmlGenericControl spnDescricaoEmpresa = (HtmlGenericControl)this.Page.Master.FindControl("spnDescricaoEmpresa");
            spnDescricaoEmpresa.InnerText = this.EmpresaUsuario != null ? string.Concat(Utilidades.String.Left(this.EmpresaUsuario.RazaoSocial, 50), " - ", this.EmpresaUsuario.CNPJ_Formatado, " (", this.EmpresaUsuario.DescricaoEmissao, ")") : string.Empty;
        }

        private void SetarNomeUsuario()
        {
            HtmlGenericControl spnDescricaoUsuario = (HtmlGenericControl)this.Page.Master.FindControl("spnDescricaoUsuario");
            spnDescricaoUsuario.InnerText = this.Usuario != null ? string.Concat("Bem-vindo, ", Utilidades.String.Left(this.Usuario.Nome, 50)) : string.Empty;
        }

        private void SetarEmailEmpresa()
        {
            HtmlAnchor ancEmailEmpresa = (HtmlAnchor)this.Page.Master.FindControl("ancEmailEmpresa");
            ancEmailEmpresa.HRef = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? string.Concat("mailto:", this.EmpresaUsuario.EmpresaPai.Email) : string.Empty;
            ancEmailEmpresa.InnerText = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Email : string.Empty;
        }

        private void SetarTelefoneEmpresa()
        {
            HtmlGenericControl lblTelefoneEmpresa = (HtmlGenericControl)this.Page.Master.FindControl("lblTelefoneEmpresa");
            lblTelefoneEmpresa.InnerText = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? string.Concat(this.EmpresaUsuario.EmpresaPai.Telefone, " | ", this.EmpresaUsuario.EmpresaPai.Fax) : string.Empty;
        }

        private void SetarSkypeEmpresa()
        {
            HtmlAnchor ancSkypeEmpresa = (HtmlAnchor)this.Page.Master.FindControl("ancSkypeEmpresa");
            ancSkypeEmpresa.HRef = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? string.Concat("skype:", this.EmpresaUsuario.EmpresaPai.Skype, "?chat") : string.Empty;
            ancSkypeEmpresa.InnerText = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Skype : string.Empty;
        }

        private void SetarSiteEmpresa()
        {
            HtmlAnchor ancSiteEmpresa = (HtmlAnchor)this.Page.Master.FindControl("ancSiteEmpresa");
            ancSiteEmpresa.HRef = this.EmpresaUsuario != null && this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.URLSistema != null ? this.EmpresaUsuario.EmpresaPai.URLSistema : "http://www.multicte.com.br/";
        }

        private string SetarVersao()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = $" v. {fvi.FileMajorPart}.{fvi.FileMinorPart:00}";
            if (fvi.FilePrivatePart > 0)
                version = $" v. {fvi.FileMajorPart}.{fvi.FileMinorPart:00}.{fvi.FileBuildPart}.{fvi.FilePrivatePart:00}";

            return version;
        }

        #endregion

    }
}