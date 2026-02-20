using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/AutorizacaoPagamentoCIOTParcela")]
    public class AutorizacaoPagamentoCIOTParcelaController : BaseController
    {
		#region Construtores

		public AutorizacaoPagamentoCIOTParcelaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AutorizarPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela tipoPagamento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela>("TipoParcelaPagamento");

                List<int> codigosCIOTs = repCIOT.ObterCodigosConsulta(filtros);

                if (codigosCIOTs.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CIOT's com os filtros realizados para autorizar o pagamento.");

                foreach (int codigoCIOT in codigosCIOTs)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                    if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado &&
                        ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                        continue;

                    if (ciot.SaldoPago &&
                        tipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Saldo)
                        continue;

                    if (ciot.AdiantamentoPago &&
                        tipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Adiantamento)
                        continue;

                    if (ciot.AbastecimentoPago &&
                        tipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Abastecimento)
                        continue;

                    if (!Servicos.Embarcador.CIOT.CIOT.IntegrarAutorizacaoPagamentoParcela(out string mensagemErro, tipoPagamento, ciot, Usuario, Auditado, TipoServicoMultisoftware, unitOfWork))
                    {
                        unitOfWork.Start();

                        ciot.Mensagem = mensagemErro;

                        repCIOT.Atualizar(ciot);

                        unitOfWork.CommitChanges();

                        return new JsonpResult(false, true, mensagemErro);
                    }
                    else
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, mensagemErro, unitOfWork);
                    }

                    unitOfWork.FlushAndClear();
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao autorizar o pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT()
            {
                CPFCNPJTransportador = Request.GetNullableDoubleParam("Transportador"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>("Situacao"),
                SelecionarTodos = Request.GetNullableBoolParam("SelecionarTodos"),
                ListaCodigosCIOT = Request.GetNullableListParam<int>("ListaCodigosCIOT"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                TipoAutorizacaoPagamentoCIOTParcela = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela>("TipoPagamento")
            };
        }

        private Models.Grid.Grid ObterConfiguracaoGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cód. Verificador", "CodigoVerificador", 6, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Protocolo", "ProtocoloAutorizacao", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Final da Viagem", "DataFinalViagem", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nº Ped. Embarcador", "NumeroPedidoEmbarcador", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cargas", "Cargas", 6, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor CT-e", "ValorCTE", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Acréscimos", "Acrescimos", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Descontos", "Descontos", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Adiantamento", "ValorAdiantamento", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Saldo", "Saldo", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Sit. Adiantamento", "SituacaoAdiantamento", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Sit. Abastecimento", "SituacaoAbastecimento", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Sit. Saldo", "SituacaoSaldo", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 10, Models.Grid.Align.left, false);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterConfiguracaoGrid();
                Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                int totalRegistros = repCIOT.ContarConsulta(filtros);

                List<Dominio.Entidades.Embarcador.Documentos.CIOT> listaCIOT = totalRegistros > 0 ? repCIOT.Consultar(filtros, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Documentos.CIOT>();

                var retorno = (from obj in listaCIOT
                               select new
                               {
                                   obj.Codigo,
                                   Numero = obj.Numero,
                                   CodigoVerificador = obj.CodigoVerificador,
                                   obj.ProtocoloAutorizacao,
                                   DataFinalViagem = obj.DataFinalViagem.ToString("dd/MM/yyyy"),
                                   Transportador = obj.Transportador?.Descricao,
                                   ValorCTE = obj.CTes?.Sum(o => o.CargaCTe?.Carga?.ValorAReceberCTes),
                                   Acrescimos = obj.CargaCIOT?.Sum(o => o.ContratoFrete?.ValoresAdicionais.Where(va => va.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Sum(va => va.Valor)),
                                   Descontos = obj.CargaCIOT?.Sum(o => o.ContratoFrete?.ValoresAdicionais.Where(va => va.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Sum(va => va.Valor)),
                                   ValorAdiantamento = obj.CargaCIOT?.Sum(o => o.ContratoFrete?.ValorAdiantamento)?.ToString("n2"),
                                   Saldo = obj.CargaCIOT?.Sum(o => o.ContratoFrete?.SaldoAReceber)?.ToString("n2"),
                                   Situacao = obj.DescricaoSituacao,
                                   NumeroPedidoEmbarcador = string.Join(", ", obj.CargaCIOT?.Select(o => string.Join(", ", o.Carga.Pedidos.Select(p => p.Pedido.NumeroPedidoEmbarcador)))),
                                   Cargas = string.Join(", ", obj.CargaCIOT?.Select(o => o.Carga.CodigoCargaEmbarcador)),
                                   Mensagem = obj.Mensagem,
                                   SituacaoAdiantamento = obj.AdiantamentoPago ? "Pago" : "Pendente",
                                   SituacaoSaldo = obj.SaldoPago ? "Pago" : "Pendente",
                                   SituacaoAbastecimento = obj.AbastecimentoPago ? "Pago" : "Pendente"
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}
