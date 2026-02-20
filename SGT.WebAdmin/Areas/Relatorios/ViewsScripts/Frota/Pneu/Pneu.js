/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoBandaRodagemPneu.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPneuTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaPneu.js" />

//********MAPEAMENTO KNOCKOUT*********

var _relatorioPneu, _gridPneuRelatorio, _pesquisaPneuRelatorio, _CRUDRelatorioPneu, _CRUDFiltrosRelatorioPneu;

var PesquisaPneuRelatorio = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Pneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pneu: ", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid() });
    this.ModeloPneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo do pneu: ", idBtnSearch: guid() });
    this.MarcaPneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca do pneu: ", idBtnSearch: guid() });
    this.TipoBandaRodagem = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoBandaRodagemPneu.obterOpcoesPesquisa(), text: "Tipo de banda: " });
    this.StatusPneu = PropertyEntity({ text: "Status do pneu: ", options: EnumSituacaoPneuTMS.obterOpcoesPesquisa(), val: ko.observable(0), def: 0 });
    this.Movimentacao = PropertyEntity({ text: "Movimentação: ", options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.VidaUtil = PropertyEntity({ text: "Vida Útil: ", val: ko.observable(EnumVidaPneu.Todas), options: EnumVidaPneu.obterOpcoesPesquisa(), def: EnumVidaPneu.Todas });
};

var CRUDFiltrosRelatorioPneu = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPneuRelatorio.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    /* BUSCA AVANÇADA
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPneuRelatorio.Visible.visibleFade() === true) {
                _pesquisaPneuRelatorio.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPneuRelatorio.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });*/
}

var CRUDRelatorioPneu = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadPneuRelatorio() {
    _pesquisaPneuRelatorio = new PesquisaPneuRelatorio();
    _CRUDRelatorioPneu = new CRUDRelatorioPneu();
    _CRUDFiltrosRelatorioPneu = new CRUDFiltrosRelatorioPneu();

    _gridPneuRelatorio = new GridView(_CRUDFiltrosRelatorioPneu.Preview.idGrid, "Relatorios/Pneu/Pesquisa", _pesquisaPneuRelatorio, null, null, 10);
    _gridPneuRelatorio.SetPermitirEdicaoColunas(true);

    _relatorioPneu = new RelatorioGlobal("Relatorios/Pneu/BuscarDadosRelatorio", _gridPneuRelatorio, function () {
        _relatorioPneu.loadRelatorio(function () {
            KoBindings(_pesquisaPneuRelatorio, "knockoutPesquisaPneu", false);
            KoBindings(_CRUDRelatorioPneu, "knockoutCRUDPesquisaPneu", false);
            KoBindings(_CRUDFiltrosRelatorioPneu, "knockoutCRUDFiltrosPesquisaPneu", false);

            BuscarPneu(_pesquisaPneuRelatorio.Pneu);
            BuscarVeiculos(_pesquisaPneuRelatorio.Veiculo);
            BuscarModeloPneu(_pesquisaPneuRelatorio.ModeloPneu);
            BuscarMarcaPneu(_pesquisaPneuRelatorio.MarcaPneu);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPneuRelatorio);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPneu.gerarRelatorio("Relatorios/Pneu/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPneu.gerarRelatorio("Relatorios/Pneu/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}