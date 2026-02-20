/// <reference path="../../../../../ViewsScripts/Consultas/Almoxarifado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/DimensaoPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/BandaRodagemPneu.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioPneuCustoEstoque, _gridPneuCustoEstoque, _pesquisaPneuCustoEstoque, _CRUDRelatorio, _CRUDFiltrosRelatorio;


var PesquisaPneuCustoEstoque = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataAquisicaoInicial = PropertyEntity({ text: "Data de Aquisição Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataAquisicaoFinal = PropertyEntity({ text: "Data de Aquisição Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataAquisicaoInicial.dateRangeLimit = this.DataAquisicaoFinal;
    this.DataAquisicaoFinal.dateRangeInit = this.DataAquisicaoInicial;

    this.Vida = PropertyEntity({ text: "Vida:", options: EnumVidaPneu.obterOpcoesPesquisa(), val: ko.observable(EnumVidaPneu.Todas), def: EnumVidaPneu.Todas });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Almoxarifado:", idBtnSearch: guid() });
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banda de Rodagem:", idBtnSearch: guid() });
    this.Dimensao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Dimnesão:", idBtnSearch: guid() });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo:", idBtnSearch: guid() });
    this.Pneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pneu:", idBtnSearch: guid() });
    this.ServicoVeiculoFrota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço Veículo:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.EstadoPneu = PropertyEntity({ text: "Estado do Pneu:", getType: typesKnockout.selectMultiple, options: EnumEstadoPneu.obterOpcoesPesquisa(), val: ko.observable(new Array()), def: new Array() });
    this.Situacao = PropertyEntity({ text: "Situação:", getType: typesKnockout.selectMultiple , options: EnumSituacaoPneuTMS.obterOpcoes(), val: ko.observable(new Array()), def: new Array() });

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report })

};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPneuCustoEstoque.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    /* BUSCA AVANÇADA
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPneuCustoEstoque.Visible.visibleFade() === true) {
                _pesquisaPneuCustoEstoque.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPneuCustoEstoque.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });*/
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadPneuCustoEstoque() {
    _pesquisaPneuCustoEstoque = new PesquisaPneuCustoEstoque();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPneuCustoEstoque = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PneuCustoEstoque/Pesquisa", _pesquisaPneuCustoEstoque, null, null, 10);
    _gridPneuCustoEstoque.SetPermitirEdicaoColunas(true);

    _relatorioPneuCustoEstoque = new RelatorioGlobal("Relatorios/PneuCustoEstoque/BuscarDadosRelatorio", _gridPneuCustoEstoque, function () {
        _relatorioPneuCustoEstoque.loadRelatorio(function () {
            KoBindings(_pesquisaPneuCustoEstoque, "knockoutPesquisaPneuCustoEstoque", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPneuCustoEstoque", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPneuCustoEstoque", false);

            new BuscarAlmoxarifado(_pesquisaPneuCustoEstoque.Almoxarifado);
            new BuscarBandaRodagemPneu(_pesquisaPneuCustoEstoque.BandaRodagem);
            new BuscarDimensaoPneu(_pesquisaPneuCustoEstoque.Dimensao);
            new BuscarMarcaPneu(_pesquisaPneuCustoEstoque.Marca);
            new BuscarModeloPneu(_pesquisaPneuCustoEstoque.Modelo);
            new BuscarPneu(_pesquisaPneuCustoEstoque.Pneu);
            new BuscarServicoVeiculo(_pesquisaPneuCustoEstoque.ServicoVeiculoFrota);
            new BuscarVeiculos(_pesquisaPneuCustoEstoque.Veiculo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPneuCustoEstoque);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPneuCustoEstoque.gerarRelatorio("Relatorios/PneuCustoEstoque/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPneuCustoEstoque.gerarRelatorio("Relatorios/PneuCustoEstoque/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}