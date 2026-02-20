/// <reference path="FluxoEncerramentoCarga.js" />

function LoadSignalREncerramento() {
    SignalRCargaEncerramentoAtualizadoEvent = VerificarEncerramentoAlterado;
}

function VerificarEncerramentoAlterado(retorno) {
    if (retorno.CodigoEncerramento == _fluxoEncerramentoCarga.Codigo.val()) {
        BuscarFluxoEncerramentoCargaPorCodigo(false);
    }
}