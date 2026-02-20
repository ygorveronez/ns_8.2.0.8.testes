function LoadSignalRFatura() {
    SignalRFaturaFechamentoEvent = AtualizarQuantidadeDocumentosProcessadosFechamentoFatura;
    SignalRFaturaCancelamentoEvent = AtualizarQuantidadeDocumentosProcessadosCancelamentoFatura;
    SignalRFaturaAtualizacaoEvent = InformarFaturaAtualizadaEvent;

}

function AtualizarQuantidadeDocumentosProcessadosFechamentoFatura(dados) {
    if (_fatura != null && _fechamentoFatura != null && _fatura.Codigo.val() == dados.CodigoFatura) {
        _fechamentoFatura.PercentualProcessadoFechamento.val(((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal).toFixed(0).toString() + "%");
    }
}

function AtualizarQuantidadeDocumentosProcessadosCancelamentoFatura(dados) {
    if (_fatura != null && _cabecalhoFatura != null && _fatura.Codigo.val() == dados.CodigoFatura) {
        _cabecalhoFatura.PercentualProcessadoCancelamento.val(((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal).toFixed(0).toString() + "%");
    }
}

function InformarFaturaAtualizadaEvent(dados) {
    if (_fatura != null && _fatura.Codigo.val() == dados.CodigoFatura)
        editarFatura({ Codigo: dados.CodigoFatura });
}