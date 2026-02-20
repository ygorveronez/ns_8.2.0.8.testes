
function LoadConexaoSignalRMensagens() {
    SignalRPedidosMensagemChatEnviadaEvent = processarMensagemChatEnviadaEvent;
    SignalRPedidosMensagemRecebidaEvent = processarMensagemRecebidaEvent;
}

function confirmarLeituraMensagem(codigoMensagem) {
    SignalRPedidos.server.confirmarLeituraMensagem(codigoMensagem);
}