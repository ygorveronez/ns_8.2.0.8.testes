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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMarcaProduto;
var _marcaProduto;
var _pesquisaMarcaProduto;

var PesquisaMarcaProduto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMarcaProduto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var MarcaProduto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", maxlength: 50, issue: 15 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDMarcaProduto = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadMarcaProduto() {
    _marcaProduto = new MarcaProduto();
    KoBindings(_marcaProduto, "knockoutCadastroMarcaProduto");

    HeaderAuditoria("MarcaProduto", _marcaProduto);

    _crudMarcaProduto = new CRUDMarcaProduto();
    KoBindings(_crudMarcaProduto, "knockoutCRUDMarcaProduto");

    _pesquisaMarcaProduto = new PesquisaMarcaProduto();
    KoBindings(_pesquisaMarcaProduto, "knockoutPesquisaMarcaProduto", false, _pesquisaMarcaProduto.Pesquisar.id);

    buscarMarcaProduto();
}

function adicionarClick(e, sender) {
    Salvar(_marcaProduto, "MarcaProduto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridMarcaProduto.CarregarGrid();
                limparCamposMarcaProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_marcaProduto, "MarcaProduto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMarcaProduto.CarregarGrid();
                limparCamposMarcaProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Marca " + _marcaProduto.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_marcaProduto, "MarcaProduto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMarcaProduto.CarregarGrid();
                    limparCamposMarcaProduto();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposMarcaProduto();
}

//*******MÉTODOS*******

function buscarMarcaProduto() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMarcaProduto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMarcaProduto = new GridView(_pesquisaMarcaProduto.Pesquisar.idGrid, "MarcaProduto/Pesquisa", _pesquisaMarcaProduto, menuOpcoes, null);
    _gridMarcaProduto.CarregarGrid();
}

function editarMarcaProduto(marcaProdutoGrid) {
    limparCamposMarcaProduto();
    _marcaProduto.Codigo.val(marcaProdutoGrid.Codigo);
    BuscarPorCodigo(_marcaProduto, "MarcaProduto/BuscarPorCodigo", function (arg) {
        _pesquisaMarcaProduto.ExibirFiltros.visibleFade(false);
        _crudMarcaProduto.Atualizar.visible(true);
        _crudMarcaProduto.Cancelar.visible(true);
        _crudMarcaProduto.Excluir.visible(true);
        _crudMarcaProduto.Adicionar.visible(false);
    }, null);
}

function limparCamposMarcaProduto() {
    _crudMarcaProduto.Atualizar.visible(false);
    _crudMarcaProduto.Cancelar.visible(false);
    _crudMarcaProduto.Excluir.visible(false);
    _crudMarcaProduto.Adicionar.visible(true);
    LimparCampos(_marcaProduto);
}