
//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaConfiguracaoSubcontratacaoTabelaFrete;

var PesquisaConfiguracaoSubcontratacaoTabelaFrete = function () {
    
    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 0 });
    this.TabelaFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tabela de Frete:", idBtnSearch: guid(), issue: 0 });
    
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioConfiguracaoSubcontratacaoTabelaFrete() {
    _pesquisaConfiguracaoSubcontratacaoTabelaFrete = new PesquisaConfiguracaoSubcontratacaoTabelaFrete();
    KoBindings(_pesquisaConfiguracaoSubcontratacaoTabelaFrete, "knockoutPesquisaConfiguracaoSubcontratacaoTabelaFrete", false);

    new BuscarGruposPessoas(_pesquisaConfiguracaoSubcontratacaoTabelaFrete.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarTabelasDeFrete(_pesquisaConfiguracaoSubcontratacaoTabelaFrete.TabelaFrete, null, EnumTipoTabelaFrete.tabelaCliente);
}

function GerarRelatorioPDFClick(e, sender) {
    GerarRelatorioConfiguracaoSubcontratacaoTabelaFrete(EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    GerarRelatorioConfiguracaoSubcontratacaoTabelaFrete(EnumTipoArquivoRelatorio.XLS);
}

function GerarRelatorioConfiguracaoSubcontratacaoTabelaFrete(tipoArquivo) {
    var dados = RetornarObjetoPesquisa(_pesquisaConfiguracaoSubcontratacaoTabelaFrete);

    dados["TipoArquivo"] = tipoArquivo;

    executarDownload("Relatorios/ConfiguracaoSubcontratacaoTabelaFrete/GerarRelatorio", dados);
}