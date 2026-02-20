using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Avarias
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Avarias/Analitico")]
    public class AnaliticoController : BaseController
    {
		#region Construtores

		public AnaliticoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R051_Avaria_Analitico;

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Avarias", "Avarias", "Avarias.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, false, 7);

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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Avarias.Analitico servicoRelatorioAvarias = new Servicos.Embarcador.Relatorios.Avarias.Analitico(unitOfWork, TipoServicoMultisoftware, Cliente, cancellationToken);

                servicoRelatorioAvarias.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria> listaAvarias, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAvarias);

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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
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

            bool ordena = true;
            bool agrupa = false;

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Avaria", "Avaria", 1, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Lote", "Lote", 1, Models.Grid.Align.right, ordena, false, false, true, true);
            grid.AdicionarCabecalho("Código do Produto", "CodigoProduto", 3, Models.Grid.Align.right, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Descrição do Produto", "DescricaoProduto", 3, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Qtde Caixas", "QuantidadeCaixas", 3, Models.Grid.Align.right, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Qtde Unidades", "QuantidadeUnidades", 3, Models.Grid.Align.right, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Vl Unitário", "ValorUnitario", 2, Models.Grid.Align.right, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("Vl da Avaria", "ValorAvaria", 2, Models.Grid.Align.right, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Vl Desconto", "ValorDescontoAvaria", 2, Models.Grid.Align.right, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Situação Avaria", "SituacaoAvaria", 2, Models.Grid.Align.right, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Data da Solicitação", "DataSolicitacao", 2, Models.Grid.Align.center, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Data Criação do Lote", "DataCriacao", 2, Models.Grid.Align.center, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Data Última Aprovação", "DataAprovacao", 2, Models.Grid.Align.center, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 3, Models.Grid.Align.left, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("Etapa", "EtapaLote", 2, Models.Grid.Align.center, ordena, false, false, agrupa, false);
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Filial", "Filial", 3, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            else
                grid.AdicionarCabecalho("Filial", false);
            grid.AdicionarCabecalho("Transportadora", "Transportadora", 3, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Criador", "Criador", 3, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Data de Entrega", "DataEntrega", 2, Models.Grid.Align.center, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("CT-e", "CTe", 3, Models.Grid.Align.right, false, false, false, agrupa, true);
            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 3, Models.Grid.Align.right, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Viagem", "Viagem", 2, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 2, Models.Grid.Align.left, ordena, false, false, agrupa, true);
            grid.AdicionarCabecalho("Origem", "Origem", 3, Models.Grid.Align.left, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("Destino", "Destino", 3, Models.Grid.Align.left, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("Motivo de Avaria", "MotivoAvaria", 2, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Nome do Motorista", "Motorista", 2, Models.Grid.Align.left, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("RG", "RGMotorista", 2, Models.Grid.Align.left, false, false, false, agrupa, false);
            grid.AdicionarCabecalho("Placa", "Placa", 1, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Data Integração Lote", "DataIntegracao", 1, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Data Avaria", "DataAvaria", 1, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", 2, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", 2, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Data Viagem", "DataViagem", 2, Models.Grid.Align.left, ordena, false, false, agrupa, false);
            grid.AdicionarCabecalho("Data da Carga", "DataCarga", 2, Models.Grid.Align.center, ordena, false, false, agrupa, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                DataSolicitacaoInicial = Request.GetDateTimeParam("DataSolicitacaoInicial"),
                DataSolicitacaoFinal = Request.GetDateTimeParam("DataSolicitacaoFinal"),
                DataGeracaoLoteInicial = Request.GetDateTimeParam("DataGeracaoLoteInicial"),
                DataGeracaoLoteFinal = Request.GetDateTimeParam("DataGeracaoLoteFinal"),
                DataIntegracaoLoteInicial = Request.GetDateTimeParam("DataIntegracaoLoteInicial"),
                DataIntegracaoLoteFinal = Request.GetDateTimeParam("DataIntegracaoLoteFinal"),
                CodigoSolicitante = Request.GetIntParam("Solicitante"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                NumeroAvaria = Request.GetIntParam("NumeroAvaria"),
                SituacaoAvaria = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria>("SituacaoAvaria"),
                Etapa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote>("Etapa")
            };
        }

        #endregion
    }
}
