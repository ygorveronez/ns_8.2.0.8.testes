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

var _gridTipoBaseline;
var _tipoBaseline;
var _pesquisaTipoBaseline;

var PesquisaTipoBaseline = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoBaseline.CarregarGrid();
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

var TipoBaseline = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", maxlength: 50, issue: 15 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDTipoBaseline = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTipoBaseline() {
    _tipoBaseline = new TipoBaseline();
    KoBindings(_tipoBaseline, "knockoutCadastroTipoBaseline");

    HeaderAuditoria("TipoBaseline", _tipoBaseline);

    _crudTipoBaseline = new CRUDTipoBaseline();
    KoBindings(_crudTipoBaseline, "knockoutCRUDTipoBaseline");

    _pesquisaTipoBaseline = new PesquisaTipoBaseline();
    KoBindings(_pesquisaTipoBaseline, "knockoutPesquisaTipoBaseline", false, _pesquisaTipoBaseline.Pesquisar.id);

    buscarTipoBaseline();
}

function adicionarClick(e, sender) {
    Salvar(_tipoBaseline, "TipoBaseline/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoBaseline.CarregarGrid();
                limparCamposTipoBaseline();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoBaseline, "TipoBaseline/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoBaseline.CarregarGrid();
                limparCamposTipoBaseline();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de Baseline " + _tipoBaseline.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoBaseline, "TipoBaseline/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoBaseline.CarregarGrid();
                    limparCamposTipoBaseline();
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
    limparCamposTipoBaseline();
}

//*******MÉTODOS*******

function buscarTipoBaseline() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoBaseline, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoBaseline = new GridView(_pesquisaTipoBaseline.Pesquisar.idGrid, "TipoBaseline/Pesquisa", _pesquisaTipoBaseline, menuOpcoes, null);
    _gridTipoBaseline.CarregarGrid();
}

function editarTipoBaseline(tipoBaselineGrid) {
    limparCamposTipoBaseline();
    _tipoBaseline.Codigo.val(tipoBaselineGrid.Codigo);
    BuscarPorCodigo(_tipoBaseline, "TipoBaseline/BuscarPorCodigo", function (arg) {
        _pesquisaTipoBaseline.ExibirFiltros.visibleFade(false);
        _crudTipoBaseline.Atualizar.visible(true);
        _crudTipoBaseline.Cancelar.visible(true);
        _crudTipoBaseline.Excluir.visible(true);
        _crudTipoBaseline.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoBaseline() {
    _crudTipoBaseline.Atualizar.visible(false);
    _crudTipoBaseline.Cancelar.visible(false);
    _crudTipoBaseline.Excluir.visible(false);
    _crudTipoBaseline.Adicionar.visible(true);
    LimparCampos(_tipoBaseline);
}