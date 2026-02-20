/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumAbaContratoPrestacaoServico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoPrestacaoServico.js" />
/// <reference path="ContratoPrestacaoServico.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _contratoPrestacaoServicoEtapa;
var _etapaAprovacaoAtiva = false;

/*
 * Declaração das Classes
 */

var ContratoPrestacaoServicoEtapa = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.EtapaAprovacao = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAprovacaoClick,
        step: ko.observable(3),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aprovação")
    });

    this.EtapaDados = PropertyEntity({
        text: "Dados", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaDadosClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Dados")
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadContratoPrestacaoServicoEtapa() {
    _contratoPrestacaoServicoEtapa = new ContratoPrestacaoServicoEtapa();
    KoBindings(_contratoPrestacaoServicoEtapa, "knockoutEtapaContratoPrestacaoServico");

    etapaDadosLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaAprovacaoClick() {
    if (_etapaAprovacaoAtiva) {
        _abaAtiva = EnumAbaContratoPrestacaoServico.Aprovacao;

        controlarBotoesHabilitados();
    }
}

function etapaDadosClick() {
    _abaAtiva = EnumAbaContratoPrestacaoServico.Dados;

    controlarBotoesHabilitados();
}

/*
 * Declaração das Funções Públicas
 */

function setarEtapaInicial() {
    etapaDadosLiberada();

    $("#" + _contratoPrestacaoServicoEtapa.EtapaDados.idTab).click();
}

function setarEtapas(situacaoContratoPrestacaoServico) {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaDados.idTab).click();

    switch (situacaoContratoPrestacaoServico) {
        case EnumSituacaoContratoPrestacaoServico.AguardandoAprovacao:
            etapaAprovacaoAguardando();
            break;

        case EnumSituacaoContratoPrestacaoServico.AprovacaoRejeitada:
            etapaAprovacaoRejeitada();
            break;

        case EnumSituacaoContratoPrestacaoServico.Aprovado:
            etapaAprovacaoAprovada();
            break;

        case EnumSituacaoContratoPrestacaoServico.SemRegraAprovacao:
            etapaAprovacaoSemRegra();
            break;

        case EnumSituacaoContratoPrestacaoServico.Todas:
            etapaDadosLiberada();
            break;
    }
}

/*
 * Declaração das Funções
 */

function etapaSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. O contrato de prestação de serviço permanecerá aguardando autorização.");

    _CRUDContratoPrestacaoServico.ReprocessarRegras.visible(true);
}

/*
 * Declaração das Funções da Etapa Um (Dados)
 */

function etapaDadosAprovada() {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaDados.idTab).attr("data-bs-toggle", "tab");
    $("#" + _contratoPrestacaoServicoEtapa.EtapaDados.idTab + " .step").attr("class", "step green");
}

function etapaDadosLiberada() {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaDados.idTab).attr("data-bs-toggle", "tab");
    $("#" + _contratoPrestacaoServicoEtapa.EtapaDados.idTab + " .step").attr("class", "step yellow");

    etapaAprovacaoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Aprovação)
 */

function etapaAprovacaoAguardando() {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab + " .step").attr("class", "step yellow");

    _etapaAprovacaoAtiva = true;

    etapaDadosAprovada();
}

function etapaAprovacaoAprovada() {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab + " .step").attr("class", "step green");

    _etapaAprovacaoAtiva = true;

    etapaDadosAprovada();
}

function etapaAprovacaoDesabilitada() {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab).removeAttr("data-bs-toggle");
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab + " .step").attr("class", "step");

    _etapaAprovacaoAtiva = false;
}

function etapaAprovacaoRejeitada() {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    etapaDadosAprovada();
}

function etapaAprovacaoSemRegra() {
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _contratoPrestacaoServicoEtapa.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;

    etapaDadosAprovada();
    etapaSemRegra();
}