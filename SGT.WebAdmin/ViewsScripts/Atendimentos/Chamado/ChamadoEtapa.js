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
/// <reference path="Chamado.js" />
/// <reference path="ChamadoAnexos.js" />
/// <reference path="ChamadoRespostas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaChamado;
var _etapaAtual;

var EtapaChamado = function () {
    this.Etapa1 = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Chamado, type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid() });
    this.Etapa2 = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Anexos, type: types.local, enable: ko.observable(false), visible: ko.observable(true), idGrid: guid(), idTab: guid() });
    this.Etapa3 = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Respostas, type: types.local, enable: ko.observable(false), visible: ko.observable(true), idGrid: guid(), idTab: guid() });

    this.Anterior = PropertyEntity({ eventClick: AnteriorClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Anterior, visible: ko.observable(true), enable: ko.observable(false) });
    this.Proximo = PropertyEntity({ eventClick: ProximoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Proximo, visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadEtapaChamado() {
    _etapaChamado = new EtapaChamado();
    KoBindings(_etapaChamado, "knockoutEtapaChamado");
}

function AnteriorClick(e, sender) {
    _etapaAtual = 1;
    PosicionarEtapa();
}

function ProximoClick(e, sender) {
    _etapaAtual = 2;
    PosicionarEtapa();
}

//*******MÉTODOS*******

function PosicionarEtapa() {
    //_etapaChamado.Anterior.enable(false);
    //_etapaChamado.Proximo.enable(false);

    //if (_etapaAtual == 1) {
    //    _etapaChamado.Proximo.enable(true);
    //} //else if (_etapaAtual == 2) {
    //    _etapaChamado.Anterior.enable(false);
    //}
}