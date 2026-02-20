using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Administrativo
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Administrativo/LogEnvioSMS")]
    public class LogEnvioSMSController : BaseController
    {
		#region Construtores

		public LogEnvioSMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R205_LogEnvioSMS;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Logs de Envio de SMS", "Administrativo", "LogEnvioSMS.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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
                Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeAgrupar);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                Repositorio.Embarcador.NotaFiscal.LogEnvioSMS repositorio = new Repositorio.Embarcador.NotaFiscal.LogEnvioSMS(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorio(filtrosPesquisa, propriedades);
                var lista = totalRegistros > 0 ? await repositorio.ConsultarRelatorioAsync(filtrosPesquisa, propriedades, parametrosConsulta, cancellationToken) : new List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioSMS>();

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeAgrupar);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
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

        private async Task GerarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtrosPesquisa,
            List<PropriedadeAgrupamento> propriedades,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.NotaFiscal.LogEnvioSMS repositorio = new Repositorio.Embarcador.NotaFiscal.LogEnvioSMS(unitOfWork);
                var dataSource = await repositorio.ConsultarRelatorioAsync(filtrosPesquisa, propriedades, parametrosConsulta, cancellationToken);
                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = servicoRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork, null, null, true, TipoServicoMultisoftware);
                List<Parametro> parametros = await ObterParametrosRelatorio(unitOfWork, filtrosPesquisa, parametrosConsulta, cancellationToken);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Administrativo/LogEnvioSMS", parametros, relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),

                CodigoPessoa = Request.GetDoubleParam("Pessoa"),

                NumeroNotaInicial = Request.GetIntParam("NumeroNotaInicial"),
                NumeroNotaFinal = Request.GetIntParam("NumeroNotaFinal"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            decimal _tamanhoColunasValores = (decimal)1.75;
            decimal _tamanhoColunasDescricoes = (decimal)3.50;

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Data", "DataFormatada", _tamanhoColunasValores, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", _tamanhoColunasDescricoes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Nota", "Nota", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Celular", "Celular", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Link", "Link", _tamanhoColunasDescricoes, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Comunicou?", "Status", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        private async Task<List<Parametro>> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioSMS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, CancellationToken cancellationToken)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork, cancellationToken);

            Dominio.Entidades.Cliente pessoa = filtrosPesquisa.CodigoPessoa > 0 ? await repPessoa.BuscarPorCPFCNPJAsync(filtrosPesquisa.CodigoPessoa) : null;

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("NumeroNota", filtrosPesquisa.NumeroNotaInicial, filtrosPesquisa.NumeroNotaFinal));
            parametros.Add(new Parametro("Pessoa", pessoa?.Descricao));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataFormatada")
                return "Data";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
