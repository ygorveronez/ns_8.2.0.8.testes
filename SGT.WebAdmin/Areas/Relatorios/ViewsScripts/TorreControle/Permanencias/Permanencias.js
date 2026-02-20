var _gridPermanencias, _pesquisaPermanencias, _CRUDRelatorio, _relatorioPermanencias, _CRUDFiltrosRelatorio

var PesquisaPermanencias = function () {
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.Permanencias.TipoDoRelatorio.getFieldDescription(), issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Carga = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.Carga.getFieldDescription() });
    this.DataCarregamentoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataInicioCarregamento.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable() });
    this.DataCarregamentoFinal = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataFimCarregamento.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable() });
    this.Placa = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.Placa.getFieldDescription() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.Permanencias.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.Permanencias.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid() });
    this.DataCriacaoCargaInicial = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataInicioCriacaoCarga.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataCriacaoCargaFinal = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataFimCriacaoCarga.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataAgendamentoColetaInicial = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataInicioAgendamentoColeta.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataAgendamentoColetaFinal = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataFimAgendamentoColeta.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataAgendamentoEntregaInicial = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataInicioAgendamentoEntrega.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataAgendamentoEntregaFinal = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.DataFimAgendamentoEntrega.getFieldDescription(), getType: typesKnockout.dateTime });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.Permanencias.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.TorreControle.Permanencias.Cliente.getFieldDescription(), idBtnSearch: guid() });
    this.TipoParada = PropertyEntity({ text: Localization.Resources.Relatorios.TorreControle.Permanencias.TipoParada.getFieldDescription(), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Relatorios.TorreControle.Permanencias.Coleta, Localization.Resources.Relatorios.TorreControle.Permanencias.Entrega), val: ko.observable(""), def: "" });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPermanencias.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Relatorios.TorreControle.Permanencias.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)

    });
}

var CrudRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPermanenciasPDFClick, type: types.event, text: Localization.Resources.Relatorios.TorreControle.Permanencias.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioPermanenciasExcelClick, type: types.event, text: Localization.Resources.Relatorios.TorreControle.Permanencias.GerarPlanilhaExcel, idGrid: guid() });
};

loadRelatorioPermanencias = function () {
    _pesquisaPermanencias = new PesquisaPermanencias();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _CRUDRelatorio = new CrudRelatorio();

    _gridPermanencias = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Permanencias/Pesquisa", _pesquisaPermanencias, null, null, 10, null, null, null, null, 20);
    _gridPermanencias.SetPermitirEdicaoColunas(true);
    _gridPermanencias.SetSalvarPreferenciasGrid(true);
    _gridPermanencias.SetHabilitarScrollHorizontal(true, 200);

    _relatorioPermanencias = new RelatorioGlobal("Relatorios/Permanencias/BuscarDadosRelatorio", _gridPermanencias, function () {
        _relatorioPermanencias.loadRelatorio(function () {
            KoBindings(_pesquisaPermanencias, "knockoutPesquisaPermanencias");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPermanencias");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPermanencias");

            new BuscarTransportadores(_pesquisaPermanencias.Transportador, null, null, true);
            new BuscarGruposPessoas(_pesquisaPermanencias.GrupoPessoas);
            new BuscarFilial(_pesquisaPermanencias.Filial);
            new BuscarClientes(_pesquisaPermanencias.Cliente);

        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPermanencias);
}

function GerarRelatorioPermanenciasPDFClick(e, sender) {
    _relatorioPermanencias.gerarRelatorio("Relatorios/Permanencias/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioPermanenciasExcelClick(e, sender) {
    _relatorioPermanencias.gerarRelatorio("Relatorios/Permanencias/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}