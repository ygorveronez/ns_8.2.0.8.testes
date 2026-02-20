function visualizarHistoricoMonitoramento() {
    exibirHistoricoMonitoramentoPorCarga(_etapaAtualFluxo.Carga.val());
}

function visualizarDetalhesMonitoramento() {
    if (_etapaAtualFluxo.CodigoMonitoramento.val() == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A carga não possui monitoramento");
        return;
    }

    exibirDetalhesMonitoramentoPorCodigo(_etapaAtualFluxo.CodigoMonitoramento.val());
}