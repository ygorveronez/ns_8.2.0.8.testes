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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumStatusFaturamentoMensal.js" />
/// <reference path="../../Enumeradores/EnumEtapaFaturamentoMensal.js" />
/// <reference path="Liberacao.js" />
/// <reference path="LiberacaoPagamentoProvedor.js" />

var _etapaLiberacaoPagamentoProvedor;

var EtapaLiberacaoPagamentoProvedor = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("25%") });

    this.Etapa1 = PropertyEntity({
        text: "Documentos da Empresa Filial", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados iniciais para a geração de um pagamento provedor."),
        tooltipTitle: ko.observable("Documentos da Empresa Filial")
    });
    this.Etapa2 = PropertyEntity({
        text: "Documentos do Provedor", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Esta etapa é onde são informados os documentos do provedor."),
        tooltipTitle: ko.observable("Documentos do Provedor")
    });
    this.Etapa3 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Caso o pagamento necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa4 = PropertyEntity({
        text: "Liberação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(4),
        tooltip: ko.observable("Esta é a etapa final."),
        tooltipTitle: ko.observable("Liberação")
    });
}

//*******EVENTOS*******

function loadLiberacaoPagamentoProvedorEtapa() {
    _etapaLiberacaoPagamentoProvedor = new EtapaLiberacaoPagamentoProvedor();
    KoBindings(_etapaLiberacaoPagamentoProvedor, "knockoutEtapaLiberacaoPagamentoProvedor");

    Etapa1Aguardando();
}

function SetarEtapasPagamentoProvedor(status, situacao) {

    if (situacao == EnumSituacaoLiberacaoPagamentoProvedor.Cancelada || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
        _documentosEmpresaFilial.Cancelar.visible(false);
        _CRUDAprovacao.Cancelar.visible(false);
        _documentosProvedor.Cancelar.visible(false);
        _liberacaoPagamentoProvedor.Cancelar.visible(false);
    }
    else {
        _documentosEmpresaFilial.Cancelar.visible(true);
        _CRUDAprovacao.Cancelar.visible(true);
        _documentosProvedor.Cancelar.visible(true);
        _liberacaoPagamentoProvedor.Cancelar.visible(true);
    }

    switch (status) {
        case EnumEtapaLiberacaoPagamentoProvedor.DetalhesCarga:
            if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Cancelada) {
                Etapa1Cancelada();
            } else if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Rejeitada) {
                Etapa1Rejeitada();
            } else {
                Etapa1Aguardando();
            }
            break;

        case EnumEtapaLiberacaoPagamentoProvedor.DocumentoProvedor:
            if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Cancelada) {
                Etapa2Cancelada();
            } else if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Rejeitada) {
                Etapa2Rejeitada();
            } else {
                Etapa2Liberada();
            }
            break;

        case EnumEtapaLiberacaoPagamentoProvedor.Aprovacao:
            if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Cancelada) {
                Etapa3Cancelada();
            } else if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Rejeitada) {
                Etapa3Rejeitada();
            } else {
                Etapa3Liberada();
            }
            break;

        case EnumEtapaLiberacaoPagamentoProvedor.Liberacao:
            if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Cancelada) {
                Etapa4Cancelada();
            } else if (situacao === EnumSituacaoLiberacaoPagamentoProvedor.Rejeitada) {
                Etapa4Rejeitada();
            } else {
                Etapa4Liberada();
            }
            break;

        default: break;
    }
}

//*******MÉTODOS*******

function Etapa1Aguardando() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab).tab("show");
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
}

function Etapa1Aceita() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa2Liberada() {
    Etapa1Aceita();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).tab("show");
}

function Etapa2Aceita() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab + " .step").attr("class", "step green");
}

function Etapa3Liberada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa4Desabilitada();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab + " .step").attr("class", "step yellow");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).tab("show");
}

function Etapa3Aceita() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab + " .step").attr("class", "step green");
}

function Etapa4Liberada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa3Aceita();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab + " .step").attr("class", "step green");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab).tab("show");
}

function Etapa2Desabilitada() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab + " .step").attr("class", "step");
}

function Etapa3Desabilitada() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab + " .step").attr("class", "step");
}

function Etapa4Desabilitada() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab + " .step").attr("class", "step");
}
function Etapa1Cancelada() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab + " .step").attr("class", "step");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab).tab("show");
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
}

function Etapa1Rejeitada() {
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab + " .step").attr("class", "step red");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa1.idTab).tab("show");
    Etapa2Desabilitada();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
}

function Etapa2Cancelada() {
    Etapa1Aceita();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab + " .step").attr("class", "step");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).tab("show");
}

function Etapa2Rejeitada() {
    Etapa1Aceita();
    Etapa3Desabilitada();
    Etapa4Desabilitada();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab + " .step").attr("class", "step red");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa2.idTab).tab("show");
}

function Etapa3Cancelada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa4Desabilitada();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab + " .step").attr("class", "step");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).tab("show");
}
function Etapa3Rejeitada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa4Desabilitada();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab + " .step").attr("class", "step red");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa3.idTab).tab("show");
}

function Etapa4Cancelada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa3Aceita();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab + " .step").attr("class", "step");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab).tab("show");
}

function Etapa4Rejeitada() {
    Etapa1Aceita();
    Etapa2Aceita();
    Etapa3Aceita();
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab + " .step").attr("class", "step red");
    $("#" + _etapaLiberacaoPagamentoProvedor.Etapa4.idTab).tab("show");
}