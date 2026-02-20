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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscalObservacaoCartaCorrecao;
var _notaFiscalObservacaoCartaCorrecao;
var _pesquisaNotaFiscalObservacaoCartaCorrecao;

var PesquisaNotaFiscalObservacaoCartaCorrecao = function () {
    this.Especificacao = PropertyEntity({ text: "Especificação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotaFiscalObservacaoCartaCorrecao.CarregarGrid();
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

var NotaFiscalObservacaoCartaCorrecao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Especificacao = PropertyEntity({ text: "*Especificação: ", required: true, maxlength: 500 });
    this.Mensagem = PropertyEntity({ text: "*Mensagem: ", required: true, maxlength: 1000 });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadNotaFiscalObservacaoCartaCorrecao() {

    _pesquisaNotaFiscalObservacaoCartaCorrecao = new PesquisaNotaFiscalObservacaoCartaCorrecao();
    KoBindings(_pesquisaNotaFiscalObservacaoCartaCorrecao, "knockoutPesquisaNotaFiscalObservacaoCartaCorrecao", false, _pesquisaNotaFiscalObservacaoCartaCorrecao.Pesquisar.id);

    _notaFiscalObservacaoCartaCorrecao = new NotaFiscalObservacaoCartaCorrecao();
    KoBindings(_notaFiscalObservacaoCartaCorrecao, "knockoutCadastroNotaFiscalObservacaoCartaCorrecao");

    HeaderAuditoria("NotaFiscalObservacaoCartaCorrecao", _notaFiscalObservacaoCartaCorrecao);

    buscarNotaFiscalObservacaoCartaCorrecaos();
}

function adicionarClick(e, sender) {
    Salvar(e, "NotaFiscalObservacaoCartaCorrecao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridNotaFiscalObservacaoCartaCorrecao.CarregarGrid();
                limparCamposNotaFiscalObservacaoCartaCorrecao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "NotaFiscalObservacaoCartaCorrecao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridNotaFiscalObservacaoCartaCorrecao.CarregarGrid();
                limparCamposNotaFiscalObservacaoCartaCorrecao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a observação " + _notaFiscalObservacaoCartaCorrecao.Especificacao.val() + "?", function () {
        ExcluirPorCodigo(_notaFiscalObservacaoCartaCorrecao, "NotaFiscalObservacaoCartaCorrecao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridNotaFiscalObservacaoCartaCorrecao.CarregarGrid();
                limparCamposNotaFiscalObservacaoCartaCorrecao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposNotaFiscalObservacaoCartaCorrecao();
}

//*******MÉTODOS*******


function buscarNotaFiscalObservacaoCartaCorrecaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarNotaFiscalObservacaoCartaCorrecao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridNotaFiscalObservacaoCartaCorrecao = new GridView(_pesquisaNotaFiscalObservacaoCartaCorrecao.Pesquisar.idGrid, "NotaFiscalObservacaoCartaCorrecao/Pesquisa", _pesquisaNotaFiscalObservacaoCartaCorrecao, menuOpcoes, null);
    _gridNotaFiscalObservacaoCartaCorrecao.CarregarGrid();
}

function editarNotaFiscalObservacaoCartaCorrecao(notaFiscalObservacaoCartaCorrecaoGrid) {
    limparCamposNotaFiscalObservacaoCartaCorrecao();
    _notaFiscalObservacaoCartaCorrecao.Codigo.val(notaFiscalObservacaoCartaCorrecaoGrid.Codigo);
    BuscarPorCodigo(_notaFiscalObservacaoCartaCorrecao, "NotaFiscalObservacaoCartaCorrecao/BuscarPorCodigo", function (arg) {
        _pesquisaNotaFiscalObservacaoCartaCorrecao.ExibirFiltros.visibleFade(false);
        _notaFiscalObservacaoCartaCorrecao.Atualizar.visible(true);
        _notaFiscalObservacaoCartaCorrecao.Cancelar.visible(true);
        _notaFiscalObservacaoCartaCorrecao.Excluir.visible(true);
        _notaFiscalObservacaoCartaCorrecao.Adicionar.visible(false);
    }, null);
}

function limparCamposNotaFiscalObservacaoCartaCorrecao() {
    _notaFiscalObservacaoCartaCorrecao.Atualizar.visible(false);
    _notaFiscalObservacaoCartaCorrecao.Cancelar.visible(false);
    _notaFiscalObservacaoCartaCorrecao.Excluir.visible(false);
    _notaFiscalObservacaoCartaCorrecao.Adicionar.visible(true);
    LimparCampos(_notaFiscalObservacaoCartaCorrecao);
}
