using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.TorreControle
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/TorreControle/Permanencias")]
    public class PermanenciasController : BaseController
    {
        #region Construtores

        public PermanenciasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R353_Permanencias;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Permanencias", "TorreControle", "", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Carga", "desc", "", "", codigoRelatorio, unitOfWork, false, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.TorreControle.Permanencias.ErroBuscarDados);
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
                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioPermanencias filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.TorreControle.Permanencias servicoRelatorioPermanencias = new Servicos.Embarcador.Relatorios.TorreControle.Permanencias(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioPermanencias.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.Permanencias> listaPermanencias, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPermanencias);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.TorreControle.Permanencias.ErroBuscarDados);
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
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioPermanencias filtrosPesquisa = ObterFiltrosPesquisa();

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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
                return new JsonpResult(false, Localization.Resources.Relatorios.TorreControle.Permanencias.ErroGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork, HttpRequest request = null)
        {
            Models.Grid.Grid grid;

            grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>(),
            };

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.Carga, "Carga", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.Placa, "Placa", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.Transportador, "Transportador", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.Motoristas, "Motoristas", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.Cliente, "Cliente", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.GrupoDePessoas, "GrupoPessoas", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.TipoOperacao, "TipoOperacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.Filial, "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.OrigemCarga, "OrigemCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DestinoCarga, "DestinoCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.SituacaoMonitoramento, "DescricaoSituacaoMonitoramento", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.EtapaMonitoramento, "EtapaMonitoramento", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.TempoTotalEtapaMonitoramento, "DescricaoTempoTotalEtapaMonitoramento", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.EntregaForaDoRaio, "DescricaoEntregaForaDoRaio", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataCarregamento, "DataCarregamentoFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataCriacaoCarga, "DataCriacaoCargaFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataAgendamento, "DataAgendamentoFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataConfirmacao, "DataConfirmacaoFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataEntregaAtualizada, "DataEntregaAtualizadaFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.TempoAguardoNFE, "DescricaoTempoAguardoNFE", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataPrimeiroEspelhamento, "DataPrimeiroEspelhamentoFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataUltimoEspelhamento, "DataUltimoEspelhamentoFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.TipoParada, "DescricaoTipoParada", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataEntradaArea, "DataEntradaAreaFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataSaidaArea, "DataSaidaAreaFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.TempoArea, "DescricaoTempoArea", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.SubArea, "SubArea", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.TipoSubArea, "TipoSubArea", _tamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataEntradaSubArea, "DataEntradaSubAreaFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.DataSaidaSubArea, "DataSaidaSubAreaFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.TorreControle.Permanencias.TempoSubArea, "DescricaoTempoSubArea", _tamanhoColunaPequena, Models.Grid.Align.left, true, true);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "/Relatorios/Permanencias/Pesquisa", "gridPreviewRelatorio");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;

        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioPermanencias ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioPermanencias filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioPermanencias()
            {
                Carga = Request.GetStringParam("Carga"),
                Placa = Request.GetStringParam("Placa"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoCliente = Request.GetIntParam("Cliente"),
                CodigoTipoParada = Request.GetNullableBoolParam("TipoParada"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFinal"),
                DataCriacaoCargaInicial = Request.GetNullableDateTimeParam("DataCriacaoCargaInicial"),
                DataCriacaoCargaFinal = Request.GetNullableDateTimeParam("DataCriacaoCargaFinal"),
                DataAgendamentoColetaInicial = Request.GetNullableDateTimeParam("DataAgendamentoColetaInicial"),
                DataAgendamentoColetaFinal = Request.GetNullableDateTimeParam("DataAgendamentoColetaFinal"),
                DataAgendamentoEntregaInicial = Request.GetNullableDateTimeParam("DataAgendamentoEntregaInicial"),
                DataAgendamentoEntregaFinal = Request.GetNullableDateTimeParam("DataAgendamentoEntregaFinal")
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
