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

var _gridTipoResponsavelAtrasoEntrega;
var _tipoResponsavelAtrasoEntrega;
var _crudTipoResponsavelAtrasoEntrega;
var _pesquisaTipoResponsavelAtrasoEntrega;

var PesquisaTipoResponsavelAtrasoEntrega = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Ativo: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoResponsavelAtrasoEntrega.CarregarGrid();
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

var TipoResponsavelAtrasoEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Ativo: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 5000 });
};

var CRUDTipoResponsavelAtrasoEntrega = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTipoResponsavelAtrasoEntrega() {
    _tipoResponsavelAtrasoEntrega = new TipoResponsavelAtrasoEntrega();
    KoBindings(_tipoResponsavelAtrasoEntrega, "knockoutCadastroTipoResponsavelAtrasoEntrega");

    HeaderAuditoria("TipoResponsavelAtrasoEntrega", _tipoResponsavelAtrasoEntrega);

    _crudTipoResponsavelAtrasoEntrega = new CRUDTipoResponsavelAtrasoEntrega();
    KoBindings(_crudTipoResponsavelAtrasoEntrega, "knockoutCRUDTipoResponsavelAtrasoEntrega");

    _pesquisaTipoResponsavelAtrasoEntrega = new PesquisaTipoResponsavelAtrasoEntrega();
    KoBindings(_pesquisaTipoResponsavelAtrasoEntrega, "knockoutPesquisaTipoResponsavelAtrasoEntrega", false, _pesquisaTipoResponsavelAtrasoEntrega.Pesquisar.id);

    buscarTipoResponsavelAtrasoEntrega();
}

function adicionarClick(e, sender) {
    Salvar(_tipoResponsavelAtrasoEntrega, "TipoResponsavelAtrasoEntrega/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoResponsavelAtrasoEntrega.CarregarGrid();
                limparCamposTipoResponsavelAtrasoEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoResponsavelAtrasoEntrega, "TipoResponsavelAtrasoEntrega/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoResponsavelAtrasoEntrega.CarregarGrid();
                limparCamposTipoResponsavelAtrasoEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo " + _tipoResponsavelAtrasoEntrega.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoResponsavelAtrasoEntrega, "TipoResponsavelAtrasoEntrega/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoResponsavelAtrasoEntrega.CarregarGrid();
                    limparCamposTipoResponsavelAtrasoEntrega();
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
    limparCamposTipoResponsavelAtrasoEntrega();
}

//*******MÉTODOS*******

function buscarTipoResponsavelAtrasoEntrega() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoResponsavelAtrasoEntrega, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoResponsavelAtrasoEntrega = new GridView(_pesquisaTipoResponsavelAtrasoEntrega.Pesquisar.idGrid, "TipoResponsavelAtrasoEntrega/Pesquisa", _pesquisaTipoResponsavelAtrasoEntrega, menuOpcoes);
    _gridTipoResponsavelAtrasoEntrega.CarregarGrid();
}

function editarTipoResponsavelAtrasoEntrega(tipoResponsavelAtrasoEntregaGrid) {
    limparCamposTipoResponsavelAtrasoEntrega();
    _tipoResponsavelAtrasoEntrega.Codigo.val(tipoResponsavelAtrasoEntregaGrid.Codigo);
    BuscarPorCodigo(_tipoResponsavelAtrasoEntrega, "TipoResponsavelAtrasoEntrega/BuscarPorCodigo", function (arg) {
        _pesquisaTipoResponsavelAtrasoEntrega.ExibirFiltros.visibleFade(false);
        _crudTipoResponsavelAtrasoEntrega.Atualizar.visible(true);
        _crudTipoResponsavelAtrasoEntrega.Cancelar.visible(true);
        _crudTipoResponsavelAtrasoEntrega.Excluir.visible(true);
        _crudTipoResponsavelAtrasoEntrega.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoResponsavelAtrasoEntrega() {
    _crudTipoResponsavelAtrasoEntrega.Atualizar.visible(false);
    _crudTipoResponsavelAtrasoEntrega.Cancelar.visible(false);
    _crudTipoResponsavelAtrasoEntrega.Excluir.visible(false);
    _crudTipoResponsavelAtrasoEntrega.Adicionar.visible(true);
    LimparCampos(_tipoResponsavelAtrasoEntrega);
}