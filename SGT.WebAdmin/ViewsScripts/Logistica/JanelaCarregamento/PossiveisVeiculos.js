
/// <reference path="TipoTransportadorCarga.js" />
/// <reference path="CalendarioCarregamento.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../../js/plugin/jquery-countdown/jquery.countdown.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />

var _cargaVeiculosDisponiveis;
var _gridCargaVeiculosDisponiveis;

var CargaVeiculosDisponiveis = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: true, text: Localization.Resources.Cargas.Carga.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.Placa = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Placa.getFieldDescription(), maxlength: 7 });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ModeloVeicularDeCarga.getFieldDescription(), required: true, idBtnSearch: guid() });
    this.AcertoViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.AcertoDeViagem.getFieldDescription(), required: false, idBtnSearch: guid(), visible: false });
    this.NumeroFrota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroFrota.getFieldDescription(), maxlength: 30 });
    this.TipoCarga = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0), required: false });
    this.SomenteDisponveis = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.SomenteEmpresasAtivas = PropertyEntity({ visible: false, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0), required: false });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            GridConsulta.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true, idGrid: guid()
    });
}

function LoadCargaVeiculosDisponiveis() {
    _cargaVeiculosDisponiveis = new CargaVeiculosDisponiveis();
    KoBindings(_cargaVeiculosDisponiveis, "modalVeiculosDisponiveis", false);
    _gridCargaVeiculosDisponiveis = new GridView(_cargaVeiculosDisponiveis.Pesquisar.idGrid, "Veiculo/Pesquisa", _cargaVeiculosDisponiveis, null, null);
}

function visualizarVeiculosDisponviveisClick(carga) {
    _cargaVeiculosDisponiveis.TipoCarga.codEntity(carga.TipoCarga.Codigo);
    _cargaVeiculosDisponiveis.TipoCarga.val(carga.TipoCarga.Codigo);
    _cargaVeiculosDisponiveis.ModeloVeicularCarga.codEntity(carga.ModeloVeiculo.Codigo);
    _cargaVeiculosDisponiveis.ModeloVeicularCarga.val(carga.ModeloVeiculo.Codigo);
    _cargaVeiculosDisponiveis.CentroCarregamento.codEntity(carga.CentroCarregamento.Codigo);
    _cargaVeiculosDisponiveis.CentroCarregamento.val(carga.CentroCarregamento.Codigo);
    _gridCargaVeiculosDisponiveis.CarregarGrid(function () {
        Global.abrirModal("modalVeiculosDisponiveis");
    });
}