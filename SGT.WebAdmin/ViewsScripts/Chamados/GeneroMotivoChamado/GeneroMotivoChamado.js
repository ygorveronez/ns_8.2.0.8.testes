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

var _gridGeneroMotivoChamado;
var _generoMotivoChamado;
var _crudGeneroMotivoChamado
var _pesquisaGeneroMotivoChamado;

var PesquisaGeneroMotivoChamado = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGeneroMotivoChamado.CarregarGrid();
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

var GeneroMotivoChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 200 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", required: false, maxlength: 500 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDGeneroMotivoChamado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadGeneroMotivoChamado() {
    _generoMotivoChamado = new GeneroMotivoChamado();
    KoBindings(_generoMotivoChamado, "knockoutCadastroGeneroMotivoChamado");

    HeaderAuditoria("GeneroMotivoChamado", _generoMotivoChamado);

    _crudGeneroMotivoChamado = new CRUDGeneroMotivoChamado();
    KoBindings(_crudGeneroMotivoChamado, "knockoutCRUDGeneroMotivoChamado");

    _pesquisaGeneroMotivoChamado = new PesquisaGeneroMotivoChamado();
    KoBindings(_pesquisaGeneroMotivoChamado, "knockoutPesquisaGeneroMotivoChamado", false, _pesquisaGeneroMotivoChamado.Pesquisar.id);

    buscarGeneroMotivoChamado();
}

function adicionarClick(e, sender) {
    Salvar(_generoMotivoChamado, "GeneroMotivoChamado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridGeneroMotivoChamado.CarregarGrid();
                limparCamposGeneroMotivoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_generoMotivoChamado, "GeneroMotivoChamado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridGeneroMotivoChamado.CarregarGrid();
                limparCamposGeneroMotivoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Gênero " + _generoMotivoChamado.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_generoMotivoChamado, "GeneroMotivoChamado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridGeneroMotivoChamado.CarregarGrid();
                    limparCamposGeneroMotivoChamado();
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
    limparCamposGeneroMotivoChamado();
}

//*******MÉTODOS*******

function buscarGeneroMotivoChamado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarGeneroMotivoChamado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridGeneroMotivoChamado = new GridView(_pesquisaGeneroMotivoChamado.Pesquisar.idGrid, "GeneroMotivoChamado/Pesquisa", _pesquisaGeneroMotivoChamado, menuOpcoes);
    _gridGeneroMotivoChamado.CarregarGrid();
}

function editarGeneroMotivoChamado(registroSelecionado) {
    limparCamposGeneroMotivoChamado();

    _generoMotivoChamado.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_generoMotivoChamado, "GeneroMotivoChamado/BuscarPorCodigo", function (arg) {
        _pesquisaGeneroMotivoChamado.ExibirFiltros.visibleFade(false);
        _crudGeneroMotivoChamado.Atualizar.visible(true);
        _crudGeneroMotivoChamado.Cancelar.visible(true);
        _crudGeneroMotivoChamado.Excluir.visible(true);
        _crudGeneroMotivoChamado.Adicionar.visible(false);
    }, null);
}

function limparCamposGeneroMotivoChamado() {
    _crudGeneroMotivoChamado.Atualizar.visible(false);
    _crudGeneroMotivoChamado.Cancelar.visible(false);
    _crudGeneroMotivoChamado.Excluir.visible(false);
    _crudGeneroMotivoChamado.Adicionar.visible(true);
    LimparCampos(_generoMotivoChamado);
}