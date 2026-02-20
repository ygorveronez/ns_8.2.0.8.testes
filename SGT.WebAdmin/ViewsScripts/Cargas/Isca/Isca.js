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

var _gridIsca;
var _isca;
var _pesquisaIsca;

var PesquisaIsca = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridIsca.CarregarGrid();
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

var Isca = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", required: ko.observable(false), maxlength: 50 });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.CodigoEmpresaIsca = PropertyEntity({ text: "Código da Empresa Isca: ", required: ko.observable(true), maxlength: 50 });
    this.Site = PropertyEntity({ text: "Site: ", required: ko.observable(true), maxlength: 150 });
    this.Login = PropertyEntity({ text: "Login: ", required: ko.observable(true), maxlength: 150 });
    this.Senha = PropertyEntity({ text: "Senha: ", required: ko.observable(true), maxlength: 150 });
};

var CRUDIsca = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadIsca() {
    _isca = new Isca();
    KoBindings(_isca, "knockoutCadastroIsca");

    HeaderAuditoria("Isca", _isca);

    _crudIsca = new CRUDIsca();
    KoBindings(_crudIsca, "knockoutCRUDIsca");

    _pesquisaIsca = new PesquisaIsca();
    KoBindings(_pesquisaIsca, "knockoutPesquisaIsca", false, _pesquisaIsca.Pesquisar.id);
    
    loadGridIsca();
}

function loadGridIsca() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarIsca, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridIsca = new GridView(_pesquisaIsca.Pesquisar.idGrid, "Isca/Pesquisa", _pesquisaIsca, menuOpcoes, null);
    _gridIsca.CarregarGrid();
}

function adicionarClick(e, sender) {
    Salvar(_isca, "Isca/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridIsca.CarregarGrid();
                limparCamposIsca();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_isca, "Isca/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridIsca.CarregarGrid();
                limparCamposIsca();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de Lacre " + _isca.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_isca, "Isca/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridIsca.CarregarGrid();
                limparCamposIsca();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposIsca();
}

//*******MÉTODOS*******

function editarIsca(isca) {
    limparCamposIsca();
    _isca.Codigo.val(isca.Codigo);
    BuscarPorCodigo(_isca, "Isca/BuscarPorCodigo", function (arg) {
        _pesquisaIsca.ExibirFiltros.visibleFade(false);
        _crudIsca.Atualizar.visible(true);
        _crudIsca.Cancelar.visible(true);
        _crudIsca.Excluir.visible(true);
        _crudIsca.Adicionar.visible(false);
    }, null);
}

function limparCamposIsca() {
    _crudIsca.Atualizar.visible(false);
    _crudIsca.Cancelar.visible(false);
    _crudIsca.Excluir.visible(false);
    _crudIsca.Adicionar.visible(true);
    LimparCampos(_isca);
}