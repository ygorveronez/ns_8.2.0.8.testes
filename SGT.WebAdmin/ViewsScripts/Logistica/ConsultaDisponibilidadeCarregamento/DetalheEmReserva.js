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



//*******MAPEAMENTO KNOUCKOUT*******
var _detalhesEmReserva;
var _gridDetalhesEmReserva;
var DetalhesEmReserva = function () {
    this.CentroCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Rota = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PrevisaoCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Detalhes = PropertyEntity({ idGrid: guid() });
    this.CargasEmReserva = PropertyEntity({ idGrid: guid() });
}



//*******EVENTOS*******

function loadDetalhesEmReserva() {
    _detalhesEmReserva = new DetalhesEmReserva();
    KoBindings(_detalhesEmReserva, "knockoutDetalhesEmReserva");

    _gridDetalhesEmReserva = new GridView(_detalhesEmReserva.Detalhes.idGrid, "ConsultaDisponibilidadeCarregamento/DetalhesEmReserva", _detalhesEmReserva);
}



//*******METODOS*******

function AbrirDetalhesEmReserva(dados) {
    _detalhesEmReserva.CentroCarregamento.val(dados.CentroCarregamento);
    _detalhesEmReserva.Rota.val(dados.Rota);
    _detalhesEmReserva.Data.val(dados.Data);
    _detalhesEmReserva.PrevisaoCarregamento.val(dados.PrevisaoCarregamento);
    _gridDetalhesEmReserva.CarregarGrid();
    Global.abrirModal("divModalDetalhesEmReserva");
}