function LoadSignalRCargaEmLote() {
    SignalRCargaEmLoteFechamentoEvent = InformarCargaEmLoteFinalizadoEvent;
    SignalRCargaEmLoteAtualizacaoEvent = AtualizarQuantidadeProcessadosCargaEmLote;
}

function InformarCargaEmLoteFinalizadoEvent(dados) {
    if (_percentualCargaEmLote != null) 
        SetarCargaEmLoteFinalizado(dados);
}

function AtualizarQuantidadeProcessadosCargaEmLote(dados) {
    if (_percentualCargaEmLote != null)
        SetarPercentualCargaEmLote((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal);
}