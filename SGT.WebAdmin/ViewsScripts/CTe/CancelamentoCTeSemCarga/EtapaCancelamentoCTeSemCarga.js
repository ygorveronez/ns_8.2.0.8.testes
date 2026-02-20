/// <reference path="CTe.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoCTeSemCarga.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCancelamentoCTe;

var EtapaCancelamentoCTeSemCarga = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(verificarTamanhoEtapa) });

    this.Etapa1 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Cancelamento, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable("Cancelamento"),
        tooltipTitle: ko.observable("Cancelamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "CT-e", type: types.local, enable: ko.observable(false), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable("CT-e"),
        tooltipTitle: ko.observable("CT-e")
    });
    this.Etapa3 = PropertyEntity({
        text: Localization.Resources.Gerais.Geral.Integracao, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable("Integração"),
        tooltipTitle: ko.observable("Integração")
    });
}

//*******EVENTOS*******

function LoadEtapaCancelamentoCTeSemCarga() {
    _etapaCancelamentoCTe = new EtapaCancelamentoCTeSemCarga();
    KoBindings(_etapaCancelamentoCTe, "knockoutEtapaCancelamentoCTeSemCarga");
    Etapa1CTeSemCargaAguardando();
}

function SetarEtapaInicioCancelamentoSemCarga() {
    DesabilitarTodasEtapas();
    Etapa1CTeSemCargLiberada();
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab)[0].click();
}

function SetarEtapaCancelamentoCTeSemCarga() {
    var situacaoCancelamento = _cancelamentoCTe.Situacao.val();
    var situacaoCTe = "";
    if (situacaoCancelamento == null)
        Etapa1CTeSemCargaAguardando();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCTeSemCarga.AgCancelamentoCTe)
        Etapa1CTeSemCargaLiberada();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCTeSemCarga.AgCancelamentoCTe)
        Etapa2CTeSemCargaAguardando();
    //else if (situacaoCancelamento == EnumSituacaoCancelamentoCTeSemCarga.AgCancelamentoIntegracao)
    //    Etapa3CTeSemCargaAguardando();
    else if (situacaoCancelamento == EnumSituacaoCancelamentoCTeSemCarga.Cancelado)
        Etapa2CTeSemCargaAprovada();
    else if (
        situacaoCancelamento == EnumSituacaoCancelamentoCTeSemCarga.RejeicaoCancelamento
    ) {
       Etapa2CTeSemCargaReprovada();
    }
}

function DesabilitarTodasEtapas() {
    Etapa2CTeSemCargaDesabilitada();
    Etapa3CTeSemCargaDesabilitada();
}

//*******Etapa 1*******

function Etapa1CTeSemCargaLiberada() {
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2CTeSemCargaAguardando();
}

function Etapa1CTeSemCargaAguardando() {
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2CTeSemCargaDesabilitada();
}

function Etapa1CTeSemCargaAprovada() {
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1CTeSemCargaReprovada() {
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa1.idTab + " .step").attr("class", "step red");
    Etapa2CTeSemCargaDesabilitada();
}

//*******Etapa 2*******

function Etapa2CTeSemCargaDesabilitada() {
    _etapaCancelamentoCTe.Etapa2.eventClick = function () { };
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3CTeSemCargaDesabilitada();
}

function Etapa2CTeSemCargaLiberada() {
    _etapaCancelamentoCTe.Etapa2.eventClick = BuscarCTesCancelamento;
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1CTeSemCargaAprovada();
}

function Etapa2CTeSemCargaAguardando() {
    _etapaCancelamentoCTe.Etapa2.eventClick = BuscarCTesCancelamento;
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1CTeSemCargaAprovada();
}

function Etapa2CTeSemCargaAprovada() {
    _etapaCancelamentoCTe.Etapa2.eventClick = BuscarCTesCancelamento;
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1CTeSemCargaAprovada();
}

function Etapa2CTeSemCargaReprovada() {
    _etapaCancelamentoCTe.Etapa2.eventClick = BuscarCTesCancelamento;
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1CTeSemCargaAprovada();
}

//*******Etapa 3*******

function Etapa3CTeSemCargaDesabilitada() {
    _etapaCancelamentoCTe.Etapa3.eventClick = function () { };
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3CTeSemCargaLiberada() {
    _etapaCancelamentoCTe.Etapa3.eventClick = BuscarCTes;
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2CTeSemCargaAprovada();
}

function Etapa3CTeSemCargaAguardando() {
    _etapaCancelamentoCTe.Etapa3.eventClick = BuscarCTes;
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2CTeSemCargaAprovada();
}

function Etapa3CTeSemCargaAprovada() {
    _etapaCancelamentoCTe.Etapa3.eventClick = BuscarCTes;
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2CTeSemCargaAprovada();
}

function Etapa3CTeSemCargaReprovada() {
    _etapaCancelamentoCTe.Etapa3.eventClick = BuscarCTes;
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoCTe.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2CTeSemCargaAprovada();
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