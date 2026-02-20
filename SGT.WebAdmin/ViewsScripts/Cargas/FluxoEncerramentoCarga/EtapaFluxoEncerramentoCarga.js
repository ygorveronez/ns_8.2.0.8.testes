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
/// <reference path="FluxoEncerramentoCargaCIOT.js" />
/// <reference path="FluxoEncerramentoCargaMDFe.js" />
/// <reference path="FluxoEncerramentoCargaIntegracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEncerramentoCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEncerramentoDocumentosCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFluxoEncerramentoCarga;

var EtapaFluxoEncerramentoCarga = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Detalhes", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Detalhes do encerramento da carga"),
        tooltipTitle: ko.observable("Detalhes")
    });
    this.Etapa2 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Encerramento dos documentos da carga"),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Etapa de integrações do encerramento da carga"),
        tooltipTitle: ko.observable("Integração")
    });
}

//*******EVENTOS*******

function LoadEtapaFluxoEncerramentoCarga() {
    _etapaFluxoEncerramentoCarga = new EtapaFluxoEncerramentoCarga();
    KoBindings(_etapaFluxoEncerramentoCarga, "knockoutEtapaFluxoEncerramentoCarga");
    Etapa1Liberada();
}

function SetarEtapaInicioFluxoEncerramentoCarga() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab).click();
}

function SetarEtapaFluxoEncerramentoCarga() {
    var situacaoEncerramento = _fluxoEncerramentoCarga.Situacao.val();
    var situacaoCIOT = _fluxoEncerramentoCarga.SituacaoCIOT.val();
    var situacaoMDFes = _fluxoEncerramentoCarga.SituacaoMDFes.val();
    var situacaoIntegracoes = _fluxoEncerramentoCarga.SituacaoIntegracoes.val();

    if (situacaoEncerramento == EnumSituacaoEncerramentoCarga.EmEncerramento)
        Etapa1Aguardando();
    else if (situacaoEncerramento == EnumSituacaoEncerramentoCarga.AgEncerramentoDocumentos)
        Etapa2Aguardando();
    else if (situacaoEncerramento == EnumSituacaoEncerramentoCarga.Encerrada)
        Etapa3Aprovada();
    else if (situacaoEncerramento == EnumSituacaoEncerramentoCarga.RejeicaoEncerramento ||
        situacaoEncerramento == EnumSituacaoEncerramentoCarga.AgEncerramentoCIOT ||
        situacaoEncerramento == EnumSituacaoEncerramentoCarga.AgEncerramentoMDFe ||
        situacaoEncerramento == EnumSituacaoEncerramentoCarga.AgIntegracao) {
        if (situacaoMDFes == EnumSituacaoEncerramentoDocumentosCarga.Encerrando)
            Etapa2Aguardando();
        else if (situacaoMDFes == EnumSituacaoEncerramentoDocumentosCarga.Rejeicao)
            Etapa2Reprovada();
        else if (situacaoMDFes == EnumSituacaoEncerramentoDocumentosCarga.Sucesso) {
            if (situacaoCIOT == EnumSituacaoEncerramentoDocumentosCarga.Encerrando)
                Etapa2Aguardando();
            else if (situacaoCIOT == EnumSituacaoEncerramentoDocumentosCarga.Rejeicao)
                Etapa2Reprovada();
            else if (situacaoCIOT == EnumSituacaoEncerramentoDocumentosCarga.Sucesso) {
                if (situacaoIntegracoes == EnumSituacaoEncerramentoDocumentosCarga.Encerrando)
                    Etapa3Aguardando();
                else if (situacaoIntegracoes == EnumSituacaoEncerramentoDocumentosCarga.Rejeicao)
                    Etapa3Reprovada();
                else if (situacaoIntegracoes == EnumSituacaoEncerramentoDocumentosCarga.Sucesso)
                    Etapa3Aprovada();
            }
        }
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovada() {
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa1.idTab + " .step").attr("class", "step red");
    Etapa2Desabilitada();
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaFluxoEncerramentoCarga.Etapa2.eventClick = function () { };
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Liberada() {
    _etapaFluxoEncerramentoCarga.Etapa2.eventClick = BuscarDocumentosFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaFluxoEncerramentoCarga.Etapa2.eventClick = BuscarDocumentosFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaFluxoEncerramentoCarga.Etapa2.eventClick = BuscarDocumentosFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaFluxoEncerramentoCarga.Etapa2.eventClick = BuscarDocumentosFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaFluxoEncerramentoCarga.Etapa3.eventClick = function () { };
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Liberada() {
    _etapaFluxoEncerramentoCarga.Etapa3.eventClick = BuscarIntegracoesFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaFluxoEncerramentoCarga.Etapa3.eventClick = BuscarIntegracoesFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaFluxoEncerramentoCarga.Etapa3.eventClick = BuscarIntegracoesFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaFluxoEncerramentoCarga.Etapa3.eventClick = BuscarIntegracoesFluxoEncerramentoCarga;
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoEncerramentoCarga.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}
