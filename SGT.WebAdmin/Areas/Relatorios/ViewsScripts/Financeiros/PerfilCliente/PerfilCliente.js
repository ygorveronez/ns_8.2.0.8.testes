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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioPerfilClientes, _gridPerfilClientes, _pesquisaPerfilClientes, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _estadoPerfilCliente = [
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
];

var PesquisaPerfilClientes = function () {
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.Estado = PropertyEntity({ val: ko.observable(""), options: _estadoPerfilCliente, def: "", text: "Estado: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPerfilClientes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioPerfilClientes() {

    _pesquisaPerfilClientes = new PesquisaPerfilClientes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPerfilClientes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PerfilCliente/Pesquisa", _pesquisaPerfilClientes, null, null, 10);
    _gridPerfilClientes.SetPermitirEdicaoColunas(true);

    _relatorioPerfilClientes = new RelatorioGlobal("Relatorios/PerfilCliente/BuscarDadosRelatorio", _gridPerfilClientes, function () {
        _relatorioPerfilClientes.loadRelatorio(function () {
            KoBindings(_pesquisaPerfilClientes, "knockoutPesquisaPerfilClientes");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPerfilClientes");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPerfilClientes");
            new BuscarClientes(_pesquisaPerfilClientes.Pessoa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPerfilClientes);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPerfilClientes.gerarRelatorio("Relatorios/PerfilCliente/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPerfilClientes.gerarRelatorio("Relatorios/PerfilCliente/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}