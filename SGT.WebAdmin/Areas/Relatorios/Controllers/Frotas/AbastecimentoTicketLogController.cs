using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/AbastecimentoTicketLog")]
    public class AbastecimentoTicketLogController : BaseController
    {
		#region Construtores

		public AbastecimentoTicketLogController(Conexao conexao) : base(conexao) { }

		#endregion

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R307_AbastecimentoTicketLog;

        private readonly decimal TamanhoColunaPequena = 1.75m;
        private readonly decimal TamanhoColunaGrande = 5.50m;
        private readonly decimal TamanhoColunaMedia = 3m;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Abastecimentos Ticket Log", "Frotas", "AbastecimentoTicketLog.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.AbastecimentoTicketLog servicoRelatorioAbastecimentoTicketLog = new Servicos.Embarcador.Relatorios.Frotas.AbastecimentoTicketLog(unitOfWork, TipoServicoMultisoftware, Cliente);

                var listaAbastecimentoTicketLog = await servicoRelatorioAbastecimentoTicketLog.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(listaAbastecimentoTicketLog.Count);
                grid.AdicionaRows(listaAbastecimentoTicketLog);

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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                 return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
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
            grid.AdicionarCabecalho("Código da Transação", "CodigoTransacao", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor da Transação", "ValorTransacao", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data da Transação", "DataTransacaoFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Fornecedor", "CNPJFornecedorFormatado", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Litros", "Litros", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo do Combustível", "TipoCombustivel", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor do Litro", "ValorLitro", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Quilometragem", "Quilometragem", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true); 
            grid.AdicionarCabecalho("Data da Integração", "DataIntegracaoFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Código do Abastecimento", "CodigoAbastecimento", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, true, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimentoTicketLog()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CNPJFornecedor = Request.GetDoubleParam("Fornecedor"),
                CodigoTransacao = Request.GetIntParam("CodigoTransacao")
            };
        }

        #endregion
    }
}
