using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/MovimentoFinanceiro")]
    public class MovimentoFinanceiroController : BaseController
    {
		#region Construtores

		public MovimentoFinanceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R259_MovimentoFinanceiro;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Movimentação Financeira", "Financeiros", "MovimentoFinanceiro.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "RazaoSocial", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.MovimentoFinanceiro servicoRelatorioMovimentoFinanceiro = new Servicos.Embarcador.Relatorios.Financeiros.MovimentoFinanceiro(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioMovimentoFinanceiro.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiro> listaMovimentoFinanceiro, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaMovimentoFinanceiro);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            decimal TamanhoColunaPequena = 2.25m;
            decimal TamanhoColunaGrande = 5m;
            decimal TamanhoColunaMedia = 3.5m;

            grid.AdicionarCabecalho("Código", "Codigo", TamanhoColunaPequena, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Numero Documento", "NumeroDocumento", TamanhoColunaMedia, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Conta de Entrada", "PlanoDebito", TamanhoColunaGrande, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Conta de Saída", "PlanoCredito", TamanhoColunaGrande, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo Movimento", "TipoMovimento", TamanhoColunaMedia, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Valor", "ValorMovimento", TamanhoColunaMedia, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", TamanhoColunaMedia, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data", "Data", TamanhoColunaMedia, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Data Base", "DataBaseFinanceiro", TamanhoColunaMedia, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Grupo Pessoas", "GrupoPessoa", TamanhoColunaMedia, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunaMedia, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Usuario", "Usuario", TamanhoColunaMedia, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", TamanhoColunaMedia, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Conta Gerencial Entrada", "ContaPlanoDebito", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Código da Conta Entrada", "CodigoPlanoDebito", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Conta Gerencial Saída", "ContaPlanoCredito", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Código da Conta Saída", "CodigoPlanoCredito", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Cód. Conta Contábil Pessoa", "ContaFornecedorEBS", TamanhoColunaGrande, Models.Grid.Align.left, false, false);


            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro()
            {
                DataMovimentoInicial = Request.GetDateTimeParam("DataMovimentoInicial"),
                DataMovimentoFinal = Request.GetDateTimeParam("DataMovimentoFinal"),
                DataBaseFinanceiro = Request.GetDateTimeParam("DataBaseFinanceiro"),
                ValorMovimento = Request.GetDecimalParam("ValorMovimento"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                Observacao = Request.GetStringParam("Observacao"),
                TipoMovimento = Request.GetIntParam("TipoMovimento"),
                CentroResultado = Request.GetIntParam("CentroResultado"),
                PlanoCredito = Request.GetIntParam("PlanoCredito"),
                PlanoDebito = Request.GetIntParam("PlanoDebito"),
                Pessoa = Request.GetIntParam("Pessoa"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                Codigo = Request.GetIntParam("Codigo"),
                VisualizarTitulosPagamentoSalario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Usuario.PermiteVisualizarTitulosPagamentoSalario : true
            };
        }

        #endregion
    }
}
