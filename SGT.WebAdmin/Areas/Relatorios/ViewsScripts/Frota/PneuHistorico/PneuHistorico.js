/// <reference path="../../../../../ViewsScripts/Consultas/BandaRodagemPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/DimensaoPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Pneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ServicoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumVidaPneu.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPneuTMS.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoAquisicaoPneu.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />



//*******MAPEAMENTO KNOUCKOUT*******

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridPneuHistorico;
var _pesquisaPneuHistorico;
var _relatorioPneuHistorico;

var PesquisaPneuHistorico = function () {
    this.BandaRodagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Banda de Rodagem:"), idBtnSearch: guid() });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Serviço Veículo:"), idBtnSearch: guid() });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.Dimensao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Dimensão:"), idBtnSearch: guid() });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Marca:"), idBtnSearch: guid() });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Modelo:"), idBtnSearch: guid() });
    this.Pneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Pneu:"), idBtnSearch: guid() });
    this.Vida = PropertyEntity({ val: ko.observable(EnumVidaPneu.Todas), options: EnumVidaPneu.obterOpcoesPesquisa(), def: EnumVidaPneu.Todas, text: "Vida: " });
    this.SomenteSucata = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Somente Sucata?", def: false });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo:"), idBtnSearch: guid() });
    this.SituacaoPneu = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoPneuTMS.Todos), options: EnumSituacaoPneuTMS.obterOpcoesPesquisa(), def: EnumSituacaoPneuTMS.Todos });
    this.DTO = PropertyEntity({ text: "DOT:", maxlength: 500 });
    this.MotivoSucata = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motivo Sucata:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Almoxarifado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Almoxarifado :", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoAquisicao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumTipoAquisicaoPneu.obterOpcoes(), text: "Tipo Aquisicao:", visible: ko.observable(true) });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.UsuarioOperador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Usuario/Operador:"), idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPneuHistorico.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadPneuHistorico() {
    _pesquisaPneuHistorico = new PesquisaPneuHistorico();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridPneuHistorico = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PneuHistorico/Pesquisa", _pesquisaPneuHistorico);

    _gridPneuHistorico.SetPermitirEdicaoColunas(true);
    _gridPneuHistorico.SetQuantidadeLinhasPorPagina(20);

    _relatorioPneuHistorico = new RelatorioGlobal("Relatorios/PneuHistorico/BuscarDadosRelatorio", _gridPneuHistorico, function () {
        _relatorioPneuHistorico.loadRelatorio(function () {
            KoBindings(_pesquisaPneuHistorico, "knockoutPesquisaPneuHistorico", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPneuHistorico", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPneuHistorico", false);

            new BuscarBandaRodagemPneu(_pesquisaPneuHistorico.BandaRodagem);
            new BuscarDimensaoPneu(_pesquisaPneuHistorico.Dimensao);
            new BuscarMarcaPneu(_pesquisaPneuHistorico.Marca);
            new BuscarModeloPneu(_pesquisaPneuHistorico.Modelo);
            new BuscarPneu(_pesquisaPneuHistorico.Pneu);
            new BuscarServicoVeiculo(_pesquisaPneuHistorico.Servico);
            new BuscarVeiculos(_pesquisaPneuHistorico.Veiculo);
            new BuscarMotivoSucateamentoPneu(_pesquisaPneuHistorico.MotivoSucata);
            new BuscarAlmoxarifado(_pesquisaPneuHistorico.Almoxarifado);
      
            
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPneuHistorico);
}

function GerarRelatorioPDFClick() {
    _relatorioPneuHistorico.gerarRelatorio("Relatorios/PneuHistorico/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioPneuHistorico.gerarRelatorio("Relatorios/PneuHistorico/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}