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
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoCategoria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCategoria;
var _categoria;
var _pesquisaCategoria;

var PesquisaCategoria = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", val: ko.observable(""), maxlength: 100, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: true })

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCatergoria, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
};

var Categoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 300, visible: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração: ", val: ko.observable(""), maxlength: 50, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesBooleano("Ativo", "Inativo"), def: true, text: "Situação: " });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, maxlength: 300, text: "Observação: ", enable: ko.observable(true) });
};

var CRUDCategoria = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadCategoria() {
    _pesquisaCategoria = new PesquisaCategoria();
    KoBindings(_pesquisaCategoria, "knockoutPesquisaCategoria", false, _pesquisaCategoria.Pesquisar.id);

    _categoria = new Categoria();
    KoBindings(_categoria, "knockoutCadastroCategoria");

    HeaderAuditoria("Categoria", _categoria);

    _crudCategoria = new CRUDCategoria();
    KoBindings(_crudCategoria, "knockoutCRUDCategoria");

    loadGridCategoria();
}

function loadGridCategoria() {
    var opcoesEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: function (item) { editarCategoria(item) }, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcoesEditar] };

    _gridCategoria = new GridView(_pesquisaCategoria.Pesquisar.idGrid, "Categoria/Pesquisa", _pesquisaCategoria, menuOpcoes);
    _gridCategoria.CarregarGrid();
}

function adicionarClick(e, sender) {
    Salvar(_categoria, "Categoria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                recarregarGridCatergoria();
                limparCamposCategoria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_categoria, "Categoria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    recarregarGridCatergoria();
                    limparCamposCategoria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function atualizarClick(e, sender) {
    Salvar(_categoria, "Categoria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                recarregarGridCatergoria();
                limparCamposCategoria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick() {
    limparCamposCategoria();
}

//*******MÉTODOS*******

function editarCategoria(categoriaGrid) {
    limparCamposCategoria();
    _categoria.Codigo.val(categoriaGrid.Codigo);
    BuscarPorCodigo(_categoria, "Categoria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaCategoria.ExibirFiltros.visibleFade(false);
                controleBotoesHabilitados(true);
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg)
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg)
        }
    }, null);
}

function limparCamposCategoria() {
    let isEdicao = false

    controleBotoesHabilitados(isEdicao)
    LimparCampos(_categoria);
    exibirFiltros()
}

function recarregarGridCatergoria() {
    _gridCategoria.CarregarGrid();
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade())
}

function controleBotoesHabilitados(isEdicao) {
    _crudCategoria.Atualizar.visible(isEdicao)
    _crudCategoria.Excluir.visible(isEdicao)
    _crudCategoria.Cancelar.visible(isEdicao)
    _crudCategoria.Adicionar.visible(!isEdicao)
}

function exibirFiltros() {
    if (!_pesquisaCategoria.ExibirFiltros.visibleFade()) {
        _pesquisaCategoria.ExibirFiltros.visibleFade(true)
    }
}