function LoadSignalRCarregamentoAutomatico() {
    SignalRCarregamentoAutomaticoFechamentoEvent = InformarCarregamentoAutomaticoFinalizadoEvent;
    SignalRCarregamentoAutomaticoAtualizacaoEvent = AtualizarQuantidadeProcessadosCarregamentoAutomatico;
}

function AtualizarQuantidadeProcessadosCarregamentoAutomatico(dados) {

    if (_percentualCarregamentoAutomatico != null) 
        SetarPercentualCarregamentoAutomatico((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal);
}

function InformarCarregamentoAutomaticoFinalizadoEvent(dados) {
    if (_percentualCarregamentoAutomatico != null) 
        SetarCarregamentoAutomaticoFinalizado(dados);
}