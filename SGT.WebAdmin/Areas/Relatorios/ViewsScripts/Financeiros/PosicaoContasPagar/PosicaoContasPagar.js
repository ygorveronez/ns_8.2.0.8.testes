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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPosicaoContasPagar, _pesquisaPosicaoContasPagar, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioPosicaoContasPagar;

var _statusPosicaoContasPagar = [
    { text: "Todos", value: 0 },
    { text: "Em Aberto", value: 1 },
    { text: "Quitado", value: 3 }
];

var PesquisaPosicaoContasPagar = function () {
    this.DataPosicao = PropertyEntity({ text: "*Data Posição: ", getType: typesKnockout.date, required: true });
    this.DataInicial = PropertyEntity({ text: "Emissão Inicial: ", getType: typesKnockout.date, required: false });
    this.DataFinal = PropertyEntity({ text: "Emissão Final: ", getType: typesKnockout.date, required: false });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _statusPosicaoContasPagar, def: 0, text: "Situação: ", required: false });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaPosicaoContasPagar)) {
                _gridPosicaoContasPagar.CarregarGrid();
            } else {
                exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
            }
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioPosicaoContasPagar() {

    _pesquisaPosicaoContasPagar = new PesquisaPosicaoContasPagar();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPosicaoContasPagar = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PosicaoContasPagar/Pesquisa", _pesquisaPosicaoContasPagar);

    _gridPosicaoContasPagar.SetPermitirEdicaoColunas(true);
    _gridPosicaoContasPagar.SetQuantidadeLinhasPorPagina(10);

    _relatorioPosicaoContasPagar = new RelatorioGlobal("Relatorios/PosicaoContasPagar/BuscarDadosRelatorio", _gridPosicaoContasPagar, function () {
        _relatorioPosicaoContasPagar.loadRelatorio(function () {
            KoBindings(_pesquisaPosicaoContasPagar, "knockoutPesquisaPosicaoContasPagar", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPosicaoContasPagar", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFaturamento", false);
            
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPosicaoContasPagar);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarCamposObrigatorios(_pesquisaPosicaoContasPagar)) {
        _relatorioPosicaoContasPagar.gerarRelatorio("Relatorios/PosicaoContasPagar/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    }
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarCamposObrigatorios(_pesquisaPosicaoContasPagar)) {
        _relatorioPosicaoContasPagar.gerarRelatorio("Relatorios/PosicaoContasPagar/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
    }
}