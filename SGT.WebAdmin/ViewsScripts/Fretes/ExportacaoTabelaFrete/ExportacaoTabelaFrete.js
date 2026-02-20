var _exportacaoTabelaFrete;

var ExportacaoTabelaFrete = function () {
    this.ExportarValorFrete = PropertyEntity({ eventClick: ExportarValorFreteClick, type: types.event, text: "Exportar Valor do Frete", idGrid: guid(), visible: ko.observable(true), icon: "fa fa-download" });
    this.ExportarPedagio = PropertyEntity({ eventClick: ExportarPedagioClick, type: types.event, text: "Exportar Valor do Pedágio", idGrid: guid(), visible: ko.observable(true), icon: "fa fa-download" });
};

function LoadExportacaoTabelaFrete() {
    _exportacaoTabelaFrete = new ExportacaoTabelaFrete();
    KoBindings(_exportacaoTabelaFrete, "knockoutExportacaoTabelaFrete");
}

function ExportarValorFreteClick() {
    executarDownload("ExportacaoTabelaFrete/DownloadValorFrete");
}

function ExportarPedagioClick() {
    executarDownload("ExportacaoTabelaFrete/DownloadValorPedagio");
}