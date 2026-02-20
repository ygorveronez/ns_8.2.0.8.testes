using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Logisitca
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Logistica/MonitoramentoAlerta")]
    public class MonitoramentoAlertaController : BaseController
    {
        #region Construtores

        public MonitoramentoAlertaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R247_MonitoramentoAlerta;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Alerta", "Logistica", "MonitoramentoAlerta.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Logistica.MonitoramentoAlerta svcMonitoramentoAlerta = new Servicos.Embarcador.Relatorios.Logistica.MonitoramentoAlerta(unitOfWork, TipoServicoMultisoftware, Cliente);

                svcMonitoramentoAlerta.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta> listaMonitoramentoAlerta, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaMonitoramentoAlerta);

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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
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

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigGeral = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral = repConfigGeral.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta> dataSourceMonitoramentoTratativaAlerta = repMonitoramento.ConsultarRelatorioAlerta(filtrosPesquisa, propriedades, parametrosConsulta);
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/MonitoramentoAlerta", new List<Parametro>(), relatorioControleGeracao, relatorioTemporario, dataSourceMonitoramentoTratativaAlerta, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                Placa = Request.GetStringParam("Placa"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Motorista = Request.GetIntParam("Motorista"),
                Transportador = Request.GetIntParam("Transportador"),
                AlertaMonitorStatus = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus>("AlertaMonitorStatus"),
                TipoAlerta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>("TipoAlerta"),
                ApenasComPosicaoTardia = Request.GetBoolParam("ExibirApenasComPosicaoTardia"),
                Filiais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Número Alerta", "Codigo", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Alerta", "NomeAlerta", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo", "TipoDescricao", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor", "Descricao", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Status", "StatusDescricao", 5, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data do evento do alerta", "DataFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data criação do alerta", "DataCriacaoFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Latitude", "LatitudeFormatada", 5, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Longitude", "LongitudeFormatada", 5, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 5, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de operação", "TipoOperacao", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Placa", "Placa", 5, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 5, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 5, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data da tratativa", "DataTratativaFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Usuário", "Usuario", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tratativa", "Acao", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Posição Retroativa", "AlertaPossuiPosicaoRetroativaDescricao", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Finalizada Sem Ret.Sinal", "FinalizadoSemRetornoSinalDescricao", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Centro resultado da carga", "CentroResultadoCarga", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF Motorista", "CPFMotorista", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Causa", "Causa", 10, Models.Grid.Align.left, true, false, false, true, false);
            return grid;
        }

        #endregion
    }
}
