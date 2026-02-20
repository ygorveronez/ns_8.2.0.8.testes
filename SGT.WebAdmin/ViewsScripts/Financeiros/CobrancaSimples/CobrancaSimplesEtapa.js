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
/// <reference path="EnvioEmail.js" />
/// <reference path="GeracaoBoleto.js" />
/// <reference path="SelecaoDados.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCobrancaSimples;
var _etapaAtual;

var EtapaCobrancaSimples = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Seleção de Dados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaSelecaoDadoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se seleciona os dados desejados para fazer a geração dos boletos."),
        tooltipTitle: ko.observable("Seleção de Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Geração de Boletos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaGeracaoBoletoClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde serão gerados os boletos e com possibilidade de impri-los."),
        tooltipTitle: ko.observable("Geração de Boletos")
    });
    this.Etapa3 = PropertyEntity({
        text: "Envio de E-mail", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaEnvioEmailClick,
        step: ko.observable(3),
        tooltip: ko.observable("A etapa 3 terá a possíbilidade de enviar por e-mail os boletos gerados."),
        tooltipTitle: ko.observable("Envio de E-mail")
    });
}


//*******EVENTOS*******

function loadEtapaCobrancaSimples() {
    _etapaCobrancaSimples = new EtapaCobrancaSimples();
    KoBindings(_etapaCobrancaSimples, "knockoutEtapaCobrancaSimples");

    HeaderAuditoria("Titulo", _etapaCobrancaSimples);

    $("#" + _etapaCobrancaSimples.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCobrancaSimples.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaCobrancaSimples.Etapa1.idTab).click();
}

function etapaSelecaoDadoClick(e, sender) {
    _etapaAtual = 1;
    PosicionarEtapa();
}

function etapaGeracaoBoletoClick(e, sender) {
    if (_etapaAtual < 2) {
        ProximoSelecaoDadoClick(e, sender);
    }
    _etapaAtual = 2;
    PosicionarEtapa()
}

function etapaEnvioEmailClick(e, sender) {
    if (_etapaAtual < 3) {
        ProximoGeracaoBoletoClick(e, sender);
    }
    _etapaAtual = 3;
    PosicionarEtapa();
}

//*******MÉTODOS*******

function PosicionarEtapa() {
    LimparOcultarAbas();

    $("#" + _etapaCobrancaSimples.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCobrancaSimples.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCobrancaSimples.Etapa3.idTab).attr("data-bs-toggle", "tab");

    if (_etapaAtual == 1) {
        $("#" + _etapaCobrancaSimples.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaCobrancaSimples.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    } else if (_etapaAtual == 2) {
        $("#" + _etapaCobrancaSimples.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaCobrancaSimples.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaCobrancaSimples.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 3) {
        $("#" + _etapaCobrancaSimples.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaCobrancaSimples.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaCobrancaSimples.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaCobrancaSimples.Etapa3.idTab + " .step").attr("class", "step lightgreen");
    }
}

function LimparOcultarAbas() {
    $("#" + _etapaCobrancaSimples.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCobrancaSimples.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaCobrancaSimples.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCobrancaSimples.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaCobrancaSimples.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCobrancaSimples.Etapa3.idTab + " .step").attr("class", "step");
}