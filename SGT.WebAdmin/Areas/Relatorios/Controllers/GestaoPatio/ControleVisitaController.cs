using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.GestaoPatio
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/GestaoPatio/ControleVisita")]
    public class ControleVisitaController : BaseController
    {
        #region Construtores

        public ControleVisitaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R184_ControleVisita;
        
        private readonly decimal TamanhoColunasPequeno = 4m;
       
        private readonly decimal TamanhoColunasMedia = 6m;
        
        private readonly decimal TamanhoColunasDescritivos = 10m;
        
        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle Visita", "GestaoPatio", "ControleVisita.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, false, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
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

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.GestaoPatio.ControleVisita servicoRelatorioControleVisita = new Servicos.Embarcador.Relatorios.GestaoPatio.ControleVisita(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioControleVisita.ExecutarPesquisa(
                    out List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ControleVisita> listaControleVisita,
                    out int countRegistros,
                    filtrosPesquisa,
                    agrupamentos,
                    parametrosConsulta);

                grid.AdicionaRows(listaControleVisita);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa = ObterFiltrosPesquisa();

                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        #endregion

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Número", "Numero", TamanhoColunasPequeno, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Entrada", "DescricaoEntrada", TamanhoColunasMedia, Models.Grid.Align.center);
            grid.AdicionarCabecalho("Prev. Saída", "DescricaoPrevisaoSaida", TamanhoColunasMedia, Models.Grid.Align.center);
            grid.AdicionarCabecalho("Saída", "DescricaoSaida", TamanhoColunasMedia, Models.Grid.Align.center);
            grid.AdicionarCabecalho("CPF", "CPF", TamanhoColunasMedia, Models.Grid.Align.left, false, false, false, true);
            grid.AdicionarCabecalho("Nome", "Nome", TamanhoColunasDescritivos, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Nascimento", "DescricaoNascimento", TamanhoColunasMedia, Models.Grid.Align.center);
            grid.AdicionarCabecalho("Identidade", "Identidade", TamanhoColunasMedia, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Org. Emissor", "OrgaoEmissor", TamanhoColunasMedia, Models.Grid.Align.left);
            grid.AdicionarCabecalho("UF", "Estado", TamanhoColunasPequeno, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Empresa", "Empresa", TamanhoColunasDescritivos, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Setor", "Setor", TamanhoColunasMedia, Models.Grid.Align.left, false, false, false, true);
            grid.AdicionarCabecalho("Autorizador", "Autorizador", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, true);
            grid.AdicionarCabecalho("Veiculo", "PlacaVeiculo", TamanhoColunasMedia, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", TamanhoColunasMedia, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunasDescritivos, Models.Grid.Align.left);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita()
            {
                DataInicialEntrada = Request.GetDateTimeParam("DataInicialEntrada"),
                DataFinalEntrada = Request.GetDateTimeParam("DataFinalEntrada"),
                DataInicialSaida = Request.GetDateTimeParam("DataInicialSaida"),
                DataFinalSaida = Request.GetDateTimeParam("DataFinalSaida"),
                Setor = Request.GetIntParam("Setor"),
                Autorizador = Request.GetIntParam("Autorizador"),
                CPF = Request.GetStringParam("CPF").ObterSomenteNumeros().ToDouble(),
            };

            return filtrosPesquisa;
        }
    }
}
