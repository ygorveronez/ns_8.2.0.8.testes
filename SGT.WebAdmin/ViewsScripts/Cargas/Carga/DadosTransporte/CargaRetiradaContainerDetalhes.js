var _gridCargaRetiradaContainerDetalhes;
var _cargaRetiradaContainerDetalhes;

var CargaRetiradaContainerDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoLocal = PropertyEntity({ val: ko.observable(), def: 0, getType: typesKnockout.decimal });
    this.CodigoTipoContainer = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Grid = PropertyEntity({
        eventClick: function (e) {
            _gridCargaRetiradaContainerDetalhes.CarregarGrid();
        }, type: types.event, idGrid: guid(), visible: ko.observable(true)
    });
};

function loadCargaRetiradaContainerDetalhes() {
    _cargaRetiradaContainerDetalhes = new CargaRetiradaContainerDetalhe();
    KoBindings(_cargaRetiradaContainerDetalhes, "knockoutCargaRetiradaContainerDetalhes");

    loadGridCargaRetiradaContainerDetalhes();
}

function loadGridCargaRetiradaContainerDetalhes() {
    _gridCargaRetiradaContainerDetalhes = new GridView(_cargaRetiradaContainerDetalhes.Grid.idGrid, "Carga/PesquisaDetalhesCargaRetiradaContainer", _cargaRetiradaContainerDetalhes);
}

function ObterDetalhesLocalRetiradaContainerModalBusca(retorno) {
    _cargaRetiradaContainerDetalhes.Codigo.val(retorno.Codigo);
    _cargaRetiradaContainerDetalhes.CodigoLocal.val(retorno.Codigo);
    _cargaRetiradaContainerDetalhes.CodigoTipoContainer.val(retorno.CodigoTipoContainer);

    Global.abrirModal('divModalInformarRetiradaContainerDetalhes');

    _gridCargaRetiradaContainerDetalhes.CarregarGrid();

    $("#divModalInformarRetiradaContainerDetalhes").one('hidden.bs.modal', function () {

    });
}