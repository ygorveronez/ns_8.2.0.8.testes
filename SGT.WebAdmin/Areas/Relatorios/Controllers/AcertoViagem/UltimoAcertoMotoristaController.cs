using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.AcertoViagem
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/AcertoViagem/UltimoAcertoMotorista")]
    public class UltimoAcertoMotoristaController : BaseController
    {
		#region Construtores

		public UltimoAcertoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados
        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R206_UltimoAcertoMotorista;
        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório do Último Acerto do Motorista", "AcertoViagem", "UltimoAcertoMotorista.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "CodigoIntegracao", "desc", "", "", Codigo, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioUltimoAcertoMotorista filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.AcertoViagem.UltimoAcertoMotorista servicoRelatorioUltimoAcertoMotorista = new Servicos.Embarcador.Relatorios.AcertoViagem.UltimoAcertoMotorista(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioUltimoAcertoMotorista.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.UltimoAcertoMotorista> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioUltimoAcertoMotorista filtrosPesquisa = ObterFiltrosPesquisa();

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
            grid.AdicionarCabecalho("Cod. Integração", "CodigoIntegracao", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CPF", "CPF", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Contratação", "DataContratacaoFormatada", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Acerto", "DataAcertoFormatada", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Inicial", "DataInicialFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data Final", "DataFinalFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data Fechamento", "DataFechamentoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Situação Acerto", "Situacao", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Frota", "Frota", 10, Models.Grid.Align.left, true, false, false, true, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioUltimoAcertoMotorista ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioUltimoAcertoMotorista filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioUltimoAcertoMotorista()
            {
                StatusMotorista = Request.GetEnumParam<SituacaoAtivoPesquisa>("StatusMotorista"),
                TipoMotorista = Request.GetEnumParam<TipoMotorista>("TipoMotorista"),

            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
