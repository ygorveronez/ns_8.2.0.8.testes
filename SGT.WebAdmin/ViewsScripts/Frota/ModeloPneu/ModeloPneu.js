/// <reference path="../../Consultas/DimensaoPneu.js" />
/// <reference path="../../Consultas/MarcaPneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDModeloPneu;
var _modeloPneu;
var _pesquisaModeloPneu;
var _gridModeloPneu;

/*
 * Declaração das Classes
 */

var ModeloPneu = function () {
    this.Codigo = PropertyEntity({ text: "Código" ,val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: true, enable: false });
    this.Descricao = PropertyEntity({ text: "*Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: true });
    this.Dimensao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Dimensão:", idBtnSearch: guid(), required: true });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Marca:", idBtnSearch: guid(), required: true });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var CRUDModeloPneu = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaModeloPneu = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Dimensao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Dimensão:", idBtnSearch: guid() });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridModeloPneu, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridModeloPneu() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "ModeloPneu/ExportarPesquisa", titulo: "Modelo de Pneu" };

    _gridModeloPneu = new GridViewExportacao(_pesquisaModeloPneu.Pesquisar.idGrid, "ModeloPneu/Pesquisa", _pesquisaModeloPneu, menuOpcoes, configuracoesExportacao);
    _gridModeloPneu.CarregarGrid();
}

function loadModeloPneu() {
    _modeloPneu = new ModeloPneu();
    KoBindings(_modeloPneu, "knockoutModeloPneu");

    HeaderAuditoria("ModeloPneu", _modeloPneu);

    _CRUDModeloPneu = new CRUDModeloPneu();
    KoBindings(_CRUDModeloPneu, "knockoutCRUDModeloPneu");

    _pesquisaModeloPneu = new PesquisaModeloPneu();
    KoBindings(_pesquisaModeloPneu, "knockoutPesquisaModeloPneu", false, _pesquisaModeloPneu.Pesquisar.id);

    new BuscarDimensaoPneu(_pesquisaModeloPneu.Dimensao);
    new BuscarMarcaPneu(_pesquisaModeloPneu.Marca);
    new BuscarDimensaoPneu(_modeloPneu.Dimensao);
    new BuscarMarcaPneu(_modeloPneu.Marca);

    loadGridModeloPneu();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_modeloPneu, "ModeloPneu/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridModeloPneu();
                limparCamposModeloPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_modeloPneu, "ModeloPneu/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridModeloPneu();
                limparCamposModeloPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposModeloPneu();
}

function editarClick(registroSelecionado) {
    limparCamposModeloPneu();

    _modeloPneu.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_modeloPneu, "ModeloPneu/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaModeloPneu.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_modeloPneu, "ModeloPneu/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridModeloPneu();
                    limparCamposModeloPneu();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDModeloPneu.Atualizar.visible(isEdicao);
    _CRUDModeloPneu.Excluir.visible(isEdicao);
    _CRUDModeloPneu.Cancelar.visible(isEdicao);
    _CRUDModeloPneu.Adicionar.visible(!isEdicao);
}

function limparCamposModeloPneu() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_modeloPneu);
}

function recarregarGridModeloPneu() {
    _gridModeloPneu.CarregarGrid();
}