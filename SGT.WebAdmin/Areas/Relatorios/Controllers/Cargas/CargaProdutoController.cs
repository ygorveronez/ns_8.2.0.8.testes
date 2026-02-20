using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/CargaProduto")]
    public class CargaProdutoController : BaseController
    {
        #region Construtores

        public CargaProdutoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R146_CargaProduto;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;
        private readonly decimal _tamanhoColumaExtraPequena = 1m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Cargas Produtos", "Cargas", "CargaProduto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Carga.CargaProduto servicoCargaProduto = new Servicos.Embarcador.Relatorios.Carga.CargaProduto(unitOfWork, TipoServicoMultisoftware, Cliente);
                servicoCargaProduto.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, propriedades, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        // TODO (ct-reports): Repassar CT
        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            List<int> situacao = Request.GetListParam<int>("Situacao");
            bool situacaoFaturada = situacao.Exists(o => o == 99);
            bool situacaoNaoFaturada = situacao.Exists(o => o == 98);

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                CodigoPedido = Request.GetIntParam("Pedido"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork),
                CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacao = Request.GetListEnumParam<SituacaoCargaJanelaCarregamento>("Situacao"),
                SituacaoFaturada = situacaoFaturada,
                SituacaoNaoFaturada = situacaoNaoFaturada,
                ExibirCodigoBarras = Request.GetBoolParam("ExibirCodigoBarras")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Transportador", "RazaoSocialTransportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Transportador", "CnpjTransportador", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, true).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Pedido", "NumeroPedidoEmbarcador", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Criação", "DataCriacaoFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, true, false);
            grid.AdicionarCabecalho("Sequência Roteirização", "SequenciaRoteirizacao", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Produto", "CodigoProdutoEmbarcador", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, true).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Produto", "DescricaoProduto", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("UM", "UnidadeMedida", _tamanhoColumaExtraPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Peso Unitário", "PesoUnitario", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Total", "PesoTotal", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetenteFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CPFCNPJDestinatarioFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Loc. Destinatário", "LocalidadeDestinatario", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Janela", "DescricaoSituacaoJanelaCarregamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Carregamento", "DataCarregamentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Total Pallets", "TotalPallets", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Carga/Produto", "CargaProdutoDescricao", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bloco", "Bloco", _tamanhoColumaExtraPequena, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Sequência de Carregamento", "OrdemCarregamento", _tamanhoColumaExtraPequena, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Sequência de Entrega", "OrdemEntrega", _tamanhoColumaExtraPequena, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cód. Int. Grupo de Produto", "CodigoGrupoProduto", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo do Produto", "DescricaoGrupoProduto", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Cubagem (M³)", "CubagemPedido", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CPFCNPJRecebedorFormatado", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Recebedor", "RecebedorDescricao", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Loc. Recebedor", "LocalidadeRecebedor", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor da Mercadoria", "ValorMercadoria", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Saldo", "Saldo", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Placa Veículo", "PlacaVeiculos", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Motoristas", "Motoristas", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Início da Janela", "DataInicioJanelaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Fim da Janela", "DataFimJanelaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Pedido c/ Agenda", "PedidoComAgenda", _tamanhoColumaExtraPequena, Models.Grid.Align.center, false, false, false, true, false);


            return grid;
        }

        #endregion
    }
}
