using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Escrituracao
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Escrituracao/IntegracaoLotePagamento")]
    public class IntegracaoLotePagamentoController : BaseController
    {
		#region Construtores

		public IntegracaoLotePagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R304_IntegracaoLotePagamento;

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Integração Lote de Pagamento", "Escrituracao", "IntegracaoLotePagamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Escrituracao.IntegracaoLotePagamento servicoRelatorioIntegracaoLotePagamento = new Servicos.Embarcador.Relatorios.Escrituracao.IntegracaoLotePagamento(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioIntegracaoLotePagamento.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.IntegracaoLotePagamento> listaIntegracaoLotePagamento, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaIntegracaoLotePagamento);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número do Documento", "NumeroDocumento", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série do Documento", "SerieDocumento", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Chave", "Chave", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão Documento", "DataEmissao", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Documento", "TipoDocumento", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Emissor (Transportador)", "Emissor", 8, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Pagamento", "NumeroPagamento", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação do Pagamento", "SituacaoPagamentoFormatada", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação da Integração", "SituacaoIntegracaoFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Retorno da Integração", "RetornoIntegracao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número cancelamento", "NumeroCancelamento", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação cancelamento", "SituacaoCancelamento", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo cancelamento", "MotivoCancelamento", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo do CTe", "ProtocoloCTe", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Envio Integração", "DataEnvioIntegracaoFormatada", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação da Carga", "SituacaoCargaFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }


        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoCTe = Request.GetIntParam("CTe"),
                NumeroPagamento = Request.GetIntParam("NumeroPagamento"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao"),
                SituacaoPagamento = Request.GetNullableEnumParam<SituacaoPagamento>("SituacaoPagamento"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                DataInicialEmissaoDocumento = Request.GetDateTimeParam("DataInicialEmissaoDocumento"),
                DataFinalEmissaoDocumento = Request.GetDateTimeParam("DataFinalEmissaoDocumento"),
                SituacaoCarga = Request.GetNullableEnumParam<SituacaoCarga>("SituacaoCarga"),
                ExibirUltimoRegistroQuandoExistirProtocoloCTeDuplicado = Request.GetBoolParam("ExibirUltimoRegistroQuandoExistirProtocoloCTeDuplicado"),
            };
        }

        #endregion
    }
}
