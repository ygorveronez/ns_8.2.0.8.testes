using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/AcertoViagem/ComissaoAcertoViagem")]
    public class ComissaoAcertoViagemController : BaseController
    {
		#region Construtores

		public ComissaoAcertoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R183_ComissaoAcertoViagem;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Comissão do Acerto de Viagem", "AcertoViagem", "ComissaoAcertoViagem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroAcerto", "desc", "", "", Codigo, unitOfWork, true, true);
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
        }

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.AcertoViagem.ComissaoAcertoViagem servicoRelatorioComissaoAcertoViagem = new Servicos.Embarcador.Relatorios.AcertoViagem.ComissaoAcertoViagem(unitOfWork, TipoServicoMultisoftware, Cliente);

                // TODO (ct-reports): Repassar CT
                servicoRelatorioComissaoAcertoViagem.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ComissaoAcertoViagem> listaComissaoAcertoViagem, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaComissaoAcertoViagem);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa();

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
            grid.AdicionarCabecalho("CodigoMotorista", false);
            grid.AdicionarCabecalho("Número Acerto", "NumeroAcerto", 6, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cavalo", "Cavalo", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Reboques", "Reboques", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Mod. Veículo", "ModeloVeiculo", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Segmento do Acerto", "Segmento", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Inicial", "DataInicialAcertoFormatada", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data Final", "DataFinalAcertoFormatada", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Média", "Media", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Consumo Combustivel", "ConsumoCombustivel", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Bruto Acerto", "ValorBruto", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Bonificações", "Bonificacoes", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Descontos", "Descontos", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("% Comissão", "PercentualComissao", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Comissão", "ValorComissao", 8, Models.Grid.Align.right, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                CodigoAcertoViagem = Request.GetIntParam("AcertoViagem"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoSegmento = Request.GetIntParam("Segmento"),
                ExibirOcorrencias = Request.GetBoolParam("ExibirOcorrencias")
            };
        }

        #endregion
    }
}
