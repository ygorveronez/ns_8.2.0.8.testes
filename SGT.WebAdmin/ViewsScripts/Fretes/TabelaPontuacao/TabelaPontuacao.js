
function loadTabelaPontuacao() {
    loadPontuacaoPorTempoAtividade();
    loadPontuacaoPorQuantidadeCarga();
    loadPontuacaoPorQuantidadeCargaGanhaCotacao();
    loadPontuacaoPorTipoOperacao();
    loadPontuacaoPorTipoCarga();
    loadPontuacaoPorModeloCarroceria();
    loadPontuacaoPorPessoaClassificacao();
}

function tabModeloCarroceriaClick() {
    limparAbas()
    _gridPontuacaoPorModeloCarroceria.CarregarGrid();
    HeaderAuditoria("PontuacaoPorModeloCarroceria", _pontuacaoPorModeloCarroceria);
}

function tabTempoAtividadeClick() {
    limparAbas()
    _gridPontuacaoPorTempoAtividade.CarregarGrid();
    HeaderAuditoria("PontuacaoPorTempoAtividade", _pontuacaoPorTempoAtividade);
}

function tabPessoaClassificacaoClick() {
    limparAbas()
    _gridPontuacaoPorPessoaClassificacao.CarregarGrid();
    HeaderAuditoria("PontuacaoPorPessoaClassificacao", _pontuacaoPorPessoaClassificacao);
}

function tabQuantidadeCargaClick() {
    limparAbas()
    _gridPontuacaoPorQuantidadeCarga.CarregarGrid();
    HeaderAuditoria("PontuacaoPorQuantidadeCarga", _pontuacaoPorQuantidadeCarga);
}

function tabQuantidadeCargaGanhaCotacaoClick() {
    limparAbas()
    _gridPontuacaoPorQuantidadeCargaGanhaCotacao.CarregarGrid();
    HeaderAuditoria("PontuacaoPorQuantidadeCargaGanhaCotacao", _pontuacaoPorQuantidadeCargaGanhaCotacao);
}

function tabTipoOperacaoClick() {
    limparAbas()
    _gridPontuacaoPorTipoOperacao.CarregarGrid();
    HeaderAuditoria("PontuacaoPorTipoOperacao", _pontuacaoPorTipoOperacao);
}

function tabTipoCargaClick() {
    limparAbas()
    _gridPontuacaoPorTipoCarga.CarregarGrid();
    HeaderAuditoria("PontuacaoPorTipoCarga", _pontuacaoPorTipoCarga);
}


function limparAbas() {
    limparCamposPontuacaoPorModeloCarroceria();
    limparCamposPontuacaoPorQuantidadeCarga();
    limparCamposPontuacaoPorTempoAtividade();
    limparCamposPontuacaoPorTipoOperacao();
}