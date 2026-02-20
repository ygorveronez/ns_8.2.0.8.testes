using System;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [AllowAnonymous]
    public class GetPedidoController : BaseController
    {
		#region Construtores

		public GetPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAnonymous]
        public async Task<IActionResult> ValidarToken()
        {
            string caminhoBaseViews = "~/Views/Pedidos/ConsultaPedido";

            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                MontaLayoutBase(unitOfWork);

                string token = Request.GetStringParam("token");
                string codUserSap = Request.GetStringParam("codUserSap");
                string numOv = Request.GetStringParam("numOv");

                ViewBag.codUserSap = codUserSap;
                ViewBag.numOv = numOv;
                ViewBag.Token = token;
                ViewBag.ClasseCorBotoes = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LayoutPersonalizadoFornecedor ?? "botoes-amarelo";

                var retorno = new
                {
                    Codigo = 1,
                    Pais = ""
                };

                ViewBag.ConfiguracaoPadrao = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);


                if (!ValidarEDefineParametrosViewConsultaPedido(token))
                    return View($"{caminhoBaseViews}/Erro.cshtml");

                return View($"{caminhoBaseViews}/GetPedido.cshtml");
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View($"~/Views/GestaoEntregas/Erro.cshtml", $"{caminhoBaseViews}/GetNumOVPedido.cshtml");
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

                Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Usuario retornoUser = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Usuario();
                retornoUser = servicoIntegracaoSaintGobain.ConsultarUsuarioAndamentoPedido(CodUserSap);

                if (retornoUser != null)
                {
                    var retorno = new
                    {
                        nomeUsuario = retornoUser.nomeUsuario,
                        eMail = retornoUser.eMail,
                        codUsuario = retornoUser.codUsuario,
                        Empresa = retornoUser.Empresa,
                        codUserSap = retornoUser.codUsuario,
                        tipoUsuario = retornoUser.tipoUsuario,
                        usuarioOk = true
                    };

                    return new JsonpResult(retorno);
                }
                else
                    return new JsonpResult(true, "Usuário com codigo: " + CodUserSap + " não foi encontrado");

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



        private void MontaLayoutBase(Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.UnitOfWork adminMultisoftwareUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminMultisoftwareUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(_conexao.ObterHost);

                Servicos.Embarcador.Configuracoes.Layout.MontarLayout(ViewBag, clienteURLAcesso, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                adminMultisoftwareUnitOfWork.Dispose();
            }
        }

        private bool ValidarEDefineParametrosViewConsultaPedido(string token)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
            Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorToken(token);

            if (integradora == null)
                return false;

            return true;
        }


    }
}
