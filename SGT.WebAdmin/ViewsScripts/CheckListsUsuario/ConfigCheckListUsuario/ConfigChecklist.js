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
/// <reference path="ConfigChecklistPergunta.js" />
/// <reference path="ConfigChecklistPerguntaOpcoes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaChecklist;
var _gridChecklist;
var _checklist;
var _crudChecklist;

var PesquisaChecklist = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridChecklist.CarregarGrid();
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

var Checklist = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", required: true });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", val: ko.observable(""), def: "" });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "" });

    this.Perguntas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
};

var CRUDChecklist = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

function loadChecklist() {
    _pesquisaChecklist = new PesquisaChecklist();
    KoBindings(_pesquisaChecklist, "knockoutPesquisaChecklist");

    _checklist = new Checklist();
    KoBindings(_checklist, "knockoutChecklist");

    _crudChecklist = new CRUDChecklist();
    KoBindings(_crudChecklist, "knockoutCRUDChecklist");

    loadChecklistPergunta();
    buscarGridChecklist();
}

function buscarGridChecklist() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarChecklist, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridChecklist = new GridView(_pesquisaChecklist.Pesquisar.idGrid, "ConfigCheckListUsuario/Pesquisa", _pesquisaChecklist, menuOpcoes);
    _gridChecklist.CarregarGrid();
}

function editarChecklist(data) {
    limparCamposChecklist();
    _checklist.Codigo.val(data.Codigo);
    BuscarPorCodigo(_checklist, "ConfigCheckListUsuario/BuscarPorCodigo", function (ret) {

        PreencherObjetoKnout(_checklist, ret);
        preecherRetorno(ret);

        _pesquisaChecklist.ExibirFiltros.visibleFade(false);
        _crudChecklist.Atualizar.visible(true);
        _crudChecklist.Cancelar.visible(true);
        _crudChecklist.Excluir.visible(true);
        _crudChecklist.Adicionar.visible(false);
    }, null);
}


function preecherRetorno(arg) {
    preencherPerguntas(arg.Data.Perguntas);
}

function adicionarClick() {

    executarReST("ConfigCheckListUsuario/Adicionar", ObterCheckListSalvar(), function (ret) {
        if (ret.Success) {
            if (ret.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridChecklist.CarregarGrid();
                limparCamposChecklist();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", ret.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", ret.Msg);
        }
    });

}

function adicionarClick(e, sender) {
    ObterCheckListSalvar();

    Salvar(_checklist, "ConfigCheckListUsuario/Adicionar", function (ret) {
        if (ret.Success) {
            if (ret.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridChecklist.CarregarGrid();
                limparCamposChecklist();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", ret.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", ret.Msg);
        }
    }, sender);

}


function atualizarClick(e, sender) {
    ObterCheckListSalvar();

    Salvar(_checklist, "ConfigCheckListUsuario/Atualizar", function (ret) {
        if (ret.Success) {
            if (ret.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridChecklist.CarregarGrid();
                limparCamposChecklist();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", ret.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", ret.Msg);
        }
    }, sender);
}


function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o checklist" + _checklist.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_checklist, "ConfigCheckListUsuario/ExcluirPorCodigo", function (ret) {
            if (ret.Success) {
                if (ret.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridChecklist.CarregarGrid();
                    limparCamposChecklist();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", ret.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", ret.Msg);
            }
        }, null);
    });
}

function cancelarClick() {
    limparCamposChecklist();
}

function limparCamposChecklist() {
    limparCamposPerguntas();
    LimparCampos(_checklist);
    recarregarGridPergunta();

    _crudChecklist.Atualizar.visible(false);
    _crudChecklist.Cancelar.visible(false);
    _crudChecklist.Excluir.visible(false);
    _crudChecklist.Adicionar.visible(true);
}

function ObterCheckListSalvar() {

    $.each(_checklist.Perguntas.list, function (i, pergunta) {

        if (pergunta.Opcoes.list.length == 0) {
            _checklist.Perguntas.list[i].Opcoes.list = new Array();
            _checklist.Perguntas.list[i].OpcoesSalvar.val = "[]";
        } else {
            _checklist.Perguntas.list[i].OpcoesSalvar.val = JSON.stringify(pergunta.Opcoes.list);
        }

    });

}
