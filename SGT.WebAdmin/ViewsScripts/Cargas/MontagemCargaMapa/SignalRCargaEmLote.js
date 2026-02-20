function LoadSignalRCargaEmLote() {
    SignalRCargaEmLoteFechamentoEvent = InformarCargaEmLoteFinalizadoEvent;
    SignalRCargaEmLoteAtualizacaoEvent = AtualizarQuantidadeProcessadosCargaEmLote;
    SignalRCargaBackgroundFinalizadoEvent = InformarCargaBackgroundFinalizadoEvent;
}

function InformarCargaEmLoteFinalizadoEvent(dados) {
    if (_percentualCargaEmLote != null) {
        var cod_sessao = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
        if (dados.SessaoRoterizador == cod_sessao) {
            SetarCargaEmLoteFinalizado(dados);
        }
    }
}

function AtualizarQuantidadeProcessadosCargaEmLote(dados) {
    if (_percentualCargaEmLote != null) {
        var cod_sessao = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
        if (dados.SessaoRoterizador == cod_sessao) {
            SetarPercentualCargaEmLote((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal);
        }
    }
}

function InformarCargaBackgroundFinalizadoEvent(dados) {
    if (_percentualCargaEmLote != null) {
        var cod_sessao = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
        if (dados.SessaoRoterizador == cod_sessao) {
            SetarGerandoCargaBackgroundFinalizado(dados);
        }
    }
}