/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusLicenca.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Licenca.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioLicencaVeiculo, _gridLicencaVeiculo, _pesquisaLicencaVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaLicencaVeiculo = function () {
    this.DataInicial = PropertyEntity({ text: "Data Vencimento Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Vencimento Final: ", getType: typesKnockout.date });
    this.Descricao = PropertyEntity({ text: "Descrição Licença: ", getType: typesKnockout.string });
    this.NumeroLicenca = PropertyEntity({ text: "N° Licença: ", getType: typesKnockout.string });
    this.Licenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Licença:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Responsavel:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Marca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Renavam = PropertyEntity({ text: "Renavam: ", getType: typesKnockout.string });
    this.Status = PropertyEntity({ text: "Status da Licenca: ", val: ko.observable(""), options: EnumStatusLicenca.obterOpcoesPesquisa(), def: "" });
    this.StatusVeiculo = PropertyEntity({ text: "Status do Veículo:", options: _statusPesquisa, val: ko.observable(_statusPesquisa.Todos), def: _statusPesquisa.Todos });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridLicencaVeiculo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioLicencaVeiculo() {

    _pesquisaLicencaVeiculo = new PesquisaLicencaVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridLicencaVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/LicencaVeiculo/Pesquisa", _pesquisaLicencaVeiculo, null, null, 10);
    _gridLicencaVeiculo.SetPermitirEdicaoColunas(true);

    _relatorioLicencaVeiculo = new RelatorioGlobal("Relatorios/LicencaVeiculo/BuscarDadosRelatorio", _gridLicencaVeiculo, function () {
        _relatorioLicencaVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaLicencaVeiculo, "knockoutPesquisaLicencaVeiculo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaLicencaVeiculo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaLicencaVeiculo");

            new BuscarLicenca(_pesquisaLicencaVeiculo.Licenca);
            new BuscarFuncionario(_pesquisaLicencaVeiculo.Funcionario);
            new BuscarCentroResultado(_pesquisaLicencaVeiculo.CentroResultado)
            new BuscarMarcasVeiculo(_pesquisaLicencaVeiculo.Marca);
            new BuscarModelosVeiculo(_pesquisaLicencaVeiculo.Modelo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaLicencaVeiculo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioLicencaVeiculo.gerarRelatorio("Relatorios/LicencaVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioLicencaVeiculo.gerarRelatorio("Relatorios/LicencaVeiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}