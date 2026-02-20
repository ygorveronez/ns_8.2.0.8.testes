/// <reference path="../../Global/SignalR/SignalR.js" />


function LoadConexaoSignalRAvarias() {
    SignalRAvariaAlteradaEvent = VerificarAvariaAlteradaEvent;
}

function VerificarAvariaAlteradaEvent(retorno) {
    if (retorno.CodigoLote == _lote.Codigo.val()) {
        _RequisicaoIniciada = true;
        BuscarLotePorCodigo(function () {
            _RequisicaoIniciada = false;
        });
    }
}