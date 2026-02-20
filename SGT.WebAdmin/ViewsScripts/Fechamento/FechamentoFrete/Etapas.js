/// <reference path="SolicitacaoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFechamento;

var EtapaFechamento = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Dados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados do fechamento."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Valores do Fechamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É onde se altera os valores do fechamento."),
        tooltipTitle: ko.observable("Valores do Fechamento")
    });
    this.Etapa3 = PropertyEntity({
        text: "Complementos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Etapa onde os complementos são emitidos, se necessário."),
        tooltipTitle: ko.observable("Complementos")
    });

    this.Etapa4 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Etapa onde as integrações são feitas."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function LoadEtapasFechamento() {
    _etapaFechamento = new EtapaFechamento();
    KoBindings(_etapaFechamento, "knockoutEtapasFechamento");
    Etapa1FechamentoLiberada();
    SetarEtapaInicioFechamento();
}

function SetarEtapaInicioFechamento() {
    DesabilitarTodasEtapasFechamento();
    Etapa1FechamentoLiberada();

    let triggerEl = document.querySelector('#knockoutEtapasFechamento li:first-child button');

    if (triggerEl != null) {
        let instance = bootstrap.Tab.getInstance(triggerEl);

        if (instance != null)
            instance.show();
    }
}

function SetarEtapasFechamento() {
    var situacao = _fechamentoFrete.Situacao.val();

    if (situacao == EnumSituacaoFechamentoFrete.Aberto)
        Etapa2FechamentoLiberada();
    else if (situacao == EnumSituacaoFechamentoFrete.Cancelado)
        Etapa2FechamentoLiberada();
    else if (situacao == EnumSituacaoFechamentoFrete.EmEmissaoComplemento)
        Etapa3FechamentoAguardando();
    else if (situacao == EnumSituacaoFechamentoFrete.PendenciaEmissao)
        Etapa3FechamentoReprovada();
    else if (situacao == EnumSituacaoFechamentoFrete.AgIntegracao)
        Etapa4FechamentoAguardando();
    else if (situacao == EnumSituacaoFechamentoFrete.ProblemaIntegracao)
        Etapa4FechamentoReprovada();
    else if (situacao == EnumSituacaoFechamentoFrete.Fechado)
        Etapa4FechamentoAprovada();
}

function DesabilitarTodasEtapasFechamento() {
    Etapa2FechamentoDesabilitada();
    Etapa3FechamentoDesabilitada();
    Etapa4FechamentoDesabilitada();
}

//*******Etapa 1*******

function Etapa1FechamentoLiberada() {
    $("#" + _etapaFechamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2FechamentoDesabilitada();
}

function Etapa1FechamentoAprovada() {
    $("#" + _etapaFechamento.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2FechamentoDesabilitada() {
    $("#" + _etapaFechamento.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3FechamentoDesabilitada();
}

function Etapa2FechamentoAprovada() {
    $("#" + _etapaFechamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1FechamentoAprovada();
}

function Etapa2FechamentoReprovada() {
    $("#" + _etapaFechamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1FechamentoAprovada();
}

function Etapa2FechamentoLiberada() {
    $("#" + _etapaFechamento.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa2.idTab + " .step").attr("class", "step yellow");
    //$("#" + _etapaFechamento.Etapa2.idTab).click();
    Etapa1FechamentoAprovada();
}


//*******Etapa 3*******

function Etapa3FechamentoDesabilitada() {
    $("#" + _etapaFechamento.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step");
    _etapaFechamento.Etapa3.eventClick = function () { };
    Etapa4FechamentoDesabilitada();
}

function Etapa3FechamentoAguardando() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaFechamento.Etapa3.eventClick = preencherFechamentoFreteDocumentoComplementar;
    Etapa2FechamentoAprovada();
}

function Etapa3FechamentoAprovada() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");

    if (_fechamentoFrete.AguardandoNFSManual.val())
        $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step cyan");
    else
        $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step green");

    _etapaFechamento.Etapa3.eventClick = preencherFechamentoFreteDocumentoComplementar;
    Etapa2FechamentoAprovada();
}

function Etapa3FechamentoReprovada() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaFechamento.Etapa3.eventClick = preencherFechamentoFreteDocumentoComplementar;
    Etapa2FechamentoAprovada();
}

function Etapa3FechamentoLiberada() {
    $("#" + _etapaFechamento.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa3.idTab + " .step").attr("class", "step yellow");
    //$("#" + _etapaFechamento.Etapa3.idTab).click();
    _etapaFechamento.Etapa3.eventClick = preencherFechamentoFreteDocumentoComplementar;

    Etapa2FechamentoAprovada();
}

//*******Etapa 4*******

function Etapa4FechamentoDesabilitada() {
    $("#" + _etapaFechamento.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamento.Etapa4.idTab + " .step").attr("class", "step");
}

function Etapa4FechamentoAguardando() {
    $("#" + _etapaFechamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa4.idTab + " .step").attr("class", "step yellow");
    Etapa3FechamentoAprovada();
}


function Etapa4FechamentoAprovada() {
    $("#" + _etapaFechamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa4.idTab + " .step").attr("class", "step green");
    Etapa3FechamentoAprovada();
}

function Etapa4FechamentoReprovada() {
    $("#" + _etapaFechamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa4.idTab + " .step").attr("class", "step red");
    Etapa3FechamentoAprovada();
}

function Etapa4FechamentoLiberada() {
    $("#" + _etapaFechamento.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamento.Etapa4.idTab + " .step").attr("class", "step yellow");
    //$("#" + _etapaFechamento.Etapa4.idTab).click();
    Etapa3FechamentoAprovada();
}