using System;
using System.Web;

namespace EmissaoCTe.WebAdmin
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lnkNomeUsuario.Text = HttpContext.Current.User.Identity.Name;
            this.lnkNomeEmpresa.Text = "Administrativo";

            string boolPodeAuditar = this._PodeAuditar ? "true" : "false";
#if DEBUG
            boolPodeAuditar = "true";
#endif
            hddConfiguracaoAuditoria.Value = boolPodeAuditar;
        }

        private Dominio.Entidades.Usuario _UsuarioAdministradorLogado;
        private Dominio.Entidades.Usuario BuscarUsuarioAdministrativo()
        {
            if (this._UsuarioAdministradorLogado == null)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                this._UsuarioAdministradorLogado = repositorioUsuario.BuscarPorCodigo(this.IdUsuarioAdmin);
            }

            return this._UsuarioAdministradorLogado;
        }

        private bool _PodeAuditar
        {
            get
            {
                return this.UsuarioAdministrativo != null;
            }
        }
        private int IdUsuarioAdmin
        {
            get
            {
                if (Session["IdUsuario"] != null)
                    return (int)Session["IdUsuario"];
                else
                    return 0;
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
    }
}