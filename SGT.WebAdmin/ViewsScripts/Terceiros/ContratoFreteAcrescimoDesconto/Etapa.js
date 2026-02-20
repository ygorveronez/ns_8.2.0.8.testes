/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteAcrescimoDesconto.js" />
/// <reference path="ContratoFreteAcrescimoDesconto.js" />
/// <reference path="Aprovacao.js" />
/// <reference path="Integracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaContratoFreteAcrescimoDesconto;

var EtapaContratoFreteAcrescimoDesconto = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Contrato de Frete", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia o cadastro."),
        tooltipTitle: ko.observable("Contrato de Frete")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de aprovação."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É a etapa de integração."),
        tooltipTitle: ko.observable("Integração")
    });
};

//*******EVENTOS*******

function LoadEtapaContratoFreteAcrescimoDesconto() {
    _etapaContratoFreteAcrescimoDesconto = new EtapaContratoFreteAcrescimoDesconto();
    KoBindings(_etapaContratoFreteAcrescimoDesconto, "knockoutEtapaContratoFreteAcrescimoDesconto");
    Etapa1Liberada();
}

function SetarEtapaInicioContratoFreteAcrescimoDesconto() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa1.idTab).click();
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa1.idTab).tab("show");
}

function SetarEtapaContratoFreteAcrescimoDesconto() {
    var situacao = _contratoFreteAcrescimoDesconto.Situacao.val();

    Etapa1Aprovada();
    if (situacao === EnumSituacaoContratoFreteAcrescimoDesconto.AgAprovacao || situacao === EnumSituacaoContratoFreteAcrescimoDesconto.SemRegra)
        Etapa2Aguardando();
    else if ((situacao === EnumSituacaoContratoFreteAcrescimoDesconto.Aprovado || situacao === EnumSituacaoContratoFreteAcrescimoDesconto.Finalizado) && _contratoFreteAcrescimoDesconto.DisponibilizarFechamentoDeAgregado.val()) {
        Etapa2Aprovada();
        Etapa3Desabilitada();
    }
    else if (situacao === EnumSituacaoContratoFreteAcrescimoDesconto.Finalizado)
        Etapa3Aprovada();
    else if (situacao === EnumSituacaoContratoFreteAcrescimoDesconto.Rejeitado || situacao === EnumSituacaoContratoFreteAcrescimoDesconto.Cancelado)
        Etapa3Aprovada();
    else if (situacao === EnumSituacaoContratoFreteAcrescimoDesconto.Aprovado || situacao === EnumSituacaoContratoFreteAcrescimoDesconto.AgIntegracao || situacao === EnumSituacaoContratoFreteAcrescimoDesconto.AplicacaoValorRejeitado)
        Etapa3Aguardando();
    else if (situacao === EnumSituacaoContratoFreteAcrescimoDesconto.FalhaIntegracao)
        Etapa3Reprovada();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa2.eventClick = function () { };
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada();
}

function Etapa2Liberada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa2.eventClick = CarregarAprovacaoContratoFreteAcrescimoDesconto;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaContratoFreteAcrescimoDesconto.Etapa2.eventClick = CarregarAprovacaoContratoFreteAcrescimoDesconto;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa2.eventClick = CarregarAprovacaoContratoFreteAcrescimoDesconto;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa2.eventClick = CarregarAprovacaoContratoFreteAcrescimoDesconto;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa3.eventClick = function () { };
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Liberada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa3.eventClick = recarregarIntegracoes;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaContratoFreteAcrescimoDesconto.Etapa3.eventClick = recarregarIntegracoes;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa3.eventClick = recarregarIntegracoes;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaContratoFreteAcrescimoDesconto.Etapa3.eventClick = recarregarIntegracoes;
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaContratoFreteAcrescimoDesconto.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}