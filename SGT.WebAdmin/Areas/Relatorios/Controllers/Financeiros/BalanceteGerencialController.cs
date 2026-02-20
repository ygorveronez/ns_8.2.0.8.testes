using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/BalanceteGerencial")]
    public class BalanceteGerencialController : BaseController
    {
		#region Construtores

		public BalanceteGerencialController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R041_BalanceteGerencial;

        private decimal TamanhoColunaGrande = 6m;
        private decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Balancete Gerencial", "Financeiros", "BalanceteGerencial.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Plano", "asc", "", "", codigoRelatorio, unitOfWork, false, true);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.BalanceteGerencial servicoRelatorioBalanceteGerencial = new Servicos.Embarcador.Relatorios.Financeiros.BalanceteGerencial(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioBalanceteGerencial.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BalanceteGerencial> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Plano", "Plano", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Saldo Anterior", "SaldoInicial", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Entrada", "Entrada", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Saída", "Saida", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Saldo Atual", "SaldoFinal", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Número Centro de Resultado", "PlanoCentroResultado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Plano Conta", "CodigoPlanoConta", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Conta", "TipoContaDescricao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                TipoConta = Request.GetNullableEnumParam<AnaliticoSintetico>("TipoConta"),
                CodigoPlanoContaSintetica = Request.GetIntParam("PlanoContaSintetico"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.TipoAmbiente : 0,
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado")
            };

            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = filtrosPesquisa.CodigoPlanoContaSintetica > 0 ? repPlanoConta.BuscarPorCodigo(filtrosPesquisa.CodigoPlanoContaSintetica) : null;
            filtrosPesquisa.NumeroPlanoContaSintetica = planoConta?.Plano;

            return filtrosPesquisa;
        }

        #endregion
    }
}
