using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/AgendamentoEntregaPedido")]
    public class AgendamentoEntregaPedidoController : BaseController
    {
		#region Construtores

		public AgendamentoEntregaPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R265_AgendamentoEntregaPedido;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Agendamento Entrega Pedido", "Cargas", "AgendamentoEntregaPedido.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Carga", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.AgendamentoEntregaPedido servicoRelatorioAgendamentoEntregaPedido = new Servicos.Embarcador.Relatorios.Carga.AgendamentoEntregaPedido(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAgendamentoEntregaPedido.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AgendamentoEntregaPedido> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAcerto);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação do Agendamento", "SituacaoAgendamentoEntregaPedidoFormatada", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação Reagendamento", "ObservacaoReagendamento", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Cliente", "Cliente", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Sugestão de Entrega", "DataSugestaoEntregaFormatada", 15, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Início de Carregamento", "DataCarregamentoInicialFormatada", 15, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Término de Carregamento", "DataCarregamentoFinalFormatada", 15, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data de Agendamento", "DataAgendamentoFormatada", 15, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação da Viagem", "SituacaoCargaFormatada", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Previsão de Entrega", "DataPrevisaoEntregaFormatada", 15, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Criação Pedido", "DataCriacaoPedidoFormatada", 15, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Volumes", "QtdVolumes", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. M³", "QtdMetrosCubicos", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Contato Cliente", "ContatoCliente", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Contato Transportador", "ContatoTransportador", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 15, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega()
            {
                Carga = Request.GetStringParam("Carga"),
                CodigoCliente = Request.GetDoubleParam("Cliente"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataAgendamentoInicial = Request.GetDateTimeParam("DataAgendamentoInicial"),
                DataAgendamentoFinal = Request.GetDateTimeParam("DataAgendamentoFinal"),
                DataCarregamentoInicial = Request.GetDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetDateTimeParam("DataCarregamentoFinal"),
                SituacaoAgendamento = Request.GetNullableEnumParam<SituacaoAgendamentoEntregaPedido>("SituacaoAgendamento"),
                DataPrevisaoEntregaInicial = Request.GetDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetDateTimeParam("DataPrevisaoEntregaFinal"),
                DataCriacaoPedidoInicial = Request.GetDateTimeParam("DataCriacaoPedidoInicial"),
                DataCriacaoPedidoFinal = Request.GetDateTimeParam("DataCriacaoPedidoFinal"),
                PossuiDataTerminoCarregamento = Request.GetNullableBoolParam("PossuiDataTermioCarregamento"),
                PossuiDataSugestaoEntrega = Request.GetNullableBoolParam("PossuiDataSugestaoEntrega"),
                DataInicialSugestaoEntrega = Request.GetDateTimeParam("DataInicialSugestaoEntrega"),
                DataFinalSugestaoEntrega = Request.GetDateTimeParam("DataFinalSugestaoEntrega"),
                ExibirCargasAgrupadas = Request.GetBoolParam("ExibirCargasAgrupadas")
            };
        }

        #endregion
    }
}
