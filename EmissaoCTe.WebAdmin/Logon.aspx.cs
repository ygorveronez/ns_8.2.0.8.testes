using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Security;
using EmissaoCTe.WebAdmin.Class;

namespace EmissaoCTe.WebAdmin
{
    public partial class Logon : System.Web.UI.Page
    {
        #region Variáveis Globais

        Dominio.Entidades.Usuario _Usuario;

        #endregion

        #region Manipuladores de Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string errorMessage = Request.QueryString["errorMessage"];
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    this.divMsgErroSSO.Style["display"] = "";
                    this.lblErroSSO.Text = Server.HtmlEncode(errorMessage);
                }
            }
            else
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
                string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];

                if (!string.IsNullOrWhiteSpace(this.txtUsuario.Text) && !string.IsNullOrWhiteSpace(this.txtSenha.Text))
                {
                    Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                    this._Usuario = repositorioUsuario.BuscarPorLogin(this.txtUsuario.Text, Dominio.Enumeradores.TipoAcesso.Admin);

                    if (this._Usuario != null)
                    {
                        //Se o usuário for diferente do anterior, zera as tentativas de login
                        if (Session["IdUsuario"] == null || this._Usuario.Codigo != (int)Session["IdUsuario"])
                            Session["TentativasLogin"] = 0;

                        //Seta os dados do usuario
                        Session["IdUsuario"] = this._Usuario.Codigo;
                        Session["TentativasLogin"] = Session["TentativasLogin"] != null ? (int)Session["TentativasLogin"] + 1 : 1;

                        if ((int)Session["TentativasLogin"] > 3)
                        {
                            if (this._Usuario.Status.ToUpper().Equals("A"))
                            {
                                //Trocar senha e disparar o e-mail
                                Servicos.Senha svcSenha = new Servicos.Senha();
                                Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
                                this._Usuario.Senha = svcSenha.GerarSenha(8);
                                repositorioUsuario.Atualizar(this._Usuario);
                                svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", this._Usuario.Email, "", "", "Emissão de Nova Senha para o Sistema", "Devido ao número de tentativas inválidas de acesso " + (!string.IsNullOrWhiteSpace(ambiente) ? " ao ambiente " + ambiente : "") + ", o sistema gerou automaticamente uma nova senha para seu usuário (" + this._Usuario.Login + "): " + this._Usuario.Senha, "179.127.8.8", null, string.Empty, false, "cte@multisoftware.com.br");
                                svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "cte@multisoftware.com.br", "", "", "Usuário Bloqueado", "Devido ao número de tentativas inválidas de acesso " + (!string.IsNullOrWhiteSpace(ambiente) ? " ao ambiente " + ambiente : "") + ", foi gerada uma nova senha para o usuário (Nome: " + this._Usuario.Nome + ", Login: " + this._Usuario.Login + "): " + this._Usuario.Senha, "179.127.8.8", null, string.Empty, false, "cte@multisoftware.com.br");
                                this.divMsgEnvioEmail.Style["display"] = "";
                                Session["TentativasLogin"] = 0;
                            }
                            else
                            {
                                this.divMsgErroLogin.Style["display"] = "";
                            }
                        }
                        else
                        {
                            this._Usuario = repositorioUsuario.BuscarPorLoginESenha(this.txtUsuario.Text, this.txtSenha.Text, Dominio.Enumeradores.TipoAcesso.Admin);
                            this.CriarLogDeAcesso(unitOfWork);

                            if (this._Usuario != null)
                            {
                                #if !DEBUG
                                AtualizarVersaoEmSegundoPlano();
                                #endif

                                if (this._Usuario.Status.ToUpper().Equals("A"))
                                {

                                    this._Usuario.UltimoAcesso = DateTime.Now;
                                    repositorioUsuario.Atualizar(this._Usuario);

                                    Session["TentativasLogin"] = 0;
                                    Session["IdEmpresa"] = this._Usuario.Empresa.Codigo;
                                    Session["IdUsuario"] = this._Usuario.Codigo;
                                    this.SetarPermissoesUsuario(unitOfWork);

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
                    }
                    else
                    {
                        Session["IdUsuario"] = null;
                        Session["TentativasLogin"] = null;
                        this.divMsgErroLogin.Style["display"] = "";
                    }
                }
                else
                {
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

                    if (versao.VersaoEmissaoCTEWebAdmin != version)
                    {
                        versao.CodigoCliente = this._Usuario.Empresa.Codigo;
                        versao.VersaoEmissaoCTEWebAdmin = version;
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

        private void SetarPermissoesUsuario(Repositorio.UnitOfWork unitOfWork)
        {
            string key = string.Concat("CTeUserPages_", this._Usuario.Codigo);
            if (Cache.Get(key) != null)
                Cache.Remove(key);
            Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
            List<Dominio.Entidades.PaginaUsuario> paginasUsuario = repPaginaUsuario.BuscarPorUsuario(this._Usuario.Codigo);
            Cache.Add(key, paginasUsuario, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
        }

        #endregion

    }
}