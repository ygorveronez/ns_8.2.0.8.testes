using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/DescontoAcrescimoFatura")]
    public class DescontoAcrescimoFaturaController : BaseController
    {
		#region Construtores

		public DescontoAcrescimoFaturaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R046_DescontoAcrescimoFatura;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Descontos e Acréscimos Aplicados em Faturas", "Financeiros", "DescontoAcrescimoFatura.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroFatura", "asc", "", "", Codigo, unitOfWork, true, false);
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
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.DescontoAcrescimoFatura servicoRelatorioDescontoAcrescimoFatura = new Servicos.Embarcador.Relatorios.Financeiros.DescontoAcrescimoFatura(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioDescontoAcrescimoFatura.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoFatura> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAcerto);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura filtrosPesquisa = ObterFiltrosPesquisa();

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

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura()
            {
                Pessoa = Request.GetDoubleParam("Pessoa"),
                ConhecimentoDeTransporte = Request.GetIntParam("ConhecimentoDeTransporte"),
                Fatura = Request.GetIntParam("Fatura"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                DataInicialEmissao = Request.GetNullableDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetNullableDateTimeParam("DataFinalEmissao"),
                DataInicialQuitacao = Request.GetNullableDateTimeParam("DataInicialQuitacao"),
                DataFinalQuitacao = Request.GetNullableDateTimeParam("DataFinalQuitacao"),
            };
        }

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Número Fatura", "NumeroFatura", 8, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Inicial", "DataInicial", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Final", "DataFinal", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Total Acréscimos", "TotalAcrescimos", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Total Descontos", "TotalDescontos", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Total Fatura", "TotalFatura", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Pessoa", "Grupo", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Data Quitação", "DataQuitacao", 8, Models.Grid.Align.center, true, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
