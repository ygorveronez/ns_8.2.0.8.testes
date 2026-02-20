using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;

namespace EmissaoCTe.WebApp
{
    public partial class EncodedLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (Request["x"] != null && Request["y"] != null)
                {
                    string login = Servicos.Criptografia.Descriptografar(Request["x"], string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh")));
                    string senha = Servicos.Criptografia.Descriptografar(Request["y"], string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh")));

                    int codigoUsuarioAdmin = 0;
                    int.TryParse(!string.IsNullOrWhiteSpace(Request["z"]) ? Servicos.Criptografia.Descriptografar(Request["z"], string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"))) : "", out codigoUsuarioAdmin);

                    if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(senha))
                    {
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                        Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorLoginESenha(login, senha, Dominio.Enumeradores.TipoAcesso.Emissao);
                        Dominio.Entidades.Usuario usuarioAdmin = !string.IsNullOrWhiteSpace(Request["z"]) ? repUsuario.BuscarPorCodigo(codigoUsuarioAdmin) : null;

                        if (usuario != null)
                        {
                            if (usuario.Status.ToUpper().Equals("A") && usuario.Empresa.Status.ToUpper().Equals("A"))
                            {
                                usuario.Session = Guid.NewGuid().ToString();
                                usuario.UltimoAcesso = DateTime.Now;
                                repUsuario.Atualizar(usuario);

                                Session["TentativasLogin"] = 0;
                                Session["IdEmpresa"] = usuario.Empresa.Codigo;
                                Session["IdUsuario"] = usuario.Codigo;
                                Session["IdUsuarioAdmin"] = usuarioAdmin != null ? usuarioAdmin.Codigo : 0;
                                Session["GuidUser"] = usuario.Session;

                                if (Response.Cookies.Get("User") != null)
                                    Response.Cookies.Remove("User");
                                Response.Cookies.Add(new HttpCookie("User", usuario.Codigo.ToString()));

                                if (Response.Cookies.Get("NomeEmpresa") != null)
                                    Response.Cookies.Remove("NomeEmpresa");
                                Response.Cookies.Add(new HttpCookie("NomeEmpresa", System.Web.HttpUtility.HtmlAttributeEncode(string.Concat(usuario.Empresa.RazaoSocial.Substring(0, usuario.Empresa.RazaoSocial.Length > 50 ? 50 : usuario.Empresa.RazaoSocial.Length), usuario.Empresa.RazaoSocial.Length > 50 ? "..." : ""))));

                                if (Response.Cookies.Get("TipoAmbienteEmpresa") != null)
                                    Response.Cookies.Remove("TipoAmbienteEmpresa");
                                Response.Cookies.Add(new HttpCookie("TipoAmbienteEmpresa", System.Web.HttpUtility.HtmlAttributeEncode(usuario.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? "PRODUÇÃO" : "HOMOLOGAÇÃO")));

                                if (Response.Cookies.Get("GuidUser") != null)
                                    Response.Cookies.Remove("GuidUser");
                                Response.Cookies.Add(new HttpCookie("GuidUser", usuario.Session));

                                this.SetarPermissoesUsuario(usuario, unitOfWork);
                                this.CriarLogDeAcesso(login, senha, usuario, unitOfWork);

                                if (new Class.LogonService().VerificarStatusFinanceiro(usuario, unitOfWork))
                                    Page.Response.Redirect("AvisoEmpresaBloqueioFinanceiro.aspx", false);
                                else
                                    FormsAuthentication.RedirectFromLoginPage(usuario.Nome, false);
                                //SingleSessionPreparation.CreateAndStoreSessionToken(usuario);

                                return;
                            }
                        }
                    }
                }
                Response.Redirect("Logon.aspx");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Response.Redirect("Logon.aspx");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private void CriarLogDeAcesso(string login, string senha, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LogAcesso repLogAcesso = new Repositorio.LogAcesso(unitOfWork);
            Dominio.Entidades.LogAcesso logAcesso = new Dominio.Entidades.LogAcesso();

            logAcesso.Data = DateTime.Now;
            logAcesso.IPAcesso = Request.UserHostAddress;
            logAcesso.Login = login;
            logAcesso.Senha = senha;
            logAcesso.SessionID = Session.SessionID;
            logAcesso.Tipo = Dominio.Enumeradores.TipoLogAcesso.Entrada;
            logAcesso.Usuario = usuario;

            repLogAcesso.Inserir(logAcesso);
        }

        private void SetarPermissoesUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            string key = string.Concat("CTeUserPages_", usuario.Codigo);
            if (Cache.Get(key) != null)
                Cache.Remove(key);
            Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
            List<Dominio.Entidades.PaginaUsuario> paginasUsuario = repPaginaUsuario.BuscarPorUsuario(usuario.Codigo);
            Cache.Add(key, paginasUsuario, null, DateTime.Now.AddHours(6), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
        }

        #endregion
    }


}
