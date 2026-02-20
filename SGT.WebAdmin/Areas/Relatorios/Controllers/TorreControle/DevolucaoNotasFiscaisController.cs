using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.TorreControle
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/TorreControle/DevolucaoNotasFiscais")]
    public class DevolucaoNotasFiscaisController : BaseController
    {
        #region Construtores

        public DevolucaoNotasFiscaisController(Conexao conexao) : base(conexao)
        {
        }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R355_DevolucaoNotasFiscais;
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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Devoluções Por Notas Fiscais", "TorreControle", "", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NotaFiscal", "desc", "", "", codigoRelatorio, unitOfWork, false, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.ErroBuscarDados);
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
                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.TorreControle.DevolucaoNotasFiscais servicoRelatorioDevolucaoNotasFiscais = new Servicos.Embarcador.Relatorios.TorreControle.DevolucaoNotasFiscais(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioDevolucaoNotasFiscais.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.DevolucaoNotasFiscais> listaDevolucaoNotasFiscais, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaDevolucaoNotasFiscais);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.ErroBuscarDados);
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
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa = ObterFiltrosPesquisa();

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, Dominio.Enumeradores.TipoArquivoRelatorio.CSV, null, unitOfWork, TipoServicoMultisoftware);

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
                return new JsonpResult(false, Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.ErroGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais()
            {
                DataInicialEmissaoNFD = Request.GetDateTimeParam("DataInicialEmissaoNFD"),
                DataFinalEmissaoNFD = Request.GetDateTimeParam("DataFinalEmissaoNFD"),
                DataInicialChamado = Request.GetDateTimeParam("DataInicialChamado"),
                DataFinalChamado = Request.GetDateTimeParam("DataFinalChamado"),
                CodigosNotaFiscalDevolucao = Request.GetStringParam("NotaFiscalDevolucao"),
                CodigosNotaFiscalOrigem = Request.GetListParam<int>("NotaFiscalOrigem"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigosGrupoTipoOperacao = Request.GetListParam<int>("GrupoTipoOperacao"),
                CodigosCargas = Request.GetListParam<int>("Carga"),
                CodigosChamados = Request.GetListParam<int>("Chamado"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoCliente = Request.GetIntParam("Cliente"),
                PedidoEmbarcador = Request.GetStringParam("PedidoEmbarcador"),
                PedidoCliente = Request.GetStringParam("PedidoCliente"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork, HttpRequest request = null)
        {
            Models.Grid.Grid grid;

            grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>(),
                listTabs = new List<Models.Grid.Tab>(),
            };

            grid.AdicionarTab("Nota Fiscal", out Models.Grid.Tab tabNotaFiscal);
            grid.AdicionarTab("Produto", out Models.Grid.Tab tabProdutos);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DataEmissaoNFOrigem, "DataEmissaoNFOrigemFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DataEmissaoNFD, "DataEmissaoNFDFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.NotaFiscalOrigem, "NotaFiscalOrigem", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Carga, "Carga", _tamanhoColunaPequena, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.GrupoTipoOperacao, "GrupoTipoOperacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.TipoOperacao, "TipoOperacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Chamado, "Chamado", _tamanhoColunaPequena, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.UFOrigemNota, "UFOrigemNota", _tamanhoColunaPequena, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.CidadeOrigemNota, "CidadeOrigemNota", _tamanhoColunaMedia, Models.Grid.Align.left, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Filial, "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DataFinalizacaoOcorrencia, "DataFinalizacaoOcorrenciaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.PedidoEmbarcador, "PedidoEmbarcador", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.PedidoCliente, "PedidoCliente", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.MotivoAtendimento, "MotivoAtendimento", _tamanhoColunaMedia, Models.Grid.Align.left, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.CNPJTransportadora, "CNPJTransportadora", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Transportadora, "Transportadora", _tamanhoColunaGrande, Models.Grid.Align.left, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.NumeroNotaDevolucao, "NumeroNotaDevolucao", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.TipoDevolucao, "TipoDevolucaoFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.CodigoIntegracaoCliente, "CodigoIntegracaoCliente", _tamanhoColunaPequena, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Cliente, "Cliente", _tamanhoColunaGrande, Models.Grid.Align.left, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.ValorNotaFiscalOrigem, "ValorNotaFiscalOrigem", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.PesoNotaFiscalOrigem, "PesoNotaFiscalOrigem", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabNotaFiscal);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.CodigoProduto, "CodigoProduto", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabProdutos);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DescricaoProduto, "DescricaoProduto", _tamanhoColunaGrande, Models.Grid.Align.left, true, true, tabProdutos);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.QuantidadeDevolvida, "QuantidadeDevolvida", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabProdutos);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.PesoProdutoNFD, "PesoProdutoNFD", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabProdutos);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.ValorProdutoNFD, "ValorProdutoNFD", _tamanhoColunaMedia, Models.Grid.Align.center, true, true, tabProdutos);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "/Relatorios/DevolucaoNotasFiscais/Pesquisa", "gridPreviewRelatorio");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;

        }

        #endregion
    }
}
