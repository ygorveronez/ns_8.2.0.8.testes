/// <reference path="Chat.js" />

function LoadConexaoSignalRChat() {
    SignalRChatNotificarMensagemUsuarioEvent = AtualizarNotificarMensagemUsuario;
    SignalRChatAtualizarStatusUsuarioEvent = AtualizarStatusUsuario;
}

function AtualizarNotificarMensagemUsuario(dados) {
    if (dados != null) {
        openChatClienteSignalR(dados.idLink, dados.idChat, dados.msg, dados.dataHoraEnvio, dados.quemEnviou);
    }
}

function AtualizarStatusUsuario(dados) {
    if (dados != null) {
        atualizaStatusUsuarioSignalR(dados.codigoUsuario, dados.conectado);
    }
}