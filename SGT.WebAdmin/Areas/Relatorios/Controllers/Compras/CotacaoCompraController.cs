using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Compras
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Compras/CotacaoCompra")]
    public class CotacaoCompraController : BaseController
    {
		#region Construtores

		public CotacaoCompraController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R203_CotacaoCompra;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Cotação de Compra", "Compras", "CotacaoCompra.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Compras.CotacaoCompra servicoRelatorioCotacaoCompra = new Servicos.Embarcador.Relatorios.Compras.CotacaoCompra(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCotacaoCompra.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompra> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa = ObterFiltrosPesquisa();

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

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 5;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequena = 3;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("NumeroCotacao").Nome("Nº Cotação").Tamanho(TamanhoColunasPequena).Align(Models.Grid.Align.right).Agr(true);
            grid.Prop("DescricaoCotacao").Nome("Descrição").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataEmissaoFormatada").Nome("Data Emissão").Tamanho(TamanhoColunasPequena).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Fornecedor").Nome("Fornecedor").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("Produto").Nome("Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);

            grid.Prop("Quantidade").Nome("Qtd.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("QuantidadeRetornado").Nome("Qtd. Ret.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("ValorUnitario").Nome("Vlr. Unit.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("ValorUnitarioRetornado").Nome("Vlr. Unit. Ret.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("ValorTotal").Nome("Vlr. Total").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("ValorTotalRetornado").Nome("Vlr. Total Ret.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigosProduto = Request.GetListParam<int>("Produto"),
                CodigosFornecedor = Request.GetListParam<double>("Fornecedor")
            };
        }

        #endregion
    }
}
