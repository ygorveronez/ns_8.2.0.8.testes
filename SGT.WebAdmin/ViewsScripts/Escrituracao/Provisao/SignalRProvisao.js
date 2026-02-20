function LoadSignalRProvisao() {
    SignalRProvisaoFechamentoEvent = AtualizarQuantidadeDocumentosProcessadosFechamentoProvisao;
    SignalRProvisaoAtualizacaoEvent = InformarProvisaoAtualizadaEvent;

}

function AtualizarQuantidadeDocumentosProcessadosFechamentoProvisao(dados) {
    if (_provisao != null && _percentualFechamentoProvisao != null && _provisao.Codigo.val() == dados.CodigoProvisao) {
        setarPercentualProcessamentoFechamentoProvisao((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal);
    }
}

function InformarProvisaoAtualizadaEvent(dados) {
    if (_provisao != null && _provisao.Codigo.val() == dados.CodigoProvisao) {
        _ControlarManualmenteProgresse = true;
        BuscarProvisaoPorCodigo(dados.CodigoProvisao, function () {
            _ControlarManualmenteProgresse = false;
        });
    }
        
}