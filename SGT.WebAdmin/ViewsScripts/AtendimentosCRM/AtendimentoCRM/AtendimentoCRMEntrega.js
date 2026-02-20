/// <reference path="../../Consultas/PortifolioAtendimento.js" />
/// <reference path="../../Consultas/MotivoChamado.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />

var _atendimentoEntrega;
var _confirmaEntrega;
var _entregaNaoRealizada;

function AtendimentoEntrega() {
    this.DataTentativaEntrega = PropertyEntity({ text: "Data agendamento:", required: true, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarTentativaEntrega, text: "Salvar", visible: ko.observable(true) });
}

function ConfirmaEntrega() {
    this.DataEntrega = PropertyEntity({ text: "Data da entrega:", required: true, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.FinalizarEntrega = PropertyEntity({ type: types.event, eventClick: finalizar, text: "Sim", visible: ko.observable(true) });
    this.NaoFinalizar = PropertyEntity({ type: types.event, eventClick: voltar, text: "N\u00E3o", visible: ko.observable(true) });
}

function EntregaNaoRealizada() {
    this.DataFinalizacaoEntrega = PropertyEntity({ text: "Data da finaliza\u00e7\u00E3o:", required: true, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.finalizar = PropertyEntity({ type: types.event, eventClick: entregaNaoRealizadaFinalizar, text: "Sim", visible: ko.observable(true) });
    this.NaoFinalizar = PropertyEntity({ type: types.event, eventClick: voltar, text: "N\u00E3o", visible: ko.observable(true) });
}

function AdicionarTentativaEntrega(){
    console.log("tem que implementar");
}

function finalizar() {
    console.log("necessario implementar")
}

function voltar() {
    console.log("implementar")
}

function entregaNaoRealizadaFinalizar() {
    console.log("implementar")
}

function loadAtendimentoCRMEntrega(divId) {
    _atendimentoEntrega = new AtendimentoEntrega();
    KoBindings(_atendimentoEntrega, "knockoutTentativaEntregaCRM");


    _confirmaEntrega = new ConfirmaEntrega();
    KoBindings(_confirmaEntrega, "knockoutConfirmaEntregaCRM");

    _entregaNaoRealizada = new EntregaNaoRealizada();
    KoBindings(_entregaNaoRealizada, "knockoutEntregaNaoRealizada");

    Global.abrirModal(divId);
}

function carregarModalAtendimentoCrmHTML(callback, divId) {
    $.get("Content/Static/AtendimentoCRM/AtendimentoCRMModais.html?dyn=" + guid(), function (data) {
        $("#ModaisAtendimentoCRM").html(data);
        callback(divId);
    });
}


