var _gridDevolucaoNotasFiscais, _pesquisaDevolucaoNotasFiscais, _CRUDRelatorio, _relatorioDevolucaoNotasFiscais, _CRUDFiltrosRelatorio

var PesquisaDevolucaoNotasFiscais = function () {
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.TipoDoRelatorio.getFieldDescription(), issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicialEmissaoNFD = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DataInicialEmissaoNFD.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: ko.observable(false) });
    this.DataFinalEmissaoNFD = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DataFinalEmissaoNFD.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: ko.observable(false) });
    this.DataInicialChamado = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DataInicialChamado.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: ko.observable(false) });
    this.DataFinalChamado = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.DataFinalChamado.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(), required: ko.observable(false) });
    this.NotaFiscalDevolucao = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.NotasFiscaisDevolucao.getFieldDescription() });
    this.NotaFiscalOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.NotasFiscaisOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.TiposOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.GrupoTipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.GrupoTipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Cargas.getFieldDescription(), idBtnSearch: guid() });
    this.Chamado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Chamados.getFieldDescription(), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Cliente.getFieldDescription(), idBtnSearch: guid() });
    this.PedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.PedidoEmbarcador.getFieldDescription() });
    this.PedidoCliente = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.PedidoCliente.getFieldDescription() });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
        _gridDevolucaoNotasFiscais.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)

    });
}

var CrudRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioDevolucaoNotasFiscaisPDFClick, type: types.event, text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioDevolucaoNotasFiscaisExcelClick, type: types.event, text: Localization.Resources.Relatorios.TorreControle.DevolucaoNotasFiscais.GerarPlanilhaExcel, idGrid: guid() });
};

loadRelatorioDevolucaoNotasFiscais = function () {
    _pesquisaDevolucaoNotasFiscais = new PesquisaDevolucaoNotasFiscais();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _CRUDRelatorio = new CrudRelatorio();

    _gridDevolucaoNotasFiscais = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/DevolucaoNotasFiscais/Pesquisa", _pesquisaDevolucaoNotasFiscais, null, null, 10, null, null, null, null, 20);
    _gridDevolucaoNotasFiscais.SetPermitirEdicaoColunas(true);
    _gridDevolucaoNotasFiscais.SetSalvarPreferenciasGrid(true);
    _gridDevolucaoNotasFiscais.SetHabilitarScrollHorizontal(true, 200);

    _relatorioDevolucaoNotasFiscais = new RelatorioGlobal("Relatorios/DevolucaoNotasFiscais/BuscarDadosRelatorio", _gridDevolucaoNotasFiscais, function () {
        _relatorioDevolucaoNotasFiscais.loadRelatorio(function () {
            KoBindings(_pesquisaDevolucaoNotasFiscais, "knockoutPesquisaDevolucaoNotasFiscais");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaDevolucaoNotasFiscais");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaDevolucaoNotasFiscais");

            new BuscarTransportadores(_pesquisaDevolucaoNotasFiscais.Transportador, null, null, true);
            new BuscarClientes(_pesquisaDevolucaoNotasFiscais.Cliente);
            new BuscarCargas(_pesquisaDevolucaoNotasFiscais.Carga);
            new BuscarTiposOperacao(_pesquisaDevolucaoNotasFiscais.TipoOperacao);
            new BuscarXMLNotaFiscal(_pesquisaDevolucaoNotasFiscais.NotaFiscalOrigem);
            new BuscarGrupoTipoOperacao(_pesquisaDevolucaoNotasFiscais.GrupoTipoOperacao);
            new BuscarChamadosParaOcorrencia(_pesquisaDevolucaoNotasFiscais.Chamado);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDevolucaoNotasFiscais);
}

function GerarRelatorioDevolucaoNotasFiscaisPDFClick(e, sender) {
    _relatorioDevolucaoNotasFiscais.gerarRelatorio("Relatorios/DevolucaoNotasFiscais/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioDevolucaoNotasFiscaisExcelClick(e, sender) {
    _relatorioDevolucaoNotasFiscais.gerarRelatorio("Relatorios/DevolucaoNotasFiscais/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}