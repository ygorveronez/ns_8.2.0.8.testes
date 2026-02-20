using System;
using System.Collections.Generic;
using System.Web.Security;

namespace EmissaoCTe.InternalWebAdmin
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
            if (this.Usuario == null)
                Response.Redirect("Logon.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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

        #endregion
    }
}