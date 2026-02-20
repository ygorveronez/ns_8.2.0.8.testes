function LoadSignalRCancelamentoPagamento() {
    SignalRCancelamentoPagamentoFechamentoEvent = AtualizarQuantidadeDocumentosProcessadosFechamentoCancelamentoPagamento;
    SignalRCancelamentoPagamentoAtualizacaoEvent = InformarCancelamentoPagamentoAtualizadaEvent;
}

function AtualizarQuantidadeDocumentosProcessadosFechamentoCancelamentoPagamento(dados) {
    if (_cancelamentoPagamento != null && _percentualFechamentoCancelamento != null && _cancelamentoPagamento.Codigo.val() == dados.CodigoCancelamentoPagamento) {
        SetarPercentualProcessamentoFechamentoCancelamento((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal);
    }
}

function InformarCancelamentoPagamentoAtualizadaEvent(dados) {
    if (_cancelamentoPagamento != null && _cancelamentoPagamento.Codigo.val() == dados.CodigoCancelamentoPagamento) {
        _ControlarManualmenteProgresse = true;
        BuscarCancelamentoPorCodigo(dados.CodigoCancelamentoPagamento, function () {
            _ControlarManualmenteProgresse = false;
        });
    }
}