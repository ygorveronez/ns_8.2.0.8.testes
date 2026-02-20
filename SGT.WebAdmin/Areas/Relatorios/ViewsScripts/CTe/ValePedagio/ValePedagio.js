/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ValePedagio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoValePedagio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoIntegracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var ValePedagio = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Carga = PropertyEntity({ text: "Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:", type: types.entity, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataCargaInicial = PropertyEntity({ text: "Data Carga Inicial: ", val: ko.observable(""), getType: typesKnockout.date, visible: ko.observable(true), def: "" });
    this.DataCargaFinal = PropertyEntity({ text: "Data Carga Final: ", val: ko.observable(""), getType: typesKnockout.date, visible: ko.observable(true), def: "" });
    this.DataCargaInicial.dateRangeLimit = this.DataCargaFinal;
    this.DataCargaFinal.dateRangeInit = this.DataCargaInicial;

    this.DataCompraVPRInicial = PropertyEntity({ text: "Data compra VPR inicial:", getType: typesKnockout.date, def: "" });
    this.DataCompraVPRFinal = PropertyEntity({ text: "Data compra VPR final:", getType: typesKnockout.date, def: "" });
    this.DataCompraVPRInicial.dateRangeLimit = this.DataCompraVPRFinal;
    this.DataCompraVPRFinal.dateRangeInit = this.DataCompraVPRInicial;

    this.NumeroValePedagio = PropertyEntity({ text: "Número vale pedágio: ", type: types.multiplesEntities, codEntity: ko.observable(0), val: ko.observable("") , idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoValePedagio = PropertyEntity({ text: "Situação do Vale Pedágio", val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoValePedagio.obterOpcoes() });
    this.SituacaoIntegracaoValePedagio = PropertyEntity({ text: "Situação Integração Vale Pedágio", val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoIntegracao.obterOpcoes() });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: "Motorista:", type: types.entity, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veiculo:", type: types.entity, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ text: "Expedidor:", type: types.entity, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Recebedor = PropertyEntity({ text: "Recebedor:", type: types.entity, codEntity: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ExibirCargasAgrupadas = PropertyEntity({ text: "Visualizar as cargas agrupadas (apenas para cargas que foram agrupadas)?", type: types.bool, val: ko.observable(false), visible: ko.observable(!_ConfiguracaoRelatorio.ExibirTodasCargasNoRelatorioDeValePedagio), def: false });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridValePedagio.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_valePedagio.Visible.visibleFade()) {
                _valePedagio.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _valePedagio.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioValePedagio() {
    _valePedagio = new ValePedagio();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridValePedagio = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ValePedagio/Pesquisa", _valePedagio);

    _gridValePedagio.SetPermitirEdicaoColunas(true);
    _gridValePedagio.SetQuantidadeLinhasPorPagina(10);

    _relatorioValePedagio = new RelatorioGlobal("Relatorios/ValePedagio/BuscarDadosRelatorio", _gridValePedagio, function () {
        _relatorioValePedagio.loadRelatorio(function () {
            KoBindings(_valePedagio, "knockoutValePedagio", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDValePedagio", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosValePedagio", false);

            BuscarTiposOperacao(_valePedagio.TipoOperacao);
            BuscarCargas(_valePedagio.Carga);
            BuscarFilial(_valePedagio.Filial);
            BuscarTransportadores(_valePedagio.Transportador);
            BuscarMotoristas(_valePedagio.Motorista);
            BuscarVeiculos(_valePedagio.Veiculo);
            BuscarClientes(_valePedagio.Expedidor);
            BuscarClientes(_valePedagio.Recebedor);
            BuscarValePedagio(_valePedagio.NumeroValePedagio);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
                _valePedagio.ExibirCargasAgrupadas.visible(false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _valePedagio);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioValePedagio.gerarRelatorio("Relatorios/ValePedagio/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioValePedagio.gerarRelatorio("Relatorios/ValePedagio/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}