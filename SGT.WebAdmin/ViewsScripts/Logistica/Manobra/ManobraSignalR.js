/// <reference path="Manobra.js" />
/// <reference path="ManobraTracao.js" />

/*
 * Declaração das Funções de Inicialização
 */

function loadManobraSignalR() {
    SignalRManobraAlteradaEvent = manobraAlterada;
    SignalRManobraTracaoAlteradaEvent = manobraTracaoAlterada;
    SignalRManobraTracaoRemovidaEvent = manobraTracaoRemovida;
}

/*
 * Declaração das Funções
 */

function manobraAlterada(dadosAlteracao) {
    if (isAtualizarManobra(dadosAlteracao))
        _gridManobra.CarregarGrid();
}

function manobraTracaoAlterada(dadosManobraTracao) {
    if (isAtualizarManobraTracao(dadosManobraTracao))
        adicionarOuAtualizarManobraTracaoDados(dadosManobraTracao);
}

function manobraTracaoRemovida(codigoManobraTracao) {
    removerManobraTracaoDados(codigoManobraTracao);
}

function isAtualizarManobra(dadosAlteracao) {
    return (
        isCentroCarregamentoAtualizado(dadosAlteracao.CentroCarregamento) &&
        isGridManobraPermiteAtualizarManobras()
    );
}

function isAtualizarManobraTracao(dadosManobraTracao) {
    return (
        isCentroCarregamentoAtualizado(dadosManobraTracao.CentroCarregamento)
    );
}

function isCentroCarregamentoAtualizado(centroCarregamento) {
    var codigoCentroCarregamentoPesquisa = parseInt(_pesquisaManobraAuxiliar.CentroCarregamento.codEntity());

    return (codigoCentroCarregamentoPesquisa > 0) && (centroCarregamento == codigoCentroCarregamentoPesquisa);
}

function isGridManobraPermiteAtualizarManobras() {
    var isPrimeiraPagina = _gridManobra.ObterPaginaAtual() == 0;

    return isPrimeiraPagina;
}