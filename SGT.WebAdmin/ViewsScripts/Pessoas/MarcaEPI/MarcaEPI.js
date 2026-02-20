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

var _gridMarcaEPI;
var _marcaEPI;
var _pesquisaMarcaEPI;

var PesquisaMarcaEPI = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMarcaEPI.CarregarGrid();
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

var MarcaEPI = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDMarcaEPI = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadMarcaEPI() {
    _marcaEPI = new MarcaEPI();
    KoBindings(_marcaEPI, "knockoutCadastroMarcaEPI");

    HeaderAuditoria("MarcaEPI", _marcaEPI);

    _crudMarcaEPI = new CRUDMarcaEPI();
    KoBindings(_crudMarcaEPI, "knockoutCRUDMarcaEPI");

    _pesquisaMarcaEPI = new PesquisaMarcaEPI();
    KoBindings(_pesquisaMarcaEPI, "knockoutPesquisaMarcaEPI", false, _pesquisaMarcaEPI.Pesquisar.id);

    buscarMarcaEPI();
}

function adicionarClick(e, sender) {
    Salvar(_marcaEPI, "MarcaEPI/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridMarcaEPI.CarregarGrid();
                limparCamposMarcaEPI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_marcaEPI, "MarcaEPI/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMarcaEPI.CarregarGrid();
                limparCamposMarcaEPI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Marca EPI " + _marcaEPI.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_marcaEPI, "MarcaEPI/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMarcaEPI.CarregarGrid();
                    limparCamposMarcaEPI();
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
    limparCamposMarcaEPI();
}

//*******MÉTODOS*******

function buscarMarcaEPI() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMarcaEPI, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMarcaEPI = new GridView(_pesquisaMarcaEPI.Pesquisar.idGrid, "MarcaEPI/Pesquisa", _pesquisaMarcaEPI, menuOpcoes);
    _gridMarcaEPI.CarregarGrid();
}

function editarMarcaEPI(marcaEPIGrid) {
    limparCamposMarcaEPI();
    _marcaEPI.Codigo.val(marcaEPIGrid.Codigo);
    BuscarPorCodigo(_marcaEPI, "MarcaEPI/BuscarPorCodigo", function (arg) {
        _pesquisaMarcaEPI.ExibirFiltros.visibleFade(false);
        _crudMarcaEPI.Atualizar.visible(true);
        _crudMarcaEPI.Cancelar.visible(true);
        _crudMarcaEPI.Excluir.visible(true);
        _crudMarcaEPI.Adicionar.visible(false);
    }, null);
}

function limparCamposMarcaEPI() {
    _crudMarcaEPI.Atualizar.visible(false);
    _crudMarcaEPI.Cancelar.visible(false);
    _crudMarcaEPI.Excluir.visible(false);
    _crudMarcaEPI.Adicionar.visible(true);
    LimparCampos(_marcaEPI);
}