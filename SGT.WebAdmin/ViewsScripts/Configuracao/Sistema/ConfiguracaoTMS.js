/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />

var EnumTipoServicoMultisoftware = {
    MultiEmbarcador: 1,
    MultiTMS: 2,
    MultiCTe: 3,
    CallCenter: 4,
    Terceiros: 5,
    MultiNFe: 6,
    MultiNFeAdmin: 7,
    Fornecedor: 8,
    MultiMobile: 9,
    MultiBus: 10,
    MultiBusTransportador: 11,
    TransportadorTerceiro: 12
};

//var _CONFIGURACAO_TMS = {
//    UsuarioAdministrador: false,
//    FiltrarBuscaVeiculosPorEmpresa: true,
//    PermitirConfirmacaoImpressaoME: false,
//    SituacaoCargaAposConfirmacaoImpressao: 9,
//    DescricaoProdutoPredominatePadrao: "",
//    PermitirOperadorInformarValorFreteMaiorQueTabela: false,
//    PreencherMotoristaAutomaticamenteAoInformarVeiculo: false,
//    BuscaProdutoPredominanteNoPedido: false,
//    CadastrarMotoristaMobileAutomaticamente: true,
//    PermitirRetornoAgNotasFiscais: false,
//    PermitirCancelamentoTotalCarga: false,
//    PermiteAdicionarNotaManualmente: false,
//    PermitirTransportadorAlterarModeloVeicular: false,
//    ProvisionarDocumentosEmitidos: false,
//    TipoChamado: EnumTipoChamado.PadraoTransportador,
//    ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos: false,
//    TipoServicoMultisoftware: 0,
//    SempreDuplicarCargaCancelada: false,
//    ObrigatorioRegrasOcorrencia: false,
//    NaoPermiteEmitirCargaSemAverbacao: false,
//    ExigeNumeroDeAprovadoresNasAlcadas: false,
//    ExigirChamadoParaAbrirOcorrencia: false,
//    CalcularFreteFilialEmissoraPorTabelaDeFrete: false,
//    PermiteAuditar: false,
//    ObrigatorioInformarDadosContratoFrete: false,
//    UsuarioMultisoftware: false,
//    CargaTransbordoNaEtapaInicial: false,
//    TipoMontagemCargaPadrao: EnumTipoMontagemCarga.Todos,
//    ExigirCodigoIntegracaoTransportador: false,
//    UtilizaChat: false,
//    ExigirRotaRoteirizadaNaCarga: false,
//    NaoExigeAceiteTransportadorParaNFDebito: false,
//    UtilizarRegraICMSParaDescontarValorICMS: false,
//    NaoExigeInformarDisponibilidadeDeVeiculo: false,
//    DefaultTrueDuplicarCarga: false,
//    ValidarSomenteFreteLiquidoNaImportacaoCTe: false,
//    ValidarPorRaizDoTransportadorNaImportacaoCTe: false,
//    ExigirAceiteTermoUsoSistema: false,
//    PermitirDownloadDANFE: false,
//    PadraoTagValePedagioVeiculos: false,
//    TipoIntegracaoValePedagio: EnumTipoIntegracao.NaoInformada,
//    NaoExigirExpedidorNoRedespacho: false,
//    GerarPagamentoBloqueado: false,
//    PermiteInformarChamadosNoLancamentoOcorrencia: false,
//    ExibirAliquotaEtapaFreteCarga: false,
//    UsarGrupoDeTipoDeOperacaoNoMonitoramento: false,
//    UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem: false,
//    GerarOcorrenciaParaCargaAgrupada: false,
//    TelaMonitoramentoPadraoFiltroDataInicialFinal: false,
//    CalcularFreteCliente: false,
//    VisualizarValorNFSeDescontandoISSRetido: false,
//    NaoValidarDadosParticipantesNaImportacaoCTe: false,
//    DocumentoImpressaoPadraoCarga: false,
//    PermitirGerarNotaMesmoPedidoCarga: false,
//    PermitirGerarNotaMesmaCarga: false,
//    PossuiMonitoramento: false,
//    HabilitarDescontoGestaoDocumento: false,
//    ExigirMotivoOcorrencia: false,
//    PermiteImportarPlanilhaValoresFreteNFSManual: false,
//    NaoValidarDadosParticipantesNaImportacaoCTe: false,
//    NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga: false,
//    TelaCargaApresentarUltimaPosicaoVeiculo: false,
//    CentroResultadoPedidoObrigatorio: false,
//    EmitirNFSManualParaTransportadorEFiliais: false,
//    OrdenarCargasMobileCrescente: false,
//    PermitirCriacaoDiretaMalotes: false,
//    PermitirAgrupamentoDeCargasOrdenavel: false,
//    SistemaEstrangeiro: false
//};

function setarConfiguracaoPadrao(bagConfiguracaoPadrao) {

    if (bagConfiguracaoPadrao)
        _CONFIGURACAO_TMS = bagConfiguracaoPadrao;

    if (ObterCulturaConfigurada() != "pt-BR") {
        Globalize.cultures["default"] = Globalize.cultures["en"];
        Globalize.cultureSelector = "en";
    }
}

function ObterCulturaConfigurada() {
    if (Boolean(_CONFIGURACAO_TMS) && Boolean(_CONFIGURACAO_TMS.Culture))
        return _CONFIGURACAO_TMS.Culture;

    return "pt-BR";
}

function ObterListaRecursivaMenu(ul, array, path) {
    ul.children().each(
        function () {
            let pathRaiz = "";

            if (path != null)
                pathRaiz = path;

            $(this).children().each(
                function () {
                    if ($(this).is("a")) {
                        if ($(this).attr("href") != "#")
                            array.push({ label: pathRaiz + $(this).text(), value: $(this).attr("href") });
                        else
                            pathRaiz += $(this).text() + " / ";
                    }
                    else {
                        if ($(this).is("ul"))
                            ObterListaRecursivaMenu($(this), array, pathRaiz);
                    }
                }
            );
        }
    );
}
