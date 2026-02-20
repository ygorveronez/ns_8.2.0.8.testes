using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Logistica/JanelaAgendamento")]
    public class JanelaAgendamentoController : BaseController
    {
        #region Construtores

        public JanelaAgendamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R237_JanelaAgendamento;
       
        private readonly decimal TamanhoColunaPequena = 5m;
       
        private readonly decimal TamanhoColunaMedia = 10m;
        
        private readonly decimal TamanhoColunaGrande = 15m;
       
        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao); ;
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Janelas do Agendamento", "Logistica", "JanelaAgendamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Logistica.JanelaAgendamento servicoRelatorioJanelaAgendamento = new Servicos.Embarcador.Relatorios.Logistica.JanelaAgendamento(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioJanelaAgendamento.ExecutarPesquisa(
                    out List<Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaAgendamento> listaJanelaAgendamento,
                    out int countRegistros,
                    filtrosPesquisa,
                    agrupamentos,
                    parametrosConsulta);

                grid.AdicionaRows(listaJanelaAgendamento);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento()
            {
                JanelaExcedente = Request.GetEnumParam<OpcaoSimNaoPesquisa>("JanelaExcedente"),
                Fornecedor = Request.GetDoubleParam("Fornecedor"),
                TipoDeCarga = Request.GetIntParam("TipoDeCarga"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                SituacaoAgendamento = Request.GetListEnumParam<SituacaoAgendamentoColeta>("SituacaoAgendamento"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                Senha = Request.GetStringParam("Senha"),
                RaizCnpjFornecedor = Request.GetStringParam("RaizCnpjFornecedor"),
                CentroDescarregamento = Request.GetIntParam("CentroDescarregamento")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Data Descarregamento", "DataDescarregamento", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Hora Descarregamento", "HoraDescarregamento", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Tentativa", "DataTentativa", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Senha", "Senha", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("CNPJ Fornecedor", "CnpjFornecedorFormatado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Modalidade", "Modalidade", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade Caixas", "QuantidadeCaixas", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Qtd. Caixas Devolvidas", "QuantidadeCaixasDevolvidas", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Qtd. Caixas não Entregues", "QuantidadeCaixasNaoEntregues", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade Itens", "QuantidadeItens", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Modelo Veícular", "ModeloVeicular", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Agendamento", "DescricaoSituacaoAgendamento", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Ag. Extra", "AgendaExtraDescricao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Resp. Confirmação", "ResponsavelConfirmacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Confirmação", "DataConfirmacaoAgendaFormatada", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Data Solicitação", "DataSolicitacaoAgendaFormatada", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Valor Total Pedido", "ValorTotalPedido", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Caixas Pedido", "QuantidadeCaixasPedido", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Médio Caixa", "ValorMedioCaixa", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Agendado", "ValorAgendado", TamanhoColunaGrande, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Operador que realizou a Agenda", "OperadorAgendamento", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo de Produto", "GrupoProduto", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Produto", "CodigoIntegracaoProduto", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Descrição Produto", "DescricaoProduto", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Produto Agendado", "ValorProdutoAgendado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Produto Agendado", "QtdProdutoAgendado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Itens Agendado", "QtdItensAgendado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Stuação Janela", "SituacaoJanelaDescricao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
