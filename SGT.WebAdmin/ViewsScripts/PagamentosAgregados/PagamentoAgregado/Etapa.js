/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoAgregado.js" />
/// <reference path="PagamentoAgregado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaPagamentoAgregado;

var EtapaPagamentoAgregado = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("30%") });

    this.Etapa1 = PropertyEntity({
        text: "Pagamento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada ao preenchimento de dados para iniciar o pagamento ao agregado."),
        tooltipTitle: ko.observable("Pagamento")
    });
    this.Etapa2 = PropertyEntity({
        text: "Documento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Documentos a serem pagos ao agregado."),
        tooltipTitle: ko.observable("Documento")
    });
    this.Etapa3 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Lista de aprovadores."),
        tooltipTitle: ko.observable("Aprovação")
    });
};

//*******EVENTOS*******

function loadEtapaPagamentoAgregado() {
    _etapaPagamentoAgregado = new EtapaPagamentoAgregado();
    KoBindings(_etapaPagamentoAgregado, "knockoutEtapaPagamentoAgregado");
    Etapa1Liberada();
}

function SetarEtapaInicioLancamento() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaPagamentoAgregado.Etapa1.idTab).click();
}

function SetarEtapaDocumento() {
    DesabilitarTodasEtapas();
    SetarEtapaPagamentoAgregado();

    var situacao = _pagamento.Situacao.val();
    if (situacao == EnumSituacaoPagamentoAgregado.AgAprovacao) {
        ControleCamposPagamento(false);
        ControleCamposDocumento(false);
        _CRUDPagamento.Finalizar.visible(false);
        Etapa3Aprovada();
        $("#" + _etapaPagamentoAgregado.Etapa3.idTab).click();
    }
    else if (situacao == EnumSituacaoPagamentoAgregado.Finalizado) {
        ControleCamposPagamento(false);
        ControleCamposDocumento(false);
        _CRUDPagamento.Finalizar.visible(false);
        Etapa3Aprovada();
        $("#" + _etapaPagamentoAgregado.Etapa3.idTab).click();
    }
    else if (situacao == EnumSituacaoPagamentoAgregado.Rejeitada || situacao == EnumSituacaoPagamentoAgregado.Cancelado) {
        ControleCamposPagamento(false);
        ControleCamposDocumento(false);
        _CRUDPagamento.Finalizar.visible(false);
        Etapa3Reprovada();
        $("#" + _etapaPagamentoAgregado.Etapa3.idTab).click();
    }
    else if (situacao == EnumSituacaoPagamentoAgregado.SemRegra) {
        ControleCamposPagamento(false);
        ControleCamposDocumento(false);
        _CRUDPagamento.Finalizar.visible(false);
        Etapa3Aguardando();
        $("#" + _etapaPagamentoAgregado.Etapa3.idTab).click();
    }
    else {
        ControleCamposPagamento(true);
        ControleCamposDocumento(true);
        Etapa1Liberada();
        Etapa2Liberada();
        Etapa3Desabilitada();
        $("#" + _etapaPagamentoAgregado.Etapa2.idTab).click();
    }
}

function SetarEtapaPagamentoAgregado() {
    var situacao = _pagamento.Situacao.val();

    if (situacao == EnumSituacaoPagamentoAgregado.AgAprovacao) {
        Etapa3Aguardando();
    }
    else if (situacao == EnumSituacaoPagamentoAgregado.Finalizado)
        Etapa3Aprovada();
    else if (situacao == EnumSituacaoPagamentoAgregado.Rejeitada || situacao == EnumSituacaoPagamentoAgregado.Cancelado)
        Etapa3Reprovada();
    else if (situacao == EnumSituacaoPagamentoAgregado.SemRegra)
        Etapa3Reprovada();
    else {
        Etapa1Liberada();
        Etapa2Liberada();
    }
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaPagamentoAgregado.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa3Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaPagamentoAgregado.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaPagamentoAgregado.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaPagamentoAgregado.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa2Liberada() {
    $("#" + _etapaPagamentoAgregado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa3Desabilitada();
}

function Etapa2Aprovada() {
    $("#" + _etapaPagamentoAgregado.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa2.idTab + " .step").attr("class", "step green");
}

//*******Etapa 3*******

function Etapa3Liberada() {
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab + " .step").attr("class", "step yellow");
}

function Etapa3Desabilitada() {
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa3Aprovada() {
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
    Etapa2Aprovada();
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab).click();
}

function Etapa3Reprovada() {
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaPagamentoAgregado.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    Etapa2Aprovada();
}