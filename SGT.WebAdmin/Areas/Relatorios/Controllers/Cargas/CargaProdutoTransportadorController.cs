using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/CargaProdutoTransportador")]
    public class CargaProdutoTransportadorController : BaseController
    {
        #region Construtores

        public CargaProdutoTransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R319_CargaProdutoTransportador;

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Cargas Produtos", "Cargas", "CargaProdutoTransportador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
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
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Carga.CargaProdutoTransportador servicoCargaProdutoTransportador = new Servicos.Embarcador.Relatorios.Carga.CargaProdutoTransportador(unitOfWork, TipoServicoMultisoftware, Cliente);
                servicoCargaProdutoTransportador.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProdutoTransportador> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, cancellationToken);
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
        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            List<int> situacao = Request.GetListParam<int>("Situacao");
            bool situacaoFaturada = situacao.Any(o => o == 99);
            bool situacaoNaoFaturada = situacao.Any(o => o == 98);

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador()
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
                ExibirCodigoBarras = Request.GetBoolParam("ExibirCodigoBarras"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Transportador", "RazaoSocialTransportador", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Transportador", "CnpjTransportador", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 7, Models.Grid.Align.center, true, false, false, true, true).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Pedido", "NumeroPedidoEmbarcador", 7, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Sequência Roteirização", "SequenciaRoteirizacao", 7, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Produto", "CodigoProdutoEmbarcador", 7, Models.Grid.Align.center, false, false, false, false, true).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Produto", "DescricaoProduto", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UM", "UnidadeMedida", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Peso Unitário", "PesoUnitario", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Total", "PesoTotal", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CPFCNPJDestinatarioFormatado", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Loc. Destinatário", "LocalidadeDestinatario", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Endereço Destinatário", "EnderecoDestinatario", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bairro Destinatário", "BairroDestinatario", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número End. Destinatário", "NumeroEnderecoDestinatario", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Complemento End. Destinatário", "ComplementoEnderecoDestinatario", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Janela", "DescricaoSituacaoJanelaCarregamento", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Carregamento", "DataCarregamentoFormatada", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Total Pallets", "TotalPallets", 7, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Carga/Produto", "CargaProdutoDescricao", 7, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bloco", "Bloco", 5, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Sequência de Carregamento", "OrdemCarregamento", 4, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Sequência de Entrega", "OrdemEntrega", 4, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Grupo do Produto", "GrupoProduto", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cubagem (M³)", "CubagemPedido", 10, Models.Grid.Align.center, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CPFCNPJRecebedorFormatado", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Recebedor", "RecebedorDescricao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Loc. Recebedor", "LocalidadeRecebedor", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Endereço Recebedor", "EnderecoRecebedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bairro Recebedor", "BairroRecebedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número End. Recebedor", "NumeroEnderecoRecebedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Complemento End. Recebedor", "ComplementoEnderecoRecebedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 10, Models.Grid.Align.center, false);

            return grid;
        }

        #endregion
    }
}
