using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Login
{
    [CustomAuthorize("Login/AtualizacaoSenhaObrigatoria")]
    public class AtualizacaoSenhaObrigatoriaController : BaseController
    {
        #region Construtores

        public AtualizacaoSenhaObrigatoriaController(Conexao conexao) : base(conexao) { }

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
            var viewModel = new Models.AtualizacaoSenhaObrigatoria { ReturnUrl = returnUrl };

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


        private static bool ValidarSenhaAtual(Dominio.Entidades.Usuario usuario, string senhaAtual)
        {
            if (usuario.SenhaCriptografada)
            {
                string senhaSHA256 = Servicos.Criptografia.GerarHashSHA256(senhaAtual);
                string senhaMD5 = Servicos.Criptografia.GerarHashMD5(senhaAtual);
                return senhaSHA256 == usuario.Senha || senhaMD5 == usuario.Senha;
            }

            return senhaAtual == usuario.Senha;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(Models.AtualizacaoSenhaObrigatoria viewModel)
        {
            string stringConexaoAdmin = _conexao.AdminStringConexao;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);
            unitOfWorkAdmin.Dispose();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
            {
                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
                return View(viewModel);
            }

            try
            {
                Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();
                Dominio.Entidades.Usuario usuario = this.Usuario;
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);

                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoAcesso = clienteURLAcesso.TipoServicoMultisoftware;
                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(tipoServicoAcesso);
                if (politicaSenha == null)
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoNull();

                string senhaAtual = viewModel.senhaAtual;
                string novaSenha = viewModel.novaSenha;
                string confirmacao = viewModel.confirmacaoSenha;
                string mensagemErro = "";

                if (this.Usuario != null)
                {
                    if (this.Usuario.TipoComercial.HasValue && (this.Usuario.TipoComercial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial.Vendedor || this.Usuario.TipoComercial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial.Gerente))
                        politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                    bool senhaAtualValida = false;

                    if (politicaSenha != null &&
                        (tipoServicoAcesso != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ||
                         politicaSenha.TipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe))
                    {
                        // Verifica se o usuário precisa alterar senha (expirada ou obrigatória)
                        //TODO: ISSO AQUI É REGRA DE NEGOCIO E PRECISA ESTAR NA CAMADA DE DOMINIO
                        senhaAtualValida = 
                        usuario.AlterarSenhaAcesso ||
                            (string.IsNullOrWhiteSpace(usuario.Senha) &&
                             (!usuario.DataUltimaAlteracaoSenhaObrigatoria.HasValue ||
                              usuario.DataUltimaAlteracaoSenhaObrigatoria.Value.AddDays(politicaSenha.PrazoExpiraSenha) < DateTime.Now))                          
                           && ValidarSenhaAtual(usuario, senhaAtual);   
                        
                    }
                    else
                    {
                        if (senhaAtual == usuario.Senha)
                        {
                            if (usuario.Senha != novaSenha)
                                senhaAtualValida = true;
                            else
                                mensagemErro = Localization.Resources.AtualizacaoSenhaObrigatoria.AtualizacaoSenhaObrigatoria.NovaSenhaPrecisaSerDiferenteDaSenhaAtual;
                        }
                    }


                    if (senhaAtualValida)
                    {
                        if (novaSenha == confirmacao)
                        {
                            usuario.Senha = novaSenha;

                            string retornoPoliticaSenha = "";
                            if (politicaSenha != null && (tipoServicoAcesso != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || politicaSenha.TipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe))
                                retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);


                            if (string.IsNullOrWhiteSpace(retornoPoliticaSenha))
                            {
                                Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior repFuncionarioSenhaAnterior = new Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior(unitOfWork);

                                Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior funcionarioSenhaAnterior = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior
                                {
                                    Senha = usuario.Senha,
                                    SenhaCriptografada = usuario.SenhaCriptografada,
                                    Usuario = usuario
                                };

                                repFuncionarioSenhaAnterior.Inserir(funcionarioSenhaAnterior);

                                usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                                usuario.AlterarSenhaAcesso = false;

                                repUsuario.Atualizar(usuario);

                                return RedirectToLocal(viewModel.ReturnUrl);
                            }
                            else
                            {
                                mensagemErro = retornoPoliticaSenha;
                            }
                        }
                        else
                        {
                            mensagemErro = Localization.Resources.AtualizacaoSenhaObrigatoria.AtualizacaoSenhaObrigatoria.AConfirmacaoEstaDiferenteNovaSenha;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(mensagemErro))
                            mensagemErro = Localization.Resources.AtualizacaoSenhaObrigatoria.AtualizacaoSenhaObrigatoria.ASenhaInformadaDiferenteDaSenhaAtual;
                    }
                }
                else
                {
                    mensagemErro = Localization.Resources.AtualizacaoSenhaObrigatoria.AtualizacaoSenhaObrigatoria.SuaSessaoExpirouAcesseNovamente;
                }

                ModelState.AddModelError("", mensagemErro);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);

                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}

