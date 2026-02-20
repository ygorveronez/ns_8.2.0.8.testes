
function LoadSignalRBaixaTituloReceber() {
    SignalRBaixaTituloReceberGeracaoEvent = AtualizarProgressGeracaoBaixaSignalR;
    SignalRBaixaTituloReceberFinalizacaoEvent = AtualizarProgressFinalizacaoBaixaSignalR;
    SignalRBaixaTituloReceberAtualizacaoEvent = AtualizarTituloBaixaSignalR;
}

function AtualizarTituloBaixaSignalR(retorno) {
    if (_baixaTituloReceber != null && _baixaTituloReceber.Codigo.val() == retorno.CodigoTituloBaixa)
        EditarTituloReceber({ Codigo: retorno.CodigoTituloBaixa });
}

function AtualizarProgressGeracaoBaixaSignalR(retorno) {
    if (_baixaTituloReceber != null && _baixaTituloReceber.Codigo.val() == retorno.CodigoTituloBaixa) {
        _baixaTituloReceber.PercentualProcessadoGeracao.val(((retorno.QuantidadeProcessada * 100) / retorno.QuantidadeTotal).toFixed(0).toString() + "%");
    }
}

function AtualizarProgressFinalizacaoBaixaSignalR(retorno) {
    if (_negociacaoBaixa != null && _baixaTituloReceber != null && _baixaTituloReceber.Codigo.val() == retorno.CodigoTituloBaixa) {
        _progressNegociacaoBaixaReceber.PercentualProcessadoFinalizacao.val(((retorno.QuantidadeProcessada * 100) / retorno.QuantidadeTotal).toFixed(0).toString() + "%");
    }
}