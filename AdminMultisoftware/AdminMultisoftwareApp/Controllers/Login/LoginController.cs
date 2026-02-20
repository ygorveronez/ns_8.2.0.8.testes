using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AdminMultisoftwareApp.Controllers.Login
{
    public class LoginController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();

            return View();
        }

        [HttpPost]
        public ActionResult Index(AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario viewModel)
        {
            var msg = "Usúario ou senha inválidos!";

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Session["IdUsuario"] = null;
                Session["NomeUsuario"] = null;
                Session["GrupoRedmine"] = null;

                var viewModelSenha = viewModel.Senha;
                AdminMultisoftware.Repositorio.Pessoas.Usuario repUsuario = new AdminMultisoftware.Repositorio.Pessoas.Usuario(unitOfWork);

                if (!string.IsNullOrEmpty(viewModel.Senha))
                    viewModel.Senha = Criptografar(viewModel.Senha);

                AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario usuario = repUsuario.BuscarPorLoginESenha(viewModel.Login, viewModel.Senha);
                if (usuario != null)
                {
                    Session["IdUsuario"] = usuario.Codigo;
                    Session["NomeUsuario"] = usuario.Nome;

                    FormsAuthentication.SetAuthCookie(usuario.Login, true);

                    return Redirect("/#Home");
                }

                Dominio.ObjetosDeValor.Integracao.Redmine.User redmineUser = Servicos.Admin.Integracao.Redmine.Users.AutenticarUsuario(viewModel.Login, viewModelSenha, unitOfWork);
                if (redmineUser != null && redmineUser.status == 1)
                {
                    List<string> grupos = redmineUser.groups != null
                        ? redmineUser
                            .groups
                            .Where(grupo => grupo != null)
                            .Select(grupo => grupo.name)
                            .ToList()
                        : new List<string>();

                    Session["IdUsuario"] = redmineUser.id;
                    Session["NomeUsuario"] = redmineUser.firstname + ' ' + redmineUser.lastname;
                    Session["GrupoRedmine"] = grupos;

                    FormsAuthentication.SetAuthCookie(redmineUser.firstname + redmineUser.lastname, true);
                    return Redirect("/#Home");
                }

                ModelState.AddModelError("", msg);

                return View("Index");
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                ModelState.AddModelError("", "Ocorreu uma falha ao tentar acessar!");

                return View("Index");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private string Criptografar(string senha)
        {
            senha = Servicos.Criptografia.GerarHashMD5(senha);

            return senha;
        }
    }
}