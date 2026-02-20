/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMarcaPneu;
var _marcaPneu;
var _pesquisaMarcaPneu;
var _gridMarcaPneu;

/*
 * Declaração das Classes
 */

var MarcaPneu = function () {
    this.Codigo = PropertyEntity({ text: "Código", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: false });
    this.Descricao = PropertyEntity({ text: "*Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: true });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var CRUDMarcaPneu = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaMarcaPneu = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMarcaPneu, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMarcaPneu() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MarcaPneu/ExportarPesquisa", titulo: "Marca de Pneu" };

    _gridMarcaPneu = new GridViewExportacao(_pesquisaMarcaPneu.Pesquisar.idGrid, "MarcaPneu/Pesquisa", _pesquisaMarcaPneu, menuOpcoes, configuracoesExportacao);
    _gridMarcaPneu.CarregarGrid();
}

function loadMarcaPneu() {
    _marcaPneu = new MarcaPneu();
    KoBindings(_marcaPneu, "knockoutMarcaPneu");

    HeaderAuditoria("MarcaPneu", _marcaPneu);

    _CRUDMarcaPneu = new CRUDMarcaPneu();
    KoBindings(_CRUDMarcaPneu, "knockoutCRUDMarcaPneu");

    _pesquisaMarcaPneu = new PesquisaMarcaPneu();
    KoBindings(_pesquisaMarcaPneu, "knockoutPesquisaMarcaPneu", false, _pesquisaMarcaPneu.Pesquisar.id);

    loadGridMarcaPneu();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_marcaPneu, "MarcaPneu/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMarcaPneu();
                limparCamposMarcaPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_marcaPneu, "MarcaPneu/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMarcaPneu();
                limparCamposMarcaPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMarcaPneu();
}

function editarClick(registroSelecionado) {
    limparCamposMarcaPneu();

    _marcaPneu.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_marcaPneu, "MarcaPneu/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMarcaPneu.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_marcaPneu, "MarcaPneu/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMarcaPneu();
                    limparCamposMarcaPneu();
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
    _CRUDMarcaPneu.Atualizar.visible(isEdicao);
    _CRUDMarcaPneu.Excluir.visible(isEdicao);
    _CRUDMarcaPneu.Cancelar.visible(isEdicao);
    _CRUDMarcaPneu.Adicionar.visible(!isEdicao);
}

function limparCamposMarcaPneu() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_marcaPneu);
}

function recarregarGridMarcaPneu() {
    _gridMarcaPneu.CarregarGrid();
}