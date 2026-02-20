/// <reference path="FilaCarregamento.js" />
/// <reference path="FilaCarregamentoResumo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilaCarregamentoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoFilaCarregamentoAlteracao.js" />

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoSignalR() {
    SignalRFilaCarregamentoAlteradaEvent = filaCarregamentoAlterada;
    SignalRFilaCarregamentoSituacaoAlteradaEvent = situacaoFilaCarregamentoAlterada;
}

/*
 * Declaração das Funções
 */

function filaCarregamentoAlterada(dadosAlteracao) {
    if (isAtualizarDados(dadosAlteracao)) {
        if (isAtualizarGridNaFila(dadosAlteracao)) {
            _gridFilaCarregamento.CarregarGrid();
            recarregarGraficoFilaCarregamentoResumo();
            recarregarTotalizadoresPorSituacaoFilaCarregamento();
        }

        if (isAtualizarGridEmTransicao(dadosAlteracao))
            _gridFilaEmTransicao.CarregarGrid();

        if (isAtualizarGridMotorista(dadosAlteracao))
            _gridFilaMotorista.CarregarGrid();
    }
}

function isAtualizarDados(dadosAlteracao) {
    return (
        (isCentroCarregamentoAtualizado(dadosAlteracao.CentrosCarregamento) || isFilialAtualizada(dadosAlteracao.Filiais)) &&
        isGrupoModeloVeicularCargaAtualizado(dadosAlteracao.GruposModelosVeicularesCarga) &&
        isModeloVeicularCargaAtualizado(dadosAlteracao.ModelosVeicularesCarga)
    );
}

function isAtualizarGridEmTransicao(dadosAlteracao) {
    if (!isExibirTodasFilasCarregamento())
        return false;

    var isDadosFilaEmTrasicaoAtualizados = dadosAlteracao.Tipos.indexOf(EnumTipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculoEmTransicao) > -1;
    var isPrimeiraPagina = _gridFilaEmTransicao.ObterPaginaAtual() == 0;

    return isDadosFilaEmTrasicaoAtualizados && (isPrimeiraPagina || dadosAlteracao.RecarregarInformacoes);
}

function isAtualizarGridMotorista(dadosAlteracao) {
    if (!isExibirTodasFilasCarregamento())
        return false;

    var isDadosFilaMotoristaAtualizados = dadosAlteracao.Tipos.indexOf(EnumTipoFilaCarregamentoAlteracao.FilaCarregamentoMotorista) > -1;
    var isPrimeiraPagina = _gridFilaMotorista.ObterPaginaAtual() == 0;

    return isDadosFilaMotoristaAtualizados && (isPrimeiraPagina || dadosAlteracao.RecarregarInformacoes);
}

function isAtualizarGridNaFila(dadosAlteracao) {
    var isPrimeiraPagina = _gridFilaCarregamento.ObterPaginaAtual() == 0;
    var isDadosFilaAtualizados = dadosAlteracao.Tipos.indexOf(EnumTipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo) > -1;

    return isDadosFilaAtualizados && (isPrimeiraPagina || dadosAlteracao.RecarregarInformacoes);
}

function isCentroCarregamentoAtualizado(centrosCarregamento) {
    var codigoCentroCarregamento = parseInt(_pesquisaFilaCarregamentoAuxiliar.CentroCarregamento.codEntity());

    return (codigoCentroCarregamento > 0) && ((centrosCarregamento.length == 0) || (centrosCarregamento.indexOf(codigoCentroCarregamento) > -1));
}

function isFilialAtualizada(filiais) {
    var codigoFilial = parseInt(_pesquisaFilaCarregamentoAuxiliar.Filial.codEntity());

    return (codigoFilial > 0) && ((filiais.length == 0) || (filiais.indexOf(codigoFilial) > -1));
}

function isGrupoModeloVeicularCargaAtualizado(gruposModelosVeicularesCarga) {
    var codigoGrupoModeloVeicular = parseInt(_pesquisaFilaCarregamentoAuxiliar.GrupoModeloVeicular.codEntity());

    return (codigoGrupoModeloVeicular === 0) || (gruposModelosVeicularesCarga.indexOf(codigoGrupoModeloVeicular) > -1);
}

function isModeloVeicularCargaAtualizado(modelosVeicularesCarga) {
    var listaCodigoModeloVeicularCarga = _pesquisaFilaCarregamentoAuxiliar.ModeloVeicular.multiplesEntities().map(modelo => parseInt(modelo.Codigo));

    if (listaCodigoModeloVeicularCarga.length == 0)
        return true;

    return listaCodigoModeloVeicularCarga.filter(codigoModeloVeicular => modelosVeicularesCarga.indexOf(codigoModeloVeicular) > -1).length > 0;
}

function situacaoFilaCarregamentoAlterada(dadosAlteracao) {
    $("#grid-fila-carregamento tr#" + dadosAlteracao.Codigo).css("background-color", dadosAlteracao.CorLinha);
}
