/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCIOT.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaCIOT, _pesquisaCargaCIOT, _CRUDRelatorio, _relatorioCargaCIOT, _CRUDFiltrosRelatorio;

var PesquisaCargaCIOT = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Numero = PropertyEntity({ text: "Número: " });
    this.Carga = PropertyEntity({ text: "Carga: " });

    this.DataEncerramentoInicial = PropertyEntity({ text: "Data Encerramento Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEncerramentoFinal = PropertyEntity({ text: "Data Encerramento Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataAberturaInicial = PropertyEntity({ text: "Data Abertura Inicial: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataAberturaFinal = PropertyEntity({ text: "Data Abertura Final: ", issue: 2, getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.DataEncerramentoInicial.dateRangeLimit = this.DataEncerramentoFinal;
    this.DataEncerramentoFinal.dateRangeInit = this.DataEncerramentoInicial;
    this.DataAberturaInicial.dateRangeLimit = this.DataAberturaFinal;
    this.DataAberturaFinal.dateRangeInit = this.DataAberturaInicial;

    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoCIOT.Todos), options: EnumSituacaoCIOT.ObterOpcoesPesquisa(), def: EnumSituacaoCIOT.Todos });
    this.RegimeTributario = PropertyEntity({ text: "Regime Tributário:", val: ko.observable(EnumRegimeTributario.NaoSelecionado), options: EnumRegimeTributario.obterOpcoesPesquisa(), def: EnumRegimeTributario.NaoSelecionado });

    this.Proprietario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Proprietário:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCIOT.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaCIOT.Visible.visibleFade() === true) {
                _pesquisaCargaCIOT.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaCIOT.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadRelatorioCargaCIOT() {

    _pesquisaCargaCIOT = new PesquisaCargaCIOT();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCargaCIOT = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/CargaCIOT/Pesquisa", _pesquisaCargaCIOT, null, null, 10, null, null, null, null, 20);
    _gridCargaCIOT.SetPermitirEdicaoColunas(true);

    _relatorioCargaCIOT = new RelatorioGlobal("Relatorios/CargaCIOT/BuscarDadosRelatorio", _gridCargaCIOT, function () {
        _relatorioCargaCIOT.loadRelatorio(function () {
            KoBindings(_pesquisaCargaCIOT, "knockoutPesquisaCargaCIOT");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaCIOT");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaCIOTs");

            new BuscarClientes(_pesquisaCargaCIOT.Proprietario);
            new BuscarVeiculos(_pesquisaCargaCIOT.Veiculo);
            new BuscarMotoristas(_pesquisaCargaCIOT.Motorista);
            new BuscarTransportadores(_pesquisaCargaCIOT.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaCIOT);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaCIOT.gerarRelatorio("Relatorios/CargaCIOT/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaCIOT.gerarRelatorio("Relatorios/CargaCIOT/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
