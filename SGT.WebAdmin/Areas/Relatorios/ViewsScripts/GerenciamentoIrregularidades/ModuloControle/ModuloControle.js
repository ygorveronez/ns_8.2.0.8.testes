/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioModuloControle;
var _pesquisaModuloControle;
var _gridModuloControle;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;

var PesquisaModuloControle = function () {

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicialEmissao = PropertyEntity({ text: ko.observable("Data emissão inicial: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true)});
    this.DataFinalEmissao = PropertyEntity({ text: ko.observable("Data emissão final: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialIrregularidade = PropertyEntity({ text: ko.observable("Data irregularidade inicial: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalIrregularidade = PropertyEntity({ text: ko.observable("Data irregularidade final: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: ko.observable("Situação: "), val: ko.observable(""), def: "", getType: typesKnockout.select, options: EnumSituacaoControleDocumento.obterOpcoesPesquisa(), visible: ko.observable(true) });

    this.NumeroCTe = PropertyEntity({ text: ko.observable("Numero CTe: "), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true) });
    this.SerieCTe = PropertyEntity({ text: ko.observable("Série CTe: "), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true) });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 63, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", issue: 195, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Portifolio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portifólio:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Irregularidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Irregularidade:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridModuloControle.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaModuloControle.Visible.visibleFade()) {
                _pesquisaModuloControle.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaModuloControle.Visible.visibleFade(true);
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

function LoadModuloControle() {
    _pesquisaModuloControle = new PesquisaModuloControle();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridModuloControle = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ModuloControle/Pesquisa", _pesquisaModuloControle);
    _gridModuloControle.SetPermitirEdicaoColunas(true);
    
    _relatorioModuloControle = new RelatorioGlobal("Relatorios/ModuloControle/BuscarDadosRelatorio", _gridModuloControle, function () {
        _relatorioModuloControle.loadRelatorio(function () {
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaModuloControle");
            KoBindings(_pesquisaModuloControle, "knockoutPesquisaModuloControle", false, _CRUDFiltrosRelatorio.Preview.id);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaModuloControle", false);

            new BuscarCargas(_pesquisaModuloControle.Carga);
            new BuscarTransportadores(_pesquisaModuloControle.Transportador);
            new BuscarFilial(_pesquisaModuloControle.Filial);
            new BuscarSetorFuncionario(_pesquisaModuloControle.Setor);
            new BuscarPortfolioModuloControle(_pesquisaModuloControle.Portifolio);
            new BuscarIrregularidades(_pesquisaModuloControle.Irregularidade)


        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaModuloControle);
   
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioModuloControle.gerarRelatorio("Relatorios/ModuloControle/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioModuloControle.gerarRelatorio("Relatorios/ModuloControle/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
