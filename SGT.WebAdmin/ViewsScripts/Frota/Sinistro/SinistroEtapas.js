/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumEtapaSinistro.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEtapaFluxoSinistro.js" />

var _etapaFluxoSinistro;

var EtapaFluxoSinistro = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("20%") });

    this.Etapa1 = PropertyEntity({
        text: "Dados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Nessa etapa são preenchidos os dados sobre o sinistro."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Documentação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa são informados os dados dos envolvidos e a documentação."),
        tooltipTitle: ko.observable("Documentação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Manutenção", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Nesta etapa são informadas as manutenções necessárias."),
        tooltipTitle: ko.observable("Manutenção")
    });
    this.Etapa4 = PropertyEntity({
        text: "Indicação do Pagador", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Nessa etapa é informado quem será o pagador dos prejuízos."),
        tooltipTitle: ko.observable("Indicação do Pagador")
    });
    this.Etapa5 = PropertyEntity({
        text: "Acompanhamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(5),
        tooltip: ko.observable("Nessa etapa são informados os históricos referentes ao sinistro."),
        tooltipTitle: ko.observable("Acompanhamento")
    });
};

function loadEtapasFluxoSinistro() {
    _etapaFluxoSinistro = new EtapaFluxoSinistro();
    KoBindings(_etapaFluxoSinistro, "knockoutEtapasFluxoSinistro");

    $("[rel=popover-hover]").popover({ trigger: "hover" });
    $("#tabDados").click();
}

function ajustarEtapas(etapa, situacao) {
    switch (etapa) {
        case EnumEtapaSinistro.Dados:
            etapaDadosAguardando();
            break;
        case EnumEtapaSinistro.Documentacao:
            etapaDocumentacaoAguardando();
            break;
        case EnumEtapaSinistro.Manutencao:
            etapaManutencaoAguardando();
            break;
        case EnumEtapaSinistro.IndicacaoPagador:
            etapaIndicacaoPagadorAguardando();
            break;
        case EnumEtapaSinistro.Acompanhamento:
            if (situacao == EnumSituacaoEtapaFluxoSinistro.Aberto)
                etapaAcompanhamentoAguardando();
            else
                etapaAcompanhamentoFinalizada();
            break;
    }
}

function etapaDadosConcluida() {
    $("#" + _etapaFluxoSinistro.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa1.idTab + " .step").attr("class", "step green");
}

function etapaDocumentacaoConcluida() {
    $("#" + _etapaFluxoSinistro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa2.idTab + " .step").attr("class", "step green");

    etapaDadosConcluida();
}

function etapaManutencaoConcluida() {
    $("#" + _etapaFluxoSinistro.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa3.idTab + " .step").attr("class", "step green");

    etapaDocumentacaoConcluida();
}

function etapaIndicacaoPagadorConcluida() {
    $("#" + _etapaFluxoSinistro.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa4.idTab + " .step").attr("class", "step green");

    etapaManutencaoConcluida();
}

function etapaAcompanhamentoConcluida() {
    $("#" + _etapaFluxoSinistro.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa5.idTab + " .step").attr("class", "step green");

    etapaIndicacaoPagadorConcluida();
}

function etapaDadosAguardando() {
    $("#" + _etapaFluxoSinistro.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa1.idTab + " .step").attr("class", "step yellow");

    etapaDocumentacaoBloqueada();
    etapaManutencaoBloqueada();
    etapaIndicacaoPagadorBloqueada();
    etapaAcompanhamentoBloqueada();

    $("#tabDados").click();
}

function etapaDocumentacaoAguardando() {
    $("#" + _etapaFluxoSinistro.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa2.idTab + " .step").attr("class", "step yellow");

    etapaDadosConcluida();
    etapaManutencaoBloqueada();

    $("#tabDocumentacao").click();
}

function etapaManutencaoAguardando() {
    $("#" + _etapaFluxoSinistro.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa3.idTab + " .step").attr("class", "step yellow");

    etapaDocumentacaoConcluida();
    etapaIndicacaoPagadorBloqueada();

    $("#tabManutencao").click();
}

function etapaIndicacaoPagadorAguardando() {
    $("#" + _etapaFluxoSinistro.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa4.idTab + " .step").attr("class", "step yellow");

    etapaManutencaoConcluida();
    etapaAcompanhamentoBloqueada();
    $("#tabIndicacaoPagador").click();
}

function etapaAcompanhamentoAguardando() {
    $("#" + _etapaFluxoSinistro.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa5.idTab + " .step").attr("class", "step yellow");

    etapaIndicacaoPagadorConcluida();

    $("#tabAcompanhamento").click();
}

function etapaAcompanhamentoFinalizada() {
    $("#" + _etapaFluxoSinistro.Etapa5.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFluxoSinistro.Etapa5.idTab + " .step").attr("class", "step green");

    etapaIndicacaoPagadorConcluida();

    $("#tabAcompanhamento").click();
}

function etapaDocumentacaoBloqueada() {
    $("#" + _etapaFluxoSinistro.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFluxoSinistro.Etapa2.idTab + " .step").attr("class", "step");

    etapaManutencaoBloqueada();
}

function etapaManutencaoBloqueada() {
    $("#" + _etapaFluxoSinistro.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFluxoSinistro.Etapa3.idTab + " .step").attr("class", "step");

    etapaIndicacaoPagadorBloqueada();
}

function etapaIndicacaoPagadorBloqueada() {
    $("#" + _etapaFluxoSinistro.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFluxoSinistro.Etapa4.idTab + " .step").attr("class", "step");

    etapaAcompanhamentoBloqueada();
}

function etapaAcompanhamentoBloqueada() {
    $("#" + _etapaFluxoSinistro.Etapa5.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFluxoSinistro.Etapa5.idTab + " .step").attr("class", "step");
}