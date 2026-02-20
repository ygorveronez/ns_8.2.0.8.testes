using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Login
{
    public class EncodedLoginController : SignController
    {
		#region Construtores

		public EncodedLoginController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		//Utilizada para o acesso as Empresas Filhos pelo Transportadores/GerenciarTransportadores

		// GET: EncodedLogin
		public async Task<IActionResult> Index()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (Request.Params("x") != null && Request.Params("y") != null)
                {
                    string login = Servicos.Criptografia.Descriptografar(Request.Params("x"), string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh")));
                    string senha = Servicos.Criptografia.Descriptografar(Request.Params("y"), string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh")));

                    int codigoUsuarioAdmin = 0;
                    int.TryParse(!string.IsNullOrWhiteSpace(Request.Params("z")) ? Servicos.Criptografia.Descriptografar(Request.Params("z"), string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"))) : "", out codigoUsuarioAdmin);

                    if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(senha))
                    {
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                        Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorLoginESenha(login, senha, Dominio.Enumeradores.TipoAcesso.Emissao);

                        //var user = await _manager.FindByEmailAsync(viewModel.Email);

                        // If a user was found
                        if (usuario != null)
                        {
                            // Then create an identity for it and sign it in
                            base.SignIn(usuario, gerenciarTransportadores: true);

                            // If the user came from a specific page, redirect back to it
                            return RedirectToLocal();
                        }
                    }
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

            return RedirectToLocal();
        }

        #endregion

        #region Métodos Privados

        private IActionResult RedirectToLocal(string returnUrl = "")
        {
            // If the return url starts with a slash "/" we assume it belongs to our site
            // so we will redirect to this "action"
            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.Length > 0)
                return Redirect("/#" + returnUrl);

            // If we cannot verify if the url is local to our host we redirect to a default location
            return Redirect("/#Home");
        }

        #endregion
    }
}