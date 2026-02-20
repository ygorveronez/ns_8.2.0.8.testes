using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPatio;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.GestaoPatio
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/GestaoPatio/TemposGestaoPatio")]
    public class TemposGestaoPatioController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio>
    {
		#region Construtores

		public TemposGestaoPatioController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos

		Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R099_TemposGestaoPatio;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de tempos de gestão de pátio", "GestaoPatio", "TemposGestaoPatio.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true, 7);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.GestaoPatio.TemposGestaoPatio servicoRelatorioTemposGestaoPatio = new Servicos.Embarcador.Relatorios.GestaoPatio.TemposGestaoPatio(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioTemposGestaoPatio.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.TemposGestaoPatio> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
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

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio()
            {
                DataInicioCarregamento = Request.GetDateTimeParam("DataInicioCarregamento"),
                DataFimCarregamento = Request.GetDateTimeParam("DataFimCarregamento"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoRota = Request.GetIntParam("Rota"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                ListarCargasCanceladas = Request.GetBoolParam("ListarCargasCanceladas"),
                EtapaFluxoGestaoPatio = Request.GetEnumParam<EtapaFluxoGestaoPatio>("EtapaFluxoGestaoPatio"),
                Situacao = Request.GetNullableEnumParam<SituacaoEtapaFluxoGestaoPatio>("Situacao")
            };
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            string aux = "";
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            TipoFluxoGestaoPatio tipoFluxoGestaoPatio = TipoFluxoGestaoPatio.Origem;

            grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaFormatada", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 8, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 8, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false, true, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tração", "Veiculo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Reboques", "VeiculosVinculados", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modelo Veículo", "ModeloVeiculo", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo da Carga", "TipoCarga", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("D. Programada", "DataCarregamentoFormatada", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("N. Doca", "NumeroDoca", 10, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Rota", "Rota", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Observação Fluxo Pátio", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Soma total dos tempos", "SomaTotalDosTempos", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.InformarDoca, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataDocaInformadaFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgInformarDocaDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.ChegadaVeiculo, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataChegadaVeiculoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgChegadaVeiculoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Dif. " + aux, "PrevistoRealizadoChegadaVeiculoDescricao", (decimal)8.5, Models.Grid.Align.center, false, TipoSumarizacao.media, false).UtilizarFormatoTexto(formatoTexto: true);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.Guarita, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataEntregaGuaritaFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgEntradaGuaritaDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.CheckList, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataFimCheckListFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgChecklistDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.TravamentoChave, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataTravaChaveFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgTravaChaveDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.InicioCarregamento, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataInicioCarregamentoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgInicioCarregamentoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.FimCarregamento, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataFimCarregamentoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgFimCarregamentoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.LiberacaoChave, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataLiberacaoChaveFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgLiberacaoChaveDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.Faturamento, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataFaturamentoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgFaturamentoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.InicioViagem, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataInicioViagemFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgInicioViagemDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.Posicao, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataPosicaoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgPosicaoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.ChegadaLoja, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataChegadaLojaFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgChegadaLojaDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.DeslocamentoPatio, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataDeslocamentoPatioFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgDeslocamentoPatioDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.SaidaLoja, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataSaidaLojaFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgSaidaLojaDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.FimViagem, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataFimViagemFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgFimViagemDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.InicioHigienizacao, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataInicioHigienizacaoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgInicioHigienizacaoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.FimHigienizacao, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataFimHigienizacaoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgFimHigienizacaoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.SolicitacaoVeiculo, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataSolicitacaoVeiculoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgSolicitacaoVeiculoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.InicioDescarregamento, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataInicioDescarregamentoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgInicioDescarregamentoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.FimDescarregamento, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataFimDescarregamentoFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgFimDescarregamentoDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.DocumentoFiscal, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataDocumentoFiscalFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgDocumentoFiscalDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.DocumentosTransporte, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataDocumentosTransporteFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgDocumentosTransporteDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.MontagemCarga, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataMontagemCargaFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgMontagemCargaDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            aux = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.SeparacaoMercadoria, tipoFluxoGestaoPatio).Descricao;
            grid.AdicionarCabecalho("D. " + aux, "DataSeparacaoMercadoriaFormatada", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("T. Ag. " + aux, "TempoAgSeparacaoMercadoriaDescricao", (decimal)8.5, Models.Grid.Align.center, false, false);

            grid.AdicionarCabecalho("Peso", "Peso", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Área do Veículo", "AreaVeiculo", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Peso Saída do Veículo", "PesoSaidaVeiculo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Peso Chegada Veículo", "PesoChegadaVeiculo", 8, Models.Grid.Align.right, false, false);

            grid.AdicionarCabecalho("Código Transportador", "CodigoTransportador", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor NF", "ValorTotalNotaFiscal", 8, Models.Grid.Align.right, false, false);

            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioTemposGestaoPatio> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
