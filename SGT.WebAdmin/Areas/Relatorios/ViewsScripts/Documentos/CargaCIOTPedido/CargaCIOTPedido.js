/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCIOT.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaCIOTPedido, _pesquisaCargaCIOTPedido, _CRUDRelatorio, _relatorioCargaCIOTPedido, _CRUDFiltrosRelatorio;

var PesquisaCargaCIOTPedido = function () {
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

    this.Proprietario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Proprietário:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCIOTPedido.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaCIOTPedido.Visible.visibleFade() === true) {
                _pesquisaCargaCIOTPedido.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaCIOTPedido.Visible.visibleFade(true);
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

function LoadRelatorioCargaCIOTPedido() {

    _pesquisaCargaCIOTPedido = new PesquisaCargaCIOTPedido();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCargaCIOTPedido = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/CargaCIOTPedido/Pesquisa", _pesquisaCargaCIOTPedido, null, null, 10, null, null, null, null, 20);
    _gridCargaCIOTPedido.SetPermitirEdicaoColunas(true);

    _relatorioCargaCIOTPedido = new RelatorioGlobal("Relatorios/CargaCIOTPedido/BuscarDadosRelatorio", _gridCargaCIOTPedido, function () {
        _relatorioCargaCIOTPedido.loadRelatorio(function () {
            KoBindings(_pesquisaCargaCIOTPedido, "knockoutPesquisaCargaCIOTPedido");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaCIOTPedido");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaCIOTPedido");

            new BuscarClientes(_pesquisaCargaCIOTPedido.Proprietario);
            new BuscarVeiculos(_pesquisaCargaCIOTPedido.Veiculo);
            new BuscarMotoristas(_pesquisaCargaCIOTPedido.Motorista);
            new BuscarTransportadores(_pesquisaCargaCIOTPedido.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaCIOTPedido);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaCIOTPedido.gerarRelatorio("Relatorios/CargaCIOTPedido/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaCIOTPedido.gerarRelatorio("Relatorios/CargaCIOTPedido/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
