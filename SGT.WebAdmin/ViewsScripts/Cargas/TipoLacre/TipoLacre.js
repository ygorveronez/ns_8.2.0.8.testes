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

var _gridTipoLacre;
var _tipoLacre;
var _pesquisaTipoLacre;

var PesquisaTipoLacre = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoLacre.CarregarGrid();
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

var TipoLacre = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", required: ko.observable(false), maxlength: 50 });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDTipoLacre = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTipoLacre() {
    _tipoLacre = new TipoLacre();
    KoBindings(_tipoLacre, "knockoutCadastroTipoLacre");

    HeaderAuditoria("TipoLacre", _tipoLacre);

    _crudTipoLacre = new CRUDTipoLacre();
    KoBindings(_crudTipoLacre, "knockoutCRUDTipoLacre");

    _pesquisaTipoLacre = new PesquisaTipoLacre();
    KoBindings(_pesquisaTipoLacre, "knockoutPesquisaTipoLacre", false, _pesquisaTipoLacre.Pesquisar.id);

    buscarTipoLacre();
}

function adicionarClick(e, sender) {
    Salvar(_tipoLacre, "TipoLacre/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoLacre.CarregarGrid();
                limparCamposTipoLacre();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoLacre, "TipoLacre/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoLacre.CarregarGrid();
                limparCamposTipoLacre();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de Lacre " + _tipoLacre.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoLacre, "TipoLacre/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoLacre.CarregarGrid();
                limparCamposTipoLacre();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoLacre();
}

//*******MÉTODOS*******


function buscarTipoLacre() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoLacre, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoLacre = new GridView(_pesquisaTipoLacre.Pesquisar.idGrid, "TipoLacre/Pesquisa", _pesquisaTipoLacre, menuOpcoes, null);
    _gridTipoLacre.CarregarGrid();
}

function editarTipoLacre(tipoLacreGrid) {
    limparCamposTipoLacre();
    _tipoLacre.Codigo.val(tipoLacreGrid.Codigo);
    BuscarPorCodigo(_tipoLacre, "TipoLacre/BuscarPorCodigo", function (arg) {
        _pesquisaTipoLacre.ExibirFiltros.visibleFade(false);
        _crudTipoLacre.Atualizar.visible(true);
        _crudTipoLacre.Cancelar.visible(true);
        _crudTipoLacre.Excluir.visible(true);
        _crudTipoLacre.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoLacre() {
    _crudTipoLacre.Atualizar.visible(false);
    _crudTipoLacre.Cancelar.visible(false);
    _crudTipoLacre.Excluir.visible(false);
    _crudTipoLacre.Adicionar.visible(true);
    LimparCampos(_tipoLacre);
}