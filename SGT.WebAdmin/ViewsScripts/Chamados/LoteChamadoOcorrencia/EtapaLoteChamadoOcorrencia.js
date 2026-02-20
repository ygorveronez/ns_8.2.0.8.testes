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
/// <reference path="../../Enumeradores/EnumSituacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaLoteChamadoOcorrencia;

var EtapaLoteChamadoOcorrencia = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(verificarTamanhoEtapa) });

    this.Etapa1 = PropertyEntity({
        text: "Atendimentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Atendimentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação Lote", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aprovação Lote")
    });
}

//*******EVENTOS*******

function LoadEtapaLoteChamadoOcorrencia() {
    _etapaLoteChamadoOcorrencia = new EtapaLoteChamadoOcorrencia();
    KoBindings(_etapaLoteChamadoOcorrencia, "knockoutEtapaLoteChamadoOcorrencia");
    Etapa1Liberada();
    SetarEtapaInicioLoteChamadoOcorrencia();
}

function SetarEtapaInicioLoteChamadoOcorrencia() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab)[0].click();
}

function SetarEtapaLoteChamadoOcorrencia() {
    let situacaoLoteChamadoOcorrencia = _atendimento.SituacaoLote.val();
    console.log(situacaoLoteChamadoOcorrencia)
    if (situacaoLoteChamadoOcorrencia == EnumSituacaoLoteChamadoOcorrencia.EmEdicao)
        Etapa1Liberada();
    else if (situacaoLoteChamadoOcorrencia == EnumSituacaoLoteChamadoOcorrencia.AgAprovacao)
        Etapa2Liberada();
    else if (situacaoLoteChamadoOcorrencia == EnumSituacaoLoteChamadoOcorrencia.Aprovado)
        Etapa2Aprovada();
    else if (situacaoLoteChamadoOcorrencia == EnumSituacaoLoteChamadoOcorrencia.Reprovado)
        Etapa2Reprovada();
    else
        Etapa1Aguardando();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

function DefinirTab() {
    if (_atendimento.SituacaoLote.val() == EnumSituacaoLoteChamadoOcorrencia.AgAprovacao) {
        $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab).tab("show");
        _etapaLoteChamadoOcorrencia.Etapa2.eventClick();
    }

    else if (_atendimento.SituacaoLote.val() == EnumSituacaoLoteChamadoOcorrencia.Reprovado) {
        $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab).tab("show");
        _etapaLoteChamadoOcorrencia.Etapa2.eventClick();
    }

    else {
        $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab).tab("show");
        _etapaLoteChamadoOcorrencia.Etapa1.eventClick();
    }


}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aguardando() {
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Reprovada() {
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa1.idTab + " .step").attr("class", "step red");
    Etapa2Desabilitada();
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaLoteChamadoOcorrencia.Etapa2.eventClick = function () { };
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    _etapaLoteChamadoOcorrencia.Etapa2.eventClick = function () { };
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaLoteChamadoOcorrencia.Etapa2.eventClick = BuscarDadosIntegracoesLoteChamadoOcorrencia;
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaLoteChamadoOcorrencia.Etapa2.eventClick = BuscarDadosIntegracoesLoteChamadoOcorrencia;
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaLoteChamadoOcorrencia.Etapa2.eventClick = BuscarDadosIntegracoesLoteChamadoOcorrencia;
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLoteChamadoOcorrencia.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

// ********* Funções ********* //

function verificarTamanhoEtapa() {
    return "50%";
}