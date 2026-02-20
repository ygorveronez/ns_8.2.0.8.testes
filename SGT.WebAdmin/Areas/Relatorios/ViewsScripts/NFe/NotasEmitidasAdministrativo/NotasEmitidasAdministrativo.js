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
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioNotasEmitidasAdministrativo, _gridNotasEmitidasAdministrativo, _pesquisaNotasEmitidasAdministrativo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusNFe = [
    { text: "Todos", value: 0 },
    { text: "Em Digitação", value: EnumStatusNFe.Emitido },
    { text: "Inutilizado", value: EnumStatusNFe.Inutilizado },
    { text: "Cancelado", value: EnumStatusNFe.Cancelado },
    { text: "Autorizado", value: EnumStatusNFe.Autorizado },
    { text: "Denegado", value: EnumStatusNFe.Denegado },
    { text: "Rejeitado", value: EnumStatusNFe.Rejeitado }
];

var _tipoEmissao = [
    { text: "Todos", value: -1 },
    { text: "Entrada", value: EnumTipoEmissaoNFe.Entrada },
    { text: "Saída", value: EnumTipoEmissaoNFe.Saida }
];

var _tipoNotaImportada = [
    { text: "Pelo Sistema", value: 1 },
    { text: "Todas", value: 2 }
];

var PesquisaNotasEmitidasAdministrativo = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa Filho: ", idBtnSearch: guid(), visible: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicialCadastro = PropertyEntity({ text: "Data Inicial Cad.: ", getType: typesKnockout.date });
    this.DataFinalCadastro = PropertyEntity({ text: "Data Final Cad.: ", getType: typesKnockout.date });
    this.Serie = PropertyEntity({ text: "Série: ", getType: typesKnockout.int });
    this.NotaImportada = PropertyEntity({ val: ko.observable(1), options: _tipoNotaImportada, def: 1, text: "Forma Emissão: " });

    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusNFe, def: 0, text: "Status: " });
    this.DataProcessamento = PropertyEntity({ text: "Data Processamento: ", getType: typesKnockout.date });
    this.DataSaida = PropertyEntity({ text: "Data Saída: ", getType: typesKnockout.date });
    this.TipoEmissao = PropertyEntity({ val: ko.observable(-1), options: _tipoEmissao, def: -1, text: "Tipo Emissão: " });

    this.SomenteClientesComEmissao = PropertyEntity({ text: "Exibir somente clientes com Total de Documentos maior que zero?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.StatusEmpresa = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status Empresa: " });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNotasEmitidasAdministrativo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioNotasEmitidasAdministrativo() {

    _pesquisaNotasEmitidasAdministrativo = new PesquisaNotasEmitidasAdministrativo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNotasEmitidasAdministrativo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/NotasEmitidasAdministrativo/Pesquisa", _pesquisaNotasEmitidasAdministrativo, null, null, 10);
    _gridNotasEmitidasAdministrativo.SetPermitirEdicaoColunas(true);

    _relatorioNotasEmitidasAdministrativo = new RelatorioGlobal("Relatorios/NotasEmitidasAdministrativo/BuscarDadosRelatorio", _gridNotasEmitidasAdministrativo, function () {
        _relatorioNotasEmitidasAdministrativo.loadRelatorio(function () {
            KoBindings(_pesquisaNotasEmitidasAdministrativo, "knockoutPesquisaNotasEmitidasAdministrativo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNotasEmitidasAdministrativo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNotasEmitidasAdministrativo");
            new BuscarEmpresa(_pesquisaNotasEmitidasAdministrativo.Empresa, retornoEmpresa, true);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNotasEmitidasAdministrativo);

}

function retornoEmpresa(data) {
    _pesquisaNotasEmitidasAdministrativo.Empresa.val(data.RazaoSocial);
    _pesquisaNotasEmitidasAdministrativo.Empresa.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNotasEmitidasAdministrativo.gerarRelatorio("Relatorios/NotasEmitidasAdministrativo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNotasEmitidasAdministrativo.gerarRelatorio("Relatorios/NotasEmitidasAdministrativo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
