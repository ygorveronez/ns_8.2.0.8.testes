using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/HistoricoEstoque")]
    public class HistoricoEstoqueController : BaseController
    {
		#region Construtores

		public HistoricoEstoqueController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R020_HistoricoEstoque;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Histórico de Estoque", "NFe", "HistoricoEstoque.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
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
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFe.HistoricoEstoque servicoRelatorioHistoricoEstoque = new Servicos.Embarcador.Relatorios.NFe.HistoricoEstoque(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioHistoricoEstoque.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoEstoque> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

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
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque filtrosPesquisa = ObterFiltrosPesquisa();

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

            grid.AdicionarCabecalho("Código", "Codigo", 5, Models.Grid.Align.center, true, true, false, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 16, Models.Grid.Align.left, true, true, false, true);
            grid.AdicionarCabecalho("Cód. Produto", "CodigoProduto", 6, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Categoria", "DescricaoCategoria", 8, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Data", "DataFormatada", 5, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Tipo", "TipoMovimento", 5, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Quantidade", "QuantidadeFormatado", 5, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Documento", "Documento", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Documento", "TipoDocumento", 6, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Estoque Atual", "QuantidadeEstoqueFormatado", 8, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Custo Médio", "CustoMedio", 8, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Custo / Valor", "Custo", 8, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Empresa", "Empresa", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Qtd. Acumulada", "QuantidadeAcumuladaFormatada", 8, Models.Grid.Align.right, false, false);

            if (ConfiguracaoEmbarcador.UtilizaMultiplosLocaisArmazenamento)
                grid.AdicionarCabecalho("Local Armazenamento", "LocalArmazenamento", 10, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigoGrupoProduto = Request.GetIntParam("GrupoProduto"),
                CodigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamentoProduto"),
                Status = Request.GetStringParam("Status"),
                Categoria = !string.IsNullOrWhiteSpace(Request.GetStringParam("Categoria")) ? Request.GetEnumParam<CategoriaProduto>("Categoria") : CategoriaProduto.Todos,
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Empresa"),
            };
        }

        #endregion
    }
}
