using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [AllowAnonymous]
    public class ConsultarAndamentoPedidoController : BaseController
    {
		#region Construtores

		public ConsultarAndamentoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAnonymous]
        public async Task<IActionResult> ObterDadosPedido()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain servicoIntegracaoSaintGobain = new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork);

            try
            {
                string numOv = Request.GetStringParam("numOv");
                string codEmpresa = Request.GetStringParam("codEmpresa");
                string codUserSap = Request.GetStringParam("codUserSap");
                string tipoUsuario = Request.GetStringParam("tipoUsuario");

                Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.AndamentoPedido pedidoRetorno = servicoIntegracaoSaintGobain.ConsultarAndamentoPedido(numOv, codEmpresa, codUserSap, tipoUsuario);

                if (pedidoRetorno != null)
                    return new JsonpResult(pedidoRetorno);
                else
                    return new JsonpResult(false, "Problemas ao buscar dados do pedido");

            }
            catch (ServicoException ex)
            {
                //Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as informações do pedido");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ObterDadosDoLoginCliente()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain servicoIntegracaoSaintGobain = new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork);

            try
            {
                string CodUserSap = Request.GetStringParam("codUserSap");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario user = user = repUsuario.BuscarPorLogin(CodUserSap);
                if (user == null)
                    user = repUsuario.BuscarPorCodigoIntegracao(CodUserSap);

                Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Usuario retornoUser = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Usuario();
                retornoUser = servicoIntegracaoSaintGobain.ConsultarUsuarioAndamentoPedido(CodUserSap);

                if (user == null)
                {
                    //buscar no sap e confirmar email para cadastrar usuario.
                    if (retornoUser != null)
                    {
                        var retorno = new
                        {
                            nomeUsuario = retornoUser.nomeUsuario,
                            eMail = retornoUser.eMail,
                            codUsuario = retornoUser.codUsuario,
                            Empresa = retornoUser.Empresa,
                            codUserSap = retornoUser.codUsuario,
                            userCadastrado = false
                        };

                        return new JsonpResult(retorno);
                    }
                    else
                        return new JsonpResult(true, "Usuário com codigo: " + CodUserSap + " não foi encontrado");

                }
                else
                {
                    var retorno = new
                    {
                        nomeUsuario = user.Nome,
                        eMail = user.Email,
                        codUsuario = user.Codigo,
                        Empresa = retornoUser?.Empresa ?? "",
                        codUserSap = user.CodigoIntegracao,
                        userCadastrado = true
                    };

                    return new JsonpResult(retorno);
                }
            }
            catch (ServicoException ex)
            {
                //Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados de login do usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAnonymous]
        public async Task<IActionResult> ValidarSenhaUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string senha = Request.GetStringParam("senha");
                int codUser = Request.GetIntParam("codigo");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario user = repUsuario.BuscarPorCodigo(codUser, false);

                if (user == null)
                    return new JsonpResult(false, "Usuário não encontrado.");

                if (user.Senha == senha)
                {
                    base.SignIn(user);
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, "Senha incorreta");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados de login do usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        [AllowAnonymous]
        public async Task<IActionResult> CadastrarNovoUsuario()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //string token = Request.GetStringParam("token");
                string CodUserSap = Request.GetStringParam("codUserSap");
                string senha = Request.GetStringParam("Senha");
                string email = Request.GetStringParam("email");
                string nomeUsuario = Request.GetStringParam("nomeUsuario");
                string CPF = Request.GetStringParam("CPF");


                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario user = user = repUsuario.BuscarPorLogin(CodUserSap);
                if (user == null)
                    user = repUsuario.BuscarPorCodigoIntegracao(CodUserSap);

                if (user == null)
                {
                    user = new Dominio.Entidades.Usuario();
                    user.CPF = CPF;
                    user.Tipo = "U";
                    user.DataAdmissao = DateTime.Today;
                    user.Nome = nomeUsuario;
                    user.Senha = senha;
                    //user.Telefone = pessoa.Telefone1;
                    //user.Localidade = pessoa.Localidade;
                    //user.Endereco = pessoa.Endereco;
                    //user.Complemento = pessoa.Complemento;
                    user.Email = email;
                    user.UsuarioAdministrador = false;
                    user.Status = "A";
                    user.CodigoIntegracao = CodUserSap;
                    user.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Fornecedor;
                    user.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };
                    user.Empresa = this.Empresa;
                    //user.RG = pessoa.RG_Passaporte;

                    repUsuario.Inserir(user, Auditado);

                }

                return new JsonpResult(true);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao cadastrar o usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
