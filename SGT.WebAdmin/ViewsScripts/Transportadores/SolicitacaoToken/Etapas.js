/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapasSolicitacaoToken;

var EtapasSolicitacaoToken = function () {
    var _this = this;

    this.Etapa1 = PropertyEntity({
        text: 'Solicitação', type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable('Etapa Destinada ao Cadastro da Solicitação'),
        tooltipTitle: ko.observable('Solicitação')
    });
    this.Etapa1.visible.subscribe(EtapaAlternada);

    this.Etapa2 = PropertyEntity({
        text: 'Aprovação', type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable('?'),
        tooltipTitle: ko.observable('Aprovação')
    });
    this.Etapa2.visible.subscribe(EtapaAlternada);

    this.Etapa3 = PropertyEntity({
        text: 'Liberação Sistêmica', type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable('?'),
        tooltipTitle: ko.observable('Liberação Sistêmica')
    });
    this.Etapa3.visible.subscribe(EtapaAlternada);

    this.Etapa4 = PropertyEntity({
        text: 'Finalização', type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable('?'),
        tooltipTitle: ko.observable('Finalização')
    });
    this.Etapa4.visible.subscribe(EtapaAlternada);

    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("") });
}

//*******EVENTOS*******


function ativaEtapa2() {

}



function loadEtapaSolicitacaoToken() {
    _etapasSolicitacaoToken = new EtapasSolicitacaoToken();
    KoBindings(_etapasSolicitacaoToken, "knockoutEtapasSolicitacaoToken");
    Etapa1Liberada();
    EtapaAlternada();
}

function setarEtapaInicioSolicitacaoToken() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab).click();
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab).tab("show");
}

function setarEtapasSolicitacaoToken() {

    if (_solicitacaoToken.Situacao.val() == null)
        return Etapa1Aguardando();

    if (_solicitacaoToken.Situacao.val() == EnumEtapaSolicitacaoToken.AgAprovacao)
        return Etapa2Aguardando();

    if (_solicitacaoToken.Situacao.val() == EnumEtapaSolicitacaoToken.SemRegraAprovacao || _solicitacaoToken.Situacao.val() == EnumEtapaSolicitacaoToken.SolicitacaoReprovada)
        return Etapa2Reprovada();

    if (_solicitacaoToken.Situacao.val() == EnumEtapaSolicitacaoToken.SolicitacaoAprovada)
        return Etapa3Aguardando();

    if (_solicitacaoToken.Situacao.val() == EnumEtapaSolicitacaoToken.LiberacaoSistematicaProblema)
        return Etapa3Reprovada();

    if (_solicitacaoToken.Situacao.val() == EnumEtapaSolicitacaoToken.Finalizada)
        return Etapa4Aprovada();
}

function EtapaAlternada() {
    var etapas_visiveis = [];

    for (var i in _etapasSolicitacaoToken) {
        var match = i.match(/^Etapa([0-9]+)/);
        if (match != null && _etapasSolicitacaoToken[i].visible()) {
            etapas_visiveis.push({
                num: parseInt(match[1]),
                prop: _etapasSolicitacaoToken[i]
            });
        }
    }

    etapas_visiveis
        // Ordena pelo nome da propriedade
        .sort(function (a, b) {
            return a.num - b.num;
        })
        // seta o numero da etapa
        .map(function (c, i) {
            c.prop.step(i + 1);
        });

    _etapasSolicitacaoToken.TamanhoEtapa.val(Math.floor(100 / etapas_visiveis.length) + "%");
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab + " .step").attr("class", "step yellow");

    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Aguardando() {
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa1.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada();
}

function Etapa2Liberada() {
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab + " .step").attr("class", "step yellow");

    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab).tab("show");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab + " .step").attr("class", "step green");

    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa2.idTab + " .step").attr("class", "step red");

    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab + " .step").attr("class", "step");

    Etapa4Desabilitada();
}

function Etapa3Liberada() {
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab + " .step").attr("class", "step yellow");

    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).tab("show");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab + " .step").attr("class", "step green");

    Etapa2Aprovada();
}

function Etapa3Problema() {
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab + " .step").attr("class", "step red");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).tab("show");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab + " .step").attr("class", "step red");
    $("#" + _etapasSolicitacaoToken.Etapa3.idTab).tab("show");
    Etapa2Aprovada();
}

//*******Etapa 4*******
function Etapa4Desabilitada() {
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab + " .step").attr("class", "step");

}

function Etapa4Aguardando() {
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab).tab("show");
    Etapa3Aprovada();
}

function Etapa4Aprovada() {
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab + " .step").attr("class", "step green");
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab).tab("show");
    Etapa3Aprovada();
}

function Etapa4Reprovado() {
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab + " .step").attr("class", "step red");
    $("#" + _etapasSolicitacaoToken.Etapa4.idTab).tab("show");
    Etapa3Aprovada();
}