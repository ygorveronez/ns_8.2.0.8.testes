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
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioComprasVendasNCM, _gridComprasVendasNCM, _pesquisaComprasVendasNCM, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _estadoCompraVendaNCM = [
    { text: "Selecione", value: "" },
    { text: "ACRE", value: "AC" },
    { text: "ALAGOAS", value: "AL" },
    { text: "AMAZONAS", value: "AM" },
    { text: "AMAPA", value: "AP" },
    { text: "BAHIA", value: "BA" },
    { text: "CEARA", value: "CE" },
    { text: "DISTRITO FEDERAL", value: "DF" },
    { text: "ESPIRITO SANTO", value: "ES" },
    { text: "EXPORTACAO", value: "EX" },
    { text: "GOIAS", value: "GO" },
    { text: "MARANHAO", value: "MA" },
    { text: "MINAS GERAIS", value: "MG" },
    { text: "MATO GROSSO DO SUL", value: "MS" },
    { text: "MATO GROSSO", value: "MT" },
    { text: "PARA", value: "PA" },
    { text: "PARAIBA", value: "PB" },
    { text: "PERNAMBUCO", value: "PE" },
    { text: "PIAUI", value: "PI" },
    { text: "PARANA", value: "PR" },
    { text: "RIO DE JANEIRO", value: "RJ" },
    { text: "RIO GRANDE DO NORTE", value: "RN" },
    { text: "RONDONIA", value: "RO" },
    { text: "RORAIMA", value: "RR" },
    { text: "RIO GRANDE DO SUL", value: "RS" },
    { text: "SANTA CATARINA", value: "SC" },
    { text: "SERGIPE", value: "SE" },
    { text: "SAO PAULO", value: "SP" },
    { text: "TOCANTINS", value: "TO" }
]

var _tipoCompraVendaNCM = [
    { text: "Todos", value: EnumCompraVenda.Todos },
    { text: "Compra", value: EnumCompraVenda.Compra },
    { text: "Venda", value: EnumCompraVenda.Venda }
];

var PesquisaComprasVendasNCM = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.NCM = PropertyEntity({ text: "NCM's (Exemplo: 87089990, 92029000, 01061900): " });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.Cidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade: ", idBtnSearch: guid(), visible: true });

    this.Estado = PropertyEntity({ val: ko.observable(""), options: _estadoCompraVendaNCM, def: "", text: "Estado: " });
    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumCompraVenda.Todos), options: _tipoCompraVendaNCM, def: EnumCompraVenda.Todos, text: "Tipo Movimento: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridComprasVendasNCM.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioComprasVendasNCM() {

    _pesquisaComprasVendasNCM = new PesquisaComprasVendasNCM();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridComprasVendasNCM = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CompraVendaNCM/Pesquisa", _pesquisaComprasVendasNCM, null, null, 10);
    _gridComprasVendasNCM.SetPermitirEdicaoColunas(true);

    _relatorioComprasVendasNCM = new RelatorioGlobal("Relatorios/CompraVendaNCM/BuscarDadosRelatorio", _gridComprasVendasNCM, function () {
        _relatorioComprasVendasNCM.loadRelatorio(function () {
            KoBindings(_pesquisaComprasVendasNCM, "knockoutPesquisaComprasVendasNCM");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaComprasVendasNCM");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaComprasVendasNCM");
            new BuscarProdutoTMS(_pesquisaComprasVendasNCM.Produto);
            new BuscarLocalidades(_pesquisaComprasVendasNCM.Cidade);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaComprasVendasNCM);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComprasVendasNCM.gerarRelatorio("Relatorios/CompraVendaNCM/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComprasVendasNCM.gerarRelatorio("Relatorios/CompraVendaNCM/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}