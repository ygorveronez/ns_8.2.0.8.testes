/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../../js/app.config.js" />


var _relatorioHistoricoParadas;
var _gridHistoricoParadas, _pesquisaHistoricoParadas, _CRUDFiltrosRelatorio, _CRUDRelatorio;

var PesquisaHistoricoParadas = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Veiculos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículos:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()), required: true });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()), required: true });
    this.ContratoFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Contrato de Frete", idBtnSearch: guid() });
    this.ApenasMonitoramentosFinalizados = PropertyEntity({ type: types.bool, codEntity: ko.observable(0), text: "Apenas monitoramentos finalizados", val: ko.observable(false), def: false });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarPDFHistoricoParadasClick, type: types.event, text: "Gerar PDF", idGrid: guid() });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarExcelHistoricoParadasClick, type: types.event, text: "Gerar Excel", idGrid: guid() });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (!ValidarCamposObrigatorios(_pesquisaHistoricoParadas)) {
                exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os campos obrigatórios.");
                return;
            }
            _gridHistoricoParadas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

function LoadHistoricoParadas() {
    _pesquisaHistoricoParadas = new PesquisaHistoricoParadas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridHistoricoParadas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "HistoricoParadas/Pesquisa", _pesquisaHistoricoParadas);

    _gridHistoricoParadas.SetPermitirEdicaoColunas(false);
    _gridHistoricoParadas.SetQuantidadeLinhasPorPagina(20);

    _relatorioHistoricoParadas = new RelatorioGlobal("HistoricoParadas/BuscarDadosRelatorio", _gridHistoricoParadas, function () {
        _relatorioHistoricoParadas.loadRelatorio(function () {

            KoBindings(_pesquisaHistoricoParadas, "knockoutPesquisaHistoricoParadas", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaHistoricoParadas", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaHistoricoParadas", false);

            new BuscarFilial(_pesquisaHistoricoParadas.Filial);
            new BuscarTransportadores(_pesquisaHistoricoParadas.Transportador);
            new BuscarVeiculos(_pesquisaHistoricoParadas.Veiculos);
            new BuscarContratoFreteTransportador(_pesquisaHistoricoParadas.ContratoFrete);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaHistoricoParadas);
}

function GerarPDFHistoricoParadasClick() {
    if (!ValidarCamposObrigatorios(_pesquisaHistoricoParadas)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os campos obrigatórios.");
        return;
    }

    var data = RetornarObjetoPesquisa(_pesquisaHistoricoParadas);
    executarReST("HistoricoParadas/GerarPDF", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function GerarExcelHistoricoParadasClick() {
    if (!ValidarCamposObrigatorios(_pesquisaHistoricoParadas)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os campos obrigatórios.");
        return;
    }

    executarDownload("HistoricoParadas/GerarExcel", RetornarObjetoPesquisa(_pesquisaHistoricoParadas));
}