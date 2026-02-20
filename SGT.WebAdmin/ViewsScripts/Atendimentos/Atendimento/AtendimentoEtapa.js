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
/// <reference path="Atendimento.js" />
/// <reference path="AtendimentoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _etapaAtendimento;
var _etapaAtual;

var EtapaAtendimento = function () {
    this.Etapa1 = PropertyEntity({ text: "Atendimento", type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid() });
    this.Etapa2 = PropertyEntity({ text: "Chamados", type: types.local, enable: ko.observable(false), visible: ko.observable(true), idGrid: guid(), idTab: guid() });

    this.Anterior = PropertyEntity({ eventClick: AnteriorClick, type: types.event, text: "Anterior", visible: ko.observable(true), enable: ko.observable(false) });
    this.Proximo = PropertyEntity({ eventClick: ProximoClick, type: types.event, text: "Próximo", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadEtapaAtendimento() {
    _etapaAtendimento = new EtapaAtendimento();
    KoBindings(_etapaAtendimento, "knockoutEtapaAtendimento");
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
    //_etapaAtendimento.Anterior.enable(false);
    //_etapaAtendimento.Proximo.enable(false);

    //if (_etapaAtual == 1) {
    //    _etapaAtendimento.Proximo.enable(true);
    //} //else if (_etapaAtual == 2) {
    //    _etapaAtendimento.Anterior.enable(false);
    //}
}