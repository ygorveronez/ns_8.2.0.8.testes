using SGT.WebAdmin.Areas.BusinessIntelligence;
using SGT.WebAdmin.Areas.Relatorios;
using WebOptimizer.Processors;

namespace SGT.WebAdmin
{
    public static class BundleConfig
    {
        public static void RegisterBundles(IServiceCollection services)
        {
            JsSettings settings = new JsSettings();
#if DEBUG
            //Utilize esse settings para debugar o javascript, por exemplo: torreControleDetalhesPedido
            settings = new JsSettings()
            {
                CodeSettings = new() { MinifyCode = false, StripDebugStatements = false }
            };
#endif

            services.AddWebOptimizer(pipeline =>
            {
                pipeline.RegisterBundlesRelatorios();
                pipeline.RegisterBundlesBI();

                #region Scripts

                #region Bibliotecas

                #region Tema Aplicação

                pipeline.AddJavaScriptBundle("/scripts/vendorsBundle", "/js-new/vendors.bundle.js");

                pipeline.AddJavaScriptBundle("/scripts/appBundle", "/js-new/app.bundle.js");

                #endregion

                pipeline.AddJavaScriptBundle("/scripts/summernote", "/js-new/formplugins/summernote/summernote.js");
                pipeline.AddJavaScriptBundle("/scripts/tempusDominus", "/js-new/miscellaneous/tempus-dominus/tempus-dominus.js");
                pipeline.AddJavaScriptBundle("/scripts/bootstrapAutoComplete", "/js/libs/bootstrap-autocomplete.js");

                pipeline.AddJavaScriptBundle("/scripts/toastr", "/js-new/notifications/toastr/toastr.js");
                pipeline.AddJavaScriptBundle("/scripts/sweetalert2", "/js-new/notifications/sweetalert2/sweetalert2.bundle.js");
                pipeline.AddJavaScriptBundle("/scripts/select2", "/js-new/formplugins/select2/select2.bundle.js");
                pipeline.AddJavaScriptBundle("/scripts/dropzone", "/js-new/formplugins/dropzone/dropzone.js");
                pipeline.AddJavaScriptBundle("/scripts/popper", "/js-new/miscellaneous/popper/popper.js");

                pipeline.AddJavaScriptBundle("/scripts/leaflet", "/js-new/miscellaneous/leaflet/leaflet.js", "/js-new/miscellaneous/leaflet/leaflet.markercluster.js");

                pipeline.AddJavaScriptBundle("/scripts/jquery", "/js/libs/jquery-3.6.0.js");
                pipeline.AddJavaScriptBundle("/scripts/jqueryMbBrowser", "/js/plugin/msie-fix/jquery.mb.browser.js");
                pipeline.AddJavaScriptBundle("/scripts/jqueryTouch", "/js/plugin/jquery-touch/jquery.ui.touch-punch.js");
                pipeline.AddJavaScriptBundle("/scripts/jqueryValidate", "/js/plugin/jquery-validate/jquery.validate.js");
                pipeline.AddJavaScriptBundle("/scripts/blockUI", "/js/libs/jquery.blockui.js");
                pipeline.AddJavaScriptBundle("/scripts/Wizard", "/js/plugin/fuelux/wizard/wizard.js");
                pipeline.AddJavaScriptBundle("/scripts/fastClick", "/js/plugin/fastclick/fastclick.js");
                pipeline.AddJavaScriptBundle("/scripts/maskMoney", "/js/libs/jquery.maskMoney.js");
                pipeline.AddJavaScriptBundle("/scripts/fileDownload", "/js/libs/jquery.filedownload.js");
                pipeline.AddJavaScriptBundle("/scripts/twbsPagination", "/js/libs/jquery.twbsPagination.js");
                pipeline.AddJavaScriptBundle("/scripts/globalize", "/js/libs/jquery.globalize.js", $"/js/libs/jquery.globalize.{System.Globalization.CultureInfo.CurrentCulture.ToString()}.js");

                pipeline.AddJavaScriptBundle("/scripts/dataTables", "/js/plugin/datatables/jquery.dataTables.js", "/js/plugin/datatables/dataTables.colReorder.js", "/js/plugin/datatables/colResizable.js", "/js/plugin/datatables/dataTables.checkboxes.js", "/js/plugin/datatables/dataTables.bootstrap.js", "/js/plugin/datatable-responsive/datatables.responsive.js", "/js/plugin/datatables/jquery.dataTables.rowReordering.js");

                pipeline.AddJavaScriptBundle("/scripts/knockout", "/js/knockout/knockout-3.3.0.js", "/js/knockout/ko.extend.js");
                pipeline.AddJavaScriptBundle("/scripts/bootstrap-rastreio-entrega", "/js/bootstrap/bootstrap-rastreio-entrega.js");
                pipeline.AddJavaScriptBundle("/scripts/maskedInput", "/js/plugin/masked-input/jquery.maskedinput.js");
                pipeline.AddJavaScriptBundle("/scripts/plupload", "/js/plugin/plupload/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/d3js", "/js/libs/d3.js", "/js/libs/d3pie.js", "/js/libs/d3-funnel.min.js");
                pipeline.AddJavaScriptBundle("/scripts/chartjs", "/js/libs/chartjs/Chart.bundle.min.js", "/js/libs/chartjs/chartjs-plugin-annotation.min.js", "/js/libs/chartjs/element-center.js");
                pipeline.AddJavaScriptBundle("/scripts/canvg", "/js/libs/canvg.js");
                pipeline.AddJavaScriptBundle("/scripts/fabric", "/js/libs/fabric.min.js");
                pipeline.AddJavaScriptBundle("/scripts/signalR", "/js/libs/signalr/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/bootstrapSelect", "/js-new/formplugins/bootstrap-select/bootstrap-select.js");
                pipeline.AddJavaScriptBundle("/scripts/bootstrapColorSelector", "/js/libs/bootstrap-colorselector.js");
                pipeline.AddJavaScriptBundle("/scripts/bootstrapToggle", "/js/libs/bootstrap-toggle.min.js");
                pipeline.AddJavaScriptBundle("/scripts/moment", "/js/libs/moment.min.js", "/js/libs/pt-br.min.js");

                pipeline.AddJavaScriptBundle("/scripts/chart", "/js/plugin/chartjs/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/morris", "/js/plugin/morris/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/flot", "/js/plugin/flot/**/*.js");

                pipeline.AddJavaScriptBundle("/scripts/fullcalendar", "/js/plugin/fullcalendar/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/countdown", "/js/plugin/jquery-countdown/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/progressbar", "/js/plugin/bootstrap-progressbar/bootstrap-progressbar.js");
                pipeline.AddJavaScriptBundle("/scripts/bootstrapmenu", "/js/plugin/bootstrap-menu/BootstrapMenu.min.js");
                pipeline.AddJavaScriptBundle("/scripts/easy-pie-chart", "/js/plugin/easy-pie-chart/jquery.easy-pie-chart.js");
                pipeline.AddJavaScriptBundle("/scripts/treeviewLoad", "/js/libs/TreeViewLoad.js");
                pipeline.AddJavaScriptBundle("/scripts/promise", "/js/plugin/promise/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/fontawesomemarkers", "/js/libs/fontawesome.markers.min.js");

                pipeline.AddJavaScriptBundle("/scripts/portalClienteApp", "/Content/portal-cliente/app/**/*.js");

                pipeline.AddJavaScriptBundle("/scripts/webcam", "/js/libs/WebCam/webcam-easy.min.js");
                pipeline.AddJavaScriptBundle("/scripts/signaturepad", "/Scripts/signature_pad.min.js");

                pipeline.AddJavaScriptBundle("/scripts/makerclusterer", "/js/libs/makerclusterer.js");

                #endregion

                #region AbastecimentoInterno

                pipeline.AddJavaScriptBundle("/scripts/movimentacaoAbastecimento", "/ViewsScripts/AbastecimentoInterno/MovimentacaoAbastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentoEntradaTanque", "/ViewsScripts/AbastecimentoInterno/MovimentoEntradaTanque/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/liberacaoAbastecimentoAutomatizado", "/ViewsScripts/AbastecimentoInterno/LiberacaoAbastecimentoAutomatizado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentacaoTanquesDetalhes", "/ViewsScripts/AbastecimentoInterno/MovimentacaoTanquesDetalhes/**/*.js").UseContentRoot();

                #endregion

                #region Acertos

                pipeline.AddJavaScriptBundle("/scripts/acertoViagem", "/ViewsScripts/Acertos/AcertoViagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoBonificacao", "/ViewsScripts/Acertos/TipoBonificacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoDespesa", "/ViewsScripts/Acertos/TipoDespesa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaDiaria", "/ViewsScripts/Acertos/TabelaDiaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaComissaoMotorista", "/ViewsScripts/Acertos/TabelaComissaoMotorista/**/*.js").UseContentRoot();

                #endregion

                #region Agendas

                pipeline.AddJavaScriptBundle("/scripts/agendaTarefa", "/ViewsScripts/Agendas/AgendaTarefa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agenda", "/ViewsScripts/Agendas/Agenda/**/*.js").UseContentRoot();

                #endregion


                #region AtendimentosCRM
                pipeline.AddJavaScriptBundle("/scripts/atendimentoCRM", "/ViewsScripts/AtendimentosCRM/AtendimentoCRM/**/*.js").UseContentRoot();

                #endregion

                #region Atendimentos

                pipeline.AddJavaScriptBundle("/scripts/tipoAtendimento", "/ViewsScripts/Atendimentos/TipoAtendimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/sistema", "/ViewsScripts/Atendimentos/Sistema/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modulo", "/ViewsScripts/Atendimentos/Modulo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tela", "/ViewsScripts/Atendimentos/Tela/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/chamado", "/ViewsScripts/Atendimentos/Chamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/atendimento", "/ViewsScripts/Atendimentos/Atendimento/**/*.js").UseContentRoot();

                #endregion

                #region Avarias

                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAvaria", "/ViewsScripts/Avarias/AutorizacaoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/produtoAvaria", "/ViewsScripts/Avarias/ProdutoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoAvaria", "/ViewsScripts/Avarias/MotivoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoAvaria", "/ViewsScripts/Avarias/RegrasAutorizacaoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/solicitacaoAvaria", "/ViewsScripts/Avarias/SolicitacaoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRemocaoLote", "/ViewsScripts/Avarias/MotivoRemocaoLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoDescontoAvaria", "/ViewsScripts/Avarias/MotivoDescontoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/lotesPendentes", "/ViewsScripts/Avarias/LotesPendentes/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/lotes", "/ViewsScripts/Avarias/Lotes/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteAvaria", "/ViewsScripts/Avarias/LoteAvarias/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/aceiteLoteAvaria", "/ViewsScripts/Avarias/AceiteLoteAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fluxoAvaria", "/ViewsScripts/Avarias/FluxoAvaria/**/*.js").UseContentRoot();

                #endregion

                #region Cargas

                pipeline.AddJavaScriptBundle("/scripts/carga", settings, "/ViewsScripts/Cargas/Carga/**/*.js", "/ViewsScripts/Cargas/Pedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaGestao", "/ViewsScripts/Cargas/CargaGestao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/enceramentoCarga", "/ViewsScripts/Cargas/EncerramentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/leilaoTipoOpereracaoConfiguracao", "/ViewsScripts/Cargas/LeilaoTipoOperacaoConfiguracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/servicoInspecaoFederal", "/ViewsScripts/Cargas/ServicoInspecaoFederal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modeloVeicularCarga", "/ViewsScripts/Cargas/ModeloVeicularCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoCarga", "/ViewsScripts/Cargas/TipoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/TipoSeparacao", "/ViewsScripts/Cargas/TipoSeparacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoCargaModeloVeicularAutoConfig", "/ViewsScripts/Cargas/TipoCargaModeloVeicularAutoConfig/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaCTeManual", "/ViewsScripts/Cargas/CargaCTeManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaCTeAgrupado", "/ViewsScripts/Cargas/CargaCTeAgrupado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaMDFeManual", "/ViewsScripts/Cargas/CargaMDFeManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaMDFeAquaviarioManual", "/ViewsScripts/Cargas/CargaMDFeAquaviarioManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaMDFeManualCancelamento", "/ViewsScripts/Cargas/CargaMDFeManualCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cancelamentoCarga", "/ViewsScripts/Cargas/CancelamentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleGeracaoEDI", "/ViewsScripts/Cargas/ControleGeracaoEDI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/transbordo", "/ViewsScripts/Cargas/Transbordo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/redespacho", "/ViewsScripts/Cargas/Redespacho/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaAgrupada", "/ViewsScripts/Cargas/CargaAgrupada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaControleExpedicao", "/ViewsScripts/Cargas/CargaControleExpedicao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/montagemCarga", "/ViewsScripts/Cargas/MontagemCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaIntegracaoEvento", "/ViewsScripts/Cargas/CargaIntegracaoEvento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integracaoNFe", "/ViewsScripts/Cargas/IntegracaoNFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/montagemCargaMapa", "/ViewsScripts/Cargas/MontagemCargaMapa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCancelamentoIntegracao", "/ViewsScripts/Cargas/PedidoCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoProdutosCarregamentos", "/ViewsScripts/Cargas/PedidoProdutosCarregamentos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAgrupamentoPedidos", "/ViewsScripts/Cargas/RegrasAgrupamentoPedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fluxoColetaEntrega", "/ViewsScripts/Cargas/FluxoColetaEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoModeloVeicular", "/ViewsScripts/Cargas/GrupoModeloVeicular/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/roteirizacao", "/ViewsScripts/Cargas/Roteirizacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentoPreAgrupamentoCarga", "/ViewsScripts/Cargas/AcompanhamentoPreAgrupamentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoCarga", "/ViewsScripts/Cargas/RegraAutorizacaoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoCarga", "/ViewsScripts/Cargas/AutorizacaoCarga/**/*.js", "/ViewsScripts/Cargas/Carga/Frete/ComposicaoFrete.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaDataCarregamentoImportacao", "/ViewsScripts/Cargas/CargaDataCarregamentoImportacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/reenvioIntegracaoEDI", "/ViewsScripts/Cargas/ReenvioIntegracaoEDI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoSolicitacaoFrete", "/ViewsScripts/Cargas/MotivoSolicitacaoFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/faixaTemperatura", "/ViewsScripts/Cargas/FaixaTemperatura/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/responsavelCarga", "/ViewsScripts/Cargas/ResponsavelCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoRetornoCarga", "/ViewsScripts/Cargas/TipoRetornoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/retornoCarga", "/ViewsScripts/Cargas/RetornoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleEntrega", "/ViewsScripts/Cargas/ControleEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleEntregaDevolucao", "/ViewsScripts/Cargas/ControleEntregaDevolucao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoAvaliacao", "/ViewsScripts/Cargas/MotivoAvaliacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/detalhesPedido", "/ViewsScripts/Cargas/Pedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/retornoCargaColetaBackhaul", "/ViewsScripts/Cargas/RetornoCargaColetaBackhaul/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoCancelamentoRetornoCargaColetaBackhaul", "/ViewsScripts/Cargas/MotivoCancelamentoRetornoCargaColetaBackhaul/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/geracaoCargaEmbarcador", "/ViewsScripts/Cargas/GeracaoCargaEmbarcador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cancelamentoCargaLote", "/ViewsScripts/Cargas/CancelamentoCargaLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/responsavelEntrega", "/ViewsScripts/Cargas/ResponsavelEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoColeta", "/ViewsScripts/Cargas/MotivoRejeicaoColeta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoFalhaNotaFiscal", "/ViewsScripts/Cargas/MotivoFalhaNotaFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRetificacaoColeta", "/ViewsScripts/Cargas/MotivoRetificacaoColeta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/impressaoLoteCarga", "/ViewsScripts/Cargas/ImpressaoLoteCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaTemperatura", "/ViewsScripts/Cargas/JustificativaTemperatura/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaVeiculoContainer", "/ViewsScripts/Cargas/CargaVeiculoContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoOcorrenciaEntrega", "/ViewsScripts/Cargas/ConfiguracaoOcorrenciaEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agendamentoEntrega", "/ViewsScripts/Cargas/AgendamentoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoCargaCancelamento", "/ViewsScripts/Cargas/RegraAutorizacaoCargaCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoCargaCancelamento", "/ViewsScripts/Cargas/AutorizacaoCargaCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaCancelamentoCarga", "/ViewsScripts/Cargas/JustificativaCancelamentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/valePedagio", "/ViewsScripts/Cargas/ValePedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/viaTransporte", "/ViewsScripts/Cargas/ViaTransporte/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoPedido", "/ViewsScripts/Cargas/GestaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleNotaDevolucao", "/ViewsScripts/Cargas/ControleNotaDevolucao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/manutencaoEntregaCarga", "/ViewsScripts/Cargas/ManutencaoEntregaCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoLacre", "/ViewsScripts/Cargas/TipoLacre/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/isca", "/ViewsScripts/Cargas/Isca/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoCarregamento", "/ViewsScripts/Cargas/RegraAutorizacaoCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoCarregamento", "/ViewsScripts/Cargas/AutorizacaoCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaTrajeto", "/ViewsScripts/Cargas/CargaTrajeto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ordemEmbarqueIntegracao", "/ViewsScripts/Cargas/OrdemEmbarqueIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoLogisticoIntegracao", "/ViewsScripts/Cargas/MonitoramentoLogisticoIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentoEntrega", "/ViewsScripts/Cargas/AcompanhamentoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteComprovanteEntrega", "/ViewsScripts/Cargas/LoteComprovanteEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaDownloadDocumentos", "/ViewsScripts/Cargas/DownloadDocumentos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/montagemFeeder", "/ViewsScripts/Cargas/MontagemFeeder/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaentregaintegracao", "/ViewsScripts/Cargas/EntregaIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaExportacaoIntegracao", "/ViewsScripts/Cargas/CargaExportacaoIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tratativaOcorrenciaEntrega", "/ViewsScripts/Cargas/TratativaOcorrenciaEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoAlertaCarga", "/ViewsScripts/Cargas/ConfiguracaoAlertaCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoResponsavelAtrasoEntrega", "/ViewsScripts/Cargas/TipoResponsavelAtrasoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaFretePendente", "/ViewsScripts/Cargas/CargaFretePendente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaFechamento", "/ViewsScripts/Cargas/CargaFechamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/aceiteTransportador", "/ViewsScripts/Cargas/AceiteTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteIntegracaoCarregamento", "/ViewsScripts/Cargas/LoteIntegracaoCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoProgramacaoCarga", "/ViewsScripts/Cargas/ConfiguracaoProgramacaoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoCarga", "/ViewsScripts/Cargas/ProgramacaoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/avaliacaoEntrega", "/ViewsScripts/Cargas/AvaliacaoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoPercurso", "/ViewsScripts/Cargas/TipoPercurso/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoDocumentoTransporte", "/ViewsScripts/Cargas/TipoDocumentoTransporte/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoAnexo", "/ViewsScripts/Cargas/Carga/TipoAnexo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/liberacaoCargaEmMassa", "/ViewsScripts/Cargas/Carga/LiberacaoCargaEmMassa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/extratoValePedagio", "/ViewsScripts/Cargas/ExtratoValePedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alterarcentroresultado", "/ViewsScripts/Cargas/AlterarCentroResultado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaRelacionada", "/ViewsScripts/Cargas/CargaRelacionada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaEntregaEventoIntegracao", "/ViewsScripts/Cargas/CargaEntregaEventoIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoLeilao", "/ViewsScripts/Cargas/RegrasAutorizacaoLeilao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoLeilao", "/ViewsScripts/Cargas/AutorizacaoLeilao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoCarregamento", "/ViewsScripts/Cargas/TipoCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tiposComprovantes", "/ViewsScripts/Cargas/ComprovanteCarga/TipoComprovante/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/comprovanteCarga", "/ViewsScripts/Cargas/ComprovanteCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoTrecho", "/ViewsScripts/Cargas/TipoTrecho/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/FluxoEncerramentoCarga", "/ViewsScripts/Cargas/FluxoEncerramentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ocultarInformacoesCarga", "/ViewsScripts/Cargas/OcultarInformacoesCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alertasTransportador", "/ViewsScripts/Cargas/AlertasTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contingenciaCargaEmissao", "/ViewsScripts/Cargas/ContingenciaCargaEmissao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoDadosColeta", "/ViewsScripts/Cargas/GestaoDadosColeta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaAutorizacaoCarga", "/ViewsScripts/Cargas/JustificativaAutorizacaoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/confirmarDocumentoEmLote", "/ViewsScripts/Cargas/ConfirmarDocumentoEmLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoDevolucao", "/ViewsScripts/Cargas/GestaoDevolucao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/categoria", "/ViewsScripts/Cargas/Categoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ofertas", "/ViewsScripts/Cargas/Ofertas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargaOferta", "/ViewsScripts/Cargas/GestaoCargaOferta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integracoesComFalha", "/ViewsScripts/Cargas/GestaoIntegracoesComFalha/**/*.js").UseContentRoot();

                #endregion

                #region Canhotos

                pipeline.AddJavaScriptBundle("/scripts/localArmazenamentoCanhoto", "/ViewsScripts/Canhotos/LocalArmazenamentoCanhoto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canhoto", "/ViewsScripts/Canhotos/Canhoto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/baixarCanhoto", "/ViewsScripts/Canhotos/BaixarCanhoto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/vincularCanhotoManual", "/ViewsScripts/Canhotos/VincularCanhotoManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canhotoMalote", "/ViewsScripts/Canhotos/CanhotoMalote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/maloteCanhoto", "/ViewsScripts/Canhotos/Malote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoInconsistenciaDigitacao", "/ViewsScripts/Canhotos/MotivoInconsistenciaDigitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canhotoIntegracao", "/ViewsScripts/Canhotos/CanhotoIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canhotoIntegracaoPorCTe", "/ViewsScripts/Canhotos/CanhotoIntegracaoPorCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoReconhecimentoCanhoto", "/ViewsScripts/Canhotos/ConfiguracaoReconhecimentoCanhoto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoAuditoria", "/ViewsScripts/Canhotos/MotivoRejeicaoAuditoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/enviarCanhotos", "/ViewsScripts/Canhotos/EnviarCanhotos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/renderizarCanhoto", "/ViewsScripts/Canhotos/RenderizarPDF/RenderizarPDF.js").UseContentRoot();

                #endregion

                #region Chamados 

                pipeline.AddJavaScriptBundle("/scripts/chamados", "/ViewsScripts/Chamados/Chamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoChamado", "/ViewsScripts/Chamados/MotivoChamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRecusaCancelamento", "/ViewsScripts/Chamados/MotivoRecusaCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAnaliseChamado", "/ViewsScripts/Chamados/RegrasAnaliseChamados/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/chamadoTMS", "/ViewsScripts/Chamados/ChamadoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleChamadoTMS", "/ViewsScripts/Chamados/ControleChamadoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoTempoChamado", "/ViewsScripts/Chamados/ConfiguracaoTempoChamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/generoMotivoChamado", "/ViewsScripts/Chamados/GeneroMotivoChamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/areaEnvolvidaMotivoChamado", "/ViewsScripts/Chamados/AreaEnvolvidaMotivoChamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAtendimentoChamado", "/ViewsScripts/Chamados/RegrasAtendimentoChamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoMotivoChamado", "/ViewsScripts/Chamados/GrupoMotivoChamado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteChamadoOcorrencia", "/ViewsScripts/Chamados/LoteChamadoOcorrencia/**/*.js").UseContentRoot();

                #endregion

                #region ChecklistUsuario

                pipeline.AddJavaScriptBundle("/scripts/cadastroChecklist", "/ViewsScripts/CheckListsUsuario/ConfigCheckListUsuario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ValidarChecklist", "/ViewsScripts/CheckListsUsuario/CheckListUsuario/**/*.js").UseContentRoot();

                #endregion

                #region CRM

                pipeline.AddJavaScriptBundle("/scripts/CRMProspeccao", "/ViewsScripts/CRM/Prospeccao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/clienteProspect", "/ViewsScripts/CRM/ClienteProspect/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/produtoProspect", "/ViewsScripts/CRM/ProdutoProspect/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/origemContatoClienteProspect", "/ViewsScripts/CRM/OrigemContatoClienteProspect/**/*.js").UseContentRoot();

                #endregion

                #region Dash

                pipeline.AddJavaScriptBundle("/scripts/Documentacao", "/ViewsScripts/Dash/Documentacao/**/*.js").UseContentRoot();

                #endregion

                #region Compras

                pipeline.AddJavaScriptBundle("/scripts/motivoCompra", "/ViewsScripts/Compras/MotivoCompra/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/condicaoPagamento", "/ViewsScripts/Compras/CondicaoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/requisicaoMercadoria", "/ViewsScripts/Compras/RequisicaoMercadoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cotacaoCompra", "/ViewsScripts/Compras/CotacaoCompra/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasRequisicaoMercadoria", "/ViewsScripts/Compras/RegrasRequisicaoMercadoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoRequisicaoMercadoria", "/ViewsScripts/Compras/AutorizacaoRequisicaoMercadoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoOrdemCompra", "/ViewsScripts/Compras/AutorizacaoOrdemCompra/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ordemCompra", "/ViewsScripts/Compras/OrdemCompra/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasOrdemCompra", "/ViewsScripts/Compras/RegrasOrdemCompra/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/respostaCotacao", "/ViewsScripts/Compras/RespostaCotacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fluxoCompra", "/ViewsScripts/Compras/FluxoCompra/**/*.js").UseContentRoot();

                #endregion

                #region Configuracao

                pipeline.AddJavaScriptBundle("/scripts/configuracaoTMS", "/ViewsScripts/Configuracao/Sistema/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoEmissaoCTe", "/ViewsScripts/Configuracao/EmissaoCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoComponentesFrete", "/ViewsScripts/Configuracao/ComponentesFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoLayoutEDI", "/ViewsScripts/Configuracao/LayoutEDI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoFatura", "/ViewsScripts/Configuracao/Fatura/**/*.js").UseContentRoot();

                #endregion

                #region Configuracoes

                pipeline.AddJavaScriptBundle("/scripts/configuracaoIntegracao", "/ViewsScripts/Configuracoes/Integracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/processoMovimento", "/ViewsScripts/Configuracoes/ProcessoMovimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoMovimento", "/ViewsScripts/Configuracoes/ConfiguracaoMovimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/arquivoImportacaoNotaFiscal", "/ViewsScripts/Configuracoes/ArquivoImportacaoNotaFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoFinanceira", "/ViewsScripts/Configuracoes/ConfiguracaoFinanceira/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/feriado", "/ViewsScripts/Configuracoes/Feriado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoEmbarcador", "/ViewsScripts/Configuracoes/Configuracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleAlerta", "/ViewsScripts/Configuracoes/ControleAlerta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/impressora", "/ViewsScripts/Configuracoes/Impressora/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/licenca", "/ViewsScripts/Configuracoes/Licenca/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/especie", "/ViewsScripts/Configuracoes/Especie/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivo", "/ViewsScripts/Configuracoes/Motivo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/corAnimal", "/ViewsScripts/Configuracoes/CorAnimal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoCIOT", "/ViewsScripts/Configuracoes/ConfiguracaoCIOT/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acordoFaturamentoCliente", "/ViewsScripts/Configuracoes/AcordoFaturamentoCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoObservacaoCTe", "/ViewsScripts/Configuracoes/ObservacaoCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contingenciaEstado", "/ViewsScripts/Configuracoes/ContingenciaEstado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoControleEntrega", "/ViewsScripts/Configuracoes/ConfiguracaoControleEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoDiariaAutomatica", "/ViewsScripts/Configuracoes/DiariaAutomatica/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoConciliacaoTransportador", "/ViewsScripts/Configuracoes/ConfiguracaoConciliacaoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoVtex", "/ViewsScripts/Configuracoes/ConfiguracaoVtex/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoDocumentacaoAFRMM", "/ViewsScripts/Configuracoes/ConfiguracaoDocumentacaoAFRMM/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoEmissaoDocumentoEmbarcador", "/ViewsScripts/Configuracoes/ConfiguracaoEmissaoDocumentoEmbarcador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutomatizacaoEmissoesEmail", "/ViewsScripts/Configuracoes/RegraAutomatizacaoEmissoesEmail/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleThread", "/ViewsScripts/Configuracoes/ControleThread/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoPreCarga", "/ViewsScripts/Configuracoes/ConfiguracaoPreCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoTaxaDescarga", "/ViewsScripts/Configuracoes/ConfiguracaoTaxaDescarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoTemplateWhatsApp", "/ViewsScripts/Configuracoes/ConfiguracaoTemplateWhatsApp/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/palletizacao", "/ViewsScripts/Configuracoes/Palletizacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notificacaoMotoristaSMS", "/ViewsScripts/Configuracoes/NotificacaoMotoristaSMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/script", "/ViewsScripts/Configuracoes/Script/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/execucaoComandos", "/ViewsScripts/Configuracoes/ExecucaoComandos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoModeloEmail", "/ViewsScripts/Configuracoes/ConfiguracaoModeloEmail/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/liberacaoIntegracao", "/ViewsScripts/Configuracoes/LiberacaoIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contingenciaMDFeEstado", "/ViewsScripts/Configuracoes/ContingenciaMDFeEstado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoEmissorDocumento", "/ViewsScripts/Configuracoes/ConfiguracaoEmissorDocumento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoOrquestradorFila", "/ViewsScripts/Configuracoes/ConfiguracaoOrquestradorFila/**/*.js").UseContentRoot();

                #endregion

                #region Contabils

                pipeline.AddJavaScriptBundle("/scripts/arquivoEBS", "/ViewsScripts/Contabils/ArquivoEBS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaValores", "/ViewsScripts/Contabils/ConsultaValores/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/arquivoContabil", "/ViewsScripts/Contabils/ArquivoContabil/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleArquivo", "/ViewsScripts/Contabils/ControleArquivo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alteracaoArquivoMercante", "/ViewsScripts/Contabils/AlteracaoArquivoMercante/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaValoresPagar", "/ViewsScripts/Contabils/ConsultaValoresPagar/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoMovimentoArquivoContabil", "/ViewsScripts/Contabils/TipoMovimentoArquivoContabil/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/calculoISS", "/ViewsScripts/Contabils/CalculoISS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/calculoPisCofins", "/ViewsScripts/Contabils/CalculoPisCofins/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/codigoIntegracaoCFOPCST", "/ViewsScripts/Contabils/CodigoIntegracaoCFOPCST/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaMercante", "/ViewsScripts/Contabils/JustificativaMercante/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/impostoValorAgregado", "/ViewsScripts/Contabils/ImpostoValorAgregado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/direitoFiscal", "/ViewsScripts/Contabils/DireitoFiscal/**/*.js").UseContentRoot();

                #endregion

                #region Container

                pipeline.AddJavaScriptBundle("/scripts/controleContainer", "/ViewsScripts/Container/ControleContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentacaoAreaContainer", "/ViewsScripts/Container/MovimentacaoAreaContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/containerRedex", "/ViewsScripts/Container/ContainerRedex/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaContainer", "/ViewsScripts/Container/JustificativaContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/conferenciaContainer", "/ViewsScripts/Container/ConferenciaContainer/**/*.js").UseContentRoot();

                #endregion

                #region ConfiguracaoContabil

                pipeline.AddJavaScriptBundle("/scripts/configuracaoCentroResultado", "/ViewsScripts/ConfiguracaoContabil/ConfiguracaoCentroResultado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoContaContabil", "/ViewsScripts/ConfiguracaoContabil/ConfiguracaoContaContabil/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoNaturezaOperacao", "/ViewsScripts/ConfiguracaoContabil/ConfiguracaoNaturezaOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoFechamentoContabilizacao", "/ViewsScripts/ConfiguracaoContabil/ConfiguracaoFechamentoContabilizacao/**/*.js").UseContentRoot();

                #endregion

                #region Contatos

                pipeline.AddJavaScriptBundle("/scripts/tipoContato", "/ViewsScripts/Contatos/TipoContato/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoContato", "/ViewsScripts/Contatos/SituacaoContato/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contatoCliente", "/ViewsScripts/Contatos/ContatoCliente/**/*.js").UseContentRoot();

                #endregion

                #region Cotacoes

                pipeline.AddJavaScriptBundle("/scripts/cotacaoPedido", "/ViewsScripts/Cotacoes/CotacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraCotacaoPedido", "/ViewsScripts/Cotacoes/RegraCotacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoCotacaoPedido", "/ViewsScripts/Cotacoes/AutorizacaoCotacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasCotacao", "/ViewsScripts/Cotacoes/RegrasCotacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cotacaoFrete", "/ViewsScripts/Cotacoes/CotacaoFrete/**/*.js").UseContentRoot();

                #endregion

                #region CTe

                pipeline.AddJavaScriptBundle("/scripts/cte", "/ViewsScripts/CTe/CTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaCTe", "/ViewsScripts/CTe/ConsultaCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/downloadLoteCTe", "/ViewsScripts/CTe/DownloadLoteCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cteCancelamento", "/ViewsScripts/CTe/CTeCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/conhecimentoEletronico", "/ViewsScripts/CTe/ConhecimentoEletronico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleEmissaoCTe", "/ViewsScripts/ControleCTe/ControleEmissaoCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleEmissaoNFSe", "/ViewsScripts/ControleNFSe/ControleEmissaoNFSe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/campoCartaCorrecao", "/ViewsScripts/CTe/CartaCorrecao/CampoCartaCorrecao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cartaCorrecao", "/ViewsScripts/CTe/CartaCorrecao/CartaCorrecao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoCTeLote", "/ViewsScripts/CTe/AutorizacaoCTeLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAverbacaoLote", "/ViewsScripts/CTe/AutorizacaoAverbacaoLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoIntegracaoCTe", "/ViewsScripts/CTe/RegraAutorizacaoIntegracaoCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoIntegracaoCTe", "/ViewsScripts/CTe/AutorizacaoIntegracaoCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/observacaoContribuinte", "/ViewsScripts/CTe/ObservacaoContribuinte/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoCteEmitidoForaEmbarcador", "/ViewsScripts/CTe/ImportacaoCTeEmitidoForaEmbarcador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cancelamentoCTeSemCarga", "/ViewsScripts/CTe/CancelamentoCTeSemCarga/**/*.js").UseContentRoot();
                #endregion

                #region Créditos

                pipeline.AddJavaScriptBundle("/scripts/hierarquiaSolicitacaoCredito", "/ViewsScripts/Creditos/HierarquiaSolicitacaoCredito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/creditoDisponivel", "/ViewsScripts/Creditos/CreditoDisponivel/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleSaldo", "/ViewsScripts/Creditos/ControleSaldo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/creditoLiberacao", "/ViewsScripts/Creditos/CreditoLiberacao/**/*.js").UseContentRoot();

                #endregion

                #region Documentos

                pipeline.AddJavaScriptBundle("/scripts/campoCCe", "/ViewsScripts/Documentos/CampoCCe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modeloDocumentoFiscal", "/ViewsScripts/Documentos/ModeloDocumentoFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/documentoDestinadoEmpresa", "/ViewsScripts/Documentos/DocumentoDestinadoEmpresa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ciot", "/ViewsScripts/Documentos/CIOT/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoDocumento", "/ViewsScripts/Documentos/GestaoDocumento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleDocumento", "/ViewsScripts/Documentos/ControleDocumento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamentoCIOT", "/ViewsScripts/Documentos/AutorizacaoPagamentoCIOT/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/trackingDocumentacao", "/ViewsScripts/Documentos/TrackingDocumentacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoNotasFiscais", "/ViewsScripts/Documentos/GestaoNotasFiscais/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAprovacaoDocumentos", "/ViewsScripts/Documentos/RegraAprovacaoGestaoDocumento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/documentacaoAFRMM", "/ViewsScripts/Documentos/DocumentacaoAFRMM/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamentoCIOTParcela", "/ViewsScripts/Documentos/AutorizacaoPagamentoCIOTParcela/**/*.js").UseContentRoot();

                #endregion

                #region Download Documentos

                pipeline.AddJavaScriptBundle("/scripts/downloadDocumentos", "/ViewsScripts/DownloadDocumentos/**/*.js").UseContentRoot();

                #endregion

                #region Escrituracao

                pipeline.AddJavaScriptBundle("/scripts/loteEscrituracao", "/ViewsScripts/Escrituracao/LoteEscrituracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteEscrituracaoMiro", "/ViewsScripts/Escrituracao/LoteEscrituracaoMiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteEscrituracaoCancelamento", "/ViewsScripts/Escrituracao/LoteEscrituracaoCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/provisao", "/ViewsScripts/Escrituracao/Provisao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cancelamentoProvisao", "/ViewsScripts/Escrituracao/CancelamentoProvisao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraEscrituracao", "/ViewsScripts/Escrituracao/RegraEscrituracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pagamento", "/ViewsScripts/Escrituracao/Pagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cancelamentopagamento", "/ViewsScripts/Escrituracao/CancelamentoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/lancamentosContabeis", "/ViewsScripts/Escrituracao/LancamentosContabeis/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoCancelamentoPagamento", "/ViewsScripts/Escrituracao/MotivoCancelamentoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoProvisao", "/ViewsScripts/Escrituracao/ConfiguracaoProvisao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraPisCofins", "/ViewsScripts/Escrituracao/RegraPisCofins/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoLiberacaoEscrituracaoPagamentoCarga", "/ViewsScripts/Escrituracao/RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoLiberacaoEscrituracaoPagamentoCarga", "/ViewsScripts/Escrituracao/AutorizacaoLiberacaoEscrituracaoPagamentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/provisaoManual", "/ViewsScripts/Escrituracao/ProvisaoManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoPagamento", "/ViewsScripts/Escrituracao/RegraAutorizacaoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamento", "/ViewsScripts/Escrituracao/AutorizacaoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoProvisaoPendente", "/ViewsScripts/Escrituracao/RegraAutorizacaoProvisaoPendente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoEstornoProvisao", "/ViewsScripts/Escrituracao/AutorizacaoEstornoProvisao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraPagamentoProvedor", "/ViewsScripts/Escrituracao/RegraPagamentoProvedor/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoFreteCliente", "/ViewsScripts/Escrituracao/ContratoFreteCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/saldoContratoFreteCliente", "/ViewsScripts/Escrituracao/SaldoContratoFreteCliente/**/*.js").UseContentRoot();

                #endregion

                #region Escala

                pipeline.AddJavaScriptBundle("/scripts/gerarEscalas", "/ViewsScripts/Escalas/GerarEscala/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/escalas", "/ViewsScripts/Escalas/Escala/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRemocaoVeiculoEscala", "/ViewsScripts/Escalas/MotivoRemocaoVeiculoEscala/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/escalaVeiculo", "/ViewsScripts/Escalas/EscalaVeiculo/**/*.js").UseContentRoot();

                #endregion

                #region EDI

                pipeline.AddJavaScriptBundle("/scripts/layoutEDI", "/ViewsScripts/EDI/LayoutEDI/**/*.js").UseContentRoot();

                #endregion

                #region Email

                pipeline.AddJavaScriptBundle("/scripts/configEmailDocTransporte", "/ViewsScripts/Email/ConfigEmailDocTransporte/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/emailDocumentacaoCarga", "/ViewsScripts/Email/EmailDocumentacaoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/emailGlobalizadoFornecedor", "/ViewsScripts/Email/EmailGlobalizadoFornecedor/**/*.js").UseContentRoot();

                #endregion

                #region Filial

                pipeline.AddJavaScriptBundle("/scripts/filial", "/ViewsScripts/Filiais/Filial/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoGestaoPatio", "/ViewsScripts/Filiais/ConfiguracaoGestaoPatio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/turno", "/ViewsScripts/Filiais/Turno/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/sequenciaGestaoPatio", "/ViewsScripts/Filiais/SequenciaGestaoPatio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoArmazem", "/ViewsScripts/Filiais/GestaoArmazem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/detalhesArmazemProduto", "/ViewsScripts/Filiais/DetalhesArmazemProduto/**/*.js").UseContentRoot();

                #endregion

                #region Frotas

                pipeline.AddJavaScriptBundle("/scripts/abastecimento", "/ViewsScripts/Frotas/Abastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedagio", "/ViewsScripts/Frotas/Pedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentacaoDePlacas", "/ViewsScripts/Frotas/MovimentacaoDePlacas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoDePedagio", "/ViewsScripts/Frotas/ImportacaoDePedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoAbastecimento", "/ViewsScripts/Frotas/ConfiguracaoAbastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoPedagio", "/ViewsScripts/Frotas/FechamentoPedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoAbastecimento", "/ViewsScripts/Frotas/FechamentoAbastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoValePedagio", "/ViewsScripts/Frotas/ConfiguracaoValePedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/reprocessarAbastecimento", "/ViewsScripts/Frotas/ReprocessarAbastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoDestinoOleo", "/ViewsScripts/Frotas/TipoDestinoOleo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoOleo", "/ViewsScripts/Frotas/TipoOleo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("~/scripts/bombaAbastecimento", "/ViewsScripts/Frotas/BombaAbastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("~/scripts/tabelaPrecoCombustivel", "/ViewsScripts/Frotas/TabelaPrecoCombustivel/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleKmReboque", "/ViewsScripts/Frotas/ControleKmReboque/**/*.js").UseContentRoot();

                #endregion

                #region Faturas

                pipeline.AddJavaScriptBundle("/scripts/fatura", "/ViewsScripts/Faturas/Fatura/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativa", "/ViewsScripts/Faturas/Justificativa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/faturaLote", "/ViewsScripts/Faturas/FaturaLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/faturaCancelamentoLote", "/ViewsScripts/Faturas/FaturaCancelamentoLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoFatura", "/ViewsScripts/Faturas/RegraAutorizacaoFatura/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoFatura", "/ViewsScripts/Faturas/AutorizacaoFatura/**/*.js").UseContentRoot();

                #endregion

                #region Financeiro

                pipeline.AddJavaScriptBundle("/scripts/mensalidade", "/ViewsScripts/Financeiros/Mensalidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/baixaTituloPagar", "/ViewsScripts/Financeiros/BaixaTituloPagar/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/baixaTituloReceber", "/ViewsScripts/Financeiros/BaixaTituloReceber/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planoConta", "/ViewsScripts/Financeiros/PlanoConta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/centroResultado", "/ViewsScripts/Financeiros/CentroResultado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoMovimento", "/ViewsScripts/Financeiros/TipoMovimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentoFinanceiro", "/ViewsScripts/Financeiros/MovimentoFinanceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/documentoEntrada", "/ViewsScripts/Financeiros/DocumentoEntrada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/geracaoMovimentoLote", "/ViewsScripts/Financeiros/GeracaoMovimentoLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tituloFinanceiro", "/ViewsScripts/Financeiros/TituloFinanceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cteTituloReceber", "/ViewsScripts/Financeiros/CTeTituloReceber/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/boletoConfiguracao", "/ViewsScripts/Financeiros/BoletoConfiguracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/boletoGeracao", "/ViewsScripts/Financeiros/BoletoGeracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoDeRetorno", "/ViewsScripts/Financeiros/BoletoImportarRetorno/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/boletoRemessa", "/ViewsScripts/Financeiros/BoletoRemessa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoPagamentoRecebimento", "/ViewsScripts/Financeiros/TipoPagamentoRecebimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoDiario", "/ViewsScripts/Financeiros/FechamentoDiario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/bancoTMS", "/ViewsScripts/Financeiros/BancoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/formasTitulo", "/ViewsScripts/Financeiros/FormasTitulo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/baixaTituloReceberNovo", "/ViewsScripts/Financeiros/BaixaTituloReceberNovo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/lancamentoConta", "/ViewsScripts/Financeiros/LancamentoConta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/bordero", "/ViewsScripts/Financeiros/Bordero/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/geracaoTituloManual", "/ViewsScripts/Financeiros/GeracaoTituloManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cobrancaSimples", "/ViewsScripts/Financeiros/CobrancaSimples/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraEntradaDocumento", "/ViewsScripts/Financeiros/RegraEntradaDocumento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/boletoAlteracao", "/ViewsScripts/Financeiros/BoletoAlteracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planoOrcamentario", "/ViewsScripts/Financeiros/PlanoOrcamentario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pagamentoDigital", "/ViewsScripts/Financeiros/PagamentoDigital/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/remessaPagamento", "/ViewsScripts/Financeiros/RemessaPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/retornoPagamento", "/ViewsScripts/Financeiros/RetornoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/geracaoMovimentoLoteCentroResultado", "/ViewsScripts/Financeiros/GeracaoMovimentoLoteCentroResultado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cheque", "/ViewsScripts/Financeiros/Cheque/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pagamentoEletronicoComandoRetorno", "/ViewsScripts/Financeiros/PagamentoEletronicoComandoRetorno/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoDespesaFinanceira", "/ViewsScripts/Financeiros/GrupoDespesaFinanceira/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoDespesaFinanceira", "/ViewsScripts/Financeiros/TipoDespesaFinanceira/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rateioDespesaVeiculo", "/ViewsScripts/Financeiros/RateioDespesaVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoFinanciamento", "/ViewsScripts/Financeiros/ContratoFinanciamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/caixaFuncionario", "/ViewsScripts/Financeiros/CaixaFuncionario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/despesaMensal", "/ViewsScripts/Financeiros/DespesaMensal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/despesaMensalProcessamento", "/ViewsScripts/Financeiros/DespesaMensalProcessamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tributoTipoDocumento", "/ViewsScripts/Financeiros/TributoTipoDocumento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tributoCodigoReceita", "/ViewsScripts/Financeiros/TributoCodigoReceita/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tributoTipoImposto", "/ViewsScripts/Financeiros/TributoTipoImposto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tributoVariacaoImposto", "/ViewsScripts/Financeiros/TributoVariacaoImposto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteContabilizacao", "/ViewsScripts/Financeiros/LoteContabilizacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/extratoBancario", "/ViewsScripts/Financeiros/ExtratoBancario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/extratoBancarioTipoLancamento", "/ViewsScripts/Financeiros/ExtratoBancarioTipoLancamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/conciliacaoBancaria", "/ViewsScripts/Financeiros/ConciliacaoBancaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamentoTitulo", "/ViewsScripts/Financeiros/AutorizacaoPagamentoTitulo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoPagamentoEletronico", "/ViewsScripts/Financeiros/RegraAutorizacaoPagamentoEletronico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamentoEletronico", "/ViewsScripts/Financeiros/AutorizacaoPagamentoEletronico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/conciliacaoTransportador", "/ViewsScripts/Financeiros/ConciliacaoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/documentosConciliacao", "/ViewsScripts/Financeiros/DocumentosConciliacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoLancamentoDocumentoEntrada", "/ViewsScripts/Financeiros/SituacaoLancamentoDocumentoEntrada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modalidadeContratoFinanciamento", "/ViewsScripts/Financeiros/ModalidadeContratoFinanciamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/aplicarAcrescimoDescontoNoTitulo", "/ViewsScripts/Financeiros/AplicarAcrescimoDescontoNoTitulo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaCancelamentoFinanceiro", "/ViewsScripts/Financeiros/JustificativaCancelamentoFinanceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/valeAvulso", "/ViewsScripts/Financeiros/ValeAvulso/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fichaCliente", "/ViewsScripts/Financeiros/FichaCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentoContaAPagar", "/ViewsScripts/Financeiros/AcompanhamentoContaAPagar/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentacaoContaPagar", "/ViewsScripts/Financeiros/MovimentacaoContaPagar/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentacaoContasPagarTransportador", "/ViewsScripts/Financeiros/MovimentacaoContasPagarTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/termoQuitacaoFinanceiro", "/ViewsScripts/Financeiros/TermoQuitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/avisoPeriodico", "/ViewsScripts/Financeiros/AvisoPeriodico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/termoQuitacaoDocumento", "/ViewsScripts/Financeiros/TermoQuitacaoDocumento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/liberacaoPagamentoProvedor", "/ViewsScripts/Financeiros/LiberacaoPagamentoProvedor/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contaBancaria", "/ViewsScripts/Financeiros/ContaBancaria/**/*.js").UseContentRoot();

                #endregion

                #region Fechamento

                pipeline.AddJavaScriptBundle("/scripts/fechamentoFrete", "/ViewsScripts/Fechamento/FechamentoFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoJustificativaAcrescimoDesconto", "/ViewsScripts/Fechamento/FechamentoJustificativaAcrescimoDesconto/**/*.js").UseContentRoot();

                #endregion

                #region FaturamentosMensais

                pipeline.AddJavaScriptBundle("/scripts/faturamentoMensalGrupo", "/ViewsScripts/FaturamentosMensais/FaturamentoMensalGrupo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/faturamentoMensalCliente", "/ViewsScripts/FaturamentosMensais/FaturamentoMensalCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/faturamentoMensal", "/ViewsScripts/FaturamentosMensais/FaturamentoMensal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planoEmissaoFaturamento", "/ViewsScripts/FaturamentosMensais/PlanoEmissaoFaturamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/validacaoFaturamentoMensal", "/ViewsScripts/FaturamentosMensais/ValidacaoFaturamentoMensal/**/*.js").UseContentRoot();

                #endregion

                #region Fretes

                pipeline.AddJavaScriptBundle("/scripts/componenteFrete", "/ViewsScripts/Fretes/ComponenteFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaFreteCliente", "/ViewsScripts/Fretes/TabelaFreteCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaFrete", "/ViewsScripts/Fretes/TabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/simuladorFrete", "/ViewsScripts/Fretes/SimuladorFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaFreteComissaoProduto", "/ViewsScripts/Fretes/TabelaFreteComissaoProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaFreteComissaoGrupoProduto", "/ViewsScripts/Fretes/TabelaFreteComissaoGrupoProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaFreteComissaoImportacao", "/ViewsScripts/Fretes/TabelaFreteComissaoImportacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaFreteRota", "/ViewsScripts/Fretes/TabelaFreteRota/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaFreteTipoOperacao", "/ViewsScripts/Fretes/TabelaFreteTipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaTabelaFrete", "/ViewsScripts/Fretes/ConsultaTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/extracaoMassivaTabelaFrete", "/ViewsScripts/Fretes/ExtracaoMassivaTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoFreteTransportador", "/ViewsScripts/Fretes/ContratoFreteTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ajusteTabelaFrete", "/ViewsScripts/Fretes/AjusteTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoReajuste", "/ViewsScripts/Fretes/MotivoReajuste/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoAdicionalFrete", "/ViewsScripts/Fretes/MotivoAdicionalFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoTabelaFrete", "/ViewsScripts/Fretes/RegrasAutorizacaoValorFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/bonificacaoTransportador", "/ViewsScripts/Fretes/BonificacaoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaReajusteTabelaFrete", "/ViewsScripts/Fretes/ConsultaReajusteTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraContratoFreteTransportador", "/ViewsScripts/Fretes/RegraContratoFreteTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoContratoFreteTransportador", "/ViewsScripts/Fretes/AutorizacaoContratoFreteTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleReajusteFretePlanilha", "/ViewsScripts/Fretes/ControleReajusteFretePlanilha/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraControleReajusteFretePlanilha", "/ViewsScripts/Fretes/RegraControleReajusteFretePlanilha/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoControleReajusteFretePlanilha", "/ViewsScripts/Fretes/AutorizacaoControleReajusteFretePlanilha/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoContratoFrete", "/ViewsScripts/Fretes/TipoContratoFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoNotificacaoContrato", "/ViewsScripts/Fretes/ConfiguracaoNotificacaoContrato/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/destinoPrioritarioCalculoFrete", "/ViewsScripts/Fretes/DestinoPrioritarioCalculoFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoDescargaCliente", "/ViewsScripts/Fretes/ConfiguracaoDescargaCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoAjusteFrete", "/ViewsScripts/Fretes/MotivoRejeicaoAjuste/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/licitacao", "/ViewsScripts/Fretes/Licitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/licitacaoParticipacao", "/ViewsScripts/Fretes/LicitacaoParticipacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/licitacaoParticipacaoAvaliacao", "/ViewsScripts/Fretes/LicitacaoParticipacaoAvaliacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agruparTabelaFreteCliente", "/ViewsScripts/Fretes/AgruparTabelaFreteCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/exportacaoTabelaFrete", "/ViewsScripts/Fretes/ExportacaoTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoTabelaFrete", "/ViewsScripts/Fretes/RegraAutorizacaoTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoTabelaFrete", "/ViewsScripts/Fretes/AutorizacaoTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoContratoPrestacaoServico", "/ViewsScripts/Fretes/RegraAutorizacaoContratoPrestacaoServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoPrestacaoServico", "/ViewsScripts/Fretes/ContratoPrestacaoServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoContratoPrestacaoServico", "/ViewsScripts/Fretes/AutorizacaoContratoPrestacaoServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoPrestacaoServicoSaldo", "/ViewsScripts/Fretes/ContratoPrestacaoServicoSaldo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaPontuacao", "/ViewsScripts/Fretes/TabelaPontuacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoPontuacao", "/ViewsScripts/Fretes/FechamentoPontuacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/apuracaoBonificacao", "/ViewsScripts/Fretes/ApuracaoBonificacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tempoEsperaPorPontuacao", "/ViewsScripts/Fretes/TempoEsperaPorPontuacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoDevolucaoEntrega", "/ViewsScripts/GestaoEntregas/MotivoDevolucaoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/produtoOpentech", "/ViewsScripts/Produtos/ProdutoOpentech/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoAdvertenciaTransportador", "/ViewsScripts/Fretes/MotivoAdvertenciaTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/advertenciaTransportador", "/ViewsScripts/Fretes/AdvertenciaTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/solicitacaoLicitacao", "/ViewsScripts/Fretes/SolicitacaoLicitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaValores", "/ViewsScripts/Fretes/TabelaValores/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoTransportadorFrete", "/ViewsScripts/Fretes/ContratoTransportadorFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/aprovacaoContratoTransporteFrete", "/ViewsScripts/Fretes/AprovacaoContratoTransporteFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoRecusaCheckin", "/ViewsScripts/Fretes/RegraAutorizacaoRecusaCheckin/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/RegraAutorizacaoTaxaDescarga", "/ViewsScripts/Fretes/RegraAutorizacaoTaxaDescarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoRecusaCheckin", "/ViewsScripts/Fretes/AutorizacaoRecusaCheckin/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoTaxaDescarga", "/ViewsScripts/Fretes/AutorizacaoTaxaDescarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasInclusaoICMS", "/ViewsScripts/Fretes/regrasInclusaoICMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rotaTabelaFreteCliente", "/ViewsScripts/Fretes/RotaTabelaFreteCliente/**/*.js").UseContentRoot();
                #endregion

                #region Frota

                pipeline.AddJavaScriptBundle("/scripts/servicoVeiculo", "/ViewsScripts/Frota/ServicoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ordemServico", "/ViewsScripts/Frota/OrdemServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/adiantamento", "/ViewsScripts/Frota/Adiantamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/finalidadeProdutoOrdemServico", "/ViewsScripts/Frota/FinalidadeProdutoOrdemServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoMovimentoMotorista", "/ViewsScripts/Frota/TipoMovimentoMotorista/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoInfracao", "/ViewsScripts/Frota/TipoInfracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/infracao", "/ViewsScripts/Frota/Infracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoInfracao", "/ViewsScripts/Frota/RegraAutorizacaoInfracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoInfracao", "/ViewsScripts/Frota/AutorizacaoInfracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/almoxarifado", "/ViewsScripts/Frota/Almoxarifado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/dimensaoPneu", "/ViewsScripts/Frota/DimensaoPneu/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/marcaPneu", "/ViewsScripts/Frota/MarcaPneu/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modeloPneu", "/ViewsScripts/Frota/ModeloPneu/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/bandaRodagemPneu", "/ViewsScripts/Frota/BandaRodagemPneu/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoSucateamentoPneu", "/ViewsScripts/Frota/MotivoSucateamentoPneu/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pneu", "/ViewsScripts/Frota/Pneu/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/movimentacaoPneu", "/ViewsScripts/Frota/MovimentacaoPneu/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoOrdemServico", "/ViewsScripts/Frota/TipoOrdemServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoAlocacao", "/ViewsScripts/Frota/ProgramacaoAlocacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoEspecialidade", "/ViewsScripts/Frota/ProgramacaoEspecialidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoLicenciamento", "/ViewsScripts/Frota/ProgramacaoLicenciamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoSituacao", "/ViewsScripts/Frota/ProgramacaoSituacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoSituacaoTMS", "/ViewsScripts/Frota/programacaoSituacaoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoMotorista", "/ViewsScripts/Frota/ProgramacaoMotorista/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoVeiculo", "/ViewsScripts/Frota/ProgramacaoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoVeiculoTMS", "/ViewsScripts/Frota/ProgramacaoVeiculoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pneuLote", "/ViewsScripts/Frota/PneuLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleTacografo", "/ViewsScripts/Frota/ControleTacografo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoServico", "/ViewsScripts/Frota/GrupoServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoOrdemServico", "/ViewsScripts/Frota/RegraAutorizacaoOrdemServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoOrdemServico", "/ViewsScripts/Frota/AutorizacaoOrdemServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/programacaoLogistica", "/ViewsScripts/Frota/ProgramacaoLogistica/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/despesaFrotaPropria", "/ViewsScripts/Frota/DespesaFrotaPropria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoPrecoCombustivel", "/ViewsScripts/Frota/ImportacaoPrecoCombustivel/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/sinistro", "/ViewsScripts/Frota/Sinistro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoPedagio", "/ViewsScripts/Frota/ImportacaoPedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/geracaoFrotaAutomatizada", "/ViewsScripts/Frota/GeracaoFrotaAutomatizada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/sugestaoMensal", "/ViewsScripts/Frota/SugestaoMensal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/listaDiariaFrota", "/ViewsScripts/Frota/ListaDiaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaDeIndisponibilidade", "/ViewsScripts/Frota/JustificativaDeIndisponibilidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rodizioPlacas", "/ViewsScripts/Frota/RodizioPlacas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/indicadoresManutencao", "/ViewsScripts/Frota/IndicadoresManutencao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoSinistro", "/ViewsScripts/Frota/TipoSinistro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gravidadeSinistro", "/ViewsScripts/Frota/GravidadeSinistro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoLocalManutencao", "/ViewsScripts/Frota/TipoLocalManutencao/**/*.js").UseContentRoot();

                #endregion

                #region Gerenciamento Irregularidades

                pipeline.AddJavaScriptBundle("/scripts/portfolioModuloControle", "/ViewsScripts/GerenciamentoIrregularidades/PortfolioModuloControle/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/irregularidade", "/ViewsScripts/GerenciamentoIrregularidades/Irregularidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivosIrregularidades", "/ViewsScripts/GerenciamentoIrregularidades/MotivosIrregularidades/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoDesacordo", "/ViewsScripts/GerenciamentoIrregularidades/MotivoDesacordo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/definicaoTratativasIrregularidade", "/ViewsScripts/GerenciamentoIrregularidades/DefinicaoTratativasIrregularidade/**/*.js").UseContentRoot();

                #endregion

                #region Gestao Patio

                pipeline.AddJavaScriptBundle("/scripts/checkListOpcoes", "/ViewsScripts/GestaoPatio/CheckListOpcoes/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/checkList", "/ViewsScripts/GestaoPatio/CheckList/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/anexosProdutor", "/ViewsScripts/GestaoPatio/AnexosProdutor/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/checkListObservacao", "/ViewsScripts/GestaoPatio/CheckListObservacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/travamentoChave", "/ViewsScripts/GestaoPatio/TravamentoChave/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fluxoPatio", "/ViewsScripts/GestaoPatio/FluxoPatio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fluxoPatioTable", "/ViewsScripts/GestaoPatio/FluxoPatioTable/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/docaCarregamento", "/ViewsScripts/GestaoPatio/DocaCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/guaritaCheckList", "/ViewsScripts/GestaoPatio/GuaritaCheckList/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/disponibilidadeVeiculo", "/ViewsScripts/GestaoPatio/DisponibilidadeVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/checkListTipo", "/ViewsScripts/GestaoPatio/CheckListTipo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleVisita", "/ViewsScripts/GestaoPatio/ControleVisita/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ocorrenciaPatio", "/ViewsScripts/GestaoPatio/OcorrenciaPatio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ocorrenciaPatioTipo", "/ViewsScripts/GestaoPatio/OcorrenciaPatioTipo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/solicitacaoVeiculo", "/ViewsScripts/GestaoPatio/SolicitacaoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/observacaoFluxoPatio", "/ViewsScripts/GestaoPatio/ObservacaoFluxoPatio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/adicionarCargaFluxoPatio", "/ViewsScripts/GestaoPatio/AdicionarCargaFluxoPatio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/checkListComponent", "/ViewsScripts/GestaoPatio/CheckListComponent/KoPergunta.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fluxoPatioPesagem", "/ViewsScripts/GestaoPatio/FluxoPatioPesagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoPesagem", "/ViewsScripts/GestaoPatio/FechamentoPesagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/documentosPesagem", "/ViewsScripts/GestaoPatio/DocumentosPesagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/observacaoGuarita", "/ViewsScripts/GestaoPatio/ObservacaoGuarita/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/montagemCargaPatio", "/ViewsScripts/GestaoPatio/MontagemCargaPatio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/separacaoMercadoria", "/ViewsScripts/GestaoPatio/SeparacaoMercadoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasMultaAtrasoRetirada", "/ViewsScripts/GestaoPatio/RegrasMultaAtrasoRetirada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fluxoPatioIntegracao", "/ViewsScripts/GestaoPatio/FluxoPatioIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoPesagem", "/ViewsScripts/GestaoPatio/RegrasAutorizacaoPesagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoToleranciaPesagem", "/ViewsScripts/GestaoPatio/ConfiguracaoToleranciaPesagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/checkListVigencia", "/ViewsScripts/GestaoPatio/CheckListVigencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPesagem", "/ViewsScripts/GestaoPatio/AutorizacaoPesagem/**/*.js").UseContentRoot();

                #endregion

                #region Gestao Entregas

                pipeline.AddJavaScriptBundle("/scripts/fluxoEntrega", "/ViewsScripts/GestaoEntregas/FluxoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/suaEntrega", "/ViewsScripts/GestaoEntregas/SuaEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acaoDevolucaoMotorista", "/ViewsScripts/GestaoEntregas/AcaoDevolucaoMotorista/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoPortalCliente", "/ViewsScripts/GestaoEntregas/ConfiguracaoPortalCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rastreioEntrega", "/ViewsScripts/GestaoEntregas/RastreioEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/novoRastreioEntrega", "/ViewsScripts/GestaoEntregas/NovoRastreioEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/renderizarEntrega", "/ViewsScripts/TorreControle/RenderizarPDFTorre/RenderizarPDFTorre.js").UseContentRoot();

                #endregion

                #region Globais

                pipeline.AddJavaScriptBundle("/scripts/consultas", "/ViewsScripts/Consultas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/enumeradores", "/ViewsScripts/Enumeradores/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/global", "/js/Global/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/componenteImportacao", "/js/Importacao/**/*.js");

                pipeline.AddJavaScriptBundle("/scripts/formularioFavorito", "/ViewsScripts/Global/FormularioFavorito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/centralProcessamentoGlobal", "/ViewsScripts/Global/CentralProcessamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notificacaoGlobal", "/ViewsScripts/Global/Notificacoes/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/signalRGlobal", "/ViewsScripts/Global/SignalR/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ajudaUsuario", "/ViewsScripts/Global/Ajuda/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/chat", "/ViewsScripts/Global/Chat/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/chatManager", "/js/smart-chat-ui/smart.chat.manager.js");
                pipeline.AddJavaScriptBundle("/scripts/chatUI", "/js/smart-chat-ui/smart.chat.ui.js");
                pipeline.AddJavaScriptBundle("/scripts/dica", "/ViewsScripts/Global/Dicas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/smartChat", "/js/smart-chat-ui/**/*.js");
                pipeline.AddJavaScriptBundle("/scripts/modeloGrid", "/ViewsScripts/Global/ModeloGrid/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modeloFiltroPesquisa", "/ViewsScripts/Global/ModeloFiltroPesquisa/**/*.js").UseContentRoot();

                #endregion

                #region Guias
                pipeline.AddJavaScriptBundle("/scripts/guias", "/ViewsScripts/Guias/GuiasRecolhimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/vincularGuia", "/ViewsScripts/Guias/VincularGuia/**/*.js").UseContentRoot();
                #endregion

                #region Home

                pipeline.AddJavaScriptBundle("/scripts/home", "/ViewsScripts/Home/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/homeCliente", "/ViewsScripts/HomeCliente/**/*.js").UseContentRoot();

                #endregion

                #region HomeCabotagem

                //pipeline.AddJavaScriptBundle("/scripts/homeCabotagem", "/ViewsScripts/HomeCabotagem/**/*.js").UseContentRoot();

                #endregion

                #region ICMS

                pipeline.AddJavaScriptBundle("/scripts/regraICMS", "/ViewsScripts/ICMS/RegraICMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/aliquotaICMS", "/ViewsScripts/ICMS/AliquotaICMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoAlteracaoRegraICMS", "/ViewsScripts/ICMS/RegraAutorizacaoAlteracaoRegraICMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAlteracaoRegraICMS", "/ViewsScripts/ICMS/AutorizacaoAlteracaoRegraICMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/coeficientePautaFiscal", "/ViewsScripts/ICMS/CoeficientePautaFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pautaFiscal", "/ViewsScripts/ICMS/PautaFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraExtensao", "/ViewsScripts/ICMS/RegraExtensao/**/*.js").UseContentRoot();

                #endregion

                #region Imposto

                pipeline.AddJavaScriptBundle("/scripts/outrasAliquotas", "/ViewsScripts/Imposto/OutrasAliquotas/**/*.js").UseContentRoot();

                #endregion


                #region Importações

                pipeline.AddJavaScriptBundle("/scripts/importacaoTabelaFrete", "/ViewsScripts/Importacoes/TabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoRPS", "/ViewsScripts/Importacoes/ImportacaoRPS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoArquivoTabelaFrete", "/ViewsScripts/Importacoes/ArquivoTabelaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoHierarquia", "/ViewsScripts/Importacoes/Hierarquia/**/*.js").UseContentRoot();

                #endregion

                #region Integrações

                pipeline.AddJavaScriptBundle("/scripts/documentoTransporteNatura", "/ViewsScripts/Integracoes/DocumentoTransporteNatura/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integradora", "/ViewsScripts/Integracoes/Integradora/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integracaoHUB", "/ViewsScripts/Integracoes/IntegracaoHUB/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/preFaturaNatura", "/ViewsScripts/Integracoes/PreFaturaNatura/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleIntegracao", "/ViewsScripts/Integracoes/ControleIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/indicadorIntegracaoCTe", "/ViewsScripts/Integracoes/IndicadorIntegracaoCTe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/indicadorIntegracaoNFe", "/ViewsScripts/Integracoes/IndicadorIntegracaoNFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/documentoHavan", "/ViewsScripts/Integracoes/DocumentoHavan/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/identificacaoMercadoriaKrona", "/ViewsScripts/Integracoes/IdentificacaoMercadoriaKrona/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteCliente", "/ViewsScripts/Integracoes/LoteCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/IntegracaoprocessamentoEDIFTP", "/ViewsScripts/Integracoes/IntegracaoProcessamentoEDIFTP/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/exportacaoArquivoIntegracao", "/ViewsScripts/Integracoes/ExportacaoArquivoIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integradoraIntegracaoRetorno", "/ViewsScripts/Integracoes/IntegradoraIntegracaoRetorno/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleDasIntegracoes", "/ViewsScripts/Integracoes/ControlesDasIntegracoes/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integracaoGhost", "/ViewsScripts/Integracoes/IntegracaoGhost/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoIntegracaoTencnologiaMonitoramento", "/ViewsScripts/Integracoes/ConfiguracaoIntegracaoTencnologiaMonitoramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integracaoEnvioProgramado", "/ViewsScripts/Integracoes/IntegracaoEnvioProgramado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/documentoElectrolux", "/ViewsScripts/Integracoes/DocumentoElectrolux/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/integracaoAssincrona", "/ViewsScripts/Integracoes/IntegracaoAssincrona/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoFilialIntegracao", "/ViewsScripts/Integracoes/configuracaoFilialIntegracao/**/*.js").UseContentRoot();

                #endregion

                #region Pessoas

                pipeline.AddJavaScriptBundle("/scripts/aliquotaISS", "/ViewsScripts/ISS/AliquotaISS/**/*.js").UseContentRoot();

                #endregion

                #region Justificativas

                pipeline.AddJavaScriptBundle("/scripts/justificativas", "/ViewsScripts/Justificativas/EncerramentoManualViagem/**/*.js").UseContentRoot();

                #endregion

                #region Localidades

                pipeline.AddJavaScriptBundle("/scripts/regiao", "/ViewsScripts/Localidades/Regiao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/localidade", "/ViewsScripts/Localidades/Localidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/mesoRegiao", "/ViewsScripts/Localidades/MesoRegiao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pais", "/ViewsScripts/Localidades/Pais/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/estado", "/ViewsScripts/Localidades/Estado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/distribuidorPorRegiao", "/ViewsScripts/Localidades/DistribuidorPorRegiao/**/*.js").UseContentRoot();


                #endregion

                #region Localization

                pipeline.AddJavaScriptBundle("/scripts/localization", "/ViewsScripts/Localization/**/*.js").UseContentRoot();

                #endregion

                #region Logística

                pipeline.AddJavaScriptBundle("/scripts/percursosEntreEstados", "/ViewsScripts/Logistica/PercursosEntreEstados/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rastreamentoMonitoramento", settings, "/ViewsScripts/Logistica/RastreamentoMonitoramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rota", "/ViewsScripts/Logistica/Rota/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fronteira", "/ViewsScripts/Logistica/Fronteira/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/centroCarregamento", "/ViewsScripts/Logistica/CentroCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/centroDescarregamento", "/ViewsScripts/Logistica/CentroDescarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/mapas", "/ViewsScripts/Logistica/Mapas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rotaFrete", "/ViewsScripts/Logistica/RotaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/prazoSituacaoCarga", "/ViewsScripts/Logistica/PrazoSituacaoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaCarregamento", "/ViewsScripts/Logistica/JanelaCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaCarregamentoTransportador", "/ViewsScripts/Logistica/JanelaCarregamentoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/guarita", "/ViewsScripts/Logistica/Guarita/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/expedicao", "/ViewsScripts/Logistica/Expedicao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaDisponibilidadeCarregamento", "/ViewsScripts/Logistica/ConsultaDisponibilidadeCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/reservaCargaGrupoPessoa", "/ViewsScripts/Logistica/ReservaCargaGrupoPessoa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaDescarregamento", "/ViewsScripts/Logistica/JanelaDescarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/JanelaDescarregamentoSituacao", "/ViewsScripts/Logistica/JanelaDescarregamentoSituacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/guaritaTMS", "/ViewsScripts/Logistica/GuaritaTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/filaCarregamento", "/ViewsScripts/Logistica/FilaCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoPunicaoVeiculo", "/ViewsScripts/Logistica/MotivoPunicaoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/punicaoVeiculo", "/ViewsScripts/Logistica/PunicaoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRetiradaFilaCarregamento", "/ViewsScripts/Logistica/MotivoRetiradaFilaCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/restricaoRodagem", "/ViewsScripts/Logistica/RestricaoRodagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pracaPedagio", "/ViewsScripts/Logistica/PracaPedagio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/marcacaoFilaCarregamento", "/ViewsScripts/Logistica/MarcacaoFilaCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoSelecaoMotoristaForaOrdem", "/ViewsScripts/Logistica/MotivoSelecaoMotoristaForaOrdem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/filaCarregamentoNotificacao", "/ViewsScripts/Logistica/FilaCarregamentoNotificacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoCentroCarregamentoVeiculo", "/ViewsScripts/Logistica/ImportacaoCentroCarregamentoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentoFilaCarregamentoReversa", "/ViewsScripts/Logistica/AcompanhamentoFilaCarregamentoReversa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/locais", "/ViewsScripts/Logistica/Locais/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/categoriaresponsavel", "/ViewsScripts/Logistica/CategoriaResponsavel/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tracking", "/ViewsScripts/Logistica/Tracking/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramento", "/ViewsScripts/Logistica/Monitoramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoNovo", "/ViewsScripts/Logistica/MonitoramentoNovo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/categoriaResponsavel", "/ViewsScripts/Logistica/CategoriaResponsavel/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoEvento", "/ViewsScripts/Logistica/MonitoramentoEvento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/posicaoDaFrotaMapa", "/ViewsScripts/Logistica/PosicaoDaFrotaMapa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alertaTratativaAcao", "/ViewsScripts/Logistica/AlertaTratativaAcao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/areaVeiculo", "/ViewsScripts/Logistica/AreaVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/manobraAcao", "/ViewsScripts/Logistica/ManobraAcao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/manobra", "/ViewsScripts/Logistica/Manobra/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/filaCarregamentoReversa", "/ViewsScripts/Logistica/FilaCarregamentoReversa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleCarregamento", "/ViewsScripts/Logistica/ControleCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoPosicao", "/ViewsScripts/Logistica/MonitoramentoPosicao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoAlteracaoPosicaoFilaCarregamento", "/ViewsScripts/Logistica/MotivoAlteracaoPosicaoFilaCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaColeta", "/ViewsScripts/Logistica/JanelaColeta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/termoQuitacao", "/ViewsScripts/Logistica/TermoQuitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoTermoQuitacao", "/ViewsScripts/Logistica/RegraAutorizacaoTermoQuitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoTermoQuitacao", "/ViewsScripts/Logistica/AutorizacaoTermoQuitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rotaFreteClassificacao", "/ViewsScripts/Logistica/RotaFreteClassificacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/PosicaoFrota", "/ViewsScripts/Logistica/PosicaoFrota/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/PosicaoFrota2", "/ViewsScripts/Logistica/PosicaoFrota2/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoSubareaCliente", "/ViewsScripts/Logistica/TipoSubareaCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/MonitoramentoGrupoStatusViagem", "/ViewsScripts/Logistica/MonitoramentoGrupoStatusViagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoStatusViagem", "/ViewsScripts/Logistica/MonitoramentoStatusViagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/excecaoCapacidadeCarregamento", "/ViewsScripts/Logistica/ExcecaoCapacidadeCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/excecaoCapacidadeDescarregamento", "/ViewsScripts/Logistica/ExcecaoCapacidadeDescarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/capacidadeCarregamentoAdicional", "/ViewsScripts/Logistica/CapacidadeCarregamentoAdicional/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/capacidadeDescarregamentoAdicional", "/ViewsScripts/Logistica/CapacidadeDescarregamentoAdicional/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaDescarga", "/ViewsScripts/Logistica/JanelaDescarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoRotaFrete", "/ViewsScripts/Logistica/ConfiguracaoRotaFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/procedimentoEmbarque", "/ViewsScripts/Logistica/ProcedimentoEmbarque/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/historicoParadas", "/ViewsScripts/Logistica/HistoricoParadas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agendamentoColeta", "/ViewsScripts/Logistica/AgendamentoColeta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleViagem", "/ViewsScripts/Logistica/ControleViagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pesagem", "/ViewsScripts/Logistica/Pesagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/exclusividadeCarregamento", "/ViewsScripts/Logistica/ExclusividadeCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoParadaCentro", "/ViewsScripts/Logistica/MotivoParadaCentro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agendamentoColetaAnexo", "/ViewsScripts/Logistica/AgendamentoColeta/AgendamentoColetaAnexo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/informacaoDescarga", "/ViewsScripts/Logistica/InformacaoDescarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/abastecimentoGas", "/ViewsScripts/Logistica/AbastecimentoGas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/aprovacaoSolicitacaoGas", "/ViewsScripts/Logistica/AprovacaoSolicitacaoGas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consolidacaoSolicitacaoGas", "/ViewsScripts/Logistica/ConsolidacaoSolicitacaoGas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAprovacaoSolicitacaoGas", "/ViewsScripts/Logistica/RegraAprovacaoSolicitacaoGas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agendamentoEntregaPedido", "/ViewsScripts/Logistica/AgendamentoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agendamentoEntregaPedidoConsulta", "/ViewsScripts/Logistica/AgendamentoEntregaConsulta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/diariaAutomatica", "/ViewsScripts/Logistica/DiariaAutomatica/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alertaMonitoramento", "/ViewsScripts/Logistica/AlertaMonitoramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/datasPreferenciaisDescarga", "/ViewsScripts/Logistica/DatasPreferenciaisDescarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/torreMonitoramento", "/ViewsScripts/Logistica/TorreMonitoramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaCarregamentoSituacao", "/ViewsScripts/Logistica/JanelaCarregamentoSituacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaDescargaSituacao", "/ViewsScripts/Logistica/JanelaDescargaSituacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoTecnologia", "/ViewsScripts/Logistica/MonitoramentoTecnologia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoProgramacaoColeta", "/ViewsScripts/Logistica/ImportacaoProgramacaoColeta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoReagendamento", "/ViewsScripts/Logistica/MotivoReagendamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoDetalhes", "/ViewsScripts/Logistica/MonitoramentoDetalhes/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitoramentoHistorico", "/ViewsScripts/Logistica/MonitoramentoHistorico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/posicaoAtualVeiculo", "/ViewsScripts/Logistica/PosicaoAtualVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoAtrasoCarregamento", "/ViewsScripts/Logistica/MotivoAtrasoCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/VistoriaCheckList", "/ViewsScripts/Logistica/VistoriaCheckList/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/centroDistribuicao", "/ViewsScripts/Logistica/CentroDistribuicao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/trechoBalsa", "/ViewsScripts/Logistica/TrechoBalsa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaCarregamentoIntegracao", "/ViewsScripts/Logistica/JanelaCarregamentoIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configFiliaisFilaCarregamento", "/ViewsScripts/Logistica/ConfigFiliaisFilaCarregamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/justificativaCancelamentoAgendamento", "/ViewsScripts/Logistica/JustificativaCancelamentoAgendamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rotaFreteAbastecimento", "/ViewsScripts/Logistica/RotaFreteAbastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/centroCustoViagem", "/ViewsScripts/Logistica/CentroCustoViagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/associacaoBalsa", "/ViewsScripts/Logistica/AssociacaoBalsa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentoChecklist", "/ViewsScripts/Logistica/acompanhamentoChecklist/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoMotoristas", "/ViewsScripts/Logistica/GrupoMotoristas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/janelaCarregamentoDetalhesCarga", "/ViewsScripts/Logistica/JanelaCarregamentoDetalhesCarga/**/*.js").UseContentRoot();

                #endregion

                #region MDF-e

                pipeline.AddJavaScriptBundle("/scripts/encerramentoTransportador", "/ViewsScripts/MDFe/EncerramentoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/encerramentoMDFe", "/ViewsScripts/MDFe/Encerramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaMDFe", "/ViewsScripts/MDFe/ConsultaMDFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleEmissaoMDFe", "/ViewsScripts/MDFe/ControleEmissaoMDFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/encerramentoTMS", "/ViewsScripts/MDFe/EncerramentoTMS/**/*.js").UseContentRoot();

                #endregion

                #region Moedas

                pipeline.AddJavaScriptBundle("/scripts/cotacao", "/ViewsScripts/Moedas/Cotacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/moeda", "/ViewsScripts/Moedas/Moeda/**/*.js").UseContentRoot();

                #endregion

                #region Notificações

                pipeline.AddJavaScriptBundle("/scripts/notificacao", "/ViewsScripts/Notificacoes/Notificacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/mensagemAviso", "/ViewsScripts/Notificacoes/MensagemAviso/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoAlerta", "/ViewsScripts/Notificacoes/ConfiguracaoAlerta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alertaEmail", "/ViewsScripts/Notificacoes/AlertaEmail/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoNCPendente", "/ViewsScripts/Notificacoes/ConfiguracaoNCPendente/**/*.js").UseContentRoot();

                #endregion

                #region NotaFiscalConsumidor

                pipeline.AddJavaScriptBundle("/scripts/notaFiscalConsumidor", "/ViewsScripts/NotasFiscaisConsumidores/NotaFiscalConsumidor/**/*.js").UseContentRoot();

                #endregion

                #region NotaFiscalServico

                pipeline.AddJavaScriptBundle("/scripts/notaFiscalServico", "/ViewsScripts/NotasFiscaisServicos/NotaFiscalServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/nfse", "/ViewsScripts/NotasFiscaisServicos/NFSe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/controleEmissaoNFSe", "/ViewsScripts/NotasFiscaisServicos/ControleEmissaoNFSe/**/*.js").UseContentRoot();

                #endregion

                #region NotaFiscal

                pipeline.AddJavaScriptBundle("/scripts/retornoSefaz", "/ViewsScripts/NotasFiscais/RetornoSefaz/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cfopNotaFiscal", "/ViewsScripts/NotasFiscais/CFOP/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/observacaoFiscal", "/ViewsScripts/NotasFiscais/ObservacaoFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/naturezaDaOperacao", "/ViewsScripts/NotasFiscais/NaturezaDaOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/servico", "/ViewsScripts/NotasFiscais/Servico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoImposto", "/ViewsScripts/NotasFiscais/GrupoImposto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modeloNotaFiscal", "/ViewsScripts/NotasFiscais/ModeloDocumentoFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaFiscalEletronica", "/ViewsScripts/NotasFiscais/NotaFiscalEletronica/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/nfeEletronica", "/ViewsScripts/NotasFiscais/NFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaFiscalObservacaoCartaCorrecao", "/ViewsScripts/NotasFiscais/NotaFiscalObservacaoCartaCorrecao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaFiscalInutilizar", "/ViewsScripts/NotasFiscais/NotaFiscalInutilizar/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/impostoIBPTNFe", "/ViewsScripts/NotasFiscais/ImpostoIBPTNFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoNotaFiscal", "/ViewsScripts/NotasFiscais/ContratoNotaFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/spedFiscal", "/ViewsScripts/NotasFiscais/SpedFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/sintegra", "/ViewsScripts/NotasFiscais/Sintegra/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/spedPISCOFINS", "/ViewsScripts/NotasFiscais/SpedPISCOFINS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaFiscalImportacao", "/ViewsScripts/NotasFiscais/NotaFiscalImportacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaFiscalSituacao", "/ViewsScripts/NotasFiscais/NotaFiscalSituacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/itemNaoConformidade", "/ViewsScripts/NotasFiscais/ItemNaoConformidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/conversaoUnidadeMedida", "/ViewsScripts/NotasFiscais/ConversaoUnidadeMedida/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/bloqueioEmissaoPorNaoConformidade", "/ViewsScripts/NotasFiscais/BloqueioEmissaoPorNaoConformidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoNaoConformidade", "/ViewsScripts/NotasFiscais/RegraAutorizacaoNaoConformidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/historicoNaoConformidade", "/ViewsScripts/NotasFiscais/HistoricoNaoConformidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoNaoConformidade", "/ViewsScripts/NotasFiscais/AutorizacaoNaoConformidade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/detalhesNotaFiscal", "/ViewsScripts/NotasFiscais/Detalhes/**/*.js").UseContentRoot();

                #endregion

                #region NFe

                pipeline.AddJavaScriptBundle("/scripts/gerarCTePorNFe", "/ViewsScripts/NFe/GerarCTePorNFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/envioXmlNota", "/ViewsScripts/NFe/UploadNotaXML/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/PainelNFeTransporte", "/ViewsScripts/NFe/PainelNFeTransporte/**/*.js").UseContentRoot();
                #endregion

                #region NFS

                pipeline.AddJavaScriptBundle("/scripts/informarNFSPendentes", "/ViewsScripts/NFS/InformarNFSPendentes/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/naturezaNFSe", "/ViewsScripts/NFS/NaturezaNFSe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/servicoNFSe", "/ViewsScripts/NFS/ServicoNFSe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/NFSManual", "/ViewsScripts/NFS/NFSManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoLancamento", "/ViewsScripts/NFS/MotivoRejeicaoLancamentoNFS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoNFSManual", "/ViewsScripts/NFS/RegrasAutorizacaoNFSManual/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoNFS", "/ViewsScripts/NFS/AutorizacaoNFS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/NFSManualCancelamento", "/ViewsScripts/NFS/NFSManualCancelamento/**/*.js").UseContentRoot();


                #endregion

                #region Ocorrências
                pipeline.AddJavaScriptBundle("/scripts/justificativaOcorrencia", "/ViewsScripts/Ocorrencias/JustificativaOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ocorrencia", "/ViewsScripts/Ocorrencias/Ocorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoOcorrencia", "/ViewsScripts/Ocorrencias/TipoOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoOcorrencia", "/ViewsScripts/Ocorrencias/GestaoOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/parametroOcorrencia", "/ViewsScripts/Ocorrencias/ParametroOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoOcorrencia", "/ViewsScripts/Ocorrencias/RegrasAutorizacaoOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoOcorrencia", "/ViewsScripts/Ocorrencias/Autorizacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoOcorrencia", "/ViewsScripts/Ocorrencias/MotivoRejeicaoOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ocorrenciaCancelamento", "/ViewsScripts/Ocorrencias/OcorrenciaCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/valorParametroOcorrencia", "/ViewsScripts/Ocorrencias/ValorParametroOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ocorrenciaParametroOcorrencia", "/ViewsScripts/Ocorrencias/OcorrenciaParametroOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/aceiteDebito", "/ViewsScripts/Ocorrencias/AceiteDebito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/logLeituraArquivoOcorrencia", "/ViewsScripts/Ocorrencias/LogLeituraArquivoOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraParcelamentoOcorrencia", "/ViewsScripts/Ocorrencias/RegraParcelamentoOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importarOcorrencia", "/ViewsScripts/Ocorrencias/ImportarOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoTipoDeOcorrencia", "/ViewsScripts/Ocorrencias/GrupoTipoDeOcorrencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ocorrenciaLote", "/ViewsScripts/Ocorrencias/OcorrenciaLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tiposCausadoresOcorrencia", "/ViewsScripts/Ocorrencias/TiposCausadoresOcorrencia/**/*.js").UseContentRoot();

                #endregion

                #region Operacional

                pipeline.AddJavaScriptBundle("/scripts/configOperador", "/ViewsScripts/Operacional/ConfigOperador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/restricaoVisualizacaoCanhotos", "/ViewsScripts/Operacional/RestricaoVisualizacaoCanhotos/**/*.js").UseContentRoot();

                #endregion Operacional

                #region Operações Fiscais

                pipeline.AddJavaScriptBundle("/scripts/cfop", "/ViewsScripts/OperacoesFiscais/CFOP/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/naturezaOperacao", "/ViewsScripts/OperacoesFiscais/NaturezaOperacao/**/*.js").UseContentRoot();

                #endregion Operações Fiscais

                #region Patrimonio

                pipeline.AddJavaScriptBundle("/scripts/bem", "/ViewsScripts/Patrimonio/Bem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/manutencaoBem", "/ViewsScripts/Patrimonio/ManutencaoBem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/baixaBem", "/ViewsScripts/Patrimonio/BaixaBem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/transferenciaBem", "/ViewsScripts/Patrimonio/TransferenciaBem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/MotivoDefeito", "/ViewsScripts/Patrimonio/MotivoDefeito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planoServico", "/ViewsScripts/Patrimonio/PlanoServico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pet", "/ViewsScripts/Patrimonio/Pet/**/*.js").UseContentRoot();

                #endregion Patrimonio

                #region Pedidos

                pipeline.AddJavaScriptBundle("/scripts/pedido", "/ViewsScripts/Pedidos/Pedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCliente", "/ViewsScripts/Pedidos/PedidoCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/atendimentoPedidoCliente", "/ViewsScripts/Pedidos/AtendimentoCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/detalheEntrega", "/ViewsScripts/Pedidos/DetalheEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoOperacao", "/ViewsScripts/Pedidos/TipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/retiradaProduto", "/ViewsScripts/Pedidos/RetiradaProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoTipoOperacao", "/ViewsScripts/Pedidos/GrupoTipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoTerminalImportacao", "/ViewsScripts/Pedidos/TipoTerminalImportacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasPedido", "/ViewsScripts/Pedidos/RegrasPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPedido", "/ViewsScripts/Pedidos/AutorizacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/classificacaoRiscoONU", "/ViewsScripts/Pedidos/ClassificacaoRiscoONU/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCancelamento", "/ViewsScripts/Pedidos/PedidoCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraTomador", "/ViewsScripts/Pedidos/RegraTomador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canalEntrega", "/ViewsScripts/Pedidos/CanalEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canalVenda", "/ViewsScripts/Pedidos/CanalVenda/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/linhaSeparacao", "/ViewsScripts/Pedidos/LinhaSeparacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/liberarPedidos", "/ViewsScripts/Pedidos/LiberarPedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tiposDetalhePedido", "/ViewsScripts/Pedidos/TipoDetalhe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planejamentoPedido", "/ViewsScripts/Pedidos/PlanejamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/navio", "/ViewsScripts/Pedidos/Navio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoImportacaoAtrasada", "/ViewsScripts/Pedidos/MotivoImportacaoAtrasada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoContainer", "/ViewsScripts/Pedidos/TipoContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/container", "/ViewsScripts/Pedidos/Container/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/viagemNavio", "/ViewsScripts/Pedidos/ViagemNavio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/porto", "/ViewsScripts/Pedidos/Porto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentopedido", "/ViewsScripts/Pedidos/AcompanhamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSVM", "/ViewsScripts/Pedidos/PedidoSVM/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/PlanejamentoPedidoConfiguracao", "/ViewsScripts/Pedidos/PlanejamentoPedidoConfiguracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoTipoPagamento", "/ViewsScripts/Pedidos/PedidoTipoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoAlteracaoPedido", "/ViewsScripts/Pedidos/MotivoRejeicaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoAlteracaoPedido", "/ViewsScripts/Pedidos/RegraAutorizacaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAlteracaoPedido", "/ViewsScripts/Pedidos/AutorizacaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAlteracaoPedidoTransportador", "/ViewsScripts/Pedidos/AutorizacaoAlteracaoPedidoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCampoObrigatorio", "/ViewsScripts/Pedidos/PedidoCampoObrigatorio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planejamentoPedidoTMS", "/ViewsScripts/Pedidos/PlanejamentoPedidoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSimplificado", "/ViewsScripts/Pedidos/PedidoSimplificado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSimplificadoTMS", "/ViewsScripts/Pedidos/PedidoSimplificadoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoPedido", "/ViewsScripts/Pedidos/ImportacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoOcorrenciaColetaEntregaIntegracao", "/ViewsScripts/Pedidos/PedidoOcorrenciaColetaEntregaIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importarEDI", "/ViewsScripts/Pedidos/ImportarEDI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoTakePay", "/ViewsScripts/Pedidos/ImportacaoTakePay/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaDeDebito", "/ViewsScripts/Pedidos/NotaDeDebito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoOcorrenciaPedido", "/ViewsScripts/Pedidos/ConfiguracaoOcorrenciaPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/numeroCarregamentoPorLote", "/ViewsScripts/Pedidos/NumeroCarregamentoPorLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/atendimentoPedido", "/ViewsScripts/Pedidos/AtendimentoAPedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultarAndamentoPedido", "/ViewsScripts/Pedidos/ConsultarAndamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaPedido", "/ViewsScripts/Pedidos/ConsultaPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/booking", "/ViewsScripts/Pedidos/Booking/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoBookingIntegracao", "/ViewsScripts/Pedidos/PedidoBookingIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraTipoOperacao", "/ViewsScripts/Pedidos/RegraTipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraPlanejamentoFrota", "/ViewsScripts/Pedidos/RegraPlanejamentoFrota/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notificacaoRetiradaProduto", "/ViewsScripts/Pedidos/NotificacaoRetiradaProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/auditoriaEMP", "/ViewsScripts/Pedidos/AuditoriaEMP/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoComercialPedido", "/ViewsScripts/Pedidos/SituacaoComercialPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteLiberacaoComercialPedido", "/ViewsScripts/Pedidos/LoteLiberacaoComercialPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoPedido", "/ViewsScripts/Pedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rolagemContainer", "/ViewsScripts/Pedidos/RolagemContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoEstoquePedido", "/ViewsScripts/Pedidos/SituacaoEstoquePedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoCancelamentoPedido", "/ViewsScripts/Pedidos/MotivoCancelamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cotacaoEspecial", "/ViewsScripts/Pedidos/Pedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedido", "/ViewsScripts/Pedidos/Pedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCliente", "/ViewsScripts/Pedidos/PedidoCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/atendimentoPedidoCliente", "/ViewsScripts/Pedidos/AtendimentoCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/detalheEntrega", "/ViewsScripts/Pedidos/DetalheEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoOperacao", "/ViewsScripts/Pedidos/TipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/retiradaProduto", "/ViewsScripts/Pedidos/RetiradaProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoTipoOperacao", "/ViewsScripts/Pedidos/GrupoTipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoTerminalImportacao", "/ViewsScripts/Pedidos/TipoTerminalImportacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasPedido", "/ViewsScripts/Pedidos/RegrasPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPedido", "/ViewsScripts/Pedidos/AutorizacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/classificacaoRiscoONU", "/ViewsScripts/Pedidos/ClassificacaoRiscoONU/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCancelamento", "/ViewsScripts/Pedidos/PedidoCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraTomador", "/ViewsScripts/Pedidos/RegraTomador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canalEntrega", "/ViewsScripts/Pedidos/CanalEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canalVenda", "/ViewsScripts/Pedidos/CanalVenda/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/linhaSeparacao", "/ViewsScripts/Pedidos/LinhaSeparacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/liberarPedidos", "/ViewsScripts/Pedidos/LiberarPedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tiposDetalhePedido", "/ViewsScripts/Pedidos/TipoDetalhe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planejamentoPedido", "/ViewsScripts/Pedidos/PlanejamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/navio", "/ViewsScripts/Pedidos/Navio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoImportacaoAtrasada", "/ViewsScripts/Pedidos/MotivoImportacaoAtrasada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoContainer", "/ViewsScripts/Pedidos/TipoContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/container", "/ViewsScripts/Pedidos/Container/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/viagemNavio", "/ViewsScripts/Pedidos/ViagemNavio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/porto", "/ViewsScripts/Pedidos/Porto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentopedido", "/ViewsScripts/Pedidos/AcompanhamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSVM", "/ViewsScripts/Pedidos/PedidoSVM/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/PlanejamentoPedidoConfiguracao", "/ViewsScripts/Pedidos/PlanejamentoPedidoConfiguracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoTipoPagamento", "/ViewsScripts/Pedidos/PedidoTipoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoAlteracaoPedido", "/ViewsScripts/Pedidos/MotivoRejeicaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoAlteracaoPedido", "/ViewsScripts/Pedidos/RegraAutorizacaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAlteracaoPedido", "/ViewsScripts/Pedidos/AutorizacaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAlteracaoPedidoTransportador", "/ViewsScripts/Pedidos/AutorizacaoAlteracaoPedidoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCampoObrigatorio", "/ViewsScripts/Pedidos/PedidoCampoObrigatorio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planejamentoPedidoTMS", "/ViewsScripts/Pedidos/PlanejamentoPedidoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSimplificado", "/ViewsScripts/Pedidos/PedidoSimplificado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSimplificadoTMS", "/ViewsScripts/Pedidos/PedidoSimplificadoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoPedido", "/ViewsScripts/Pedidos/ImportacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoOcorrenciaColetaEntregaIntegracao", "/ViewsScripts/Pedidos/PedidoOcorrenciaColetaEntregaIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importarEDI", "/ViewsScripts/Pedidos/ImportarEDI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoTakePay", "/ViewsScripts/Pedidos/ImportacaoTakePay/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaDeDebito", "/ViewsScripts/Pedidos/NotaDeDebito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoOcorrenciaPedido", "/ViewsScripts/Pedidos/ConfiguracaoOcorrenciaPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/numeroCarregamentoPorLote", "/ViewsScripts/Pedidos/NumeroCarregamentoPorLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/atendimentoPedido", "/ViewsScripts/Pedidos/AtendimentoAPedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultarAndamentoPedido", "/ViewsScripts/Pedidos/ConsultarAndamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaPedido", "/ViewsScripts/Pedidos/ConsultaPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/booking", "/ViewsScripts/Pedidos/Booking/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoBookingIntegracao", "/ViewsScripts/Pedidos/PedidoBookingIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraTipoOperacao", "/ViewsScripts/Pedidos/RegraTipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraPlanejamentoFrota", "/ViewsScripts/Pedidos/RegraPlanejamentoFrota/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notificacaoRetiradaProduto", "/ViewsScripts/Pedidos/NotificacaoRetiradaProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/auditoriaEMP", "/ViewsScripts/Pedidos/AuditoriaEMP/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoComercialPedido", "/ViewsScripts/Pedidos/SituacaoComercialPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteLiberacaoComercialPedido", "/ViewsScripts/Pedidos/LoteLiberacaoComercialPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoPedido", "/ViewsScripts/Pedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rolagemContainer", "/ViewsScripts/Pedidos/RolagemContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoEstoquePedido", "/ViewsScripts/Pedidos/SituacaoEstoquePedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoCancelamentoPedido", "/ViewsScripts/Pedidos/MotivoCancelamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedido", "/ViewsScripts/Pedidos/Pedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCliente", "/ViewsScripts/Pedidos/PedidoCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/atendimentoPedidoCliente", "/ViewsScripts/Pedidos/AtendimentoCliente/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/detalheEntrega", "/ViewsScripts/Pedidos/DetalheEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoOperacao", "/ViewsScripts/Pedidos/TipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/retiradaProduto", "/ViewsScripts/Pedidos/RetiradaProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoTipoOperacao", "/ViewsScripts/Pedidos/GrupoTipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoTerminalImportacao", "/ViewsScripts/Pedidos/TipoTerminalImportacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasPedido", "/ViewsScripts/Pedidos/RegrasPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPedido", "/ViewsScripts/Pedidos/AutorizacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/classificacaoRiscoONU", "/ViewsScripts/Pedidos/ClassificacaoRiscoONU/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCancelamento", "/ViewsScripts/Pedidos/PedidoCancelamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraTomador", "/ViewsScripts/Pedidos/RegraTomador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canalEntrega", "/ViewsScripts/Pedidos/CanalEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/canalVenda", "/ViewsScripts/Pedidos/CanalVenda/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/linhaSeparacao", "/ViewsScripts/Pedidos/LinhaSeparacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/liberarPedidos", "/ViewsScripts/Pedidos/LiberarPedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tiposDetalhePedido", "/ViewsScripts/Pedidos/TipoDetalhe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planejamentoPedido", "/ViewsScripts/Pedidos/PlanejamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/navio", "/ViewsScripts/Pedidos/Navio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoImportacaoAtrasada", "/ViewsScripts/Pedidos/MotivoImportacaoAtrasada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoContainer", "/ViewsScripts/Pedidos/TipoContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/container", "/ViewsScripts/Pedidos/Container/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/viagemNavio", "/ViewsScripts/Pedidos/ViagemNavio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/porto", "/ViewsScripts/Pedidos/Porto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentopedido", "/ViewsScripts/Pedidos/AcompanhamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSVM", "/ViewsScripts/Pedidos/PedidoSVM/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/PlanejamentoPedidoConfiguracao", "/ViewsScripts/Pedidos/PlanejamentoPedidoConfiguracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoTipoPagamento", "/ViewsScripts/Pedidos/PedidoTipoPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoRejeicaoAlteracaoPedido", "/ViewsScripts/Pedidos/MotivoRejeicaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoAlteracaoPedido", "/ViewsScripts/Pedidos/RegraAutorizacaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAlteracaoPedido", "/ViewsScripts/Pedidos/AutorizacaoAlteracaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAlteracaoPedidoTransportador", "/ViewsScripts/Pedidos/AutorizacaoAlteracaoPedidoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoCampoObrigatorio", "/ViewsScripts/Pedidos/PedidoCampoObrigatorio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoOperacaoValorPadrao", "/ViewsScripts/Pedidos/TipoOperacaoValorPadrao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planejamentoPedidoTMS", "/ViewsScripts/Pedidos/PlanejamentoPedidoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSimplificado", "/ViewsScripts/Pedidos/PedidoSimplificado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoSimplificadoTMS", "/ViewsScripts/Pedidos/PedidoSimplificadoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoPedido", "/ViewsScripts/Pedidos/ImportacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoOcorrenciaColetaEntregaIntegracao", "/ViewsScripts/Pedidos/PedidoOcorrenciaColetaEntregaIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importarEDI", "/ViewsScripts/Pedidos/ImportarEDI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoTakePay", "/ViewsScripts/Pedidos/ImportacaoTakePay/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notaDeDebito", "/ViewsScripts/Pedidos/NotaDeDebito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoOcorrenciaPedido", "/ViewsScripts/Pedidos/ConfiguracaoOcorrenciaPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/numeroCarregamentoPorLote", "/ViewsScripts/Pedidos/NumeroCarregamentoPorLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/atendimentoPedido", "/ViewsScripts/Pedidos/AtendimentoAPedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultarAndamentoPedido", "/ViewsScripts/Pedidos/ConsultarAndamentoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaPedido", "/ViewsScripts/Pedidos/ConsultaPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/booking", "/ViewsScripts/Pedidos/Booking/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pedidoBookingIntegracao", "/ViewsScripts/Pedidos/PedidoBookingIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraTipoOperacao", "/ViewsScripts/Pedidos/RegraTipoOperacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraPlanejamentoFrota", "/ViewsScripts/Pedidos/RegraPlanejamentoFrota/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/notificacaoRetiradaProduto", "/ViewsScripts/Pedidos/NotificacaoRetiradaProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/auditoriaEMP", "/ViewsScripts/Pedidos/AuditoriaEMP/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoComercialPedido", "/ViewsScripts/Pedidos/SituacaoComercialPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/loteLiberacaoComercialPedido", "/ViewsScripts/Pedidos/LoteLiberacaoComercialPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoPedido", "/ViewsScripts/Pedidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/rolagemContainer", "/ViewsScripts/Pedidos/RolagemContainer/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/situacaoEstoquePedido", "/ViewsScripts/Pedidos/SituacaoEstoquePedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoCancelamentoPedido", "/ViewsScripts/Pedidos/MotivoCancelamentoPedido/**/*.js").UseContentRoot();

                #endregion Pedidos

                #region PedidosVendas

                pipeline.AddJavaScriptBundle("/scripts/pedidoVenda", "/ViewsScripts/PedidosVendas/PedidoVenda/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ordemServicoVenda", "/ViewsScripts/PedidosVendas/OrdemServicoVenda/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/vendaDireta", "/ViewsScripts/PedidosVendas/VendaDireta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaPrecoVenda", "/ViewsScripts/PedidosVendas/TabelaPrecoVenda/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ordemServicoPet", "/ViewsScripts/PedidosVendas/OrdemServicoPet/**/*.js").UseContentRoot();

                #endregion PedidosVendas

                #region Pessoas

                pipeline.AddJavaScriptBundle("/scripts/usuario", "/ViewsScripts/Pessoas/Usuario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/cargo", "/ViewsScripts/Pessoas/Cargo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoPessoas", "/ViewsScripts/Pessoas/GrupoPessoas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pessoa", "/ViewsScripts/Pessoas/Pessoa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/descargaPessoa", "/ViewsScripts/Pessoas/Descarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alteracaoFormaPagamento", "/ViewsScripts/Pessoas/AlteracaoFormaPagamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/perfilAcesso", "/ViewsScripts/Pessoas/PerfilAcesso/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/politicaSenha", "/ViewsScripts/Pessoas/PoliticaSenha/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/representante", "/ViewsScripts/Pessoas/Representante/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/restricaoEntrega", "/ViewsScripts/Pessoas/RestricaoEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/setorFuncionario", "/ViewsScripts/Pessoas/SetorFuncionario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/funcionarioMeta", "/ViewsScripts/Pessoas/FuncionarioMeta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraFuncionarioComissao", "/ViewsScripts/Pessoas/RegraFuncionarioComissao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoFuncionarioComissao", "/ViewsScripts/Pessoas/AutorizacaoFuncionarioComissao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/funcionarioComissao", "/ViewsScripts/Pessoas/FuncionarioComissao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/categoriaPessoa", "/ViewsScripts/Pessoas/CategoriaPessoa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/colaboradorSituacao", "/ViewsScripts/Pessoas/ColaboradorSituacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/colaboradorSituacaoLancamento", "/ViewsScripts/Pessoas/ColaboradorSituacaoLancamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/perfilAcessoMobile", "/ViewsScripts/Pessoas/PerfilAcessoMobile/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contatoGrupoPessoa", "/ViewsScripts/Pessoas/ContatoGrupoPessoa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pessoaClassificacao", "/ViewsScripts/Pessoas/PessoaClassificacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pessoaCampoObrigatorio", "/ViewsScripts/Pessoas/PessoaCampoObrigatorio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/configuracaoBloqueioFinanceiro", "/ViewsScripts/Pessoas/ConfiguracaoBloqueioFinanceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/marcaEPI", "/ViewsScripts/Pessoas/MarcaEPI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoEPI", "/ViewsScripts/Pessoas/TipoEPI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/epi", "/ViewsScripts/Pessoas/EPI/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoTerceiro", "/ViewsScripts/Pessoas/TipoTerceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/clienteBuscaAutomatica", "/ViewsScripts/Pessoas/ClienteBuscaAutomatica/**/*.js").UseContentRoot();

                #endregion Pessoas

                #region Produtos

                pipeline.AddJavaScriptBundle("/scripts/grupoProduto", "/ViewsScripts/Produtos/GrupoProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/produto", "/ViewsScripts/Produtos/Produto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/produtoEmbarcador", "/ViewsScripts/Produtos/ProdutoEmbarcador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/alteracaoProduto", "/ViewsScripts/Produtos/AlteracaoProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/unidadeMedidaFornecedor", "/ViewsScripts/Produtos/UnidadeMedidaFornecedor/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoProdutoTMS", "/ViewsScripts/Produtos/GrupoProdutoTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/produtoNCMAbastecimento", "/ViewsScripts/Produtos/ProdutoNCMAbastecimento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/produtoLote", "/ViewsScripts/Produtos/ProdutoLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/localArmazenamentoProduto", "/ViewsScripts/Produtos/LocalArmazenamentoProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/marcaProduto", "/ViewsScripts/Produtos/MarcaProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoEmbalagem", "/ViewsScripts/Produtos/TipoEmbalagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoFalhaGTA", "/ViewsScripts/Produtos/MotivoFalhaGTA/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/conversaoDeUnidades", "/ViewsScripts/Produtos/ConversaoDeUnidades/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/vincularProdutosFornecedorEmbarcadorPorNFe", "/ViewsScripts/Produtos/VincularProdutosFornecedorEmbarcadorPorNFe/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ordemDeCompra", "/ViewsScripts/Produtos/OrdemDeCompra/**/*.js").UseContentRoot();

                #endregion Produtos

                #region Pagamentos Agregados

                pipeline.AddJavaScriptBundle("/scripts/regrasPagamentoAgregado", "/ViewsScripts/PagamentosAgregados/RegrasPagamentoAgregado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamentoAgregado", "/ViewsScripts/PagamentosAgregados/AutorizacaoPagamentoAgregado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pagamentoAgregado", "/ViewsScripts/PagamentosAgregados/PagamentoAgregado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ajudanteCarga", "/ViewsScripts/PagamentosAgregados/AjudanteCarga/**/*.js").UseContentRoot();

                #endregion Pagamentos Agregados

                #region Pagamentos Motorista

                pipeline.AddJavaScriptBundle("/scripts/pagamentoMotoristaTipo", "/ViewsScripts/PagamentosMotoristas/PagamentoMotoristaTipo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasPagamentoMotorista", "/ViewsScripts/PagamentosMotoristas/RegrasPagamentoMotorista/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pagamentoMotoristaTMS", "/ViewsScripts/PagamentosMotoristas/PagamentoMotoristaTMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamentoMotorista", "/ViewsScripts/PagamentosMotoristas/AutorizacaoPagamentoMotorista/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pagamentoMotoristaTMSLote", "/ViewsScripts/PagamentosMotoristas/PagamentoMotoristaTMSLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/pendenciaMotorista", "/ViewsScripts/PagamentosMotoristas/PendenciaMotorista/**/*.js").UseContentRoot();

                #endregion Pagamentos Motorista

                #region Pallets

                pipeline.AddJavaScriptBundle("/scripts/situacaoDevolucaoPallets", "/ViewsScripts/Pallets/SituacaoDevolucao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/devolucaoPallets", "/ViewsScripts/Pallets/Devolucao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ajusteSaldoPallets", "/ViewsScripts/Pallets/AjusteSaldo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/estoqueFilial", "/ViewsScripts/Pallets/EstoqueFilial/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motivoAvariaPallet", "/ViewsScripts/Pallets/MotivoAvariaPallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/transferencia", "/ViewsScripts/Pallets/Transferencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoTransferencia", "/ViewsScripts/Pallets/RegraAutorizacaoTransferencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoTransferencia", "/ViewsScripts/Pallets/AutorizacaoTransferencia/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/avaria", "/ViewsScripts/Pallets/Avaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoAvaria", "/ViewsScripts/Pallets/RegraAutorizacaoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoAvariaPallet", "/ViewsScripts/Pallets/AutorizacaoAvaria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/compraPallets", "/ViewsScripts/Pallets/CompraPallets/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/reforma", "/ViewsScripts/Pallets/Reforma/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/valePallet", "/ViewsScripts/Pallets/ValePallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/devolucaoValePallet", "/ViewsScripts/Pallets/DevolucaoValePallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoDevolucaoValePallet", "/ViewsScripts/Pallets/RegraAutorizacaoDevolucaoValePallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoDevolucaoValePallet", "/ViewsScripts/Pallets/AutorizacaoDevolucaoValePallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoPallets", "/ViewsScripts/Pallets/FechamentoPallets/**/*.js", "/ViewsScripts/Pallets/FechamentoPallets/Composicao/**/*.js").UseContentRoot();

                #endregion Pallets

                #region Planejamentos

                pipeline.AddJavaScriptBundle("/scripts/planejamentoFrota", "/ViewsScripts/Planejamentos/PlanejamentoFrota/**/*.js").UseContentRoot();

                #endregion Planejamentos

                #region PortalCliente

                pipeline.AddJavaScriptBundle("/scripts/portalCliente", "/ViewsScripts/PortalCliente/**/*.js").UseContentRoot();

                #endregion PortalCliente

                #region ProdutorRural

                pipeline.AddJavaScriptBundle("/scripts/fechamentoColetaProdutor", "/ViewsScripts/ProdutorRural/FechamentoColetaProdutor/**/*.js").UseContentRoot();

                #endregion ProdutorRural

                #region ProdutorRural

                pipeline.AddJavaScriptBundle("/scripts/fechamentoColetaProdutor", "/ViewsScripts/ProdutorRural/FechamentoColetaProdutor/**/*.js").UseContentRoot();

                #endregion ProdutorRural

                #region PreCargas

                pipeline.AddJavaScriptBundle("/scripts/preCarga", "/ViewsScripts/PreCargas/PreCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/preCargaDadosParaTransporte", "/ViewsScripts/PreCargas/DadosParaTransporte/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/preCargaTransportador", "/ViewsScripts/PreCargas/PreCargaTransportador/**/*.js").UseContentRoot();

                #endregion PreCargas

                #region Torre Controle

                pipeline.AddJavaScriptBundle("/scripts/consultaPorEntrega", "/ViewsScripts/TorreControle/ConsultaPorEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaPorNotaFiscal", "/ViewsScripts/TorreControle/ConsultaPorNotaFiscal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/consultaEntregaAtrasada", "/ViewsScripts/TorreControle/ConsultaEntregaAtrasada/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/acompanhamentoCarga", "/ViewsScripts/TorreControle/AcompanhamentoCarga/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/torreControleDetalhesPedido", settings, "/ViewsScripts/TorreControle/DetalhesPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/finalizacaoColetaEntregaEmLote", "/ViewsScripts/TorreControle/FinalizacaoColetaEntregaEmLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitorNotificacoesApp", "/ViewsScripts/TorreControle/MonitorNotificacoesApp/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitorIntegracoesSuperApp", "/ViewsScripts/TorreControle/MonitorIntegracoesSuperApp/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/planejamentoVolume", "/ViewsScripts/TorreControle/PlanejamentoVolume/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tratativaAlerta", "/ViewsScripts/TorreControle/TratativaAlerta/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraQualidadeMonitoramento", "/ViewsScripts/TorreControle/RegraQualidadeMonitoramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/monitorFinalizacaoEntregaAssincrona", "/ViewsScripts/TorreControle/MonitorFinalizacaoEntregaAssincrona/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/qualidadeEntrega", "/ViewsScripts/TorreControle/QualidadeEntrega/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/checklistSuperApp", "/ViewsScripts/TorreControle/ChecklistSuperApp/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/TendenciaParadas", "/ViewsScripts/TorreControle/TendenciaParadas/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/detalhesTorre", "/ViewsScripts/TorreControle/DetalhesTorre/**/*.js").UseContentRoot();

                #endregion Torre Controle

                #region Relatórios

                pipeline.AddJavaScriptBundle("/scripts/ctesCancelados", "/ViewsScripts/Relatorios/CTesCancelados/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ctesEmitidos", "/ViewsScripts/Relatorios/CTesEmitidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/ctesEmitidosEmbarcador", "/ViewsScripts/Relatorios/CTesEmitidosEmbarcador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/mdfesEmitidos", "/ViewsScripts/Relatorios/MDFesEmitidos/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/mdfesNaoEncerrados", "/ViewsScripts/Relatorios/MDFesNaoEncerrados/**/*.js").UseContentRoot();

                #endregion Relatórios

                #region Rateios

                pipeline.AddJavaScriptBundle("/scripts/rateioFormula", "/ViewsScripts/Rateios/RateioFormula/**/*.js").UseContentRoot();

                #endregion Rateios

                #region Registro

                pipeline.AddJavaScriptBundle("/scripts/registro", "/ViewsScripts/Registro/Registro/**/*.js").UseContentRoot();

                #endregion Registro

                #region RH

                pipeline.AddJavaScriptBundle("/scripts/ComissaoFuncionario", "/ViewsScripts/RH/ComissaoFuncionario/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/folhaInformacao", "/ViewsScripts/RH/FolhaInformacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/folhaLancamento", "/ViewsScripts/RH/FolhaLancamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/diarioBordoSemanal", "/ViewsScripts/RH/DiarioBordoSemanal/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaProdutividade", "/ViewsScripts/RH/TabelaProdutividade/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/lancamentoFolha", "/ViewsScripts/RH/LancamentoFolha/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaMediaModeloPeso", "/ViewsScripts/RH/TabelaMediaModeloPeso/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaPremioProdutividade", "/ViewsScripts/RH/TabelaPremioProdutividade/**/*.js").UseContentRoot();

                #endregion RH

                #region SAC

                pipeline.AddJavaScriptBundle("/scripts/atendimentoCliente", "/ViewsScripts/SAC/AtendimentoCliente/**/*.js").UseContentRoot();

                #endregion SAC

                #region Seguros

                pipeline.AddJavaScriptBundle("/scripts/seguradora", "/ViewsScripts/Seguros/Seguradora/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/apoliceSeguro", "/ViewsScripts/Seguros/ApoliceSeguro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/fechamentoAverbacao", "/ViewsScripts/Seguros/FechamentoAverbacao/**/*.js").UseContentRoot();

                #endregion Seguros

                #region Simulações

                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoSimulacao", "/ViewsScripts/Simulacoes/RegrasAutorizacaoSimulacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoBonificacao", "/ViewsScripts/Simulacoes/GrupoBonificacao/**/*.js").UseContentRoot();

                #endregion Simulações

                #region Painel

                pipeline.AddJavaScriptBundle("/scripts/indicador", "/ViewsScripts/Painel/Indicador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/carregamento", "/ViewsScripts/Painel/Carregamento/**/*.js").UseContentRoot();

                #endregion Painel

                #region Transportadores

                pipeline.AddJavaScriptBundle("/scripts/alteracaoMassa", "/ViewsScripts/Transportadores/AlteracaoMassa/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contaTransportador", "/ViewsScripts/Transportadores/ContaTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/dadosTransportador", "/ViewsScripts/Transportadores/DadosTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gerenciartransportadores", "/ViewsScripts/Transportadores/GerenciarTransportadores/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/grupoTransportador", "/ViewsScripts/Transportadores/GrupoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoMotorista", "/ViewsScripts/Transportadores/ImportacaoMotorista/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/importacaoVeiculo", "/ViewsScripts/Transportadores/ImportacaoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motorista", "/ViewsScripts/Transportadores/Motorista/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motoristaCampoObrigatorio", "/ViewsScripts/Transportadores/MotoristaCampoObrigatorio/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/motoristaLGPD", "/ViewsScripts/Transportadores/MotoristaLGPD/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/perfilAcessoTransportador", "/ViewsScripts/Transportadores/PerfilAcessoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/transportador", "/ViewsScripts/Transportadores/Transportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoTransportador", "/ViewsScripts/Transportadores/GestaoTransportador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tokenTransportador", "/ViewsScripts/Transportadores/AutorizacaoToken/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/transportadorCertificado", "/ViewsScripts/Transportadores/TransportadorCertificado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/transportadorIntegracao", "/ViewsScripts/Transportadores/TransportadorIntegracao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/solicitacaoToken", "/ViewsScripts/Transportadores/SolicitacaoToken/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAprovacaoToken", "/ViewsScripts/Transportadores/RegraAutorizacaoToken/**/*.js").UseContentRoot();

                #endregion Transportadores

                #region Terceiros

                pipeline.AddJavaScriptBundle("/scripts/contratoFrete", "/ViewsScripts/Terceiros/ContratoFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraContratoFreteTerceiro", "/ViewsScripts/Terceiros/RegraContratoFreteTerceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoContratoFreteTerceiro", "/ViewsScripts/Terceiros/AutorizacaoContratoFreteTerceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/imposto", "/ViewsScripts/Terceiros/Imposto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoFreteValorPadrao", "/ViewsScripts/Terceiros/ContratoFreteValorPadrao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoFreteAcrescimoDesconto", "/ViewsScripts/Terceiros/ContratoFreteAcrescimoDesconto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoContratoFreteAcrescimoDesconto", "/ViewsScripts/Terceiros/RegraAutorizacaoContratoFreteAcrescimoDesconto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoContratoFreteAcrescimoDesconto", "/ViewsScripts/Terceiros/AutorizacaoContratoFreteAcrescimoDesconto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/taxaTerceiro", "/ViewsScripts/Terceiros/TaxaTerceiro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/contratoFreteAcrescimoDescontoAutomatico", "/ViewsScripts/Terceiros/ContratoFreteAcrescimoDescontoAutomatico/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoPagamentoContratoFrete", "/ViewsScripts/Terceiros/AutorizacaoPagamentoContratoFrete/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/FechamentoAgregado", "/ViewsScripts/Terceiros/FechamentoAgregado/**/*.js").UseContentRoot();

                #endregion Terceiros

                #region Veiculos

                pipeline.AddJavaScriptBundle("/scripts/marcaVeiculo", "/ViewsScripts/Veiculos/Marca/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/marcaEquipamento", "/ViewsScripts/Veiculos/MarcaEquipamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/equipamento", "/ViewsScripts/Veiculos/Equipamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/veiculo", "/ViewsScripts/Veiculos/Veiculo/**/*.js", "/ViewsScripts/Veiculos/VeiculoLicenca/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modeloCarroceria", "/ViewsScripts/Veiculos/ModeloCarroceria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/segmentoVeiculo", "/ViewsScripts/Veiculos/SegmentoVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/macro", "/ViewsScripts/Veiculos/Macro/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/painelVeiculo", "/ViewsScripts/Veiculos/PainelVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tecnologiaRastreador", "/ViewsScripts/Veiculos/TecnologiaRastreador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoComunicacaoRastreador", "/ViewsScripts/Veiculos/TipoComunicacaoRastreador/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/responsavelVeiculo", "/ViewsScripts/Veiculos/ResponsavelVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/veiculoMonitoramento", "/ViewsScripts/Veiculos/VeiculoMonitoramento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/modeloEquipamento", "/ViewsScripts/Veiculos/ModeloEquipamento/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoPlotagem", "/ViewsScripts/Veiculos/TipoPlotagem/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/veiculoLicenca", "/ViewsScripts/Veiculos/VeiculoLicenca/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraAutorizacaoCadastroVeiculo", "/ViewsScripts/Veiculos/RegraAutorizacaoCadastroVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoCadastroVeiculo", "/ViewsScripts/Veiculos/AutorizacaoCadastroVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/corVeiculo", "/ViewsScripts/Veiculos/CorVeiculo/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/manutencaoCentroResultado", "/ViewsScripts/Veiculos/ManutencaoCentroResultado/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tabelaMediaPorSegmento", "/ViewsScripts/Veiculos/TabelaMediaPorSegmento/**/*.js").UseContentRoot();


                #endregion Veiculos

                #region WMS

                pipeline.AddJavaScriptBundle("/scripts/wmsdeposito", "/ViewsScripts/WMS/Deposito/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/transferenciaMercadoria", "/ViewsScripts/WMS/TransferenciaMercadoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/recebimentoMercadoria", "/ViewsScripts/WMS/RecebimentoMercadoria/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regraDescarteLoteProduto", "/ViewsScripts/WMS/RegraDescarteLoteProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/descarteLoteProduto", "/ViewsScripts/WMS/DescarteLoteProduto/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/separacaoWMS", "/ViewsScripts/WMS/SeparacaoWMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/selecaoWMS", "/ViewsScripts/WMS/SelecaoWMS/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoDescarteLote", "/ViewsScripts/WMS/AutorizacaoDescarteLote/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/wmsseparacaoMercadorias", "/ViewsScripts/WMS/SeparacaoMercadorias/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/separacaoPedido", "/ViewsScripts/WMS/SeparacaoPedido/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/montagemContainer", "/ViewsScripts/WMS/MontagemContainer/**/*.js").UseContentRoot();

                #endregion WMS

                #region Bidding

                pipeline.AddJavaScriptBundle("/scripts/biddingConvite", "/ViewsScripts/Bidding/BiddingConvite/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/biddingAceitacao", "/ViewsScripts/Bidding/BiddingAceitacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/biddingAvaliacao", "/ViewsScripts/Bidding/BiddingAvaliacao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoBidding", "/ViewsScripts/Bidding/TipoBidding/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/regrasAutorizacaoBidding", "/ViewsScripts/Bidding/RegrasAutorizacaoBidding/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/autorizacaoBidding", "/ViewsScripts/Bidding/AutorizacaoBidding/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/tipoBaseline", "/ViewsScripts/Bidding/TipoBaseline/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/biddingChecklistQuestionarioPadrao", "/ViewsScripts/Bidding/BiddingChecklistQuestionarioPadrao/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/RFIConvite", "/ViewsScripts/Bidding/RFIConvite/**/*.js").UseContentRoot();

                #endregion Bidding

                #region Gestão de Pallets

                pipeline.AddJavaScriptBundle("/scripts/gestaoPallet", "/ViewsScripts/GestaoPallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/agendamentoColetaPallet", "/ViewsScripts/GestaoPallet/agendamentoColetaPallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoPallet/controleSaldoPallet", "/ViewsScripts/GestaoPallet/ControleSaldoPallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoPallet/agendamentoPallet", "/ViewsScripts/GestaoPallet/AgendamentoPallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoPallet/controlePallet", "/ViewsScripts/GestaoPallet/ControlePallet/**/*.js").UseContentRoot();
                pipeline.AddJavaScriptBundle("/scripts/gestaoPallet/manutencaoPallet", "/ViewsScripts/GestaoPallet/ManutencaoPallet/**/*.js").UseContentRoot();

                #endregion Gestão de Pallets

                #endregion

                #region Styles

                pipeline.AddCssBundle("/css/customTheme1", "/css/themes/cust-theme-1.css");
                pipeline.AddCssBundle("/css/customTheme2", "/css/themes/cust-theme-2.css");
                pipeline.AddCssBundle("/css/customTheme3", "/css/themes/cust-theme-3.css");
                pipeline.AddCssBundle("/css/customTheme4", "/css/themes/cust-theme-4.css");
                pipeline.AddCssBundle("/css/customTheme5", "/css/themes/cust-theme-5.css");
                pipeline.AddCssBundle("/css/customTheme6", "/css/themes/cust-theme-6.css");
                pipeline.AddCssBundle("/css/customTheme7", "/css/themes/cust-theme-7.css");
                pipeline.AddCssBundle("/css/customTheme8", "/css/themes/cust-theme-8.css");
                pipeline.AddCssBundle("/css/customTheme9", "/css/themes/cust-theme-9.css");
                pipeline.AddCssBundle("/css/customTheme10", "/css/themes/cust-theme-10.css");
                pipeline.AddCssBundle("/css/customTheme11", "/css/themes/cust-theme-11.css");
                pipeline.AddCssBundle("/css/customTheme12", "/css/themes/cust-theme-12.css");
                pipeline.AddCssBundle("/css/customTheme13", "/css/themes/cust-theme-13.css");
                pipeline.AddCssBundle("/css/customTheme14", "/css/themes/cust-theme-14.css");
                pipeline.AddCssBundle("/css/customTheme15", "/css/themes/cust-theme-15.css");
                pipeline.AddCssBundle("/css/customThemePotalCliente", "/css/themes/cust-theme-portal-cliente.css");

                pipeline.AddCssBundle("/css/login", "/css-new/layoutLogin.css");

                pipeline.AddCssBundle("/css/summernote", "/css-new/formplugins/summernote/summernote.css");
                pipeline.AddCssBundle("/css/legacy", "/css-new/legacy/colors.css");
                pipeline.AddCssBundle("/css/appBundle", "/css-new/vendors.bundle.css", "/css-new/app.bundle.css");
                pipeline.AddCssBundle("/css/skinBundle", "/css-new/skins/skin-master.css", "/css-new/theme-fixes.css", "/css-new/custom-style.css");

                pipeline.AddCssBundle("/css/pluginBundle", "/css-new/miscellaneous/datatables/dataTables.bootstrap5.css",
                    "/css-new/notifications/toastr/toastr.css", "/css-new/notifications/sweetalert2/sweetalert2.bundle.css",
                    "/css-new/formplugins/select2/select2.bundle.css", "/css-new/formplugins/dropzone/dropzone.css",
                    "/css-new/miscellaneous/treeview/treeview.css", "/css-new/miscellaneous/tempus-dominus/tempus-dominus.css",
                    "/css/fullcalendar.css", "/css/bootstrap-colorselector.css", "/css-new/miscellaneous/leaflet/leaflet.css",
                    "/css-new/miscellaneous/leaflet/maker-cluster.css", "/css-new/miscellaneous/leaflet/maker-cluster-default.css",
                    "/css-new/formplugins/bootstrap-select/bootstrap-select.css");
                pipeline.AddCssBundle("/css/bootstrap-rastreio-entrega", "/css/bootstrap-rastreio-entrega.css");
                pipeline.AddCssBundle("/css/bootstrapSelect", "/css-new/formplugins/bootstrap-select/bootstrap-select.css");
                pipeline.AddCssBundle("/css/bootstrapToggle", "/css/bootstrap-toggle.min.css");
                pipeline.AddCssBundle("/css/dataTablesCheckboxes", "/css/dataTables.checkboxes.css");
                pipeline.AddCssBundle("/css/fontAwesome", "/css/font-awesome.css");
                pipeline.AddCssBundle("/css/googleMaps", "/css/google_maps.css");
                pipeline.AddCssBundle("/css/conferenciacanhotos", "/css/conferenciacanhotos.css");
                pipeline.AddCssBundle("/css/fluxoEntrega", "/css/fluxoEntrega.css");
                pipeline.AddCssBundle("/css/controleEntrega", "/css/controleEntrega.css");
                pipeline.AddCssBundle("/css/controleEntregaPrevisao", "/css/controleEntregaPrevisao.css");
                pipeline.AddCssBundle("/css/tracking", "/css/tracking.css");
                pipeline.AddCssBundle("/css/cargaTrajeto", "/css/cargaTrajeto.css");
                pipeline.AddCssBundle("/css/retiradaProduto", "/css/retiradaProduto.css");
                pipeline.AddCssBundle("/css/portalClientePublic", "/content/portal-cliente/public/bootstrap.min.css", "/content/portal-cliente/public/common.css", "/content/portal-cliente/public/login.css");
                pipeline.AddCssBundle("/css/portalCliente", "/content/portal-cliente/main.css", "/css/rating.css");
                pipeline.AddCssBundle("/css/novoRastreioEntrega", "/css/novoRastreioEntrega.css");
                pipeline.AddCssBundle("/css/posicaoFrota", "/css/posicaoFrota.css");
                pipeline.AddCssBundle("/css/gestaoDevolucao", "/css/gestaoDevolucao.css");
                pipeline.AddCssBundle("/css/cabecalhoPadraoTorreControle", "/css/cabecalhoPadraoTorreControle.css");
                pipeline.AddCssBundle("/css/veiculomonitoramento", "/css/veiculomonitoramento.css");
                pipeline.AddCssBundle("/css/signaturepad", "/css/signaturepad.css");
                pipeline.AddCssBundle("/css/acompanhamentoCarga", "/content/TorreControle/AcompanhamentoCarga/css/deliveryPlatform.css",
                    "/content/TorreControle/AcompanhamentoCarga/css/grid.css", "/content/TorreControle/AcompanhamentoCarga/css/reset.css",
                    "/content/TorreControle/AcompanhamentoCarga/css/setup.css");
                #endregion Styles
            });
        }

    }
}