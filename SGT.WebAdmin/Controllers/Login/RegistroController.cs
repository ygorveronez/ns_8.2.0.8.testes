using System;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Login
{
    public class RegistroController : BaseController
    {
		#region Construtores

		public RegistroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAnonymous]
        public async Task<IActionResult> Index(string returnUrl, int multi = 0)
        {
            string stringConexaoAdmin = _conexao.AdminStringConexao;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(stringConexaoAdmin);

            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

            unitOfWorkAdmin.Dispose();

            buscarConfiguracaoPadrao();

            var viewModel = new Models.Login
            {
                ReturnUrl = returnUrl,
            };

            Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, new Repositorio.UnitOfWork(_conexao.StringConexao));

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                string cpfCnpj = "";
                int tipoPessoa = Request.GetIntParam("TipoPessoa");
                if (tipoPessoa == 0) { // Se for pessoa física
                    cpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF"));
                }
                else if (tipoPessoa == 1) // Se for pessoa jurídica
                {
                    cpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CNPJ"));
                }

                double dCpfCnpj = 0;
                if (!string.IsNullOrEmpty(cpfCnpj))
                    dCpfCnpj = double.Parse(cpfCnpj);

                var pessoa = repPessoa.BuscarPorCPFCNPJ(dCpfCnpj);

                ValidarPoliticaSenha(out Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha);

                if (pessoa != null)
                {
                    if (pessoa.AtivarAcessoFornecedor)
                    {
                        // Já está ativo
                        return new JsonpResult(false, false, "Este usuário já está cadastrado.");
                    } else
                    {
                        // Apenas ativar a pessoa que já existe
                        pessoa.AtivarAcessoFornecedor = true;
                        pessoa.DataUltimaAtualizacao = DateTime.Now;
                        pessoa.Integrado = false;
                        repPessoa.Atualizar(pessoa);
                        CriarUsuarioFromPessoaEGerarConfirmacaoEmail(pessoa, Request.GetStringParam("Senha"), politicaSenha, unitOfWork);
                        return new JsonpResult(true, true, "Acesso garantido com sucesso.");
                    }
                }

                if (tipoPessoa == 0) // Se for pessoa física
                {
                    return RegistrarNovaPessoaFisica(dCpfCnpj, politicaSenha, unitOfWork);
                }
                else if (tipoPessoa == 1) // Se for pessoa jurídica
                {
                    return RegistrarNovaPessoaJuridica(dCpfCnpj, politicaSenha, unitOfWork);
                }
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao registrar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new JsonpResult(false, false, "Esse tipo de pessoa não é válido");
        }

        [AllowAnonymous]
        public async Task<IActionResult> BuscarUsuarioJaExiste()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                string cpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF_CNPJ"));

                double dCpfCnpj = 0;
                if (!string.IsNullOrEmpty(cpfCnpj))
                    dCpfCnpj = double.Parse(cpfCnpj);

                var pessoa = repPessoa.BuscarPorCPFCNPJ(dCpfCnpj);

                if (pessoa == null)
                {
                    return new JsonpResult(new { JaExiste = false, JaTemAcesso = false }, true, "Este usuário Não está cadastrado no sistema.");
                }

                if (pessoa.AtivarAcessoFornecedor)
                {
                    return new JsonpResult(new { JaExiste = true, JaTemAcesso = true }, true, "Este usuário já existe e já tem acesso ao portal de fornecedor.");
                }

                return new JsonpResult(new { JaExiste = true, JaTemAcesso = false }, true, "Este usuário já existe e e não tem acesso ao portal de fornecedor.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a conta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarConta(string token)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            var serConfirmarContaEmail = new Servicos.Global.ConfirmacaoContaEmail(unitOfWork);

            if (serConfirmarContaEmail.ConfirmarEmail(token))
            {
                return View("ConfirmacaoSucesso");
            }

            return View("ConfirmacaoFalha");
        }

        #endregion

        #region Métodos Privados

        private void ValidarPoliticaSenha(out Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminUnitOfWork);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

            adminUnitOfWork.Dispose();

            Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

            Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);

            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoAcesso = clienteURLAcesso.TipoServicoMultisoftware;
            politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(tipoServicoAcesso);

            string senha = Request.GetStringParam("Senha");
            bool senhaDeAcordo = serPoliticaSenha.SenhaEstaDeAcordo(senha, politicaSenha, out string erro);

            if(!senhaDeAcordo)
            {
                throw new ControllerException(erro);
            }
        }

        private IActionResult RegistrarNovaPessoaFisica(double dCpf, Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

            var localidade = repLocalidade.BuscarPorCodigo(Request.GetIntParam("Localidade"));
            var atividade = repAtividade.BuscarPorCodigo(Request.GetIntParam("Atividade"));

            Dominio.Entidades.Cliente pessoa = new Dominio.Entidades.Cliente()
            {
                Tipo = "F",
                CPF_CNPJ = dCpf,
                RG_Passaporte = Request.GetStringParam("RG"),
                Nome = Request.GetStringParam("Nome"),
                Telefone1 = Request.GetStringParam("TelefonePrincipal"),
                Email = Request.GetStringParam("Email"),
                CEP = Utilidades.String.OnlyNumbers(Request.Params("CEP")).PadLeft(8, '0'),
                Endereco = Request.GetStringParam("Endereco"),
                Bairro = Request.GetStringParam("Bairro"),
                Localidade = localidade,
                Atividade = atividade,
                Complemento = Request.GetStringParam("Complemento"),
                TipoLogradouro = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro>("TipoLogradouro"),
                Numero = Request.GetStringParam("Numero"),
                TipoEndereco = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco>("TipoEndereco"),
                Ativo = true,
                GeradoViaPortal = true,
                AtivarAcessoFornecedor = true,
                EnderecoDigitado = true,
            };

            repPessoa.Inserir(pessoa);
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork).GerarIntegracaoPessoa(unitOfWork, pessoa);
            CriarUsuarioFromPessoaEGerarConfirmacaoEmail(pessoa, Request.GetStringParam("Senha"), politicaSenha, unitOfWork);

            return new JsonpResult(true);
        }

        private IActionResult RegistrarNovaPessoaJuridica(double dCNPJ, Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

            var localidade = repLocalidade.BuscarPorCodigo(Request.GetIntParam("Localidade"));
            var atividade = repAtividade.BuscarPorCodigo(Request.GetIntParam("Atividade"));

            Dominio.Entidades.Cliente pessoa = new Dominio.Entidades.Cliente()
            {
                Tipo = "J",
                CPF_CNPJ = dCNPJ,
                Nome = Request.GetStringParam("Nome"),
                NomeFantasia = Request.GetStringParam("Fantasia"),
                Telefone1 = Request.GetStringParam("TelefonePrincipal"),
                Email = Request.GetStringParam("Email"),
                IE_RG = Request.GetStringParam("IE"),
                CEP = Utilidades.String.OnlyNumbers(Request.Params("CEP")).PadLeft(8, '0'),
                Endereco = Request.GetStringParam("Endereco"),
                Bairro = Request.GetStringParam("Bairro"),
                Localidade = localidade,
                Atividade = atividade,
                Complemento = Request.GetStringParam("Complemento"),
                TipoLogradouro = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro>("TipoLogradouro"),
                Numero = Request.GetStringParam("Numero"),
                TipoEndereco = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco>("TipoEndereco"),
                Ativo = true,
                GeradoViaPortal = true,
                AtivarAcessoFornecedor = true,
                EnderecoDigitado = true,
            };

            repPessoa.Inserir(pessoa);
            new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork).GerarIntegracaoPessoa(unitOfWork, pessoa);
            CriarUsuarioFromPessoaEGerarConfirmacaoEmail(pessoa, Request.GetStringParam("Senha"), politicaSenha, unitOfWork);

            return new JsonpResult(true);
        }

        private Dominio.Entidades.Usuario CriarUsuarioFromPessoaEGerarConfirmacaoEmail(Dominio.Entidades.Cliente pessoa, string senha, Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            if(politicaSenha != null && politicaSenha.HabilitarCriptografia)
            {
                senha = Servicos.Criptografia.GerarHashSHA256(senha);
            }

            Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario
            {
                CPF = pessoa.CPF_CNPJ_SemFormato,
                ClienteFornecedor = pessoa,
                Cliente = pessoa,
                Nome = pessoa.Nome,
                Telefone = pessoa.Telefone1,
                Localidade = pessoa.Localidade,
                Endereco = pessoa.Endereco,
                Complemento = pessoa.Complemento,
                Email = pessoa.Email,
                Login = pessoa.CPF_CNPJ_SemFormato,
                Senha = senha,
                SenhaCriptografada = politicaSenha != null && politicaSenha.HabilitarCriptografia,
                Status = "I", // Inativo. Deve confirmar o e-mail para ativar
                TipoAcesso = Dominio.Enumeradores.TipoAcesso.Fornecedor,
                Empresa = this.Empresa,
                UsuarioAdministrador = true
            };

            repUsuario.Inserir(usuario);

            var serConfirmacao = new Servicos.Global.ConfirmacaoContaEmail(unitOfWork);
            serConfirmacao.GerarConfirmacaoContaEEnviarEmail(usuario, Cliente.Codigo, TipoServicoMultisoftware, adminUnitOfWork, _conexao.AdminStringConexao);

            return usuario;
        }

        private void buscarConfiguracaoPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoEmbarcador != null)
                {
                    var retorno = new
                    {
                        configuracaoEmbarcador.Codigo,
                        configuracaoEmbarcador.Pais,
                    };

                    ViewBag.ConfiguracaoPadrao = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);
                }
                else
                {
                    throw new Exception("Não existe uma configuração padrão para o " + Cliente.RazaoSocial + ", por favor configure");
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

    }
}
