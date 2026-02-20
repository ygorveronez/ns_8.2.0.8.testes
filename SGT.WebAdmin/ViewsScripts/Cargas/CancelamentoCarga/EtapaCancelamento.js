/// <reference path="MDFe.js" />
/// <reference path="CTe.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoDocumentoCarga.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoCancelamentoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCancelamento;

var EtapaCancelamento = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(verificarTamanhoEtapa) });

    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Cancelamento, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(Localization.Resources.Cargas.CancelamentoCarga.AprovaReprovaCancelamento),
        tooltipTitle: ko.observable("Cancelamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "Integração Dados Cancelamento", type: types.local, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.RealizarIntegracaoDadosCancelamentoCarga), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable("Integração dos dados de cancelamento de carga."),
        tooltipTitle: ko.observable("Integração Documentos")
    });

    this.Etapa3 = PropertyEntity({
        text: "MDF-e", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(Localization.Resources.Cargas.CancelamentoCarga.CancelamentoMDFCarga),
        tooltipTitle: ko.observable("MDF-e")
    });
    this.Etapa4 = PropertyEntity({
        text: "CT-e", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(Localization.Resources.Cargas.CancelamentoCarga.CancelamentoCTesCarga),
        tooltipTitle: ko.observable("CT-e")
    });
    this.Etapa5 = PropertyEntity({
        text: "CIOT", type: types.local, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable("Integração dos dados de cancelamento de CIOT."),
        tooltipTitle: ko.observable("CIOT")
    });
    this.Etapa6 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Integracao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(Localization.Resources.Cargas.CancelamentoCarga.EtapaIntegracaoSistemas),
        tooltipTitle: ko.observable("Integração")
    });
}

//*******EVENTOS*******

function LoadEtapaCancelamento() {
    _etapaCancelamento = new EtapaCancelamento();
    KoBindings(_etapaCancelamento, "knockoutEtapaCancelamento");
    Etapa1Liberada();
}

function SetarEtapaInicioCancelamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCancelamento.Etapa1.idTab)[0].click();    
}

function SetarEtapaCancelamento() {
    var situacaoCancelamento = _cancelamento.Situacao.val();
    var situacaoMDFes = _cancelamento.SituacaoMDFes.val();
    var situacaoCTes = _cancelamento.SituacaoCTes.val();
    var situacaoIntegracoes = _cancelamento.SituacaoIntegracoes.val();
    var tipoCancelamento = _cancelamento.Tipo.val();
    var situacaoCIOT = _cancelamento.SituacaoCIOT.val();

    if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgAprovacaoSolicitacao)
        Etapa1Aguardando();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgConfirmacao)
        Etapa1Liberada();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.Cancelada || situacaoCancelamento == EnumSituacaoCancelamentoCarga.Anulada)
        Etapa6Aprovada();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.Reprovada)
        Etapa1Aprovada();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.EmCancelamento)
        Etapa1Liberada();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.SolicitacaoReprovada)
        Etapa1Reprovada();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgIntegracaoDadosCancelamento)
        Etapa2Aguardando();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT)
        Etapa5Aguardando();
    else if (
        situacaoCancelamento == EnumSituacaoCancelamentoCarga.RejeicaoCancelamento ||
        situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgCancelamentoAverbacaoCTe ||
        situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgCancelamentoAverbacaoMDFe ||
        situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgCancelamentoCTe ||
        situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgCancelamentoMDFe ||
        situacaoCancelamento == EnumSituacaoCancelamentoCarga.AgIntegracao ||
        situacaoCancelamento == EnumSituacaoCancelamentoCarga.FinalizandoCancelamento
    ) {
        if (_CONFIGURACAO_TMS.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga && situacaoCIOT == EnumSituacaoCancelamentoDocumentoCarga.Cancelando)
            Etapa5Aguardando();
        else if (_CONFIGURACAO_TMS.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga && situacaoCIOT == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao)
            Etapa5Reprovada();
        else {
            if (situacaoMDFes == EnumSituacaoCancelamentoDocumentoCarga.Cancelando)
                Etapa3Aguardando();
            else if (situacaoMDFes == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao)
                Etapa3Reprovada();
            else if (situacaoMDFes == EnumSituacaoCancelamentoDocumentoCarga.Sucesso) {

                if (tipoCancelamento == EnumTipoCancelamentoCarga.Anulacao && situacaoIntegracoes == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao) {
                    Etapa6Reprovada();
                    return;
                }

                if (situacaoCTes == EnumSituacaoCancelamentoDocumentoCarga.Cancelando)
                    Etapa4Aguardando();
                else if (situacaoCTes == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao)
                    Etapa4Reprovada();
                else if (situacaoCTes == EnumSituacaoCancelamentoDocumentoCarga.Sucesso) {
                    if (situacaoIntegracoes == EnumSituacaoCancelamentoDocumentoCarga.Cancelando)
                        Etapa6Aguardando();
                    else if (situacaoIntegracoes == EnumSituacaoCancelamentoDocumentoCarga.Rejeicao)
                        Etapa6Reprovada();
                    else if (situacaoIntegracoes == EnumSituacaoCancelamentoDocumentoCarga.Sucesso)
                        Etapa6Aprovada();
                }
            }
        }
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa4Desabilitada();
    Etapa5Desabilitada();
    Etapa6Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovada() {
    $("#" + _etapaCancelamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa1.idTab + " .step").attr("class", "step red");
    Etapa2Desabilitada();
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaCancelamento.Etapa2.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Liberada() {
    _etapaCancelamento.Etapa2.eventClick = BuscarIntegracoesDadosCancelamento;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaCancelamento.Etapa2.eventClick = BuscarIntegracoesDadosCancelamento;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaCancelamento.Etapa2.eventClick = BuscarIntegracoesDadosCancelamento;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaCancelamento.Etapa2.eventClick = BuscarIntegracoesDadosCancelamento;
    $("#" + _etapaCancelamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaCancelamento.Etapa3.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step");
    Etapa4Desabilitada()
}

function Etapa3Liberada() {
    _etapaCancelamento.Etapa3.eventClick = ConsultarCIOTCarga;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaCancelamento.Etapa3.eventClick = ConsultarCIOTCarga;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaCancelamento.Etapa3.eventClick = ConsultarCIOTCarga;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaCancelamento.Etapa3.eventClick = ConsultarCIOTCarga;
    $("#" + _etapaCancelamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}

//*******Etapa 4*******

function Etapa4Desabilitada() {
    _etapaCancelamento.Etapa4.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa4.idTab + " .step").attr("class", "step");
    Etapa5Desabilitada()
}

function Etapa4Liberada() {
    _etapaCancelamento.Etapa4.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3Aprovada();
}

function Etapa4Aguardando() {
    _etapaCancelamento.Etapa4.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3Aprovada();
}

function Etapa4Aprovada() {
    _etapaCancelamento.Etapa4.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa4.idTab + " .step").attr("class", "step green");
    Etapa3Aprovada();
}

function Etapa4Reprovada() {
    _etapaCancelamento.Etapa4.eventClick = ConsultarMDFesCarga;
    $("#" + _etapaCancelamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3Aprovada();
}


//*******Etapa 5*******

function Etapa5Desabilitada() {
    _etapaCancelamento.Etapa5.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa5.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa5.idTab + " .step").attr("class", "step");
    Etapa6Desabilitada();
}

function Etapa5Liberada() {
    _etapaCancelamento.Etapa5.eventClick = ConsultarCTesCarga;
    $("#" + _etapaCancelamento.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4Aprovada();
}

function Etapa5Aguardando() {
    _etapaCancelamento.Etapa5.eventClick = ConsultarCTesCarga;
    $("#" + _etapaCancelamento.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa5.idTab + " .step").attr("class", "step yellow");
    Etapa4Aprovada();
}

function Etapa5Aprovada() {
    _etapaCancelamento.Etapa5.eventClick = ConsultarCTesCarga;
    $("#" + _etapaCancelamento.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa5.idTab + " .step").attr("class", "step green");
    Etapa4Aprovada();
}

function Etapa5Reprovada() {
    _etapaCancelamento.Etapa5.eventClick = ConsultarCTesCarga;
    $("#" + _etapaCancelamento.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa5.idTab + " .step").attr("class", "step red");
    Etapa4Aprovada();
}


//*******Etapa 6*******

function Etapa6Desabilitada() {
    _etapaCancelamento.Etapa6.eventClick = function () { };
    $("#" + _etapaCancelamento.Etapa6.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamento.Etapa6.idTab + " .step").attr("class", "step");
}

function Etapa6Liberada() {
    _etapaCancelamento.Etapa6.eventClick = BuscarDadosIntegracoesCargaCancelamento;
    $("#" + _etapaCancelamento.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa6.idTab + " .step").attr("class", "step yellow");
    Etapa5Aprovada();
}

function Etapa6Aguardando() {
    _etapaCancelamento.Etapa6.eventClick = BuscarDadosIntegracoesCargaCancelamento;
    $("#" + _etapaCancelamento.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa6.idTab + " .step").attr("class", "step yellow");
    Etapa5Aprovada();
}

function Etapa6Aprovada() {
    _etapaCancelamento.Etapa6.eventClick = BuscarDadosIntegracoesCargaCancelamento;
    $("#" + _etapaCancelamento.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa6.idTab + " .step").attr("class", "step green");
    Etapa5Aprovada();
}

function Etapa6Reprovada() {
    _etapaCancelamento.Etapa6.eventClick = BuscarDadosIntegracoesCargaCancelamento;
    $("#" + _etapaCancelamento.Etapa6.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamento.Etapa6.idTab + " .step").attr("class", "step red");
    Etapa5Aprovada();
}

// ********* Funções ********* //

function verificarTamanhoEtapa() {
    if (_CONFIGURACAO_TMS.RealizarIntegracaoDadosCancelamentoCarga && _CONFIGURACAO_TMS.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
        return "16%";
    else if (_CONFIGURACAO_TMS.RealizarIntegracaoDadosCancelamentoCarga || _CONFIGURACAO_TMS.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
        return "20%";
    else
        return "25%";
}