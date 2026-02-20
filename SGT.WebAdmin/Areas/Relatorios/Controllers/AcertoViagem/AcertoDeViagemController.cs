using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/AcertoViagem/AcertoDeViagem")]
    public class AcertoDeViagemController : BaseController
    {
		#region Construtores

		public AcertoDeViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R060_AcertoDeViagem;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Acerto de Viagem", "AcertoViagem", "AcertoDeViagem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroAcerto", "desc", "", "", Codigo, unitOfWork, true, true);
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

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.AcertoViagem.AcertoDeViagem servicoRelatorioAcertoViagem = new Servicos.Embarcador.Relatorios.AcertoViagem.AcertoDeViagem(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAcertoViagem.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AcertoDeViagem> listaComissaoAcertoViagem, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem filtrosPesquisa = ObterFiltrosPesquisa();
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
            grid.AdicionarCabecalho("Número Acerto", "NumeroAcerto", 6, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Inicial", "DataInicial", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Final", "DataFinal", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Fechamento", "DataFechamento", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cod. Integração", "CodigoIntegracao", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF", "CPF", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Frota", "Frota", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Segmento do Veículo", "Segmento", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Situação Acerto", "Situacao", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Operador do Acerto", "Operador", 15, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Operador Inicial do Acerto", "OperadorInicio", 15, Models.Grid.Align.left, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataInicialFechamento = Request.GetDateTimeParam("DataInicialFechamento"),
                DataFinalFechamento = Request.GetDateTimeParam("DataFinalFechamento"),
                AcertoViagem = Request.GetIntParam("AcertoViagem"),
                Motorista = Request.GetIntParam("Motorista"),
                Segmento = Request.GetIntParam("Segmento"),
                VeiculoTracao = Request.GetIntParam("VeiculoTracao"),
                VeiculoReboque = Request.GetIntParam("VeiculoReboque"),
                Situacao = Request.GetEnumParam<SituacaoAcertoViagem>("Situacao"),
                TipoMotorista = Request.GetEnumParam<TipoMotorista>("TipoMotorista"),
                StatusMotorista = Request.GetEnumParam<SituacaoAtivoPesquisa>("StatusMotorista"),
                UltimoAcerto = Request.GetBoolParam("UltimoAcerto"),
            };
        }

        #endregion
    }
}
