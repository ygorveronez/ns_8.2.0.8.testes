/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentrosDescarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />

var _gridJanelaDisponivelAgendamento;
var _pesquisaJanelaDisponivelAgendamento;
var _CRUDFiltrosRelatorio;

var PesquisaJanelaDisponivelAgendamento = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataHora()), def: Global.DataAtual(), required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, val: ko.observable(), required: true });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataInicial.val.subscribe(function () {
        if (_pesquisaJanelaDisponivelAgendamento.DataInicial.val().length < 10)
            return;

        if ((_pesquisaJanelaDisponivelAgendamento.DataInicial.val().replace("_", "").length == 10) && !_pesquisaJanelaDisponivelAgendamento.DataFinal.val()) {
            var dataFinal = moment(Global.criarData(_pesquisaJanelaDisponivelAgendamento.DataInicial.val())).add(29, 'days').format('DD/MM/YYYY');

            _pesquisaJanelaDisponivelAgendamento.DataFinal.val(dataFinal);
        }
    });

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo De Carga:", idBtnSearch: guid() });
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro Descarregamento:", idBtnSearch: guid(), required: true });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular Carga:", idBtnSearch: guid(), required: false });

    this.JanelaExcedente = PropertyEntity({ text: "Janela Excedente?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (!ValidarCamposObrigatorios(_pesquisaJanelaDisponivelAgendamento)) {
                exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os campos obrigatórios.");
                return;
            }

            _gridJanelaDisponivelAgendamento.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

function loadRelatorioJanelaDisponivelAgendamento() {
    _pesquisaJanelaDisponivelAgendamento = new PesquisaJanelaDisponivelAgendamento();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridJanelaDisponivelAgendamento = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/JanelaDisponivelAgendamento/Pesquisa", _pesquisaJanelaDisponivelAgendamento, null, null, 10, null, null, null, null, 99999999);
    _gridJanelaDisponivelAgendamento.SetPermitirEdicaoColunas(true);

    _relatorioJanelaDisponivelAgendamento = new RelatorioGlobal("Relatorios/JanelaDisponivelAgendamento/BuscarDadosRelatorio", _gridJanelaDisponivelAgendamento, function () {
        _relatorioJanelaDisponivelAgendamento.loadRelatorio(function () {
            KoBindings(_pesquisaJanelaDisponivelAgendamento, "knockoutPesquisaJanelaDisponivelAgendamento", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaJanelaDisponivelAgendamento", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaJanelaDisponivelAgendamento", false);

            new BuscarTiposdeCarga(_pesquisaJanelaDisponivelAgendamento.TipoDeCarga);
            new BuscarClientes(_pesquisaJanelaDisponivelAgendamento.Fornecedor);
            new BuscarCentrosDescarregamento(_pesquisaJanelaDisponivelAgendamento.CentroDescarregamento);
            new BuscarModelosVeicularesCarga(_pesquisaJanelaDisponivelAgendamento.ModeloVeicular);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaJanelaDisponivelAgendamento);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioJanelaDisponivelAgendamento.gerarRelatorio("Relatorios/JanelaDisponivelAgendamento/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioJanelaDisponivelAgendamento.gerarRelatorio("Relatorios/JanelaDisponivelAgendamento/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
