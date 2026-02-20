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
/// <reference path="../../Enumeradores/EnumStatusFaturamentoMensal.js" />
/// <reference path="../../Enumeradores/EnumEtapaFaturamentoMensal.js" />
/// <reference path="FaturamentoMensal.js" />
/// <reference path="FaturamentoMensalBoleto.js" />
/// <reference path="FaturamentoMensalDocumento.js" />
/// <reference path="FaturamentoMensalEnvioEmail.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFaturamentoMensal;
var _etapaAtual;

var EtapaFaturamentoMensal = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Seleção de Faturamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaSelecaoFaturamentoClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde se seleciona os faturamentos mensais cadastrados."),
        tooltipTitle: ko.observable("Seleção de Faturamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "Geração de Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaGeracaoDocumentoClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde serão gerados os documentos e títulos mensais."),
        tooltipTitle: ko.observable("Geração de Documentos")
    });
    this.Etapa3 = PropertyEntity({
        text: "Geração de Boleto", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaGeracaoBoletoClick,
        step: ko.observable(3),
        tooltip: ko.observable("Esta onde será gerada os boletos dos títulos gerados anteriormente."),
        tooltipTitle: ko.observable("Geração de Boleto")
    });
    this.Etapa4 = PropertyEntity({
        text: "Envio de E-mail", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaEnvioEmailClick,
        step: ko.observable(4),
        tooltip: ko.observable("A etapa 4 terá a possíbilidade de enviar por e-mail os boletos e os documentos gerados."),
        tooltipTitle: ko.observable("Envio de E-mail")
    });
}


//*******EVENTOS*******

function loadEtapaFaturamentoMensal() {
    _etapaFaturamentoMensal = new EtapaFaturamentoMensal();
    KoBindings(_etapaFaturamentoMensal, "knockoutEtapaGeracaoFaturamento");

    $("#" + _etapaFaturamentoMensal.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaFaturamentoMensal.Etapa1.idTab).click();
}

function etapaSelecaoFaturamentoClick(e, sender) {
    PosicionarEtapa();
    _etapaAtual = 1;
    buscarFaturamentoMensalParaGeracao();
}

function etapaGeracaoDocumentoClick(e, sender) {
    buscarFaturamentoMensalDocumento();
    if (_etapaAtual < 2) {
        ProximoSelecaoFaturamentoClick(e, sender);
    }
    _etapaAtual = 2;
    PosicionarEtapa()
}

function etapaGeracaoBoletoClick(e, sender) {
    buscarFaturamentoMensalBoleto();
    if (_etapaAtual < 3) {
        ProximoGeracaoDocumentoClick(e, sender);
    }
    _etapaAtual = 3;
    PosicionarEtapa();
}

function etapaEnvioEmailClick(e, sender) {
    buscarTitulosParaEnvioEmail();
    if (_etapaAtual < 4) {
        ProximoGeracaoBoletoClick(e, sender);
    }
    _etapaAtual = 4;
    PosicionarEtapa();
}

//*******MÉTODOS*******

function PosicionarEtapa() {
    LimparOcultarAbas();

    $("#" + _etapaFaturamentoMensal.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaFaturamentoMensal.Etapa3.idTab + " .step").attr("class", "step lightgreen");
    $("#" + _etapaFaturamentoMensal.Etapa4.idTab + " .step").attr("class", "step lightgreen");

    if (_selecaoFaturamento.Status.val() == EnumStatusFaturamentoMensal.Finalizado) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa3.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa4.idTab + " .step").attr("class", "step green");
    }
    else if (_selecaoFaturamento.Status.val() == EnumStatusFaturamentoMensal.DocumentosAutorizados) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step green");
    }
    else if (_selecaoFaturamento.Status.val() == EnumStatusFaturamentoMensal.GeradoBoletos) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa3.idTab + " .step").attr("class", "step green");
    }
    else if (_selecaoFaturamento.Status.val() == EnumStatusFaturamentoMensal.GeradoDocumentos || _selecaoFaturamento.Status.val() == EnumStatusFaturamentoMensal.Iniciada) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step green");
    }
    else if (_selecaoFaturamento.Status.val() == EnumStatusFaturamentoMensal.Cancelado) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step red");
        $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step red");
        $("#" + _etapaFaturamentoMensal.Etapa3.idTab + " .step").attr("class", "step red");
        $("#" + _etapaFaturamentoMensal.Etapa4.idTab + " .step").attr("class", "step red");
    }

    if (_etapaAtual == 1) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step lightgreen");

    } else if (_etapaAtual == 2) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step green");

        $("#" + _etapaFaturamentoMensal.Etapa2.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 3) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step green");

        $("#" + _etapaFaturamentoMensal.Etapa3.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaFaturamentoMensal.Etapa3.idTab + " .step").attr("class", "step lightgreen");
    } else if (_etapaAtual == 4) {
        $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step green");
        $("#" + _etapaFaturamentoMensal.Etapa3.idTab + " .step").attr("class", "step green");

        $("#" + _etapaFaturamentoMensal.Etapa4.idTab).attr("data-bs-toggle", "tab");
        $("#" + _etapaFaturamentoMensal.Etapa4.idTab + " .step").attr("class", "step lightgreen");
    }
}

function LimparOcultarAbas() {
    $("#" + _etapaFaturamentoMensal.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa1.idTab + " .step").attr("class", "step");

    $("#" + _etapaFaturamentoMensal.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa2.idTab + " .step").attr("class", "step");

    $("#" + _etapaFaturamentoMensal.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa3.idTab + " .step").attr("class", "step");

    $("#" + _etapaFaturamentoMensal.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFaturamentoMensal.Etapa4.idTab + " .step").attr("class", "step");
}