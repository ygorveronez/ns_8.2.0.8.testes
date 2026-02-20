/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioProcessamentoModuloControle;
var _pesquisaProcessamentoModuloControle;
var _gridProcessamentoModuloControle;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;

var PesquisaProcessamentoModuloControle = function () {

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.DataInicialEmissao = PropertyEntity({ text: ko.observable("Data emissão inicial: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true)});
    this.DataFinalEmissao = PropertyEntity({ text: ko.observable("Data emissão final: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialIrregularidade = PropertyEntity({ text: ko.observable("Data irregularidade inicial: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalIrregularidade = PropertyEntity({ text: ko.observable("Data irregularidade final: "), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: ko.observable("Situação: "), val: ko.observable(""), def: "", getType: typesKnockout.select, options: EnumSituacaoControleDocumento.obterOpcoesPesquisa(), visible: ko.observable(true) });

    this.Processo = PropertyEntity({ text: ko.observable("Processo: "), val: ko.observable(""), def: "", getType: typesKnockout.select, options: EnumTipoProcessoModuloControle.obterOpcoesPesquisa(), visible: ko.observable(true) });
    this.EstornoMIRO = PropertyEntity({ text: ko.observable("Estorno MIRO"), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

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
            _gridProcessamentoModuloControle.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaProcessamentoModuloControle.Visible.visibleFade()) {
                _pesquisaProcessamentoModuloControle.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaProcessamentoModuloControle.Visible.visibleFade(true);
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

function LoadProcessamentoModuloControle() {
    _pesquisaProcessamentoModuloControle = new PesquisaProcessamentoModuloControle();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridProcessamentoModuloControle = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ProcessamentoModuloControle/Pesquisa", _pesquisaProcessamentoModuloControle);
    _gridProcessamentoModuloControle.SetPermitirEdicaoColunas(true);
    
    _relatorioProcessamentoModuloControle = new RelatorioGlobal("Relatorios/ProcessamentoModuloControle/BuscarDadosRelatorio", _gridProcessamentoModuloControle, function () {
        _relatorioProcessamentoModuloControle.loadRelatorio(function () {
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaProcessamentoModuloControle");
            KoBindings(_pesquisaProcessamentoModuloControle, "knockoutPesquisaProcessamentoModuloControle", false, _CRUDFiltrosRelatorio.Preview.id);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaProcessamentoModuloControle", false);

            new BuscarCargas(_pesquisaProcessamentoModuloControle.Carga);
            new BuscarTransportadores(_pesquisaProcessamentoModuloControle.Transportador);
            new BuscarFilial(_pesquisaProcessamentoModuloControle.Filial);
            new BuscarSetorFuncionario(_pesquisaProcessamentoModuloControle.Setor);
            new BuscarPortfolioModuloControle(_pesquisaProcessamentoModuloControle.Portifolio);
            new BuscarIrregularidades(_pesquisaProcessamentoModuloControle.Irregularidade)


        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaProcessamentoModuloControle);
   
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioProcessamentoModuloControle.gerarRelatorio("Relatorios/ProcessamentoModuloControle/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioProcessamentoModuloControle.gerarRelatorio("Relatorios/ProcessamentoModuloControle/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
