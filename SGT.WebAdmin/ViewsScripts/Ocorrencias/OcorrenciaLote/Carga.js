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
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="OcorrenciaLote.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ocorrenciaLoteCarga;
var _gridSelecaoCarga;

var OcorrenciaLoteCarga = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ClicouNaPesquisa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.DataCriacaoInicial = PropertyEntity({ text: "Data Criação Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataCriacaoFinal = PropertyEntity({ text: "Data Criação Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataCriacaoInicial.dateRangeLimit = this.DataCriacaoFinal;
    this.DataCriacaoFinal.dateRangeInit = this.DataCriacaoInicial;

    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente: ", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisarCargasClick, type: types.event, text: "Pesquisar", enable: ko.observable(true) });

    this.Cargas = PropertyEntity({ idGrid: guid() });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadOcorrenciaLoteCarga() {
    _ocorrenciaLoteCarga = new OcorrenciaLoteCarga();
    KoBindings(_ocorrenciaLoteCarga, "knockoutCarga");

    new BuscarGruposPessoas(_ocorrenciaLoteCarga.GrupoPessoas);
    new BuscarTiposOperacao(_ocorrenciaLoteCarga.TipoOperacao);
    new BuscarClientes(_ocorrenciaLoteCarga.Remetente);

    BuscarCargas();
}

function PesquisarCargasClick(callback) {
    _ocorrenciaLoteCarga.ClicouNaPesquisa.val(true);
    _ocorrenciaLoteCarga.SelecionarTodos.visible(true);
    _ocorrenciaLoteCarga.SelecionarTodos.val(false);

    _gridSelecaoCarga.CarregarGrid(callback instanceof Function ? callback : null);
}

////*******MÉTODOS*******

function PreencheCargas() {
    var cargasSelecionadas;

    if (_ocorrenciaLoteCarga.SelecionarTodos.val())
        cargasSelecionadas = _gridSelecaoCarga.ObterMultiplosNaoSelecionados();
    else
        cargasSelecionadas = _gridSelecaoCarga.ObterMultiplosSelecionados();

    var codigosCargas = new Array();

    for (var i = 0; i < cargasSelecionadas.length; i++)
        codigosCargas.push(cargasSelecionadas[i].DT_RowId);

    if (codigosCargas.length > 0 || _ocorrenciaLoteCarga.SelecionarTodos.val())
        _ocorrenciaLote.ListaCargas.val(JSON.stringify(codigosCargas));
    else
        _ocorrenciaLote.ListaCargas.val("");

    _ocorrenciaLote.FiltrosCargas.val(JSON.stringify(RetornarObjetoPesquisa(_ocorrenciaLoteCarga)));
}

function BuscarCargas() {
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _ocorrenciaLoteCarga.SelecionarTodos,
        somenteLeitura: false
    };

    _gridSelecaoCarga = new GridView(_ocorrenciaLoteCarga.Cargas.idGrid, "OcorrenciaLote/PesquisaCargas", _ocorrenciaLoteCarga, null, null, null, null, null, null, multiplaescolha);
    _gridSelecaoCarga.CarregarGrid();
}

function LimparCamposOcorrenciaLoteCarga() {
    LimparCampos(_ocorrenciaLoteCarga);
    SetarEnableCamposKnockout(_ocorrenciaLoteCarga, true);
    _ocorrenciaLoteCarga.SelecionarTodos.visible(false);
    _gridSelecaoCarga.CarregarGrid();
}