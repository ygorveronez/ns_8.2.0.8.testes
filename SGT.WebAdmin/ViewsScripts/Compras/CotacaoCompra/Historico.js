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

var _historico;
var _gridHistorico;
var _modalHistoricoCompraCotacaoCompra;

var HistoricoCotacaoCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Produto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    this.Historico = PropertyEntity({ type: types.local });

    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharHistoricoClick, text: "Fechar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadHistoricoCotacaoCompra() {
    _historico = new HistoricoCotacaoCompra();
    KoBindings(_historico, "knoutHistoricoCompra");

    _modalHistoricoCompraCotacaoCompra = new bootstrap.Modal(document.getElementById("divHistoricoCompra"), { backdrop: 'static', keyboard: true });
}

function FecharHistoricoClick(e, sender) {
    _modalHistoricoCompraCotacaoCompra.hide();
}