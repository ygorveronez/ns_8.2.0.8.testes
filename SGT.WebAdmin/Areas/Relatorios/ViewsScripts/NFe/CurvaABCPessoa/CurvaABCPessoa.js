/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumCompraVenda.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumOrdenacaoCurvaABC.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCurvaABCPessoas, _gridCurvaABCPessoas, _pesquisaCurvaABCPessoas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoCompraVenda = [
    { text: "Compra", value: EnumCompraVenda.Compra },
    { text: "Venda", value: EnumCompraVenda.Venda }
];

var _ordemCurvaABC = [
    { text: "Quantidade", value: EnumOrdenacaoCurvaABC.Quantidade },
    { text: "Valor", value: EnumOrdenacaoCurvaABC.Valor }
];

var PesquisaCurvaABCPessoas = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumCompraVenda.Venda), options: _tipoCompraVenda, def: EnumCompraVenda.Venda, text: "Tipo Movimento: " });
    this.Ordenar = PropertyEntity({ val: ko.observable(EnumOrdenacaoCurvaABC.Valor), options: _ordemCurvaABC, def: EnumOrdenacaoCurvaABC.Valor, text: "Ordenação: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCurvaABCPessoas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioCurvaABCPessoas() {

    _pesquisaCurvaABCPessoas = new PesquisaCurvaABCPessoas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCurvaABCPessoas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CurvaABCPessoa/Pesquisa", _pesquisaCurvaABCPessoas, null, null, 10);
    _gridCurvaABCPessoas.SetPermitirEdicaoColunas(true);

    _relatorioCurvaABCPessoas = new RelatorioGlobal("Relatorios/CurvaABCPessoa/BuscarDadosRelatorio", _gridCurvaABCPessoas, function () {
        _relatorioCurvaABCPessoas.loadRelatorio(function () {
            KoBindings(_pesquisaCurvaABCPessoas, "knockoutPesquisaCurvaABCPessoas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCurvaABCPessoas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCurvaABCPessoas");

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCurvaABCPessoas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCurvaABCPessoas.gerarRelatorio("Relatorios/CurvaABCPessoa/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCurvaABCPessoas.gerarRelatorio("Relatorios/CurvaABCPessoa/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}