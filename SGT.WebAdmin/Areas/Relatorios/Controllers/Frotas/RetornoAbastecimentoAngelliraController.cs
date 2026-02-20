using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/RetornoAbastecimentoAngellira")]
    public class RetornoAbastecimentoAngelliraController : BaseController
    {
		#region Construtores

		public RetornoAbastecimentoAngelliraController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R269_RetornoAbastecimentoAngellira;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Retorno de Abastecimentos Angellira", "Frotas", "RetornoAbastecimentoAngellira.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, false, true);
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

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.RetornoAbastecimentoAngellira servicoRelatorioAbastecimento = new Servicos.Embarcador.Relatorios.Frotas.RetornoAbastecimentoAngellira(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAbastecimento.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira> listaAbastecimento, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAbastecimento);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
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

            grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Data Consulta", "DataConsultaFormatada", 12, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Inicial", "DataInicialFormatada", 12, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Final", "DataFinalFormatada", 12, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 12, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Veículo", "Placa", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Frota", "Frota", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Placa Retorno", "PlacaRetorno", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Condutor Retorno", "CondutorRetorno", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Retorno", "DataRetornoFormatada", 12, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Cordenada Retorno", "CordenadaRetorno", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Latitude Retorno", "LatitudeRetorno", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Longitude Retorno", "LontitudeRetorno", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("KM", "Odometro", 8, Models.Grid.Align.center, true, true).NumberFormat("n0");
            grid.AdicionarCabecalho("Cod. Abastecimento", "CodigoAbastecimento", 8, Models.Grid.Align.center, true, true).NumberFormat("n0");
            grid.AdicionarCabecalho("Situação Abastecimento", "SituacaoAbastecimento", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Motorista", "NomeMotorista", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF Motorista", "CPFMotorista", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Posto", "CpfCnpjFornecedorFormatado", 8, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Posto", "NomePosto", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Situação Integração", "DescricaoSituacaoIntegracao", 8, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                Fornecedor = Request.GetDoubleParam("Fornecedor")
            };
        }

        #endregion
    }
}
