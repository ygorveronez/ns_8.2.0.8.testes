/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumRotaFreteClasse.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRotaFreteClassificacao;
var _gridRotaFreteClassificacao;
var _rotaFreteClassificacao;
var _pesquisaRotaFreteClassificacao;

/*
 * Declaração das Classes
 */

var CRUDRotaFreteClassificacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var RotaFreteClassificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Classe = PropertyEntity({ text: "Classe: ", val: ko.observable(EnumRotaFreteClasse.Um), options: EnumRotaFreteClasse.obterOpcoes(), def: EnumRotaFreteClasse.Um });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
}

var PesquisaRotaFreteClassificacao = function () {
    this.Classe = PropertyEntity({ text: "Classe: ", val: ko.observable(EnumRotaFreteClasse.Todas), options: EnumRotaFreteClasse.obterOpcoesPesquisa(), def: EnumRotaFreteClasse.Todas });
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridRotaFreteClassificacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridRotaFreteClassificacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "RotaFreteClassificacao/ExportarPesquisa", titulo: "Classificação de Rotas de Frete" };

    _gridRotaFreteClassificacao = new GridViewExportacao(_pesquisaRotaFreteClassificacao.Pesquisar.idGrid, "RotaFreteClassificacao/Pesquisa", _pesquisaRotaFreteClassificacao, menuOpcoes, configuracoesExportacao);
    _gridRotaFreteClassificacao.CarregarGrid();
}

function loadRotaFreteClassificacao() {
    _rotaFreteClassificacao = new RotaFreteClassificacao();
    KoBindings(_rotaFreteClassificacao, "knockoutRotaFreteClassificacao");

    HeaderAuditoria("RotaFreteClassificacao", _rotaFreteClassificacao);

    _CRUDRotaFreteClassificacao = new CRUDRotaFreteClassificacao();
    KoBindings(_CRUDRotaFreteClassificacao, "knockoutCRUDRotaFreteClassificacao");

    _pesquisaRotaFreteClassificacao = new PesquisaRotaFreteClassificacao();
    KoBindings(_pesquisaRotaFreteClassificacao, "knockoutPesquisaRotaFreteClassificacao", false, _pesquisaRotaFreteClassificacao.Pesquisar.id);

    loadGridRotaFreteClassificacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_rotaFreteClassificacao, "RotaFreteClassificacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridRotaFreteClassificacao();
                limparCamposRotaFreteClassificacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_rotaFreteClassificacao, "RotaFreteClassificacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridRotaFreteClassificacao();
                limparCamposRotaFreteClassificacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposRotaFreteClassificacao();
}

function editarClick(registroSelecionado) {
    limparCamposRotaFreteClassificacao();

    _rotaFreteClassificacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_rotaFreteClassificacao, "RotaFreteClassificacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaRotaFreteClassificacao.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_rotaFreteClassificacao, "RotaFreteClassificacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridRotaFreteClassificacao();
                    limparCamposRotaFreteClassificacao();
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
    var isEdicao = _rotaFreteClassificacao.Codigo.val() > 0;

    _CRUDRotaFreteClassificacao.Atualizar.visible(isEdicao);
    _CRUDRotaFreteClassificacao.Excluir.visible(isEdicao);
    _CRUDRotaFreteClassificacao.Adicionar.visible(!isEdicao);
}

function limparCamposRotaFreteClassificacao() {
    LimparCampos(_rotaFreteClassificacao);
    controlarBotoesHabilitados();
}

function recarregarGridRotaFreteClassificacao() {
    _gridRotaFreteClassificacao.CarregarGrid();
}