/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridObservacoes;

/*
 * Declaração das Funções de Inicialização
 */

function loadGridObservacoes() {
    var knoutTabelaFrete = {
        Codigo: _tabelaFrete.CodigoTabelaFrete
    };

    _gridObservacoes = new GridView("grid-observacoes", "AutorizacaoTabelaFrete/PesquisaObservacoesTabelaFreteCliente", knoutTabelaFrete);
    _gridObservacoes.CarregarGrid();
}

function loadObservacoes() {
    loadGridObservacoes();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

/*
 * Declaração das Funções Públicas
 */

function recarregarGridObservacoes() {
    _gridObservacoes.CarregarGrid();
}

function isSituacaoPermiteExibirAbaAnexos(situacao) {
    return (situacao === EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao) || (situacao === EnumSituacaoAlteracaoTabelaFrete.Reprovada);
}