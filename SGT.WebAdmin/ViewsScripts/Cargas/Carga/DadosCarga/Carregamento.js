function CarregamentoClick(e) {
    _pesquisaCarga.CodigoCargaEmbarcador.val("");
    _pesquisaCarga.Carregamento.val(e.Carregamento.val());
    _pesquisaCarga.Carregamento.codEntity(e.Carregamento.codEntity());
    _pesquisaCarga.Pesquisar.eventClick(e);
}