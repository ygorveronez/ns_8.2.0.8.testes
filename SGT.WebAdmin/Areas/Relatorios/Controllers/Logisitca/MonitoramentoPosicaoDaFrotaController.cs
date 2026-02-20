using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;


namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Logisitca
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/MonitoramentoPosicaoDaFrota")]
    public class MonitoramentoPosicaoDaFrotaController : BaseController
    {
		#region Construtores

		public MonitoramentoPosicaoDaFrotaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R178_MonitoramentoPosicaoDaFrota;

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Posição da Frota", "Logistica", "MonitoramentoPosicaoDaFrota.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Logistica.Monitoramento repositorio = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoDaFrota> listaMonitoramentoPosicaoDaFrotas = repositorio.ConsultarRelatorioPosicaoDaFrota(filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorioPosicaoDaFrota(filtrosPesquisa, propriedades));
                grid.AdicionaRows(listaMonitoramentoPosicaoDaFrotas);

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
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoDaFrota> dataSourceMonitoramentoPosicaoDaFrota = repositorioMonitoramento.ConsultarRelatorioPosicaoDaFrota(filtrosPesquisa, propriedades, parametrosConsulta);
                List<Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/MonitoramentoPosicaoDaFrota",parametros,relatorioControleGeracao, relatorioTemporario, dataSourceMonitoramentoPosicaoDaFrota, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota()
            {
                PlacaVeiculo = Request.GetStringParam("PlacaVeiculo"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Placa", "PlacaVeiculo", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Veículo", "TipoVeiculo", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Baú", "TipoBau", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Status", "StatusDescricao", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Posição", "Posicao", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Latitude/Longitude", "LatLng", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Regiao", "Regiao", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de transporte", "TipoDeTransporte", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 8, Models.Grid.Align.center, false, false, false, true, true);


            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa)
        {
            List<Parametro> parametros = new List<Parametro>();

            return parametros;
        }

        #endregion
    }
}
