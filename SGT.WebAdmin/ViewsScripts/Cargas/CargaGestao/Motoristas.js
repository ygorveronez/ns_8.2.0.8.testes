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
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="CargaGestao.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="Veiculos.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridMotorista;
var _pesquisaMotorista;

var PesquisaMotorista = function () {
    this.Nome = PropertyEntity({ text: "Nome: " });
    this.CPF = PropertyEntity({ text: "CPF: ", maxlength: 14, getType: typesKnockout.cpf });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotorista.CarregarGrid();
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

function loadMotoristas() {
    _pesquisaMotorista = new PesquisaMotorista();
    KoBindings(_pesquisaMotorista, "knockoutPesquisaMotoristas", false, _pesquisaMotorista.Pesquisar.id);
    buscarMotoristas();
}


//*******MÉTODOS*******

function buscarMotoristas() {
    _gridMotorista = new GridView(_pesquisaMotorista.Pesquisar.idGrid, "Motorista/PesquisaMotoristaGestaoCarga", _pesquisaMotorista, null, null, null, null, false, true);
    _gridMotorista.CarregarGrid();
}

