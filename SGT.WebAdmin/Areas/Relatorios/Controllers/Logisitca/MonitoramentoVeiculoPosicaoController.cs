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
	[CustomAuthorize("Relatorios/Logistica/MonitoramentoVeiculoPosicao")]
    public class MonitoramentoVeiculoPosicaoController : BaseController
    {
		#region Construtores

		public MonitoramentoVeiculoPosicaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R326_MonitoramentoVeiculoPosicao;

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Posição de Veiculos ", "Logistica", "MonitoramentoVeiculoPosicao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa = ObterFiltrosPesquisa();
                if (string.IsNullOrEmpty(filtrosPesquisa.PlacaVeiculo))
                    return new JsonpResult(false, "Necessário preencher filtro de Placa do veículo.");

                if (!filtrosPesquisa.DataInicial.HasValue || filtrosPesquisa.DataInicial <= DateTime.MinValue)
                    return new JsonpResult(false, "Necessário preencher filtro de Data Inicial.");

                if (!filtrosPesquisa.DataFinal.HasValue || filtrosPesquisa.DataFinal <= DateTime.MinValue)
                    return new JsonpResult(false, "Necessário preencher filtro de Data Final.");

                if ((filtrosPesquisa.DataFinal.Value.Subtract(filtrosPesquisa.DataInicial.Value)).Days > 5)
                    return new JsonpResult(false, "Permitido filtrar até 5 dias da data inicial e data final.");

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Logistica.Monitoramento repositorio = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoPosicao> listaMonitoramentoVeiculoAlvos = repositorio.ConsultarRelatorioVeiculoPosicao(filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorioVeiculoPosicao(filtrosPesquisa, propriedades));
                grid.AdicionaRows(listaMonitoramentoVeiculoAlvos);

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
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                if (string.IsNullOrEmpty(filtrosPesquisa.PlacaVeiculo))
                    return new JsonpResult(false, "Necessário preencher filtro de Placa do veículo.");

                if(!filtrosPesquisa.DataInicial.HasValue || filtrosPesquisa.DataInicial <= DateTime.MinValue)
                    return new JsonpResult(false, "Necessário preencher filtro de Data Inicial.");

                if (!filtrosPesquisa.DataFinal.HasValue || filtrosPesquisa.DataFinal <= DateTime.MinValue)
                    return new JsonpResult(false, "Necessário preencher filtro de Data Final.");

                if ((filtrosPesquisa.DataFinal.Value.Subtract(filtrosPesquisa.DataInicial.Value)).Days > 5)
                    return new JsonpResult(false, "Permitido filtrar até 5 dias da data inicial e data final.");

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

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoPosicao> dataSourceMonitoramentoVeiculoAlvo = repositorioMonitoramento.ConsultarRelatorioVeiculoPosicao(filtrosPesquisa, propriedades, parametrosConsulta);
                List<Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/MonitoramentoVeiculoPosicao",parametros, relatorioControleGeracao, relatorioTemporario, dataSourceMonitoramentoVeiculoAlvo, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao()
            {
                PlacaVeiculo = Request.GetStringParam("PlacaVeiculo"),
                DataInicial = Request.GetDateTimeParam("DataInicial") ,
                DataFinal = Request.GetDateTimeParam("DataFinal")
            };


            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Data Veículo", "DataVeiculoFormatada", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Cadastro", "DataCadastroFormatada", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Placa", "PlacaVeiculo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Descricao", "Descricao", 25, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Latitude", "Latitude", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Longitude", "Longitude", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Ignição", "Ignicao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Velocidade Descrição", "VelocidadeDescricao", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Temperatura Descrição", "TemperaturaDescricao", 10, Models.Grid.Align.right, false);

            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa)
        {
            List<Parametro> parametros = new List<Parametro>();

            return parametros;
        }

        #endregion
    }
}
