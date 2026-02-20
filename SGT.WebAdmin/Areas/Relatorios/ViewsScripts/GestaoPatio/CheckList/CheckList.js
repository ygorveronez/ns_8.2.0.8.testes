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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCheckList.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioCheckList, _gridCheckList, _pesquisaCheckList, _CRUDRelatorio, _CRUDFiltrosRelatorio, codigoFilialAnterior;

var PesquisaCheckList = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(""), options: EnumSituacaoCheckList.obterOpcoesPesquisa(), def: "" });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (!ValidarCamposObrigatorios(_pesquisaCheckList)) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "Você precisa selecionar uma filial.");
                return;
            }
            if (codigoFilialAnterior !== _pesquisaCheckList.Filial.codEntity() || !_relatorioCheckList)
                recarregarGridCheckList();
            else
                _gridCheckList.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function LoadRelatorioCheckList() {
    _pesquisaCheckList = new PesquisaCheckList();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    KoBindings(_pesquisaCheckList, "knockoutPesquisaCheckList");
    KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCheckList");
    KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCheckList");

    new BuscarFilial(_pesquisaCheckList.Filial, callbackFilial);

    $("#divConteudoRelatorio").show();

    LoadGridCheckList();
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCheckList.gerarRelatorio("Relatorios/CheckList/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function LoadGridCheckList() {
    _gridCheckList = new GridView("gridPreviewRelatorio", "Relatorios/CheckList/Pesquisa", _pesquisaCheckList, null, null, 10, null, null, null, null, null, null, null);

    _gridCheckList.SetPermitirEdicaoColunas(true);
    _gridCheckList.SetQuantidadeLinhasPorPagina(10);
}

function recarregarGridCheckList() {
    _relatorioCheckList = new RelatorioGlobal("Relatorios/CheckList/BuscarDadosRelatorio", _gridCheckList, function () {
        _relatorioCheckList.loadRelatorio(function () {
            _gridCheckList.CarregarGrid();
        });
    }, { CodigoFilial: _pesquisaCheckList.Filial.codEntity() }, null, _pesquisaCheckList, false);
}

function callbackFilial(filial) {
    _pesquisaCheckList.Filial.codEntity(filial.Codigo);
    _pesquisaCheckList.Filial.val(filial.Descricao);

    codigoFilialAnterior = _pesquisaCheckList.Filial.codEntity();
}
