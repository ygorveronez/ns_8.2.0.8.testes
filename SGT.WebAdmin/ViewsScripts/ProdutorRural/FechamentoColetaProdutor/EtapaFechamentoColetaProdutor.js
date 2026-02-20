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
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoColetaProdutor.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaFechamentoColetaProdutor;

var EtapaFechamentoColetaProdutor = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("33%") });

    this.Etapa1 = PropertyEntity({
        text: "Pedidos", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("Esta etapa é destinada à seleção dos pedidos."),
        tooltipTitle: ko.observable("Pedidos")
    });
    this.Etapa2 = PropertyEntity({
        text: "Carga", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Carga Gerada para o Fechamento."),
        tooltipTitle: ko.observable("Integração")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("Caso tenha integração, nessa etapa é possível acompanhar."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapasFechamentoColetaProdutor() {
    _etapaFechamentoColetaProdutor = new EtapaFechamentoColetaProdutor();
    KoBindings(_etapaFechamentoColetaProdutor, "knockoutEtapaFechamentoColetaProdutor");
    Etapa1Liberada();
}

function SetarEtapaInicio() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();
    $("#" + _etapaFechamentoColetaProdutor.Etapa1.idTab).click();
}

function SetarEtapasFechamentoColetaProdutor() {
    var situacao = _fechamentoColetaProdutor.Situacao.val();

    var cancelado = false;
    if (situacao == EnumSituacaoFechamentoColetaProdutor.Cancelado) {
        cancelado = true;
        situacao = _fechamentoColetaProdutor.SituacaoNoCancelamento.val();
    }
        

    if (situacao == EnumSituacaoFechamentoColetaProdutor.Todos) {
        _selecaoPedidos.Criar.visible(true);
        _selecaoPedidos.GerarCarga.visible(false);
        _selecaoPedidos.ReprocessarFrete.visible(true);
        _selecaoPedidos.Cancelar.visible(false);
        Etapa1Liberada();
    }
    else if (situacao == EnumSituacaoFechamentoColetaProdutor.EmCriacao) {
        visibilidadeBotoesCriado();
        _selecaoPedidos.GerarCarga.visible(true);
        Etapa1Liberada();
    }
    else if (situacao == EnumSituacaoFechamentoColetaProdutor.AgEmissaoCarga) {
        Etapa2Aguardando();
        visibilidadeBotoesCriado();
    }
    else if (situacao == EnumSituacaoFechamentoColetaProdutor.Finalizado) {
        Etapa3Aprovada();
        visibilidadeBotoesCriado();
    }

    if (cancelado) {
        visibilidadeBotoesCriado();
        _selecaoPedidos.Cancelar.visible(false);
    }
        
}

function visibilidadeBotoesCriado() {
    _selecaoPedidos.Criar.visible(false);
    _selecaoPedidos.GerarCarga.visible(false);
    _selecaoPedidos.ReprocessarFrete.visible(false);
    _selecaoPedidos.Cancelar.visible(true);
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
}

//*******Etapa 1*******

function SetarEtapaInicioFechamento() {
    DesabilitarTodasEtapasFechamento();
    Etapa1FechamentoLiberada();
    $("#" + _etapaFechamentoColetaProdutor.Etapa1.idTab).click();
}

function Etapa1Liberada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa1.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa1.idTab + " .step").attr("class", "step green");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab + " .step").attr("class", "step");
    _etapaFechamentoColetaProdutor.Etapa2.eventClick = function () { };
    Etapa3Desabilitada();
}
function Etapa2Aguardando() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab + " .step").attr("class", "step yellow");
    _etapaFechamentoColetaProdutor.Etapa2.eventClick = buscarCargaFechamentoProdutor;
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab + " .step").attr("class", "step red");
    _etapaFechamentoColetaProdutor.Etapa2.eventClick = buscarCargaFechamentoProdutor;
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa2.idTab + " .step").attr("class", "step green");
    _etapaFechamentoColetaProdutor.Etapa2.eventClick = buscarCargaFechamentoProdutor;
    Etapa1Aprovada();
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab).removeAttr("data-bs-toggle");
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab + " .step").attr("class", "step");
    _etapaFechamentoColetaProdutor.Etapa3.eventClick = function () { };
}
function Etapa3Aguardando() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab + " .step").attr("class", "step yellow");
    _etapaFechamentoColetaProdutor.Etapa3.eventClick = BuscarDadosIntegracoesFechamentoColetaProdutor;
    Etapa2Aprovada();
}

function Etapa3Reprovada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab + " .step").attr("class", "step red");
    _etapaFechamentoColetaProdutor.Etapa3.eventClick = BuscarDadosIntegracoesFechamentoColetaProdutor;
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab).attr("data-bs-toggle", "tab");
    $("#" + _etapaFechamentoColetaProdutor.Etapa3.idTab + " .step").attr("class", "step green");
    _etapaFechamentoColetaProdutor.Etapa3.eventClick = BuscarDadosIntegracoesFechamentoColetaProdutor;
    Etapa2Aprovada();
}

function BuscarDadosIntegracoesFechamentoColetaProdutor() { }