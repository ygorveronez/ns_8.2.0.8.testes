using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Fretes/RotaFrete")]
    public class RotaFreteController : BaseController
    {
        #region Construtores

        public RotaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R185_RotaFrete;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Rotas de Frete", "Fretes", "RotaFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Fretes.RotaFrete.OcorreuFalhaBuscarDadosRelatorio);
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

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Fretes.RotaFrete servicoRelatorioRotaFrete = new Servicos.Embarcador.Relatorios.Fretes.RotaFrete(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioRotaFrete.ExecutarPesquisa(
                    out List<Dominio.Relatorios.Embarcador.DataSource.Fretes.RotaFrete> listaCargaPedidos,
                    out int countRegistros,
                    filtrosPesquisa,
                    agrupamentos,
                    parametrosConsulta);

                grid.AdicionaRows(listaCargaPedidos);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Fretes.RotaFrete.OcorreuFalhaConsultar);
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
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, configuracaoRelatorio.PropriedadeAgrupa);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio filtrosPesquisa = ObterFiltrosPesquisa();

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Fretes.RotaFrete.OcorreuFalhaGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRotaFreteRelatorio()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                GrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                Remetente = Request.GetDoubleParam("Remetente"),
                Situacao = Request.GetNullableBoolParam("Situacao"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigosUFDestino = Request.GetListParam<string>("EstadoDestino"),
                RotaExclusivaCompraValePedagio = Request.GetNullableBoolParam("RotaExclusivaCompraValePedagio")
            };

        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Descricao, "Descricao", 12, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.CodigoIntegracao, "CodigoIntegracao", 6, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TempoViagemMinutos, "TempoViagemHoras", 6, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Quilometros, "Quilometros", 6, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.VelocidadeMediaCarregado, "VelocidadeMediaCarregado", 6, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.VelocidadeMediaVazio, "VelocidadeMediaVazio", 6, Models.Grid.Align.right, true, false, false, false, false); ;
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Observacao, "Observacao", 12, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Detalhes, "Detalhes", 12, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TipoDeRota, "DescricaoTipoRota", 6, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TipoCarregamentoIda, "DescricaoTipoCarregamentoIda", 6, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TipoCarregamentoVolta, "DescricaoTipoCarregamentoVolta", 6, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Situacao, "Situacao", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TipoDeOperacao, "TipoOperacao", 12, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.GrupoDePessoas, "GrupoPessoas", 12, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Remetente, "Remetente", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Origem, "Origem", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Destinatario, "Destinatario", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Destino, "Destino", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.EstadoDeDestino, "EstadoDestino", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.CodigoSemParar, "CodigoIntegracaoValePedagio", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.CEP, "CEP", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.LeadTimeDias, "LeadTimeDias", 6, Models.Grid.Align.left, false, false, false, false, false).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Transportador, "Transportador", 12, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.PercentualDeCarga, "PercentualCarga", 6, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Distribuidor, "Distribuidor", 12, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.Fronteira, "Fronteira", 12, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TempoCarregamento, "TempoCarregamento", 6, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TempoDescarga, "TempoDescarga", 6, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.TempoFronteira, "TempoFronteira", 6, Models.Grid.Align.center, false, false, false, false, false);

            Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repositorioConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);
            if (repositorioConfiguracaoValePedagio.PossuiIntegracaoRepomRest())
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Fretes.RotaFrete.RotaExclusivaCompraValePedagio, "RotaExclusivaCompraValePedagioFormatada", 10, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }
        #endregion
    }
}