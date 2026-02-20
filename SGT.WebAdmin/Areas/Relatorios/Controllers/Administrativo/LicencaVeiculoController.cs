using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Administrativo
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Administrativo/LicencaVeiculo")]
    public class LicencaVeiculoController : BaseController
    {
		#region Construtores

		public LicencaVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R302_LicencaVeiculo;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Licenças de Veículos", "Administrativo", "LicencaVeiculo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Administrativo.LicencaVeiculo servRelatorioLicencaVeiculo = new Servicos.Embarcador.Relatorios.Administrativo.LicencaVeiculo(unitOfWork, TipoServicoMultisoftware, Cliente);

                servRelatorioLicencaVeiculo.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LicencaVeiculo> listaLicenca, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaLicenca);
                grid.setarQuantidadeTotal(countRegistros);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição Licença", "Descricao", 20, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("N° Licença", "NumeroLicenca", 10, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimentoFormatada", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Licença", "TipoLicenca", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("N° Frota", "NumeroFrota", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Status do Veiculo", "StatusVeiculoDescricao", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Status da Licença", "StatusLicencaDescricao", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Func. Responsável", "FuncionarioResponsavel", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Marca", "Marca", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Modelo", "Modelo", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Renavam", "Renavam", 10, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Descricao = Request.GetStringParam("Descricao"),
                NumeroLicenca = Request.GetStringParam("NumeroLicenca"),
                CodigoLicenca = Request.GetIntParam("Licenca"),
                CodigoFuncionario = Request.GetIntParam("Funcionario"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                CodigoMarca = Request.GetIntParam("Marca"),
                CodigoModelo = Request.GetIntParam("Modelo"),
                Renavam = Request.GetStringParam("Renavam"),
                StatusLicenca = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca>("Status"),
                StatusVeiculo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("StatusVeiculo"),
            };

            return filtrosPesquisa;
        }
        #endregion
    }
}
