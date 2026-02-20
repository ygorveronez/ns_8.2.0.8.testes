/// <reference path="NfeRetornoResumo.js" />
/// <reference path="NfeSaidaResumo.js" />
/// <reference path="Reforma.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _envioReformaPalletQuantidade;

/*
 * Declaração das Classes
 */

var EnvioReformaPalletQuantidade = function () {
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

function loadEnvioReformaPalletQuantidade() {
    _envioReformaPalletQuantidade = new EnvioReformaPalletQuantidade();
    KoBindings(_envioReformaPalletQuantidade, "knockoutEnvioReformaPalletQuantidade");

    buscarSituacoes();
}

/*
 * Declaração das Funções
 */

function buscarSituacoes() {
    executarReST("Reforma/BuscarSituacoes", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data.length > 0)
                carregarSituacoes(retorno.Data);
            else
                _envioReformaPalletQuantidade.PossuiSituacoes.val(false);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function carregarSituacoes(envioQuantidade) {
    var quantidade = 0;

    for (var i = 0; i < envioQuantidade.length; i++) {
        var situacao = new SituacaoDevolucaoPallet(envioQuantidade[i]);

        _envioReformaPalletQuantidade.Situacoes.push(situacao);

        $("#" + situacao.Quantidade.id).maskMoney(situacao.Quantidade.configInt);

        if (envioQuantidade[i])
            quantidade += envioQuantidade[i].Quantidade
    }

    preencherNfeSaidaResumoQuantidadeEnvio(quantidade);
    preencherNfeRetornoResumoQuantidadeEnvio(quantidade);
}

function limparCamposEnvioQuantidade() {
    _envioReformaPalletQuantidade.Situacoes.removeAll();
    _envioReformaPalletQuantidade.PossuiSituacoes.val(true);
}

function obterSituacoes() {
    var situacoes = new Array();

    for (var i = 0; i < _envioReformaPalletQuantidade.Situacoes().length; i++) {
        var situacao = _envioReformaPalletQuantidade.Situacoes()[i];
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
    if (_envioReformaPalletQuantidade.Situacoes().length == 0) {
        exibirMensagem("atencao", "Situação de Devolução", "Não foi encontrada nenhuma situação de devolução de pallet avariado");

        return false;
    }
        
    for (var i = 0; i < _envioReformaPalletQuantidade.Situacoes().length; i++) {
        var situacao = _envioReformaPalletQuantidade.Situacoes()[i];
        var quantidade = Globalize.parseInt(situacao.Quantidade.val());

        if (!isNaN(quantidade) && (quantidade > 0))
            return true;
    }

    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe ao menos uma quantidade para reforma");

    return false;
}

function preencherEnvioQuantidade(dadosEnvioQuantidade) {
    carregarSituacoes(dadosEnvioQuantidade);
}