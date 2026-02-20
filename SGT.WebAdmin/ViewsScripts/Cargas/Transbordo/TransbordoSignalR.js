/// <reference path="Transbordo.js" />

function LoadConexaoSignalRTransbordo() {
    SignalRTransbordoAtualizadoEvent = VerificarTransbordoAtualizadoEvent;
}

function VerificarTransbordoAtualizadoEvent(retorno) {
    if (retorno.CodigoTransbordo == _transbordo.Codigo.val()) {
        buscarTransbordo(retorno.CodigoTransbordo);
    }
}