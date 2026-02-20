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
/// <reference path="../../Consultas/ModeloVeiculo.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaMediaModeloPeso;
var _tabelaMediaModeloPeso;
var _pesquisaTabelaMediaModeloPeso;

var PesquisaTabelaMediaModeloPeso = function () {
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaMediaModeloPeso.CarregarGrid();
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

var TabelaMediaModeloPeso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.MediaIdeal = PropertyEntity({ text: "*Media Ideal:", getType: typesKnockout.decimal, required: true });
    this.PesoInicial = PropertyEntity({ text: "*Peso Inicial:", getType: typesKnockout.decimal, required: true });
    this.PesoFinal = PropertyEntity({ text: "*Peso Final:", getType: typesKnockout.decimal, required: true });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
}

var CRUDTabelaMediaModeloPeso = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadTabelaMediaModeloPeso() {
    _tabelaMediaModeloPeso = new TabelaMediaModeloPeso();
    KoBindings(_tabelaMediaModeloPeso, "knockoutCadastroTabelaMediaModeloPeso");

    HeaderAuditoria("TabelaMediaModeloPeso", _tabelaMediaModeloPeso);

    _crudTabelaMediaModeloPeso = new CRUDTabelaMediaModeloPeso();
    KoBindings(_crudTabelaMediaModeloPeso, "knockoutCRUDTabelaMediaModeloPeso");

    _pesquisaTabelaMediaModeloPeso = new PesquisaTabelaMediaModeloPeso();
    KoBindings(_pesquisaTabelaMediaModeloPeso, "knockoutPesquisaTabelaMediaModeloPeso", false, _pesquisaTabelaMediaModeloPeso.Pesquisar.id);

    new BuscarModelosVeiculo(_tabelaMediaModeloPeso.Modelo);
    new BuscarModelosVeiculo(_pesquisaTabelaMediaModeloPeso.Modelo);

    buscarTabelaMediaModeloPeso();
}

function adicionarClick(e, sender) {
    Salvar(_tabelaMediaModeloPeso, "TabelaMediaModeloPeso/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTabelaMediaModeloPeso.CarregarGrid();
                limparCamposTabelaMediaModeloPeso();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tabelaMediaModeloPeso, "TabelaMediaModeloPeso/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaMediaModeloPeso.CarregarGrid();
                limparCamposTabelaMediaModeloPeso();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a informação da folha?", function () {
        ExcluirPorCodigo(_tabelaMediaModeloPeso, "TabelaMediaModeloPeso/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTabelaMediaModeloPeso.CarregarGrid();
                limparCamposTabelaMediaModeloPeso();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTabelaMediaModeloPeso();
}

//*******MÉTODOS*******


function buscarTabelaMediaModeloPeso() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaMediaModeloPeso, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaMediaModeloPeso = new GridView(_pesquisaTabelaMediaModeloPeso.Pesquisar.idGrid, "TabelaMediaModeloPeso/Pesquisa", _pesquisaTabelaMediaModeloPeso, menuOpcoes, null);
    _gridTabelaMediaModeloPeso.CarregarGrid();
}

function editarTabelaMediaModeloPeso(tabelaMediaModeloPesoGrid) {
    limparCamposTabelaMediaModeloPeso();
    _tabelaMediaModeloPeso.Codigo.val(tabelaMediaModeloPesoGrid.Codigo);
    BuscarPorCodigo(_tabelaMediaModeloPeso, "TabelaMediaModeloPeso/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaMediaModeloPeso.ExibirFiltros.visibleFade(false);
        _crudTabelaMediaModeloPeso.Atualizar.visible(true);
        _crudTabelaMediaModeloPeso.Cancelar.visible(true);
        _crudTabelaMediaModeloPeso.Excluir.visible(true);
        _crudTabelaMediaModeloPeso.Adicionar.visible(false);
    }, null);
}

function limparCamposTabelaMediaModeloPeso() {
    _crudTabelaMediaModeloPeso.Atualizar.visible(false);
    _crudTabelaMediaModeloPeso.Cancelar.visible(false);
    _crudTabelaMediaModeloPeso.Excluir.visible(false);
    _crudTabelaMediaModeloPeso.Adicionar.visible(true);
    LimparCampos(_tabelaMediaModeloPeso);
}