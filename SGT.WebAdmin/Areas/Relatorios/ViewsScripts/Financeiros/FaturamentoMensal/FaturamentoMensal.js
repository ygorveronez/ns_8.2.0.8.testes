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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoDataFaturamentoMensal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioFaturamentosMensais, _gridFaturamentosMensais, _pesquisaFaturamentosMensais, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaFaturamentosMensais = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.TipoData = PropertyEntity({ val: ko.observable(EnumTipoDataFaturamentoMensal.DataEmissao), options: EnumTipoDataFaturamentoMensal.obterOpcoes(), def: EnumTipoDataFaturamentoMensal.DataEmissao, text: "Tipo de Data: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFaturamentosMensais.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
    this.GerarRelatorioGrafico = PropertyEntity({ eventClick: GerarRelatorioGraficoClick, type: types.event, text: "Gerar Gráfico" });
};

//*******EVENTOS*******


function loadRelatorioFaturamentosMensais() {

    _pesquisaFaturamentosMensais = new PesquisaFaturamentosMensais();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFaturamentosMensais = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FaturamentoMensal/Pesquisa", _pesquisaFaturamentosMensais, null, null, 10);
    _gridFaturamentosMensais.SetPermitirEdicaoColunas(true);

    _relatorioFaturamentosMensais = new RelatorioGlobal("Relatorios/FaturamentoMensal/BuscarDadosRelatorio", _gridFaturamentosMensais, function () {
        _relatorioFaturamentosMensais.loadRelatorio(function () {
            KoBindings(_pesquisaFaturamentosMensais, "knockoutPesquisaFaturamentosMensais");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFaturamentosMensais");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFaturamentosMensais");

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFaturamentosMensais);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFaturamentosMensais.gerarRelatorio("Relatorios/FaturamentoMensal/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFaturamentosMensais.gerarRelatorio("Relatorios/FaturamentoMensal/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarRelatorioGraficoClick(e, sender) {
    var data = {
        DataInicial: _pesquisaFaturamentosMensais.DataInicial.val(),
        DataFinal: _pesquisaFaturamentosMensais.DataFinal.val(),
        TipoData: _pesquisaFaturamentosMensais.TipoData.val()
    };
    executarReST("Relatorios/FaturamentoMensal/BaixarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}