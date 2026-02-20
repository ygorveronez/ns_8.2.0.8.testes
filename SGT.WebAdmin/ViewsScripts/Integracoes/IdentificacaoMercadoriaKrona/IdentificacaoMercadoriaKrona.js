/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDIdentificacaoMercadoriaKrona;
var _gridIdentificacaoMercadoriaKrona;
var _identificacaoMercadoriaKrona;
var _pesquisaIdentificacaoMercadoriaKrona;

/*
 * Declaração das Classes
 */

var CRUDIdentificacaoMercadoriaKrona = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var IdentificacaoMercadoriaKrona = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.IdentificadorDescricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Identificador = PropertyEntity({ text: "*Identificador:", required: true, getType: typesKnockout.int });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaIdentificacaoMercadoriaKrona = function () {
    this.IdentificadorDescricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Identificador = PropertyEntity({ text: "Identificador:", required: true, getType: typesKnockout.int });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridIdentificacaoMercadoriaKrona, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridIdentificacaoMercadoriaKrona() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "IdentificacaoMercadoriaKrona/ExportarPesquisa", titulo: "Identificações de Mercadoria da Krona" };

    _gridIdentificacaoMercadoriaKrona = new GridViewExportacao(_pesquisaIdentificacaoMercadoriaKrona.Pesquisar.idGrid, "IdentificacaoMercadoriaKrona/Pesquisa", _pesquisaIdentificacaoMercadoriaKrona, menuOpcoes, configuracoesExportacao);
    _gridIdentificacaoMercadoriaKrona.CarregarGrid();
}

function loadIdentificacaoMercadoriaKrona() {
    _identificacaoMercadoriaKrona = new IdentificacaoMercadoriaKrona();
    KoBindings(_identificacaoMercadoriaKrona, "knockoutIdentificacaoMercadoriaKrona");

    HeaderAuditoria("IdentificacaoMercadoriaKrona", _identificacaoMercadoriaKrona);

    _CRUDIdentificacaoMercadoriaKrona = new CRUDIdentificacaoMercadoriaKrona();
    KoBindings(_CRUDIdentificacaoMercadoriaKrona, "knockoutCRUDIdentificacaoMercadoriaKrona");

    _pesquisaIdentificacaoMercadoriaKrona = new PesquisaIdentificacaoMercadoriaKrona();
    KoBindings(_pesquisaIdentificacaoMercadoriaKrona, "knockoutPesquisaIdentificacaoMercadoriaKrona", false, _pesquisaIdentificacaoMercadoriaKrona.Pesquisar.id);

    loadGridIdentificacaoMercadoriaKrona();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_identificacaoMercadoriaKrona, "IdentificacaoMercadoriaKrona/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridIdentificacaoMercadoriaKrona();
                limparCamposIdentificacaoMercadoriaKrona();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_identificacaoMercadoriaKrona, "IdentificacaoMercadoriaKrona/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridIdentificacaoMercadoriaKrona();
                limparCamposIdentificacaoMercadoriaKrona();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposIdentificacaoMercadoriaKrona();
}

function editarClick(registroSelecionado) {
    limparCamposIdentificacaoMercadoriaKrona();

    _identificacaoMercadoriaKrona.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_identificacaoMercadoriaKrona, "IdentificacaoMercadoriaKrona/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaIdentificacaoMercadoriaKrona.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();
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
        ExcluirPorCodigo(_identificacaoMercadoriaKrona, "IdentificacaoMercadoriaKrona/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridIdentificacaoMercadoriaKrona();
                    limparCamposIdentificacaoMercadoriaKrona();
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

function controlarBotoesHabilitados() {
    var isEdicao = _identificacaoMercadoriaKrona.Codigo.val() > 0;

    _CRUDIdentificacaoMercadoriaKrona.Atualizar.visible(isEdicao);
    _CRUDIdentificacaoMercadoriaKrona.Excluir.visible(isEdicao);
    _CRUDIdentificacaoMercadoriaKrona.Adicionar.visible(!isEdicao);
}

function limparCamposIdentificacaoMercadoriaKrona() {
    LimparCampos(_identificacaoMercadoriaKrona);
    controlarBotoesHabilitados();
}

function recarregarGridIdentificacaoMercadoriaKrona() {
    _gridIdentificacaoMercadoriaKrona.CarregarGrid();
}