using Dominio.Entidades.Embarcador.Usuarios;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Services.Interfaces;

namespace SGT.WebAdmin.Controllers.Login
{
    public class EsqueciSenhaController : BaseController
    {
        private readonly IEmailService _emailService;

        #region Construtores

        public EsqueciSenhaController(Conexao conexao, IEmailService emailService) : base(conexao)
        {
            _emailService = emailService;
        }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index(string returnUrl)
        {
            string stringConexaoAdmin = _conexao.AdminStringConexao;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);
            unitOfWorkAdmin.Dispose();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            // Store the originating URL so we can attach it to a form field
            var viewModel = new Models.EsqueciSenha { ReturnUrl = returnUrl };

            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);

            return View(viewModel);
        }

        private IActionResult RedirectToLocal(string returnUrl = "")
        {
            // If the return url starts with a slash "/" we assume it belongs to our site
            // so we will redirect to this "action"
            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.Length > 0)
                return Redirect("/#" + returnUrl);

            // If we cannot verify if the url is local to our host we redirect to a default location
            return Redirect("/#Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(Models.EsqueciSenha viewModel)
        {
            string stringConexaoAdmin = _conexao.AdminStringConexao;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);
            unitOfWorkAdmin.Dispose();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Ensure we have a valid viewModel to work with
                if (!ModelState.IsValid)
                {
                    Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                    ModelState.Remove("Usuario");
                    ModelState.AddModelError("", $"{Localization.Resources.Login.Login.PreenchimentoObrigatorio}.");
                    return View(viewModel);
                }

                // Verify if a user exists with the provided identity information
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);

                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(clienteURLAcesso.TipoServicoMultisoftware);

                if (politicaSenha == null)
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

                Dominio.Enumeradores.TipoAcesso tipoAcesso = Dominio.Enumeradores.TipoAcesso.Embarcador;

                if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    tipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;

                if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    tipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;

                if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    tipoAcesso = Dominio.Enumeradores.TipoAcesso.Fornecedor;

                Dominio.Entidades.Usuario usuario = null;

                if (tipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao || tipoAcesso == Dominio.Enumeradores.TipoAcesso.Fornecedor || tipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    usuario = repUsuario.BuscarPorLogin(viewModel.Usuario, tipoAcesso);

                    if (usuario == null && tipoAcesso == Dominio.Enumeradores.TipoAcesso.Fornecedor)
                    {
                        usuario = repUsuario.BuscarPorLoginVendedorOuGerente(viewModel.Usuario);

                        if (usuario == null)
                            usuario = repUsuario.BuscarPorLogin(viewModel.Usuario, Dominio.Enumeradores.TipoAcesso.Emissao);
                    }

                    if (usuario != null)
                    {
                        if (usuario.UsuarioAcessoBloqueado && !usuario.UsuarioMultisoftware)
                        {
                            ModelState.AddModelError("", $"{Localization.Resources.Login.Login.UsuarioBloqueado}.");
                            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                            return View(viewModel);
                        }
                        else if (string.IsNullOrWhiteSpace(usuario.Email))
                        {
                            ModelState.AddModelError("", $"{Localization.Resources.Login.Login.UsuarioNaoPossuiEmail}.");
                            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                            return View(viewModel);
                        }
                        else
                        {
                            usuario.TentativasInvalidas = 0;
                            usuario.UsuarioAcessoBloqueado = false;
                            usuario.AlterarSenhaAcesso = true;

                            string novaSenha = politicaSenha.CriarNovaSenha();

                            usuario.Senha = novaSenha;
                            if (politicaSenha != null && politicaSenha.HabilitarPoliticaSenha && politicaSenha.HabilitarCriptografia)
                            {
                                usuario.Senha = Servicos.Criptografia.GerarHashSHA256(novaSenha);
                                usuario.SenhaCriptografada = true;
                            }

                            repUsuario.Atualizar(usuario);

                            await _emailService.EnviarEmailNovaSenhaAsync(usuario.Email, novaSenha, usuario.Login);

                            ModelState.AddModelError("", $"{Localization.Resources.Login.Login.SolicitacaoRealizada}.");
                            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"{Localization.Resources.Login.Login.TipoDeAcessoNaoConfiguradoMudancaSenha}.");
                    Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                    return View(viewModel);
                }


                string msg = $"{Localization.Resources.Login.Login.SolicitacaoRealizada}.";
                ModelState.AddModelError("", msg);
                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);

                return View();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                ModelState.AddModelError("", $"{Localization.Resources.Login.Login.OcorreuFalhaAoGerarNovaSenha}.");
                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                return View(viewModel);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}

