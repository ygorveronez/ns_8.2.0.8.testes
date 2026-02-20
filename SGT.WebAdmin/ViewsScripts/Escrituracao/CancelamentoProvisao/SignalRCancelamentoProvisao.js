function LoadSignalRCancelamentoProvisao() {
    SignalRCancelamentoProvisaoFechamentoEvent = AtualizarQuantidadeDocumentosProcessadosFechamentoCancelamentoProvisao;
    SignalRCancelamentoProvisaoAtualizacaoEvent = InformarCancelamentoProvisaoAtualizadaEvent;

}

function AtualizarQuantidadeDocumentosProcessadosFechamentoCancelamentoProvisao(dados) {
    if (_cancelamentoProvisao != null && _percentualCancelamentoFechamentoProvisao != null && _cancelamentoProvisao.Codigo.val() == dados.CodigoCancelamentoProvisao) {
        SetarPercentualProcessamentoCancelamentoFechamentoProvisao((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal);
    }
}

function InformarCancelamentoProvisaoAtualizadaEvent(dados) {
    if (_cancelamentoProvisao != null && _cancelamentoProvisao.Codigo.val() == dados.CodigoCancelamentoProvisao) {
        _ControlarManualmenteProgresse = true;
        BuscarCancelamentoProvisaoPorCodigo(dados.CodigoCancelamentoProvisao, function () {
            _ControlarManualmenteProgresse = false;
        });
    }

}