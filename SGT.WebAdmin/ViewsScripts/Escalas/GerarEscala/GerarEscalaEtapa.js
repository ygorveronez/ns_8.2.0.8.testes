/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumAbaGerarEscala.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEscala.js" />
/// <reference path="ExpedicaoEscala.js" />
/// <reference path="GerarEscala.js" />
/// <reference path="VeiculoEscala.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gerarEscalaEtapa;
var _etapaExpedicaoAtiva = false;
var _etapaVeiculoAtiva = false;

/*
 * Declaração das Classes
 */

var GerarEscalaEtapa = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33.33%") });

    this.EtapaDadosEscala = PropertyEntity({
        text: "Dados da Escala", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaDadosEscalaClick,
        step: ko.observable(1),
        tooltip: ko.observable("É onde é informado os dados para a geração da escala."),
        tooltipTitle: ko.observable("Dados da Escala")
    });

    this.EtapaExpedicao = PropertyEntity({
        text: "Expedição", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaExpedicaoClick,
        step: ko.observable(2),
        tooltip: ko.observable("É onde é informado como será a expedição da escala."),
        tooltipTitle: ko.observable("Expedição")
    });

    this.EtapaVeiculo = PropertyEntity({
        text: "Veículos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaVeiculoClick,
        step: ko.observable(3),
        tooltip: ko.observable("É onde os veículos são adicionados na escala."),
        tooltipTitle: ko.observable("Veículos")
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGerarEscalaEtapa() {
    _gerarEscalaEtapa = new GerarEscalaEtapa();
    KoBindings(_gerarEscalaEtapa, "knockoutEtapas");

    $("[rel=popover-hover]").popover({ trigger: "hover" });

    EtapaDadosEscalaLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaDadosEscalaClick() {
    _abaAtiva = EnumAbaGerarEscala.DadosEscala;

    controlarBotoesHabilitados();
}

function etapaExpedicaoClick() {
    if (_etapaExpedicaoAtiva) {
        _abaAtiva = EnumAbaGerarEscala.Expedicao;

        controlarBotoesHabilitados();
    }
}

function etapaVeiculoClick() {
    if (_etapaVeiculoAtiva) {
        _abaAtiva = EnumAbaGerarEscala.Veiculo;

        controlarBotoesHabilitados();
    }
}

/*
 * Declaração das Funções Públicas
 */

function setarEtapaInicial() {
    EtapaDadosEscalaLiberada();

    $("#" + _gerarEscalaEtapa.EtapaDadosEscala.idTab).click();
}

function setarEtapas() {
    $("#" + _gerarEscalaEtapa.EtapaDadosEscala.idTab).click();

    var situacao = _gerarEscala.SituacaoEscala.val();

    switch (situacao) {
        case EnumSituacaoEscala.AgVeiculos:
            EtapaVeiculoLiberada();
            break;

        case EnumSituacaoEscala.EmCriacao:
            EtapaExpedicaoLiberada();
            break;

        case EnumSituacaoEscala.Finalizada:
            EtapaVeiculoAprovada();
            break;

        case EnumSituacaoEscala.Todas:
            EtapaDadosEscalaLiberada();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Dados da Escala)
 */

function EtapaDadosEscalaAprovada() {
    $("#" + _gerarEscalaEtapa.EtapaDadosEscala.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gerarEscalaEtapa.EtapaDadosEscala.idTab + " .step").attr("class", "step green");
}

function EtapaDadosEscalaLiberada() {
    $("#" + _gerarEscalaEtapa.EtapaDadosEscala.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gerarEscalaEtapa.EtapaDadosEscala.idTab + " .step").attr("class", "step yellow");

    EtapaExpedicaoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Expedição)
 */

function EtapaExpedicaoAprovada() {
    $("#" + _gerarEscalaEtapa.EtapaExpedicao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gerarEscalaEtapa.EtapaExpedicao.idTab + " .step").attr("class", "step green");

    _etapaExpedicaoAtiva = true;

    EtapaDadosEscalaAprovada();
}

function EtapaExpedicaoDesabilitada() {
    $("#" + _gerarEscalaEtapa.EtapaExpedicao.idTab).removeAttr("data-bs-toggle");
    $("#" + _gerarEscalaEtapa.EtapaExpedicao.idTab + " .step").attr("class", "step");

    _etapaExpedicaoAtiva = false;

    EtapaVeiculoDesabilitada();
}

function EtapaExpedicaoLiberada() {
    $("#" + _gerarEscalaEtapa.EtapaExpedicao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gerarEscalaEtapa.EtapaExpedicao.idTab + " .step").attr("class", "step yellow");

    _etapaExpedicaoAtiva = true;

    EtapaDadosEscalaAprovada();
    EtapaVeiculoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Três (Veículos)
 */

function EtapaVeiculoAprovada() {
    $("#" + _gerarEscalaEtapa.EtapaVeiculo.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gerarEscalaEtapa.EtapaVeiculo.idTab + " .step").attr("class", "step green");

    _etapaVeiculoAtiva = true;

    EtapaExpedicaoAprovada();
}

function EtapaVeiculoDesabilitada() {
    $("#" + _gerarEscalaEtapa.EtapaVeiculo.idTab).removeAttr("data-bs-toggle");
    $("#" + _gerarEscalaEtapa.EtapaVeiculo.idTab + " .step").attr("class", "step");

    _etapaVeiculoAtiva = false;
}

function EtapaVeiculoLiberada() {
    $("#" + _gerarEscalaEtapa.EtapaVeiculo.idTab).attr("data-bs-toggle", "tab");
    $("#" + _gerarEscalaEtapa.EtapaVeiculo.idTab + " .step").attr("class", "step yellow");

    _etapaVeiculoAtiva = true;

    EtapaExpedicaoAprovada();
}