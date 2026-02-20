using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/CargaEntregaPedido")]
    public class CargaEntregaPedidoController : BaseController
    {
        #region Construtores

        public CargaEntregaPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private readonly CodigoControleRelatorios _codigoControleRelatorio = CodigoControleRelatorios.R250_CargaEntregaPedido;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Carga Entrega Pedido", "Cargas", "CargaEntregaPedido.rpt", OrientacaoRelatorio.Retrato, "NumeroCarga", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Carga.CargaEntregaPedido servicoRelatorioParadas = new Servicos.Embarcador.Relatorios.Carga.CargaEntregaPedido(unitOfWork, TipoServicoMultisoftware, Cliente);
                servicoRelatorioParadas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaEntregaPedido> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

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

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);

                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
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

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Grade da Carga", "GradeCarga", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ do Cliente", "CNPJClienteFormatado", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Nome do Cliente", "NomeCliente", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data de Início do Carregamento", "DataInicioCarregamentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Nº da Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Nº do Pedido", "NumeroPedido", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Nº EXP", "NumeroEXP", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Previsão de Entrega do Pedido", "PrevisaoEntregaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Chegada Cliente", "DataChegadaClienteFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Número do Redespacho", "NumeroRedespacho", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação do Monitoramento", "MonitoramentoStatusDescricao", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Entrega Realizada no Prazo?", "EntregaRealizadaNoPrazoDescricao", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data de Agendamento", "DataAgendamentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);

            return grid;
        }


        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido()
            {
                CargaAgendada = Request.GetNullableBoolParam("CargaAgendada"),
                CodigosFilial = codigosFilial.Count == 0 ? await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken) : codigosFilial,
                CodigosTransportadora = Request.GetListParam<int>("Transportadora"),
                DataFinalCriacao = Request.GetDateTimeParam("DataCriacaoFinal"),
                DataFinalEntrega = Request.GetDateTimeParam("DataEntregaFinal"),
                DataInicialCriacao = Request.GetDateTimeParam("DataCriacaoInicial"),
                DataInicialEntrega = Request.GetDateTimeParam("DataEntregaInicial"),
                PossuiRedespacho = Request.GetEnumParam("PossuiRedespacho", Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos),
                SituacaoCargas = Request.GetListEnumParam<SituacaoCarga>("SituacaoCarga"),
                TipoDestino = Request.GetNullableStringParam("TipoDestino[]"),
                CodigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                PrevisaoEntregaPlanejadaInicio = Request.GetDateTimeParam("PrevisaoEntregaPlanejadaInicio"),
                PrevisaoEntregaPlanejadaFinal = Request.GetDateTimeParam("PrevisaoEntregaPlanejadaFinal"),
                StatusMonitoramento = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus"),
            };
        }

        #endregion
    }
}
