/// <reference path="../../Enumeradores/EnumSituacaoAvariaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _aprovacao;
var _detalheAutorizacao;
var _gridAutorizacoes;

/*
 * Declaração das Classes
 */

var Aprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Solicitante = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Solicitante:", enable: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data da Solicitação:", enable: ko.observable(true) });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:", enable: ko.observable(true) });

    this.DetalheAprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(true), def: true });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });

    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Exibir Aprovadores" : "Ocultar Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });
}

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAprovacaoAvariaPallet() {
    _aprovacao = new Aprovacao();
    KoBindings(_aprovacao, "knockoutAprovacaoAvariaPallet");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    loadGridAutorizacoes();
}

function loadGridAutorizacoes() {
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "Avaria/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacao.ExibirAprovadores.val();

    _aprovacao.ExibirAprovadores.val(!valorAtual);
    _aprovacao.DetalheAprovadores.visible(valorAtual);
}

function detalharAutorizacaoClick(registroSelecionado) {
    _detalheAutorizacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "Avaria/DetalhesAutorizacao", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != null) {
                Global.abrirModal('divModalDetalhesAutorizacao');
                $("#divModalDetalhesAutorizacao").one('hidden.bs.modal', function () {
                    limparCamposDetalheAutorizacao();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function preencherAprovacao(dadosAprovacao, codigoAvaria, situacaoAvaria) {
    if (situacaoAvaria === EnumSituacaoAvariaPallet.SemRegraAprovacao)
        _aprovacao.PossuiRegras.val(false);
    else {
        _aprovacao.PossuiRegras.val(true);

        if (dadosAprovacao) {
            _aprovacao.Codigo.val(codigoAvaria);

            PreencherObjetoKnout(_aprovacao, { Data: dadosAprovacao });

            _gridAutorizacoes.CarregarGrid();
        }
    }
}

function limparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
}

function limparCamposAprovacao() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacao);

    _gridAutorizacoes.CarregarGrid();
}