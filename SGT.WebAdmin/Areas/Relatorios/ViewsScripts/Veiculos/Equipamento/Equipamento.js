/// <reference path="../../../../../ViewsScripts/Consultas/ModeloEquipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/MarcaEquipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioEquipamento, _gridEquipamento, _pesquisaEquipamento, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaEquipamento = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataAquisicaoInicial = PropertyEntity({ text: "Data Aquisição Inicial: ", getType: typesKnockout.date });
    this.DataAquisicaoFinal = PropertyEntity({ text: "Data Aquisição Final: ", getType: typesKnockout.date });
    this.DataAquisicaoInicial.dateRangeLimit = this.DataAquisicaoFinal;
    this.DataAquisicaoFinal.dateRangeInit = this.DataAquisicaoInicial;

    this.AnoFabricacao = PropertyEntity({ text: "Ano Fabricação: ", getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro Resultado:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo:", idBtnSearch: guid() });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid() });
    this.Segmento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmento:", idBtnSearch: guid() });
    this.Neokohm = PropertyEntity({ text: "Neokohm:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridEquipamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadEquipamento() {
    _pesquisaEquipamento = new PesquisaEquipamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridEquipamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Equipamento/Pesquisa", _pesquisaEquipamento, null, null, 10);
    _gridEquipamento.SetPermitirEdicaoColunas(true);

    _relatorioEquipamento = new RelatorioGlobal("Relatorios/Equipamento/BuscarDadosRelatorio", _gridEquipamento, function () {
        _relatorioEquipamento.loadRelatorio(function () {
            KoBindings(_pesquisaEquipamento, "knockoutPesquisaEquipamento", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaEquipamento", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaEquipamento", false);

            new BuscarModeloEquipamentos(_pesquisaEquipamento.Modelo);
            new BuscarMarcaEquipamentos(_pesquisaEquipamento.Marca);
            new BuscarSegmentoVeiculo(_pesquisaEquipamento.Segmento);
            new BuscarVeiculos(_pesquisaEquipamento.Veiculo);
            new BuscarCentroResultado(_pesquisaEquipamento.CentroResultado);
            

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaEquipamento);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioEquipamento.gerarRelatorio("Relatorios/Equipamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioEquipamento.gerarRelatorio("Relatorios/Equipamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}