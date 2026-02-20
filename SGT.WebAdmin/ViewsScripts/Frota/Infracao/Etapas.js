/// <reference path="Infracao.js" />
/// <reference path="EmpresaInfracao.js" />
/// <reference path="../../Enumeradores/EnumAbaInfracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _etapaInfracao;
var _etapaAprovacaoAtiva = false;
var _etapaProcessamentoAtiva = false;
var _etapaEmpresaAtiva = false;

/*
 * Declaração das Classes
 */

var EtapaInfracao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.EtapaAprovacao = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaAprovacaoClick,
        step: ko.observable(3),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Aprovação")
    });

    this.EtapaInfracao = PropertyEntity({
        text: "Ocorrência", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaInfracaoClick,
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Ocorrência")
    });

    this.EtapaProcessamento = PropertyEntity({
        text: "Processamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: etapaProcessamentoClick,
        step: ko.observable(2),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Processamento")
    });

    this.EtapaEmpresa = PropertyEntity({
        text: "Empresa", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: EtapaEmpresaClick,
        step: ko.observable(4),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable("Empresa")
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadEtapaInfracao() {
    _etapaInfracao = new EtapaInfracao();
    KoBindings(_etapaInfracao, "knockoutEtapaInfracao");

    etapaInfracaoLiberada();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function etapaAprovacaoClick() {
    if (_etapaAprovacaoAtiva) {
        _abaAtiva = EnumAbaInfracao.Aprovacao;

        controlarBotoesHabilitados();
    }
}

function EtapaEmpresaClick() {
    if (_etapaEmpresaAtiva) {
        _abaAtiva = EnumAbaInfracao.Empresa;

        controlarBotoesHabilitados();
        controlarCamposEmpresaInfracao();
    }
}

function etapaProcessamentoClick() {
    if (_etapaProcessamentoAtiva) {
        _abaAtiva = EnumAbaInfracao.Processamento;

        controlarBotoesHabilitados();
    }
}

function etapaInfracaoClick() {
    _abaAtiva = EnumAbaInfracao.Infracao;

    controlarBotoesHabilitados();
}

/*
 * Declaração das Funções Públicas
 */

function etapaInfracaoSemRegra() {
    exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. A infração permanecerá aguardando autorização.");

    _CRUDInfracao.ReprocessarRegras.visible(true);
}

function setarEtapaInicialInfracao() {
    etapaInfracaoLiberada();

    $("#" + _etapaInfracao.EtapaInfracao.idTab).click();
    $("#" + _etapaInfracao.EtapaInfracao.idTab).tab("show");
}

function setarEtapasInfracao() {
    $("#" + _etapaInfracao.EtapaInfracao.idTab).click();
    $("#" + _etapaInfracao.EtapaInfracao.idTab).tab("show");

    switch (_infracao.Situacao.val()) {
        case EnumSituacaoInfracao.AguardandoAprovacao:
            etapaAprovacaoAguardando();
            break;

        case EnumSituacaoInfracao.AguardandoProcessamento:
            etapaProcessamentoLiberada();
            break;

        case EnumSituacaoInfracao.AprovacaoRejeitada:
            etapaAprovacaoRejeitada();
            break;

        case EnumSituacaoInfracao.Cancelada:
            etapaProcessamentoCancelada();
            break;

        case EnumSituacaoInfracao.Finalizada:
            etapaAprovacaoAprovada();
            break;

        case EnumSituacaoInfracao.Todas:
            etapaInfracaoLiberada();
            break;

        case EnumSituacaoInfracao.SemRegraAprovacao:
            etapaAprovacaoSemRegra();
            break;
    }
}

/*
 * Declaração das Funções da Etapa Um (Infração)
 */

function etapaInfracaoAprovada() {
    $("#" + _etapaInfracao.EtapaInfracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaInfracao.idTab + " .step").attr("class", "step green");    
}

function etapaInfracaoLiberada() {
    $("#" + _etapaInfracao.EtapaInfracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaInfracao.idTab + " .step").attr("class", "step yellow");

    etapaProcessamentoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Dois (Processamento)
 */

function etapaProcessamentoAprovada() {
    $("#" + _etapaInfracao.EtapaProcessamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaProcessamento.idTab + " .step").attr("class", "step green");

    _etapaProcessamentoAtiva = true;    
    
    etapaInfracaoAprovada();
}

function etapaProcessamentoCancelada() {
    $("#" + _etapaInfracao.EtapaProcessamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaProcessamento.idTab + " .step").attr("class", "step red");
        

    etapaEmpresaDesabilitada();
    etapaInfracaoAprovada();
    etapaAprovacaoDesabilitada();
}

function etapaProcessamentoDesabilitada() {
    $("#" + _etapaInfracao.EtapaProcessamento.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaInfracao.EtapaProcessamento.idTab + " .step").attr("class", "step");

    _etapaProcessamentoAtiva = false;
    
    etapaEmpresaDesabilitada();
    etapaAprovacaoDesabilitada();
}

function etapaProcessamentoLiberada() {
    $("#" + _etapaInfracao.EtapaProcessamento.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaProcessamento.idTab + " .step").attr("class", "step yellow");

    _etapaProcessamentoAtiva = true;
    
    etapaEmpresaDesabilitada();
    etapaInfracaoAprovada();
    etapaAprovacaoDesabilitada();
}

/*
 * Declaração das Funções da Etapa Três (Aprovação)
 */

function etapaAprovacaoAguardando() {
    $("#" + _etapaInfracao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaAprovacao.idTab + " .step").attr("class", "step yellow");

    _etapaAprovacaoAtiva = true;    

    etapaEmpresaDesabilitada();
    etapaProcessamentoAprovada();
}

function etapaAprovacaoAprovada() {
    $("#" + _etapaInfracao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaAprovacao.idTab + " .step").attr("class", "step green");

    _etapaAprovacaoAtiva = true;
    
    etapaEmpresaAprovada();
    etapaProcessamentoAprovada();
}

function etapaAprovacaoDesabilitada() {
    $("#" + _etapaInfracao.EtapaAprovacao.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaInfracao.EtapaAprovacao.idTab + " .step").attr("class", "step");

    _etapaAprovacaoAtiva = false;

    etapaEmpresaDesabilitada();
}

function etapaAprovacaoRejeitada() {
    $("#" + _etapaInfracao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;
    

    etapaEmpresaDesabilitada();
    etapaProcessamentoAprovada();
}

function etapaAprovacaoSemRegra() {
    $("#" + _etapaInfracao.EtapaAprovacao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaInfracao.EtapaAprovacao.idTab + " .step").attr("class", "step red");

    _etapaAprovacaoAtiva = true;
    
    etapaEmpresaDesabilitada();
    etapaProcessamentoAprovada();
    etapaInfracaoSemRegra();
}

function etapaEmpresaDesabilitada() {
    $("#" + _etapaInfracao.EtapaEmpresa.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaInfracao.EtapaEmpresa.idTab + " .step").attr("class", "step");
    
    _etapaEmpresaAtiva = false;
}

function etapaEmpresaAprovada() {
    $("#" + _etapaInfracao.EtapaEmpresa.idTab).attr("data-bs-toggle", "tab");
    if (_infracao.FaturadoTitulosEmpresa.val() === true)
        $("#" + _etapaInfracao.EtapaEmpresa.idTab + " .step").attr("class", "step green");
    else
        $("#" + _etapaInfracao.EtapaEmpresa.idTab + " .step").attr("class", "step yellow");
    
    _etapaEmpresaAtiva = true;    
}