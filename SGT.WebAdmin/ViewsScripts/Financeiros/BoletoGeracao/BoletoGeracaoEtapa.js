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
/// <reference path="BoletoGeracao.js" />
/// <reference path="EnvioEmail.js" />
/// <reference path="GeracaoRemessa.js" />
/// <reference path="SelecaoTitulo.js" />
/// <reference path="GeracaoFrancesinha.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaBoletoGeracao;
var _etapaAtual;

var EtapaBoletoGeracao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("20%") });

    this.Etapa1 = PropertyEntity({
        text: "Seleção de Títulos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaSelecaoTituloClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se seleciona os títulos desejados para se fazer a geração dos boletos e sua remessa.."),
        tooltipTitle: ko.observable("Seleção de Títulos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Geração de Boletos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaGeracaoBoletoClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde serão gerados os boletos e com possibilidade de impri-los."),
        tooltipTitle: ko.observable("Geração de Boletos")
    });
    this.Etapa3 = PropertyEntity({
        text: "Geração da Remessa", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaGeracaoRemessaClick,
        step: ko.observable(3),
        tooltip: ko.observable("Esta onde será gerada a remessa dos boletos gerados."),
        tooltipTitle: ko.observable("Geração da Remessa")
    });
    this.Etapa4 = PropertyEntity({
        text: "Envio de E-mail", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaEnvioEmailClick,
        step: ko.observable(4),
        tooltip: ko.observable("A etapa 4 terá a possibilidade de enviar por e-mail os boletos gerados."),
        tooltipTitle: ko.observable("Envio de E-mail")
    });
    this.Etapa5 = PropertyEntity({
        text: "Geração de Francesinha", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaGeracaoFrancesinhaClick,
        step: ko.observable(5),
        tooltip: ko.observable("A etapa 5 terá a possibilidade de baixar as francesinhas dos boletos gerados."),
        tooltipTitle: ko.observable("Francesinha")
    });
}


//*******EVENTOS*******

function loadEtapaBoletoGeracao() {
    _etapaBoletoGeracao = new EtapaBoletoGeracao();
    KoBindings(_etapaBoletoGeracao, "knockoutEtapaGeracaoBoleto");

    $("#" + _etapaBoletoGeracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaBoletoGeracao.Etapa1.idTab).click();
}

function etapaSelecaoTituloClick(e, sender) {
    _etapaAtual = 1;
    PosicionarEtapa();
}

function etapaGeracaoBoletoClick(e, sender) {
    if (_etapaAtual < 2) {
        ProximoSelecaoTituloClick(e, sender);
    }
    _etapaAtual = 2;
    PosicionarEtapa()
}

function etapaGeracaoRemessaClick(e, sender) {
    if (_etapaAtual < 3) {
        ProximoGeracaoBoletoClick(e, sender);
    }
    _etapaAtual = 3;
    PosicionarEtapa();
}

function etapaEnvioEmailClick(e, sender) {
    if (_etapaAtual < 4) {
        ProximoGeracaoRemessaClick(e, sender);
    }
    _etapaAtual = 4;
    PosicionarEtapa();
}

function etapaGeracaoFrancesinhaClick(e, sender) {
    if (_etapaAtual < 5) {
        ProximoGeracaoFrancesinhaClick(e, sender);
    }
    _etapaAtual = 5;
    PosicionarEtapa();
}

//*******MÉTODOS*******

function PosicionarEtapa() {
    LimparOcultarAbas();

    $("#" + _etapaBoletoGeracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa5.idTab).attr("data-bs-toggle", "tab");

    if (_etapaAtual == 1) {
        $("#" + _etapaBoletoGeracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    } else if (_etapaAtual == 2) {
        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoGeracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 3) {
        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoGeracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 4) {
        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoGeracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa4.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 5) {
        $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa3.idTab + " .step").attr("class", "step green");
        $("#" + _etapaBoletoGeracao.Etapa4.idTab + " .step").attr("class", "step green");

        $("#" + _etapaBoletoGeracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaBoletoGeracao.Etapa5.idTab + " .step").attr("class", "step lightgreen");
    }
}

function LimparOcultarAbas() {
    $("#" + _etapaBoletoGeracao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoGeracao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoGeracao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa3.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoGeracao.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa4.idTab + " .step").attr("class", "step");

    $("#" + _etapaBoletoGeracao.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBoletoGeracao.Etapa5.idTab + " .step").attr("class", "step");
}