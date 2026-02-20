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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _breadcrumbs;
var _navegacao;
var $wizard;

var Breadcrumbs = function () {
    this.Etapa1 = PropertyEntity({ text: "Depósito", type: types.local, enable: ko.observable(true), eventClick: EtapaSetada(1) });
    this.Etapa2 = PropertyEntity({ text: "Rua", type: types.local, enable: ko.observable(false), eventClick: EtapaSetada(2) });
    this.Etapa3 = PropertyEntity({ text: "Bloco", type: types.local, enable: ko.observable(false), eventClick: EtapaSetada(3) });
    this.Etapa4 = PropertyEntity({ text: "Posição", type: types.local, enable: ko.observable(false), eventClick: EtapaSetada(4) });
}

var Navegacao = function () {
    this.Anterior = PropertyEntity({ type: types.event, text: " Anterior", enable: ko.observable(true) });
    this.Proximo = PropertyEntity({ type: types.event, text: "Próximo ", enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadNavegacao() {
    _breadcrumbs = new Breadcrumbs();
    KoBindings(_breadcrumbs, "knockoutBreadcrumbs");

    _navegacao = new Navegacao();
    KoBindings(_navegacao, "knockoutNavegacao");

    $wizard = $("#myWizard");

    $wizard.on('change', function (e, data) {
        // Valida passagem de etapas
        if (data && data.direction && data.direction == "next") {
            if (!PordeAbrirEtapa(EtapaAtual() + 1))
                e.preventDefault();
        }
    });
}

function VoltarEtapa() {
    $wizard.wizard('previous');
}

function AvancarEtapa() {
    $wizard.wizard('next');
}

function EtapaSetada(ordem) {
    return function () {
        if (PordeAbrirEtapa(ordem))
            SetarEtapa(ordem);
    }
}


//*******MÉTODOS*******
function PordeAbrirEtapa(ordem) {
    /**
     * Só é possível focar numa etapa quando a mesma estiver liberada pra isso
     * As etapas liberadas são as que possuem a classe enable
     */
    return $("#knockoutBreadcrumbs li[data-step='" + ordem +"']").hasClass("enable");
}

function EtapaAtual() {
    return $wizard.wizard('selectedItem').step;
}

function SetarEtapa(ordem){
    $wizard.wizard('selectedItem', { step: ordem });
}

function Etapa2Liberada() {
    $("#" + _breadcrumbs.Etapa2.id).addClass("enable");
    Etapa3Desativada();
}
function Etapa2Desativada() {
    $("#" + _breadcrumbs.Etapa2.id).removeClass("enable");
    Etapa3Desativada();
}


function Etapa3Liberada() {
    $("#" + _breadcrumbs.Etapa3.id).addClass("enable");
    Etapa4Desativada();
}
function Etapa3Desativada() {
    $("#" + _breadcrumbs.Etapa3.id).removeClass("enable");
    Etapa4Desativada();
}


function Etapa4Liberada() {
    $("#" + _breadcrumbs.Etapa4.id).addClass("enable");
}
function Etapa4Desativada() {
    $("#" + _breadcrumbs.Etapa4.id).removeClass("enable");
}