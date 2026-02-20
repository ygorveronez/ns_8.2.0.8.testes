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
var _detalhesReserva;
var _gridDetalhesReserva;
var _gridCargasDetalhesReserva;
var DetalhesReserva = function () {
    this.CentroCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PrevisaoCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Detalhes = PropertyEntity({ idGrid: guid() });
    this.CargasReserva = PropertyEntity({ idGrid: guid() });
}



//*******EVENTOS*******

function loadDetalhesReserva() {
    _detalhesReserva = new DetalhesReserva();
    KoBindings(_detalhesReserva, "knockoutDetalhesReserva");

    _gridDetalhesReserva = new GridView(_detalhesReserva.Detalhes.idGrid, "ConsultaDisponibilidadeCarregamento/DetalhesReserva", _detalhesReserva);
    _gridCargasDetalhesReserva = new GridView(_detalhesReserva.CargasReserva.idGrid, "ConsultaDisponibilidadeCarregamento/DetalhesCargasReserva", _detalhesReserva);
}



//*******METODOS*******

function AbrirDetalhesReserva(dados) {
    _detalhesReserva.CentroCarregamento.val(dados.CentroCarregamento);
    _detalhesReserva.PrevisaoCarregamento.val(dados.PrevisaoCarregamento);
    _detalhesReserva.Data.val(dados.Data);
    _gridDetalhesReserva.CarregarGrid();
    _gridCargasDetalhesReserva.CarregarGrid(function () {
        Global.abrirModal("divModalDetalhesReserva");
    });
}