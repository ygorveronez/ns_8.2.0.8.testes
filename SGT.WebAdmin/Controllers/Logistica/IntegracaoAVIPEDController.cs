using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Logistica/IntegracaoAVIPED", "Logistica/JanelaCarregamento")]
    public class IntegracaoAVIPEDController : BaseController
    {
		#region Construtores

		public IntegracaoAVIPEDController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                if (codigoCarga == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Número Aviso Recebimento", "NumeroAvisoRecebimento", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Número Pedido Compra", "NumeroPedidoCompra", 20, Models.Grid.Align.left);

                Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);

                int qtd = repIntegracaoAVIPED.ContarConsultaPorCarga(codigoCarga);
                grid.setarQuantidadeTotal(qtd);

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED> listaIntegracoes = qtd > 0 ? repIntegracaoAVIPED.ConsultarPorCarga(codigoCarga, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>();

                var dynRetorno = (from obj in listaIntegracoes
                                  select new
                                  {
                                      obj.Codigo,
                                      NumeroPedido = obj.CargaPedido.Pedido.NumeroPedidoEmbarcador,
                                      NotaFiscal = obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                                      obj.NumeroAvisoRecebimento,
                                      obj.NumeroPedidoCompra,
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as integrações realizadas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                if (codigoCarga == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED> listaIntegracoes = repIntegracaoAVIPED.BuscarPorCarga(codigoCarga) ?? new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED>();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCarga(codigoCarga);

                if (listaPedidoXMLNotaFiscal.Count == 0)
                    throw new ControllerException("Notas fiscais não encontradas.");

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED> integracoesDeletar = listaIntegracoes.Where(o => !listaPedidoXMLNotaFiscal.Contains(o.PedidoXMLNotaFiscal)).ToList();

                foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED integracaoDeletar in integracoesDeletar)
                    repIntegracaoAVIPED.Deletar(integracaoDeletar);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in listaPedidoXMLNotaFiscal)
                {
                    Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED integracao = listaIntegracoes.Find(o => o.PedidoXMLNotaFiscal.Codigo == pedidoXMLNotaFiscal.Codigo);

                    if (integracao != null)
                        continue;

                    integracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED()
                    {
                        CargaPedido = pedidoXMLNotaFiscal.CargaPedido,
                        PedidoXMLNotaFiscal = pedidoXMLNotaFiscal,
                    };

                    repIntegracaoAVIPED.Inserir(integracao);
                    listaIntegracoes.Add(integracao);
                }

                Servicos.Embarcador.Integracao.Boticario.IntegracaoBoticario integracaoBoticario = new Servicos.Embarcador.Integracao.Boticario.IntegracaoBoticario(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED integracao in listaIntegracoes)
                {
                    integracaoBoticario.IntegrarAVIPED(integracao);
                    repIntegracaoAVIPED.Atualizar(integracao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Sucessos = listaIntegracoes.Where(o => string.IsNullOrEmpty(o.Mensagem)).ToList().Count,
                    Erros = listaIntegracoes.Where(o => !string.IsNullOrEmpty(o.Mensagem)).ToList().Count,
                }, true, Localization.Resources.Gerais.Geral.Sucesso);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoAVIPED integracao = repIntegracaoAVIPED.BuscarPorCodigo(codigo);

                if (integracao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                if (!repIntegracaoAVIPED.CompararComNotasDoPedido(integracao.Codigo))
                {
                    repIntegracaoAVIPED.Deletar(integracao);
                    unitOfWork.CommitChanges();

                    return new JsonpResult(true, "Nota fiscal não existe mais no pedido.");
                }

                Servicos.Embarcador.Integracao.Boticario.IntegracaoBoticario integracaoBoticario = new Servicos.Embarcador.Integracao.Boticario.IntegracaoBoticario(unitOfWork);
                integracaoBoticario.IntegrarAVIPED(integracao);

                repIntegracaoAVIPED.Atualizar(integracao);
                unitOfWork.CommitChanges();

                if (!string.IsNullOrEmpty(integracao.Mensagem))
                    return new JsonpResult(false, true, integracao.Mensagem);

                return new JsonpResult(new
                {
                    integracao.NumeroAvisoRecebimento,
                    integracao.NumeroPedidoCompra,
                    Sucesso = true,
                }, true, Localization.Resources.Gerais.Geral.Sucesso);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
