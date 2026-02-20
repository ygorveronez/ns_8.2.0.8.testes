
function LoadConexaoSignalRControleColetaEntrega() {
    SignalRColetaEntregaMensagemChatEnviadaEvent = processarMensagemChatEnviadaEvent;
    SignalRColetaEntregaMensagemRecebidaEvent = processarMensagemRecebidaEvent;
    SignalRColetaEntregaAtendimentoAtualizadoEvent = atendimentoAtualizadoEvent;
    SignalRColetaEntregaNovoAtendimentoEvent = novoAtendimentoEvent;
}

function confirmarLeituraMensagem(codigoMensagem) {
    SignalRColetaEntrega.server.confirmarLeituraMensagem(codigoMensagem);
}