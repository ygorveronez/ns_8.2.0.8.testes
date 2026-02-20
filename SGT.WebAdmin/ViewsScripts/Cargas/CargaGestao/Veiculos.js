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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="CargaGestao.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="Motoristas.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridVeiculo;
var _pesquisaVeiculo;

var PesquisaVeiculo = function () {
    this.Placa = PropertyEntity({ text: "Placa: ", maxlength: 7 });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridVeiculo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******

function loadVeiculos() {
    _pesquisaVeiculo = new PesquisaVeiculo();
    KoBindings(_pesquisaVeiculo, "knockoutPesquisaVeiculo", false, _pesquisaVeiculo.Pesquisar.id);
    $("#" + _pesquisaVeiculo.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });
    new BuscarMotoristas(_pesquisaVeiculo.Motorista);
    buscarVeiculos();
}


//*******MÉTODOS*******

function buscarVeiculos() {
    _gridVeiculo = new GridView(_pesquisaVeiculo.Pesquisar.idGrid, "Veiculo/PesquisaVeiculoGestaoCarga", _pesquisaVeiculo, null, null, null, null, false, true);
    _gridVeiculo.CarregarGrid();
}

