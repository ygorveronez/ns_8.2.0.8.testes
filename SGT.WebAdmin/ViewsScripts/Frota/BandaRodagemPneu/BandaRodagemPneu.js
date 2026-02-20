/// <reference path="../../Consultas/MarcaPneu.js" />
/// <reference path="../../Enumeradores/EnumTipoBandaRodagemPneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDBandaRodagemPneu;
var _bandaRodagemPneu;
var _pesquisaBandaRodagemPneu;
var _gridBandaRodagemPneu;

/*
 * Declaração das Classes
 */

var BandaRodagemPneu = function () {
    this.Codigo = PropertyEntity({ text: "Código:", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: false, visible: true });
    this.Descricao = PropertyEntity({ text: "*Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: true });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Marca:", idBtnSearch: guid(), required: true });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoBandaRodagemPneu.Borrachudo), options: EnumTipoBandaRodagemPneu.obterOpcoes(), def: EnumTipoBandaRodagemPneu.Borrachudo });
}

var CRUDBandaRodagemPneu = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaBandaRodagemPneu = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Tipo = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoBandaRodagemPneu.Todos), options: EnumTipoBandaRodagemPneu.obterOpcoesPesquisa(), def: EnumTipoBandaRodagemPneu.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridBandaRodagemPneu, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridBandaRodagemPneu() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "BandaRodagemPneu/ExportarPesquisa", titulo: "Banda de Rodagem de Pneu" };

    _gridBandaRodagemPneu = new GridViewExportacao(_pesquisaBandaRodagemPneu.Pesquisar.idGrid, "BandaRodagemPneu/Pesquisa", _pesquisaBandaRodagemPneu, menuOpcoes, configuracoesExportacao);
    _gridBandaRodagemPneu.CarregarGrid();
}

function loadBandaRodagemPneu() {
    _bandaRodagemPneu = new BandaRodagemPneu();
    KoBindings(_bandaRodagemPneu, "knockoutBandaRodagemPneu");

    HeaderAuditoria("BandaRodagemPneu", _bandaRodagemPneu);

    _CRUDBandaRodagemPneu = new CRUDBandaRodagemPneu();
    KoBindings(_CRUDBandaRodagemPneu, "knockoutCRUDBandaRodagemPneu");

    _pesquisaBandaRodagemPneu = new PesquisaBandaRodagemPneu();
    KoBindings(_pesquisaBandaRodagemPneu, "knockoutPesquisaBandaRodagemPneu", false, _pesquisaBandaRodagemPneu.Pesquisar.id);

    new BuscarMarcaPneu(_pesquisaBandaRodagemPneu.Marca);
    new BuscarMarcaPneu(_bandaRodagemPneu.Marca);

    loadGridBandaRodagemPneu();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_bandaRodagemPneu, "BandaRodagemPneu/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridBandaRodagemPneu();
                limparCamposBandaRodagemPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_bandaRodagemPneu, "BandaRodagemPneu/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridBandaRodagemPneu();
                limparCamposBandaRodagemPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposBandaRodagemPneu();
}

function editarClick(registroSelecionado) {
    limparCamposBandaRodagemPneu();

    _bandaRodagemPneu.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_bandaRodagemPneu, "BandaRodagemPneu/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaBandaRodagemPneu.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_bandaRodagemPneu, "BandaRodagemPneu/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridBandaRodagemPneu();
                    limparCamposBandaRodagemPneu();
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
    _CRUDBandaRodagemPneu.Atualizar.visible(isEdicao);
    _CRUDBandaRodagemPneu.Excluir.visible(isEdicao);
    _CRUDBandaRodagemPneu.Cancelar.visible(isEdicao);
    _CRUDBandaRodagemPneu.Adicionar.visible(!isEdicao);
}

function limparCamposBandaRodagemPneu() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_bandaRodagemPneu);
}

function recarregarGridBandaRodagemPneu() {
    _gridBandaRodagemPneu.CarregarGrid();
}