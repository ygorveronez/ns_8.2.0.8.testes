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
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SetorFuncionario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioControleVisita, _gridControleVisita, _pesquisaControleVisita, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaControleVisita = function () {
    this.DataInicialEntrada = PropertyEntity({ text: "Data Inicial Entrada: ", getType: typesKnockout.dateTime });
    this.DataFinalEntrada = PropertyEntity({ text: "Data Final Entrada: ", getType: typesKnockout.dateTime });
    this.DataInicialSaida = PropertyEntity({ text: "Data Inicial Saída: ", getType: typesKnockout.dateTime });
    this.DataFinalSaida = PropertyEntity({ text: "Data Final Saída: ", getType: typesKnockout.dateTime });
    
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Autorizador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Autorizador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CPF = PropertyEntity({ text: "CPF: ", getType: typesKnockout.cpf });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridControleVisita.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioControleVisita() {

    _pesquisaControleVisita = new PesquisaControleVisita();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridControleVisita = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ControleVisita/Pesquisa", _pesquisaControleVisita, null, null, 10);
    _gridControleVisita.SetPermitirEdicaoColunas(true);

    _relatorioControleVisita = new RelatorioGlobal("Relatorios/ControleVisita/BuscarDadosRelatorio", _gridControleVisita, function () {
        _relatorioControleVisita.loadRelatorio(function () {
            KoBindings(_pesquisaControleVisita, "knockoutPesquisaControleVisita");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaControleVisita");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaControleVisita");

            new BuscarSetorFuncionario(_pesquisaControleVisita.Setor);
            new BuscarFuncionario(_pesquisaControleVisita.Autorizador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaControleVisita);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioControleVisita.gerarRelatorio("Relatorios/ControleVisita/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioControleVisita.gerarRelatorio("Relatorios/ControleVisita/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}