function LoadSignalRPagamento() {
    SignalRPagamentoFechamentoEvent = AtualizarQuantidadeDocumentosProcessadosFechamentoPagamento;
    SignalRPagamentoAtualizacaoEvent = InformarPagamentoAtualizadaEvent;

}

function AtualizarQuantidadeDocumentosProcessadosFechamentoPagamento(dados) {
    if (_pagamento != null && _percentualFechamentoPagamento != null && _pagamento.Codigo.val() == dados.CodigoPagamento) {
        SetarPercentualProcessamentoFechamentoPagamento((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal);
    }
}

function InformarPagamentoAtualizadaEvent(dados) {
    if (_pagamento != null && _pagamento.Codigo.val() == dados.CodigoPagamento) {
        _ControlarManualmenteProgresse = true;
        BuscarPagamentoPorCodigo(dados.CodigoPagamento, function () {
            _ControlarManualmenteProgresse = false;
        });
    }

}