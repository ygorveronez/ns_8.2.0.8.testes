using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Containers
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Containers/HistoricoMovimentacaoContainers")]
    public class HistoricoMovimentacaoContainersController : BaseController
    {
		#region Construtores

		public HistoricoMovimentacaoContainersController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R318_HistoricoMovimentacaoContainers;

        private decimal TamanhoColunaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório Historico Movimentação Containers", "Containers", "HistoricoMovimentacaoContainers.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Container", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Containers.HistoricoMovimentacaoContainers servicoHistoricoMovimentacaoContainers = new Servicos.Embarcador.Relatorios.Containers.HistoricoMovimentacaoContainers(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoHistoricoMovimentacaoContainers.ExecutarPesquisa(out List <Dominio.Relatorios.Embarcador.DataSource.Containers.HistoricoMovimentacaoContainers.HistoricoMovimentacaoContainers> listaHistoricoMovimentacaoContainers, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaHistoricoMovimentacaoContainers);

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
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
            grid.AdicionarCabecalho("Container", "CodigoContainer", TamanhoColunaExtraPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Carga", "Carga", TamanhoColunaExtraPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Auditado", "Auditado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Área Atual", "NomeCNPJ", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Situação Container", "SituacaoContainerDescricao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Data Histórico", "DataHistorico", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Tempo Histórico", "TempoHistorico", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Origem Movimentação", "OrigemDescricao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Informação da Movimentação", "InformacaoOrigemDescricao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer()
            {
                Carga = Request.GetStringParam("Carga"),
                SituacaoContainer = Request.GetNullableEnumParam<StatusColetaContainer>("SituacaoContainer"),
                Container = Request.GetStringParam("Container"),
                DataInicialColeta = Request.GetDateTimeParam("DataInicialColeta"),
                DataFinalColeta = Request.GetDateTimeParam("DataFinalColeta"),
                DiasPosseInicial = Request.GetIntParam("DiasPosseInicial"),
                DiasPosseFinal = Request.GetIntParam("DiasPosseFinal"),
                LocalAtual = Request.GetDoubleParam("LocalAtual"),
                LocalColeta = Request.GetDoubleParam("LocalColeta"),
                LocalEsperaVazio = Request.GetDoubleParam("LocalEsperaVazio"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                DataPorto = Request.GetDateTimeParam("DataPorto"),
                DataMovimentacao = Request.GetDateTimeParam("DataMovimentacao"),
                Filial = Request.GetIntParam("Filial"),
                TipoContainer = Request.GetIntParam("TipoContainer"),
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
