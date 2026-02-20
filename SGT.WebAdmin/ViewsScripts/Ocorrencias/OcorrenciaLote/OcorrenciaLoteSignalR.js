/// <reference path="OcorrenciaLote.js" />

function LoadConexaoSignalROcorrenciaLote() {
    SignalROcorrenciaLoteAlteradaEvent = VerificarOcorrenciaLoteAlteradaEvent;
}

function VerificarOcorrenciaLoteAlteradaEvent(retorno) {
    if (retorno.CodigoOcorrenciaLote == _ocorrenciaLote.Codigo.val()) {
        buscarOcorrenciaLote(retorno.CodigoOcorrenciaLote);
    }
}