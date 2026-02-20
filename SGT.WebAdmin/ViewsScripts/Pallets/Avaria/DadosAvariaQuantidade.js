/// <reference path="Avaria.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _dadosAvariaPalletQuantidade;

/*
 * Declaração das Classes
 */

var DadosAvariaPalletQuantidade = function () {
    this.PossuiSituacoes = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.Situacoes = ko.observableArray();
}

var SituacaoDevolucaoPallet = function (situacao) {
    this.Codigo = PropertyEntity({ val: ko.observable(situacao.Codigo), getType: typesKnockout.int, def: situacao.Codigo });
    this.Quantidade = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, enable: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: "" }, def: "", maxlength: 7, text: situacao.Descricao + ":" });

    if (situacao.Quantidade) {
        this.Quantidade.val(situacao.Quantidade);
        this.Quantidade.enable(false);
    }
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDadosAvariaPalletQuantidade() {
    _dadosAvariaPalletQuantidade = new DadosAvariaPalletQuantidade();
    KoBindings(_dadosAvariaPalletQuantidade, "knockoutDadosAvariaPalletQuantidade");

    buscarSituacoes();
}

/*
 * Declaração das Funções
 */

function buscarSituacoes() {
    executarReST("Avaria/BuscarSituacoes", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data.length > 0)
                carregarSituacoes(retorno.Data);
            else
                _dadosAvariaPalletQuantidade.PossuiSituacoes.val(false);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function carregarSituacoes(dadosAvariaQuantidade) {
    for (var i = 0; i < dadosAvariaQuantidade.length; i++) {
        var situacao = new SituacaoDevolucaoPallet(dadosAvariaQuantidade[i]);

        _dadosAvariaPalletQuantidade.Situacoes.push(situacao);

        $("#" + situacao.Quantidade.id).maskMoney(situacao.Quantidade.configInt);
    }
}

function limparCamposDadosAvariaQuantidade() {
    _dadosAvariaPalletQuantidade.Situacoes.removeAll();
    _dadosAvariaPalletQuantidade.PossuiSituacoes.val(true);
}

function obterSituacoes() {
    var situacoes = new Array();

    for (var i = 0; i < _dadosAvariaPalletQuantidade.Situacoes().length; i++) {
        var situacao = _dadosAvariaPalletQuantidade.Situacoes()[i];
        var quantidade = Globalize.parseInt(situacao.Quantidade.val());

        if (!isNaN(quantidade) && (quantidade > 0)) {
            situacoes.push({
                Codigo: situacao.Codigo.val(),
                Quantidade: quantidade
            });
        }
    }

    return JSON.stringify(situacoes);
}

function validarSituacoesInformadas() {
    if (_dadosAvariaPalletQuantidade.Situacoes().length == 0) {
        exibirMensagem("atencao", "Situação de Devolução", "Não foi encontrada nenhuma situação de devolução de pallet avariado");

        return false;
    }
        
    for (var i = 0; i < _dadosAvariaPalletQuantidade.Situacoes().length; i++) {
        var situacao = _dadosAvariaPalletQuantidade.Situacoes()[i];
        var quantidade = Globalize.parseInt(situacao.Quantidade.val());

        if (!isNaN(quantidade) && (quantidade > 0))
            return true;
    }

    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe ao menos uma quantidade avariada");

    return false;
}

function preencherDadosAvariaQuantidade(dadosAvariaQuantidade) {
    carregarSituacoes(dadosAvariaQuantidade);
}