/// <reference path="../../../../../js/Global/Globais.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRotaFrete, _pesquisaRotaFrete, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioRotaFrete;

var PesquisaRotaFrete = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.TipoDoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Relatorios.Fretes.RotaFrete.Descricao.getFieldDescription() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.Remetente.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.Origem.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.Destinatario.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.Destino.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.EstadoDeDestino.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Fretes.RotaFrete.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Relatorios.Fretes.RotaFrete.Situacao.getFieldDescription(), options: [{ text: Localization.Resources.Relatorios.Fretes.RotaFrete.Todos, value: "" }].concat(_status), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.RotaExclusivaCompraValePedagio = PropertyEntity({ text: Localization.Resources.Relatorios.Fretes.RotaFrete.RotaExclusivaCompraValePedagio.getFieldDescription(), val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Relatorios.Fretes.RotaFrete.Sim, Localization.Resources.Relatorios.Fretes.RotaFrete.Nao), def: "", visible: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridRotaFrete.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Relatorios.Fretes.RotaFrete.Preview.getFieldDescription(), idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaRotaFrete.Visible.visibleFade()) {
                _pesquisaRotaFrete.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaRotaFrete.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Relatorios.Fretes.RotaFrete.Avancada.getFieldDescription(), icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Relatorios.Fretes.RotaFrete.GerarPDF.getFieldDescription() });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Relatorios.Fretes.RotaFrete.GerarPlanilhaExcel.getFieldDescription(), idGrid: guid() });
};

//*******EVENTOS*******

function LoadRotaFrete() {
    _pesquisaRotaFrete = new PesquisaRotaFrete();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridRotaFrete = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/RotaFrete/Pesquisa", _pesquisaRotaFrete);

    _gridRotaFrete.SetPermitirEdicaoColunas(true);
    _gridRotaFrete.SetQuantidadeLinhasPorPagina(10);

    _relatorioRotaFrete = new RelatorioGlobal("Relatorios/RotaFrete/BuscarDadosRelatorio", _gridRotaFrete, function () {
        _relatorioRotaFrete.loadRelatorio(function () {
            KoBindings(_pesquisaRotaFrete, "knockoutPesquisaRotaFrete", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaRotaFrete", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaRotaFrete", false);

            new BuscarClientes(_pesquisaRotaFrete.Remetente);
            new BuscarClientes(_pesquisaRotaFrete.Destinatario);
            new BuscarLocalidades(_pesquisaRotaFrete.Origem);
            new BuscarLocalidades(_pesquisaRotaFrete.Destino);
            new BuscarEstados(_pesquisaRotaFrete.EstadoDestino);
            new BuscarGruposPessoas(_pesquisaRotaFrete.GrupoPessoas);
            new BuscarTiposOperacao(_pesquisaRotaFrete.TipoOperacao);

            ConfigurarIntegracaoValePedagio();

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaRotaFrete);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioRotaFrete.gerarRelatorio("Relatorios/RotaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioRotaFrete.gerarRelatorio("Relatorios/RotaFrete/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ConfigurarIntegracaoValePedagio() {
    executarReST("RotaFrete/ObterConfiguracao", {}, function (r) {
        if (r.Success && r.Data) {
            var data = r.Data;
            if (data.TemIntegracaoRepomRest) {
                _pesquisaRotaFrete.RotaExclusivaCompraValePedagio.visible(true);
            }
        }
    });
}