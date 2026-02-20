using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Produtos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Produtos/Produto")]
    public class ProdutoController : BaseController
    {
		#region Construtores

		public ProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R212_Produto;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Produtos", "Produtos", "Produto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Produtos.Produto servicoRelatorioProduto = new Servicos.Embarcador.Relatorios.Produtos.Produto(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioProduto.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Produtos.Produto> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa = ObterFiltrosPesquisa();

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

        public async Task<IActionResult> GerarEtiquetas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            TipoArquivo tipoArquivo = Request.GetEnumParam("TipoArquivo", TipoArquivo.PDF);
            try
            {

                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Produto repositorioProduto = new Repositorio.Produto(unitOfWork);

                int totalRegistros = repositorioProduto.ContarConsultaRelatorioProdutos(filtrosPesquisa);

                if (totalRegistros == 0)
                    return new JsonpResult(false, "Nenhum produto encontrado");

                Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo = servicoArquivo.Adicionar("Etiquetas Productos", Usuario, tipoArquivo);

                string conexaoString = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarEtiquetas(conexaoString, filtrosPesquisa, controleGeracaoArquivo));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um error ao gerar etiquetas");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto()
            {
                CodigoNCM = Request.GetStringParam("CodigoNCM"),
                CodigoCEST = Request.GetStringParam("CodigoCEST"),
                CodigoBarrasEAN = Request.GetStringParam("CodigoBarrasEAN"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigoGrupo = Request.GetIntParam("Grupo"),
                CodigoMarca = Request.GetIntParam("Marca"),
                CodigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamento"),
                CodigoGrupoImposto = Request.GetIntParam("GrupoImposto"),
                Status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status"),
                CategoriaProduto = Request.GetNullableEnumParam<CategoriaProduto>("CategoriaProduto"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Código", "Codigo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Descrição Nota Fiscal", "DescricaoNotaFiscal", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("UN", "UnidadeMedidaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Produto", "CodigoProduto", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("NCM", "CodigoNCM", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CEST", "CodigoCEST", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código EAN", "CodigoBarrasEAN", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Barras", "CodigoBarras", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código Anvisa", "CodigoAnvisa", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Código ANP", "CodigoANP", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Status", "Status", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria", "CategoriaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Origem Mercadoria", "OrigemMercadoriaFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Gênero Produto", "GeneroProdutoFormatado", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Último Custo", "UltimoCusto", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Custo Médio", "CustoMedio", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Margem Lucro", "MargemLucro", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Venda", "ValorVenda", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor Mínimo Venda", "ValorMinimoVenda", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso Bruto", "PesoBruto", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso Líquido", "PesoLiquido", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Marca", "Marca", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Local Armazenamento", "LocalArmazenamento", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Imposto", "GrupoImposto", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        private async Task GerarEtiquetas(string stringConexao, Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
            Servicos.Embarcador.Produto.Produto servicoProduto = new Servicos.Embarcador.Produto.Produto(unitOfWork);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    PropriedadeOrdenar = "Codigo"
                };

                Repositorio.Produto repositorioProduto = new Repositorio.Produto(unitOfWork);
                List<Dominio.Entidades.Produto> produtos = null;

                // TODO (ct-reports): Repassar CT
                produtos = repositorioProduto.ConsultarProdutos(filtrosPesquisa, parametrosConsulta);

                if (produtos.Count == 0)
                    servicoArquivo.Remover(controleGeracaoArquivo);

                if (produtos.Count > 0)
                {
                    byte[] pdfGeracaoEtiqueta = controleGeracaoArquivo.TipoArquivo == TipoArquivo.PDF ? servicoProduto.ObterPdfTodasEtiquetasProduto(produtos) : servicoProduto.ObterPdfTodasEtiquetasCompactadas(produtos);
                    servicoArquivo.SalvarArquivo(controleGeracaoArquivo, pdfGeracaoEtiqueta);
                    servicoArquivo.Finalizar(controleGeracaoArquivo, nota: string.Format("Geração de etiquetas finalizada", controleGeracaoArquivo.TipoArquivo.ObterDescricao()), urlPagina: "Relatorios/Produtos/Produto");
                }

            }
            catch (Exception excecao)
            {
                servicoArquivo.FinalizarComFalha(controleGeracaoArquivo, nota: string.Format("Ocorreu uma falha ao gerar Arquivo de gerar etiquetas", controleGeracaoArquivo.TipoArquivo.ObterDescricao()), urlPagina: "Relatorios/Produtos/Produto", excecao: excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion
    }
}
