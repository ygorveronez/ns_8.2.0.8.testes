/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoLicitacao.js" />
/// <reference path="SolicitacaoLicitacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaSolicitacaoLicitacao;

var EtapaSolicitacaoLicitacao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("50%") });

    this.Etapa1 = PropertyEntity({
        text: "Solicitação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde informa os dados para a solicitação."),
        tooltipTitle: ko.observable("Dados")
    });
    this.Etapa2 = PropertyEntity({
        text: "Cotação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Nessa etapa é onde ocorre o preenchimento dos dados da cotação."),
        tooltipTitle: ko.observable("Cotação")
    });
}


//*******EVENTOS*******

function LoadEtapasSolicitacaoLicitacao() {
    _etapaSolicitacaoLicitacao = new EtapaSolicitacaoLicitacao();
    KoBindings(_etapaSolicitacaoLicitacao, "knockoutEtapasSolicitacaoLicitacao");
    Etapa1Liberada();
    SetarEtapaInicioSolicitacaoLicitacao();
}

function SetarEtapaInicioSolicitacaoLicitacao() {
    DesabilitarTodasEtapasSolicitacaoLicitacao();
    Etapa1Liberada();
    $("#" + _etapaSolicitacaoLicitacao.Etapa1.idTab).click();
}

function SetarEtapasSolicitacaoLicitacao() {
    var situacao = _solicitacaoLicitacao.Situacao.val();

    if (situacao === EnumSituacaoSolicitacaoLicitacao.AgCotacao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoSolicitacaoLicitacao.Finalizada)
        Etapa2Aprovada();
    else if (situacao === EnumSituacaoSolicitacaoLicitacao.Cancelada)
        Etapa2Reprovada();
    else if (situacao === EnumSituacaoSolicitacaoLicitacao.Rejeitada)
        Etapa2Reprovada();
}

function DesabilitarTodasEtapasSolicitacaoLicitacao() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaSolicitacaoLicitacao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSolicitacaoLicitacao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaSolicitacaoLicitacao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSolicitacaoLicitacao.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Aprovada() {
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab + " .step").attr("class", "step yellow");
    if (!_solicitacaoLicitacao.UsuarioLogadoCriouACotacao.val())
        $("#" + _etapaSolicitacaoLicitacao.Etapa2.idTab).click();
    Etapa1Aprovada();
}