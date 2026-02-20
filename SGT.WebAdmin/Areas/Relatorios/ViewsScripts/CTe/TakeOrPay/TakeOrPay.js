/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFatura.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioTakeOrPay, _gridTakeOrPay, _pesquisaTakeOrPay, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoFatura = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacoesFatura.EmAndamento },
    { text: "Fechada", value: EnumSituacoesFatura.Fechado },
    { text: "Cancelado", value: EnumSituacoesFatura.Cancelado }
];

var PesquisaTakeOrPay = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicialFatura = PropertyEntity({ text: "Data Fatura Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataFinalFatura = PropertyEntity({ text: "Data Fatura Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataInicialFatura.dateRangeLimit = this.DataFinalFatura;
    this.DataFinalFatura.dateRangeInit = this.DataInicialFatura;

    this.DataInicialPrevisaoSaidaNavio = PropertyEntity({ text: "Previsão Saída Navio Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataFinalPrevisaoSaidaNavio = PropertyEntity({ text: "Previsão Saída Navio Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataInicialPrevisaoSaidaNavio.dateRangeLimit = this.DataFinalPrevisaoSaidaNavio;
    this.DataFinalPrevisaoSaidaNavio.dateRangeInit = this.DataInicialPrevisaoSaidaNavio;

    this.NumeroBoleto = PropertyEntity({ text: "Núm. Boleto: " });
    this.NumeroFatura = PropertyEntity({ text: "Núm. Fatura: ", getType: typesKnockout.int });

    this.SituacaoFatura = PropertyEntity({ text: "Situação da Fatura:", options: _situacaoFatura, val: ko.observable(""), def: "" });
    this.TipoProposta = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesTakePay() });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195 });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pessoas:", idBtnSearch: guid() });

    //Emissão Multimodal
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTakeOrPay.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTakeOrPay.Visible.visibleFade()) {
                _pesquisaTakeOrPay.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTakeOrPay.Visible.visibleFade(true);
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

function LoadTakeOrPay() {
    _pesquisaTakeOrPay = new PesquisaTakeOrPay();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTakeOrPay = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TakeOrPay/Pesquisa", _pesquisaTakeOrPay, null, null, 10);
    _gridTakeOrPay.SetPermitirEdicaoColunas(true);

    _relatorioTakeOrPay = new RelatorioGlobal("Relatorios/TakeOrPay/BuscarDadosRelatorio", _gridTakeOrPay, function () {
        _relatorioTakeOrPay.loadRelatorio(function () {
            KoBindings(_pesquisaTakeOrPay, "knockoutPesquisaTakeOrPay", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTakeOrPay", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTakeOrPay", false);

            new BuscarCargas(_pesquisaTakeOrPay.Carga);
            new BuscarPedidoViagemNavio(_pesquisaTakeOrPay.Viagem);
            new BuscarPorto(_pesquisaTakeOrPay.PortoOrigem);
            new BuscarPorto(_pesquisaTakeOrPay.PortoDestino);
            new BuscarGruposPessoas(_pesquisaTakeOrPay.GrupoPessoas);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTakeOrPay);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTakeOrPay.gerarRelatorio("Relatorios/TakeOrPay/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTakeOrPay.gerarRelatorio("Relatorios/TakeOrPay/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}