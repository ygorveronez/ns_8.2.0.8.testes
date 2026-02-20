function LoadSignalRNFSManual() {
    SignalRInformarLancamentoNFSManualAtualizadaEvent = InformarLancamentoNFSManualAtualizadaEvent;
}

function InformarLancamentoNFSManualAtualizadaEvent(retorno) {
    if (retorno.CodigoNFSManual == _nfsManual.Codigo.val()) {
        BuscarNFSPorCodigo(_nfsManual.Codigo.val(), null, true);
    }
}