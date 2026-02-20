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
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Fatura.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMinutas, _pesquisaMinutas, _CRUDRelatorio, _relatorioMinutas, _CRUDFiltrosRelatorio;

var _tiposIntegradoras = [
    { value: '', text: 'Todos' },
    { value: '1', text: 'Natura' },
    { value: '2', text: 'Avon' }
];

var _tipoPropriedadeVeiculo = [
    { value: '', text: 'Todos' },
    { value: 'P', text: 'Próprio' },
    { value: 'T', text: 'Terceiro' }
];

var _tipoSituacaoIntegracao = [
    { value: '', text: 'Todos' },
    { value: true, text: 'Integrado' },
    { value: false, text: 'Não Integrado' }
];

var PesquisaMinutas = function () {
    this.DataInicialMinuta = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinalMinuta = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataInicialMinuta.dateRangeLimit = this.DataFinalMinuta;
    this.DataFinalMinuta.dateRangeInit = this.DataInicialMinuta;

    this.TipoIntegradora = PropertyEntity({ val: ko.observable(""), options: _tiposIntegradoras, def: "", text: "Integradora: " });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid() });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 12 });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 12 });
    this.TipoPropriedadeVeiculo = PropertyEntity({ text: "Propriedade do Veículo:", options: _tipoPropriedadeVeiculo, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SituacaoIntegracao = PropertyEntity({ text: "Situação de Integração:", options: _tipoSituacaoIntegracao, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMinutas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function loadRelatorioMinutas() {

    _pesquisaMinutas = new PesquisaMinutas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMinutas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Minuta/Pesquisa", _pesquisaMinutas);

    _gridMinutas.SetPermitirEdicaoColunas(true);
    _gridMinutas.SetQuantidadeLinhasPorPagina(10);

    _relatorioMinutas = new RelatorioGlobal("Relatorios/Minuta/BuscarDadosRelatorio", _gridMinutas, function () {
        _relatorioMinutas.loadRelatorio(function () {
            KoBindings(_pesquisaMinutas, "knockoutPesquisaMinutas", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMinutas", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMinuta", false);

            new BuscarFatura(_pesquisaMinutas.Fatura, RetornoFatura);
            new BuscarMotoristas(_pesquisaMinutas.Motorista);
            new BuscarEstados(_pesquisaMinutas.EstadoOrigem);
            new BuscarEstados(_pesquisaMinutas.EstadoDestino);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMinutas);
}

function RetornoFatura(data) {
    _pesquisaMinutas.Fatura.val(data.Numero);
    _pesquisaMinutas.Fatura.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMinutas.gerarRelatorio("Relatorios/Minuta/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMinutas.gerarRelatorio("Relatorios/Minuta/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}