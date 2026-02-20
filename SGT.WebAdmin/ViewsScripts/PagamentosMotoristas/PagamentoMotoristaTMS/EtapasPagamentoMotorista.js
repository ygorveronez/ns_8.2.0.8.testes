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
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoMotorista.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="PagamentoMotorista.js" />
/// <reference path="CTeComplementar.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaPagamentoMotorista;

var EtapaPagamentoMotorista = function () {
    this.TamanhoEtapa = PropertyEntity({ type: types.local, val: ko.observable("30%") });

    this.Etapa1 = PropertyEntity({
        text: "Pagamento Motorista", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable("É onde se informar os dados iniciais para a geração de um pagamento ao motorista."),
        tooltipTitle: ko.observable("Pagamento Motorista")
    });
    this.Etapa2 = PropertyEntity({
        text: "Aprovação", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(2),
        tooltip: ko.observable("Caso o pagamento necessite de aprovação ela será realizada nessa etapa."),
        tooltipTitle: ko.observable("Aprovação")
    });
    this.Etapa3 = PropertyEntity({
        text: "Integração", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(3),
        tooltip: ko.observable("A etapa 3 é responsável pela geração de arquivos para integração com outros sistemas."),
        tooltipTitle: ko.observable("Integração")
    });
}


//*******EVENTOS*******

function loadEtapaPagamentoMotorista() {
    _etapaPagamentoMotorista = new EtapaPagamentoMotorista();
    KoBindings(_etapaPagamentoMotorista, "knockoutEtapaPagamentoMotorista");
    Etapa1Liberada();
}

function setarEtapaInicioPagamentoMotorista() {
    DesabilitarTodasEtapas();
    Etapa1Liberada();

    Global.ExibirStep(_etapaPagamentoMotorista.Etapa1.idTab);
}

function setarEtapasPagamentoMotorista() {
    var situacaoPagamentoMotorista = _pagamentoMotorista.Situacao.val();
    var ocorrenciaCancelada = false;

    if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.Cancelada) {
        ocorrenciaCancelada = true;
    }

    //if (situacaoPagamentoMotorista != EnumSituacaoPagamentoMotorista.Finalizada) {
    //    _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(false);
    //}

    if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.Finalizada) {
        _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(true);
        _CRUDPagamentoMotorista.Reverter.visible(false);
    } else {
        _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(false);
    }

    if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.AgAprovacao)
        Etapa2Aguardando();
    else if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.AutorizacaoPendente)
        Etapa2Aguardando();
    else if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.Rejeitada)
        Etapa2Reprovada();
    else if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.AgIntegracao)
        Etapa3Aguardando();
    else if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.FalhaIntegracao)
        Etapa3Problema();
    else if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.Finalizada || situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.FinalizadoPagamento)
        Etapa3Aprovada();
    else if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.AgInformacoes)
        Etapa1Aguardando();
    else if (situacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.SemRegraAprovacao)
        Etapa2SemRegra(ocorrenciaCancelada);
}

function DesabilitarTodasEtapas() {
    Etapa2Desabilitada();
    Etapa3Desabilitada();
}


function EtapaSemRegra(ocorrenciaCancelada) {
    /* Quando a ocorrência esta cancelada, a situação para a etapa é a mesma de que quando ela foi cancelada
     * Mas para evitar que apareca mensagens de aviso de SEM REGRA e que apareca o botão de reprocessar regra
     * A flag de ocorrencia cancelada é enviada
     */
    if (!ocorrenciaCancelada) {
        exibirMensagem(tipoMensagem.aviso, "Regras da etapa", "Nenhuma regra encontrada. Ocorrência permanece aguardando autorização.");
        _CRUDPagamentoMotorista.ReprocessarRegras.visible(true);
    }

    _resumoPagamentoMotorista.MensagemEtapaSemRegra.visible(true);
}

//*******Etapa 1*******

function Etapa1Liberada() {
    $("#" + _etapaPagamentoMotorista.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa1.idTab + " .step").attr("class", "step yellow");
    Etapa2Desabilitada();
}

function Etapa1Aprovada() {
    $("#" + _etapaPagamentoMotorista.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa1.idTab + " .step").attr("class", "step green");
}

function Etapa1Aguardando() {
    $("#" + _etapaPagamentoMotorista.Etapa1.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa1.idTab + " .step").attr("class", "step yellow");
}

//*******Etapa 2*******

function Etapa2Desabilitada() {
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab).prop("disabled", true);
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab + " .step").attr("class", "step");
    Etapa3Desabilitada()
}

function Etapa2Liberada() {
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aguardando() {
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab + " .step").attr("class", "step yellow");
    Etapa1Aprovada();
}

function Etapa2Aprovada() {
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab + " .step").attr("class", "step green");
    Etapa1Aprovada();
}

function Etapa2Reprovada() {
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
}

function Etapa2SemRegra(ocorrenciaCancelada) {
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa2.idTab + " .step").attr("class", "step red");
    Etapa1Aprovada();
    EtapaSemRegra(ocorrenciaCancelada);
}


//*******Etapa 3*******

function Etapa3Desabilitada() {
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab).prop("disabled", true);
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab + " .step").attr("class", "step");
    _etapaPagamentoMotorista.Etapa3.eventClick = function () { };
}

function Etapa3Liberada() {
    _etapaPagamentoMotorista.Etapa3.eventClick = BuscarDadosIntegracoesPagamentoMotorista;
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aguardando() {
    _etapaPagamentoMotorista.Etapa3.eventClick = BuscarDadosIntegracoesPagamentoMotorista;
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab + " .step").attr("class", "step yellow");
    Etapa2Aprovada();
}

function Etapa3Aprovada() {
    _etapaPagamentoMotorista.Etapa3.eventClick = BuscarDadosIntegracoesPagamentoMotorista;
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab + " .step").attr("class", "step green");
    Etapa2Aprovada();
}

function Etapa3Problema() {
    _etapaPagamentoMotorista.Etapa3.eventClick = BuscarDadosIntegracoesPagamentoMotorista;
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab).prop("disabled", false);
    $("#" + _etapaPagamentoMotorista.Etapa3.idTab + " .step").attr("class", "step red");
    Etapa2Aprovada();
}