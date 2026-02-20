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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="LiberacaoPagamentoEtapa.js" />
/// <reference path="LiberacaoPagamentoProvedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _liberacaoPagamentoProvedor;

var LiberacaoPagamentoProvedor = function () {
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaLiberacaoPagamentoProvedor.Liberacao), def: EnumEtapaLiberacaoPagamentoProvedor.Liberacao, getType: typesKnockout.int });
    this.LiberacaoPagamentoProvedor = PropertyEntity({ text: "Finalizado liberação do pagamento ao provedor." });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadLiberacao() {
    _liberacaoPagamentoProvedor = new LiberacaoPagamentoProvedor();
    KoBindings(_liberacaoPagamentoProvedor, "knockoutLiberacaoPagamentoProvedor");
}