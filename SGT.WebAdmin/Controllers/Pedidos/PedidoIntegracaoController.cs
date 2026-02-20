using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    public class PedidoIntegracaoController : BaseController
    {
		#region Construtores

		public PedidoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> CarregarDadosTotalizadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);

                int codigoPedido = 0;
                int.TryParse(Request.Params("CodigoPedido"), out codigoPedido);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                var dynRetorno = new
                {
                    Codigo = pedido.Codigo,
                    TotalPedido = repPedidoIntegracao.TotalArquivos(codigoPedido).ToString("n0"),
                    AguardandoIntegracaoPedido = repPedidoIntegracao.TotalArquivosStatus(codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao).ToString("n0"),
                    IntegradoPedido = repPedidoIntegracao.TotalArquivosStatus(codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado).ToString("n0"),
                    RejeitadoPedido = repPedidoIntegracao.TotalArquivosStatus(codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao).ToString("n0")
                };

                return new JsonpResult(dynRetorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultarTotalizadoresIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaIntegracaoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPedido);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao;
                Enum.TryParse(Request.Params("TipoConsultaPedido"), out situacao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.Codigo), false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Integracao, nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.TipoIntegracao), 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.NumeroTentativas, nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.Tentativas), 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DataEnvio, nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.DataEnvio), 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.SituacaoIntegracao), 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Retorno, nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.ProblemaIntegracao), 45, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.TipoIntegracao))
                    propOrdenar = $"{nameof(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao.TipoIntegracao)}.{nameof(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao.Descricao)}";

                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> listaPedidoIntegracao = repPedidoIntegracao.BuscarPorPedido(codigoPedido, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoIntegracao.ContarBuscarPorPedido(codigoPedido, situacao));
                var dynRetorno = (from obj in listaPedidoIntegracao
                                  select new
                                  {
                                      obj.Codigo,
                                      TipoIntegracao = obj.TipoIntegracao?.Descricao ?? "",
                                      SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                      Tentativas = obj.Tentativas.ToString(),
                                      DataEnvio = obj.DataEnvio?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                                      obj.ProblemaIntegracao,
                                      DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ? "#ADD8E6" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? "#DFF0D8" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#C16565" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? "#F7F7BA" :
                                                    "#FFFFFF",
                                      DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#FFFFFF" : "#666666"
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultarIntegracoesPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarLayoutPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoIntegracao = 0;
                int.TryParse(Request.Params("Codigo"), out codigoIntegracao);

                int codigoPedido = 0;
                int.TryParse(Request.Params("CodigoPedido"), out codigoPedido);

                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao integracao = null;
                if (codigoIntegracao > 0)
                    integracao = repPedidoIntegracao.BuscarPorCodigo(codigoIntegracao);
                else if (codigoPedido > 0)
                    integracao = repPedidoIntegracao.BuscarLayoutPedidoPorPedido(codigoPedido);

                if (integracao == null)
                    return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.NaoFoiPossivelLocalizarIntegracaoPedido);

                if (integracao.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto)
                    return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.EstePedidoNaoEncontraAbertoParaEnviar);

                unitOfWork.Start();

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                integracao.IniciouConexaoExterna = false;
                integracao.ProblemaIntegracao = string.Empty;

                repPedidoIntegracao.Atualizar(integracao);

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, Localization.Resources.Pedidos.Pedido.ReenviouIntegracaoPedidoPonto, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Pedido, null, string.Format(Localization.Resources.Pedidos.Pedido.ReenviouIntegracaoPedido, integracao.Descricao), unitOfWork);

                Servicos.Embarcador.Pedido.Pedido.ProcessarPedidoIntegracao(unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoEnviarIntegracaoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarTodosLayoutPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPedido);

                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> listaIntegracao = repPedidoIntegracao.BuscarPorPedido(codigoPedido);
                unitOfWork.Start();
                foreach (var integracao in listaIntegracao)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.IniciouConexaoExterna = false;
                    integracao.ProblemaIntegracao = string.Empty;

                    repPedidoIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, Localization.Resources.Pedidos.Pedido.ReenviouIntegracaoPonto, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Pedido, null, string.Format(Localization.Resources.Pedidos.Pedido.ReenviouIntegracao, integracao.Descricao), unitOfWork);
                }
                unitOfWork.CommitChanges();

                Servicos.Embarcador.Pedido.Pedido.ProcessarPedidoIntegracao(unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoEnviarLayoutEDI);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.PedidoIntegracao repIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Data, "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.PedidoIntegracao repIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Pedidos.Pedido.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Pedidos.Pedido.NaoHaRegistrosArquivosSalvosParaEsteHistoricoConsulta);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", string.Format(Localization.Resources.Pedidos.Pedido.ArquivosIntegracaoPedido, integracao.Pedido.Numero));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoRealizarDownloadIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
