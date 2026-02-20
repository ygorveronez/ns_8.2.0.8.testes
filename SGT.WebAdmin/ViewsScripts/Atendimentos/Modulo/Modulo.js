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
/// <reference path="../../Consultas/Sistema.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridModulo;
var _modulo;
var _pesquisaModulo;

var PesquisaModulo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Sistema:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridModulo.CarregarGrid();
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

var Modulo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 300 });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Sistema:", idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadModulo() {
    _modulo = new Modulo();
    KoBindings(_modulo, "knockoutCadastroModulo");

    _pesquisaModulo = new PesquisaModulo();
    KoBindings(_pesquisaModulo, "knockoutPesquisaModulo", false, _pesquisaModulo.Pesquisar.id);

    new BuscarSistemas(_pesquisaModulo.Sistema);
    new BuscarSistemas(_modulo.Sistema);

    buscarModulos();
}

function adicionarClick(e, sender) {
    Salvar(e, "Modulo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridModulo.CarregarGrid();
                limparCamposModulo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "Modulo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridModulo.CarregarGrid();
                limparCamposModulo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Módulo " + _modulo.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_modulo, "Modulo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridModulo.CarregarGrid();
                limparCamposModulo();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposModulo();
}

//*******MÉTODOS*******

function buscarModulos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarModulo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridModulo = new GridView(_pesquisaModulo.Pesquisar.idGrid, "Modulo/Pesquisa", _pesquisaModulo, menuOpcoes, null);
    _gridModulo.CarregarGrid();
}

function editarModulo(moduloGrid) {
    limparCamposModulo();
    _modulo.Codigo.val(moduloGrid.Codigo);
    BuscarPorCodigo(_modulo, "Modulo/BuscarPorCodigo", function (arg) {
        _pesquisaModulo.ExibirFiltros.visibleFade(false);
        _modulo.Atualizar.visible(true);
        _modulo.Cancelar.visible(true);
        _modulo.Excluir.visible(true);
        _modulo.Adicionar.visible(false);
    }, null);
}

function limparCamposModulo() {
    _modulo.Atualizar.visible(false);
    _modulo.Cancelar.visible(false);
    _modulo.Excluir.visible(false);
    _modulo.Adicionar.visible(true);
    LimparCampos(_modulo);
}
