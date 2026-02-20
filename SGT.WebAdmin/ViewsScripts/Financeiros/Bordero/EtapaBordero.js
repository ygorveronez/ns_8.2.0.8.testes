//*******MAPEAMENTO KNOUCKOUT*******

var _etapaBordero;

var EtapaBordero = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33.33%") });

    this.Etapa1 = PropertyEntity({
        text: "Borderô", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se inicia um borderô de títulos a receber."),
        tooltipTitle: ko.observable("Borderô")
    });
    this.Etapa2 = PropertyEntity({
        text: "Títulos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa de seleção dos títulos do borderô."),
        tooltipTitle: ko.observable("Títulos")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("É a etapa responsável pela geração de arquivos para integração com outros sistemas."),
        tooltipTitle: ko.observable("Integração")
    });
}

//*******EVENTOS*******

function LoadEtapaBordero() {
    _etapaBordero = new EtapaBordero();
    KoBindings(_etapaBordero, "knockoutEtapaBordero");
    Etapa1Liberada();
}

function SetarEtapaBordero() {
    var situacaoBordero = _bordero.Situacao.val();

    if (situacaoBordero == EnumSituacaoBordero.EmAndamento) {
        if (_bordero.Codigo.val() == 0) {
            Etapa1Liberada();
            $("#" + _etapaBordero.Etapa1.idTab).click();
        } else {
            Etapa2Liberada();
            $('#' + _etapaBordero.Etapa2.idTab).click();
        }
    } else if (situacaoBordero == EnumSituacaoBordero.AgIntegracao) {
        Etapa3Aguardando();
        $('#' + _etapaBordero.Etapa3.idTab).click();
    } else if (situacaoBordero == EnumSituacaoBordero.IntegracaoRejeitada) {
        Etapa3Reprovada();
        $('#' + _etapaBordero.Etapa3.idTab).click();
    } else if (situacaoBordero == EnumSituacaoBordero.Quitado || situacaoBordero == EnumSituacaoBordero.Finalizado || situacaoBordero == EnumSituacaoBordero.Cancelado) {
        Etapa3Aprovada();
        $("#" + _etapaBordero.Etapa1.idTab).click();
    }
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaBordero.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaBordero.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    _etapaBordero.Etapa2.eventClick = function () { };
    $("#" + _etapaBordero.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaBordero.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Liberada() {
    _etapaBordero.Etapa2.eventClick = ExibirTitulosBordero;
    $("#" + _etapaBordero.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    _etapaBordero.Etapa2.eventClick = ExibirTitulosBordero;
    $("#" + _etapaBordero.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    _etapaBordero.Etapa2.eventClick = ExibirTitulosBordero;
    $("#" + _etapaBordero.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    _etapaBordero.Etapa2.eventClick = ExibirTitulosBordero;
    $("#" + _etapaBordero.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    _etapaBordero.Etapa3.eventClick = function () { };
    $("#" + _etapaBordero.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaBordero.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Liberada() {
    _etapaBordero.Etapa3.eventClick = function () { };
    $("#" + _etapaBordero.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaBordero.Etapa3.eventClick = function () { };
    $("#" + _etapaBordero.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaBordero.Etapa3.eventClick = function () { };
    $("#" + _etapaBordero.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    _etapaBordero.Etapa3.eventClick = function () { };
    $("#" + _etapaBordero.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaBordero.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}