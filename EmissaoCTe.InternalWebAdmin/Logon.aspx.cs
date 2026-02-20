using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace EmissaoCTe.InternalWebAdmin
{
    public partial class Logon : System.Web.UI.Page
    {
        #region Variáveis Globais

        Dominio.Entidades.Usuario _Usuario;

        #endregion

        #region Manipuladores de Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                this.divMsgErroLogin.Style["display"] = "none";
                this.divMsgEnvioEmail.Style["display"] = "none";
            }
        }

        protected void btnLogar_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (!string.IsNullOrWhiteSpace(this.txtUsuario.Text) && !string.IsNullOrWhiteSpace(this.txtSenha.Text))
                {
                    Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                    this._Usuario = repositorioUsuario.BuscarPorLoginESenha(this.txtUsuario.Text, this.txtSenha.Text, Dominio.Enumeradores.TipoAcesso.AdminInterno);
                    this.CriarLogDeAcesso(unitOfWork);

                    if (this._Usuario != null)
                    {
                        #if !DEBUG
                        AtualizarVersaoEmSegundoPlano();
                        #endif

                        if (this._Usuario.Empresa.EmpresaAdministradora == null && this._Usuario.Empresa.EmpresaPai == null)
                        {
                            if (this._Usuario.Status.ToUpper().Equals("A"))
                            {
                                Session["TentativasLogin"] = 0;
                                Session["IdEmpresa"] = this._Usuario.Empresa.Codigo;
                                Session["IdUsuario"] = this._Usuario.Codigo;
                                
                                if (Response.Cookies.Get("User") != null)
                                    Response.Cookies.Remove("User");
                                Response.Cookies.Add(new HttpCookie("User", this._Usuario.Codigo.ToString()));

                                FormsAuthentication.RedirectFromLoginPage(this._Usuario.Nome, false);
                            }
                            else
                            {
                                this.divMsgErroLogin.Style["display"] = "";
                            }
                        }
                        else
                        {
                            this.divMsgErroLogin.Style["display"] = "";
                        }
                    }
                    else
                    {
                        this.divMsgErroLogin.Style["display"] = "";
                    }
                }
                else
                {
                    Session["IdUsuario"] = null;
                    Session["TentativasLogin"] = null;
                    this.divMsgErroLogin.Style["display"] = "";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private void AtualizarVersaoEmSegundoPlano()
        {
            _ = Task.Run(() =>
            {
                try
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    string version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart:00}";
                    if (fvi.FilePrivatePart > 0)
                        version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart:00}.{fvi.FileBuildPart}.{fvi.FilePrivatePart:00}";

                    var versaoAplicacao = new Repositorio.VersaoAplicacao(new Repositorio.UnitOfWork(Conexao.AdminStringConexao));

                    var versao = versaoAplicacao.ConsultarPorAmbienteECliente(this._Usuario.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? "PROD" : "HOMOLOG", _Usuario.Empresa.Codigo);

                    if (versao is null)
                        versao = new Dominio.Entidades.VersaoAplicacao();

                    if (versao.VersaoEmissaoCTEInternalWebAdmin != version)
                    {
                        versao.CodigoCliente = this._Usuario.Empresa.Codigo;
                        versao.VersaoEmissaoCTEInternalWebAdmin = version;
                        versao.Ambiente = this._Usuario.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? "PROD" : "HOMOLOG";
                        versaoAplicacao.AtualizarNumeroVersao(versao);
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }
        private void CriarLogDeAcesso(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LogAcesso repositorioLogAcesso = new Repositorio.LogAcesso(unitOfWork);
            Dominio.Entidades.LogAcesso logAcesso = new Dominio.Entidades.LogAcesso();

            logAcesso.Data = DateTime.Now;
            logAcesso.IPAcesso = Request.UserHostAddress;
            logAcesso.Login = this.txtUsuario.Text;
            logAcesso.Senha = this.txtSenha.Text;
            logAcesso.SessionID = Session.SessionID;
            logAcesso.Tipo = Dominio.Enumeradores.TipoLogAcesso.Entrada;
            logAcesso.Usuario = this._Usuario;

            repositorioLogAcesso.Inserir(logAcesso);
        }

        #endregion
    }
}