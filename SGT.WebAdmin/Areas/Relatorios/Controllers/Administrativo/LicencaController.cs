using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Administrativo;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Administrativo
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Administrativo/Licenca")]
    public class LicencaController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca>
    {
		#region Construtores

		public LicencaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos Privados

		Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R150_Licenca;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Licenças", "Administrativo", "Licenca.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "DataVencimento", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Administrativo.Licenca servicoRelatorioLicenca = new Servicos.Embarcador.Relatorios.Administrativo.Licenca(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioLicenca.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.Licenca> listaLicenca, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

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

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoTipoLicenca = Request.GetIntParam("Licenca"),
                Entidade = Request.GetStringParam("Entidade"),
                Descricao = Request.GetStringParam("Descricao"),
                NumeroLicenca = Request.GetStringParam("NumeroLicenca"),
                CodigoFuncionario = Request.GetIntParam("Funcionario"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoPessoa = Request.GetDoubleParam("Pessoa"),
                StatusLicenca = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca>("Status"),
                TipoLicenca = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca>("TipoTelaLicenca"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0,
                StatusEntidade = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("StatusEntidade"),
            };

            return filtrosPesquisa;
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Tipo", "Identificador", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Nome Entidade", "Entidade", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Descrição Licença", "Descricao", 20, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("N° Licença", "NumeroLicenca", 10, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimentoFormatada", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Licença", "TipoLicenca", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("N° Frota", "NumeroFrota", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Status da Entidade", "StatusEntidadeDescricao", 10, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioLicenca> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
