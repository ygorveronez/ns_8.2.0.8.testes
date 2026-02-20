/// <reference path="FilaCarregamentoReversa.js" />

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoReversaSignalR() {
    SignalRFilaCarregamentoReversaAlteradaEvent = filaCarregamentoReversaAlterada;
}

/*
 * Declaração das Funções
 */

function filaCarregamentoReversaAlterada(dadosAlteracao) {
    if (isAtualizarDados(dadosAlteracao))
        recarregarGridFilaCarregamentoReversa();
}

function isAtualizarDados(dadosAlteracao) {
    return (
        isCentroCarregamentoAtualizado(dadosAlteracao.CodigoCentroCarregamento) &&
        isGrupoModeloVeicularCargaAtualizado(dadosAlteracao.CodigoGrupoModeloVeicularCarga) &&
        isModeloVeicularCargaAtualizado(dadosAlteracao.CodigoModeloVeicularCarga)
    );
}

function isCentroCarregamentoAtualizado(centroCarregamento) {
    var codigoCentroCarregamento = parseInt(_pesquisaFilaCarregamentoReversaAuxiliar.CentroCarregamento.codEntity());

    return (codigoCentroCarregamento > 0) && (codigoCentroCarregamento == centroCarregamento);
}

function isGrupoModeloVeicularCargaAtualizado(grupoModeloVeicularCarga) {
    var codigoGrupoModeloVeicular = parseInt(_pesquisaFilaCarregamentoReversaAuxiliar.GrupoModeloVeicular.codEntity());

    return (codigoGrupoModeloVeicular === 0) || (grupoModeloVeicularCarga == 0) || (codigoGrupoModeloVeicular == grupoModeloVeicularCarga);
}

function isModeloVeicularCargaAtualizado(modeloVeicularCarga) {
    var codigoModeloVeicular = parseInt(_pesquisaFilaCarregamentoReversaAuxiliar.ModeloVeicular.codEntity());

    return (codigoModeloVeicular === 0) || (modeloVeicularCarga == 0) || (codigoModeloVeicular == modeloVeicularCarga);
}
