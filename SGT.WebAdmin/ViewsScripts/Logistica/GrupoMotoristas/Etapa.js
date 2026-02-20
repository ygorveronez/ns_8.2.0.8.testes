/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="Constantes.js" />
/// <reference path="GrupoMotoristas.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGrupoMotoristas.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaGrupoMotoristas;

var EtapaGrupoMotoristas = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.EtapaCadastro = PropertyEntity({
        text: "Grupo Motorista", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
    });
    this.EtapaIntegracao = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
    });
}


//*******EVENTOS*******

function loadEtapaGrupoMotoristas() {
    _etapaGrupoMotoristas = new EtapaGrupoMotoristas();
    KoBindings(_etapaGrupoMotoristas, "knockoutEtapaGrupoMotoristas");
    EtapaCadastroLiberada();
}

function setarEtapaInicioGrupoMotoristas() {
    DesabilitarTodasEtapas();
    EtapaCadastroLiberada();
    $("#" + _etapaGrupoMotoristas.EtapaCadastro.idTab).click();
}

function setarEtapasGrupoMotoristas() {
    var situacaoGrupoMotoristas = _grupoMotoristas.Situacao.val();

    if (situacaoGrupoMotoristas == EnumSituacaoGrupoMotoristas.AguardandoIntegracoes) {
        EtapaIntegracaoAguardando();
    }
    else if (situacaoGrupoMotoristas == EnumSituacaoGrupoMotoristas.FalhaNasIntegracoes) {
        EtapaIntegracaoReprovada();
    }
    else if (situacaoGrupoMotoristas == EnumSituacaoGrupoMotoristas.Finalizado) {
        EtapaIntegracaoAprovada();
    }
    else
        EtapaCadastroAguardando();
}

function DesabilitarTodasEtapas() {
    EtapaIntegracaoDesabilitada();
}

//*******Etapa 1*******

function EtapaCadastroLiberada() {
    $("#" + _etapaGrupoMotoristas.EtapaCadastro.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaGrupoMotoristas.EtapaCadastro.idTab + " .step").attr("class", "step yellow");
    EtapaIntegracaoDesabilitada();
}

function EtapaCadastroAprovada() {
    $("#" + _etapaGrupoMotoristas.EtapaCadastro.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaGrupoMotoristas.EtapaCadastro.idTab + " .step").attr("class", "step green");
}

function EtapaCadastroAguardando() {
    $("#" + _etapaGrupoMotoristas.EtapaCadastro.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaGrupoMotoristas.EtapaCadastro.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function EtapaIntegracaoDesabilitada() {
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab + " .step").attr("class", "step");
}

function EtapaIntegracaoLiberada() {
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab + " .step").attr("class", "step yellow");
    EtapaCadastroAprovada();
}

function EtapaIntegracaoAguardando() {
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab + " .step").attr("class", "step yellow");
    EtapaCadastroAprovada();
}

function EtapaIntegracaoAprovada() {
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab + " .step").attr("class", "step green");
    EtapaCadastroAprovada();
}

function EtapaIntegracaoReprovada() {
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaGrupoMotoristas.EtapaIntegracao.idTab + " .step").attr("class", "step red");
    EtapaCadastroAprovada();
}
