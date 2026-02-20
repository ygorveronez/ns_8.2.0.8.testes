/// <reference path="SolicitacaoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoProvisao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaCancelamentoProvisao;

var EtapaCancelamentoProvisao = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? "50%" : "33%") });

    this.Etapa1 = PropertyEntity({
        text: "Documentos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção se documentos."),
        tooltipTitle: ko.observable("Documentos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Cancelamento", type: types.local, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? false : true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("É a etapa se visualiza o resumo do Cancelamento."),
        tooltipTitle: ko.observable("Cancelamento")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? 2 : 3),
        tooltip: ko.observable("Caso tenha integração, nessa etapa é possível acompanhar."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapasCancelamentoProvisao() {
    _etapaCancelamentoProvisao = new EtapaCancelamentoProvisao();
    KoBindings(_etapaCancelamentoProvisao, "knockoutEtapaCancelamentoProvisao");
    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaCancelamentoProvisao.Etapa1.idTab).click();
}

function SetarEtapasCancelamentoProvisao() {
    var situacao = _cancelamentoProvisao.Situacao.val();

    //if (situacao == EnumSituacaoCancelamentoProvisao.Cancelada)
    //situacao = _etapaCancelamentoProvisao.SituacaoNoCancelamento.val();

    if (situacao === EnumSituacaoCancelamentoProvisao.Todos)
        Etapa1Liberada();
    else if (situacao === EnumSituacaoCancelamentoProvisao.EmCancelamento)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoCancelamentoProvisao.NaoProcessado)
        Etapa2Reprovada();
    else if (situacao === EnumSituacaoCancelamentoProvisao.SemRegraAprovacao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoCancelamentoProvisao.AgAprovacaoSolicitacao)
        Etapa2Aguardando();
    else if (situacao === EnumSituacaoCancelamentoProvisao.SolicitacaoReprovada)
        Etapa2Reprovada();
    else if (situacao === EnumSituacaoCancelamentoProvisao.PendenciaCancelamento)
        Etapa3Reprovada();
    else if (situacao === EnumSituacaoCancelamentoProvisao.AgIntegracao)
        Etapa3Aguardando();
    else if (situacao === EnumSituacaoCancelamentoProvisao.Cancelado || situacao === EnumSituacaoCancelamentoProvisao.Estornado)
        Etapa3Aprovada();
    else if (situacao === EnumSituacaoCancelamentoProvisao.FalhaIntegracao)
        Etapa3Reprovada();
    else if (situacao === EnumSituacaoCancelamentoProvisao.EmIntegracao)
        Etapa3Aguardando();
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaCancelamentoProvisao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaCancelamentoProvisao.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab + " .step").attr("class", "step");
    _etapaCancelamentoProvisao.Etapa2.eventClick = function () { };
    Etapa3Desabilitada();
}
function Etapa2Aguardando() {
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab + " .step").attr("class", "step yellow");
    _etapaCancelamentoProvisao.Etapa2.eventClick = buscarDadosCancelamentoFechamentoProvisao;
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaCancelamentoProvisao.Etapa2.eventClick = buscarDadosCancelamentoFechamentoProvisao;
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab).click();
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab + " .step").attr("class", "step green");
    _etapaCancelamentoProvisao.Etapa2.eventClick = buscarDadosCancelamentoFechamentoProvisao;
    $("#" + _etapaCancelamentoProvisao.Etapa2.idTab).click();
    Etapa1Aprovada();
}

//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab + " .step").attr("class", "step");
    _etapaCancelamentoProvisao.Etapa3.eventClick = function () { };
}
function Etapa3Aguardando() {
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaCancelamentoProvisao.Etapa3.eventClick = CarregaIntegracao;
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab).click();
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaCancelamentoProvisao.Etapa3.eventClick = CarregaIntegracao;
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab).click();
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab + " .step").attr("class", "step green");
    _etapaCancelamentoProvisao.Etapa3.eventClick = CarregaIntegracao;
    $("#" + _etapaCancelamentoProvisao.Etapa3.idTab).click();
    Etapa2Aprovada();
}