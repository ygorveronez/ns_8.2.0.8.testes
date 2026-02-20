using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/AcertoViagem/CargaCompartilhada")]
    public class CargaCompartilhadaController : BaseController
    {
		#region Construtores

		public CargaCompartilhadaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados
        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R115_CargaCompartilhada;
        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Carga Compartilhada", "AcertoViagem", "CargaCompartilhada.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Carga", "desc", "", "", Codigo, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioCargaCompartilhada filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.AcertoViagem.CargaCompartilhada servicoRelatorioCargaCompartilhada = new Servicos.Embarcador.Relatorios.AcertoViagem.CargaCompartilhada(unitOfWork, TipoServicoMultisoftware, Cliente);

                // TODO (ct-reports): Repassar CT
                servicoRelatorioCargaCompartilhada.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.CargaCompartilhada> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

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
                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioCargaCompartilhada filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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
            grid.AdicionarCabecalho("Data Carga", "DataCarga", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Acerto", "DataAcerto", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Emitente", "Emitente", 20, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Nº Acerto", "NumeroAcerto", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Motorista", "Motoristas", 20, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("% Comp.", "PercentualCompartilhado", 12, Models.Grid.Align.right, true, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioCargaCompartilhada ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioCargaCompartilhada filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioCargaCompartilhada()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Carga = Request.GetIntParam("Carga"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                NumeroAcerto = Request.GetIntParam("NumeroAcerto"),
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
