/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CIOT.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ContratoFrete.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Justificativa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridContratoFreteAcrescimoDesconto, _pesquisaContratoFreteAcrescimoDesconto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioContratoFreteAcrescimoDesconto;

var PesquisaContratoFreteAcrescimoDesconto = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid() });

    this.NumCiot = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Número Ciot:", idBtnSearch: guid() });

    this.NumContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Número Contrato Frete:", idBtnSearch: guid() });
    this.NumCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Número da Carga:", idBtnSearch: guid() }); 
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid() });
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def:"" ,text: "Terceiro:", idBtnSearch: guid() });
    

    
    this.DataInicialLancamento = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Inicial:" });
    this.DataFinalLancamento = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Final:" });
    
    


    
   

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFreteAcrescimoDesconto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadContratoFreteAcrescimoDesconto() {
    _pesquisaContratoFreteAcrescimoDesconto = new PesquisaContratoFreteAcrescimoDesconto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridContratoFreteAcrescimoDesconto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ContratoFreteAcrescimoDesconto/Pesquisa", _pesquisaContratoFreteAcrescimoDesconto);

    _gridContratoFreteAcrescimoDesconto.SetPermitirEdicaoColunas(true);
    _gridContratoFreteAcrescimoDesconto.SetQuantidadeLinhasPorPagina(10);

    _relatorioContratoFreteAcrescimoDesconto = new RelatorioGlobal("Relatorios/ContratoFreteAcrescimoDesconto/BuscarDadosRelatorio", _gridContratoFreteAcrescimoDesconto, function () {
        _relatorioContratoFreteAcrescimoDesconto.loadRelatorio(function () {
            KoBindings(_pesquisaContratoFreteAcrescimoDesconto, "knockoutPesquisaContratoFreteAcrescimoDesconto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaContratoFreteAcrescimoDesconto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaContratoFreteAcrescimoDesconto", false);

            new BuscarCIOT(_pesquisaContratoFreteAcrescimoDesconto.NumCiot);
            new BuscarContratoFrete(_pesquisaContratoFreteAcrescimoDesconto.NumContratoFrete);
            new BuscarCargas(_pesquisaContratoFreteAcrescimoDesconto.NumCarga);
            new BuscarJustificativas(_pesquisaContratoFreteAcrescimoDesconto.Justificativa);
            new BuscarClientes(_pesquisaContratoFreteAcrescimoDesconto.Terceiro);


            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaContratoFreteAcrescimoDesconto);
   
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioContratoFreteAcrescimoDesconto.gerarRelatorio("Relatorios/ContratoFreteAcrescimoDesconto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioContratoFreteAcrescimoDesconto.gerarRelatorio("Relatorios/ContratoFreteAcrescimoDesconto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
