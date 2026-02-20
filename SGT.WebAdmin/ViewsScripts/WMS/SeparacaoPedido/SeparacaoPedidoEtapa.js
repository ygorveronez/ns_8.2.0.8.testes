/// <reference path="../../Enumeradores/EnumSituacaoSeparacaoPedido.js" />
/// <reference path="SeparacaoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaSeparacaoPedido;

var EtapaSeparacaoPedido = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Separação de Pedidos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde seleciona os pedidos para a Integração."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Informações", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa os dados da separação devem ser informados."),
        tooltipTitle: ko.observable("Informações")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa a integração é feita."),
        tooltipTitle: ko.observable("Integração")
    });
};


//*******EVENTOS*******

function LoadEtapasSeparacaoPedido() {
    _etapaSeparacaoPedido = new EtapaSeparacaoPedido();
    KoBindings(_etapaSeparacaoPedido, "knockoutEtapasSeparacaoPedido");
    Etapa1Liberada();
    SetarEtapaInicioSeparacaoPedido();
}

function SetarEtapaInicioSeparacaoPedido() {
    DesabilitarTodasEtapasSeparacaoPedido();
    Etapa1Liberada();
    $("#" + _etapaSeparacaoPedido.Etapa1.idTab).click();
    $("#" + _etapaSeparacaoPedido.Etapa1.idTab).tab("show");
}

function SetarEtapasSeparacaoPedido() {
    var situacao = _separacaoPedido.Situacao.val();

    if (situacao === EnumSituacaoSeparacaoPedido.Aberto) {
        Etapa2Aguardando();
        $("#" + _etapaSeparacaoPedido.Etapa2.idTab).click();
        $("#" + _etapaSeparacaoPedido.Etapa2.idTab).tab("show");
    }
    else if (situacao === EnumSituacaoSeparacaoPedido.AguardandoIntegracao) {
        Etapa3Aguardando();
        $("#" + _etapaSeparacaoPedido.Etapa3.idTab).click();
        $("#" + _etapaSeparacaoPedido.Etapa3.idTab).tab("show");
    }
    else if (situacao === EnumSituacaoSeparacaoPedido.Finalizada) {
        Etapa3Aprovada();
        $("#" + _etapaSeparacaoPedido.Etapa3.idTab).click();
        $("#" + _etapaSeparacaoPedido.Etapa3.idTab).tab("show");
    }
    else if (situacao === EnumSituacaoSeparacaoPedido.IntegracaoRejeitada) {
        Etapa3Reprovada();
        $("#" + _etapaSeparacaoPedido.Etapa3.idTab).click();
        $("#" + _etapaSeparacaoPedido.Etapa3.idTab).tab("show");
    }
        
}

function DesabilitarTodasEtapasSeparacaoPedido() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaSeparacaoPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaSeparacaoPedido.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada();
}

function Etapa2Aprovada() {
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab + " .step").attr("class", "step green");

    _separacaoInformacoes.SelecionarNotas.enable(false);
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa2.idTab + " .step").attr("class", "step yellow");
    _separacaoInformacoes.SelecionarNotas.enable(true);
    Etapa1Aprovada();
}


//*******Etapa 2*******

function Etapa3Desabilitada() {
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Aprovada() {
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
    _separacaoInformacoes.SelecionarNotas.enable(false);
}

function Etapa3Reprovada() {
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
    _separacaoInformacoes.SelecionarNotas.enable(false);
}

function Etapa3Aguardando() {
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSeparacaoPedido.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}