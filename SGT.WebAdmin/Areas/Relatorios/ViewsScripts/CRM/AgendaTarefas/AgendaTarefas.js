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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusAgendaTarefa.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAgendaTarefas, _pesquisaAgendaTarefas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioAgendaTarefas;

var PesquisaAgendaTarefas = function () {
    this.Observacao = PropertyEntity({ text: "Observação: " });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusAgendaTarefa.Todos), options: EnumStatusAgendaTarefa.obterOpcoesPesquisa(), def: EnumStatusAgendaTarefa.Todos, text: "Status: " });
    this.DataInicial = PropertyEntity({ text: "Data da tarefa de: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAgendaTarefas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioAgendaTarefas() {
    _pesquisaAgendaTarefas = new PesquisaAgendaTarefas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAgendaTarefas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/AgendaTarefas/Pesquisa", _pesquisaAgendaTarefas, null, null, 10);

    _gridAgendaTarefas.SetPermitirEdicaoColunas(true);

    _relatorioAgendaTarefas = new RelatorioGlobal("Relatorios/AgendaTarefas/BuscarDadosRelatorio", _gridAgendaTarefas, function () {
        _relatorioAgendaTarefas.loadRelatorio(function () {
            KoBindings(_pesquisaAgendaTarefas, "knockoutPesquisaAgendaTarefa", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAgendaTarefa", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAgendaTarefa", false);

            new BuscarFuncionario(_pesquisaAgendaTarefas.Usuario);
            new BuscarClientes(_pesquisaAgendaTarefas.Cliente);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAgendaTarefas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAgendaTarefas.gerarRelatorio("Relatorios/AgendaTarefas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAgendaTarefas.gerarRelatorio("Relatorios/AgendaTarefas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}