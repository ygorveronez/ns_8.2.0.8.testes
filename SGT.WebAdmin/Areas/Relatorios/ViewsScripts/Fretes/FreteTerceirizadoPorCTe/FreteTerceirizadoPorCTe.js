/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoProprietarioVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFreteTerceirizadoPorCTe, _pesquisaFreteTerceirizadoPorCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioFreteTerceirizadoPorCTe;

var _statusCTe = [
    { text: "Todos", value: EnumStatusCTe.TODOS },
    { text: "Enviado", value: EnumStatusCTe.ENVIADO },
    { text: "Autorizado", value: EnumStatusCTe.AUTORIZADO },
    { text: "Cancelado", value: EnumStatusCTe.CANCELADO },
    { text: "Em digitação", value: EnumStatusCTe.EMDIGITACAO },
    { text: "Pendente", value: EnumStatusCTe.PENDENTE },
    { text: "Rejeitado", value: EnumStatusCTe.REJEICAO },
    { text: "Inutilizado", value: EnumStatusCTe.INUTILIZADO },
    { text: "Denegado", value: EnumStatusCTe.DENEGADO },
    { text: "Anulado", value: EnumStatusCTe.ANULADO }
];

var _TiposCargaTerceiros = [
    { text: "Todas", value: EnumTiposCargaTerceiros.Todas },
    { text: "Terceiros", value: EnumTiposCargaTerceiros.Terceiro },
    { text: "Próprias", value: EnumTiposCargaTerceiros.Proprio }
    
    
]

var PesquisaFreteTerceirizadoPorCTe = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Terceiro = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.NumeroContrato = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "", text: "Número do Contrato:" });
    this.NumeroCTe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "", text: "Número do CT-e:" });
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da Carga:" });
    this.DataEmissaoContratoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data do Contrato Inicial:" });
    this.DataEmissaoContratoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data do Contrato Final:" });
    this.DataAprovacaoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Aprovação Inicial:" });
    this.DataAprovacaoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Aprovação Final:" });
    this.DataEncerramentoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Encerramento Inicial:" });
    this.DataEncerramentoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Encerramento Final:" });

    this.DataEncerramentoCIOTInicial = PropertyEntity({ getType: typesKnockout.date, text: "Dt. Encerramento CIOT Inicial:" });
    this.DataEncerramentoCIOTFinal = PropertyEntity({ getType: typesKnockout.date, text: "Dt. Encerramento CIOT Final:" });

    this.DataAberturaCIOTInicial = PropertyEntity({ getType: typesKnockout.date, text: "Dt. Abertura CIOT Inicial:" });
    this.DataAberturaCIOTFinal = PropertyEntity({ getType: typesKnockout.date, text: "Dt. Abertura CIOT Final:" });

    this.Situacao = PropertyEntity({ val: ko.observable([EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada]), def: [EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada], getType: typesKnockout.selectMultiple, options: EnumSituacaoContratoFrete.ObterOpcoes(), text: "Situação:", issue: 0, visible: ko.observable(true) });
    this.StatusCTe = PropertyEntity({ text: "Status CT-e:", options: _statusCTe, val: ko.observable(""), def: "" });
    this.TiposCargaTerceiros = PropertyEntity({ text: "Tipo Carga Terceiros:", options: _TiposCargaTerceiros, val: ko.observable(""), def: "" });
    this.NumeroCIOT = PropertyEntity({ text: "Número CIOT:", val: ko.observable(""), def: "" });
    this.TipoCTe = PropertyEntity({ text: "Tipo do CT-e:", getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), val: ko.observable(new Array()), def: new Array() });
    this.TipoProprietario = PropertyEntity({ text: "Tipo de Transportador:", options: EnumTipoProprietarioVeiculo.obterOpcoesPesquisa(), val: ko.observable(""), def: "" });

    this.DataEmissaoContratoInicial.dateRangeLimit = this.DataEmissaoContratoFinal;
    this.DataEmissaoContratoFinal.dateRangeInit = this.DataEmissaoContratoInicial;
    this.DataAprovacaoInicial.dateRangeLimit = this.DataAprovacaoFinal;
    this.DataAprovacaoFinal.dateRangeInit = this.DataAprovacaoInicial;
    this.DataEncerramentoInicial.dateRangeLimit = this.DataEncerramentoFinal;
    this.DataEncerramentoFinal.dateRangeInit = this.DataEncerramentoInicial;
    this.DataEncerramentoCIOTInicial.dateRangeLimit = this.DataEncerramentoCIOTFinal;
    this.DataEncerramentoCIOTFinal.dateRangeInit = this.DataEncerramentoCIOTInicial;
    this.DataAberturaCIOTInicial.dateRangeLimit = this.DataAberturaCIOTFinal;
    this.DataAberturaCIOTFinal.dateRangeInit = this.DataAberturaCIOTInicial;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFreteTerceirizadoPorCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFreteTerceirizadoPorCTe.Visible.visibleFade() === true) {
                _pesquisaFreteTerceirizadoPorCTe.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFreteTerceirizadoPorCTe.Visible.visibleFade(true);
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

function LoadFreteTerceirizadoPorCTe() {
    _pesquisaFreteTerceirizadoPorCTe = new PesquisaFreteTerceirizadoPorCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFreteTerceirizadoPorCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FreteTerceirizadoPorCTe/Pesquisa", _pesquisaFreteTerceirizadoPorCTe);

    _gridFreteTerceirizadoPorCTe.SetPermitirEdicaoColunas(true);
    _gridFreteTerceirizadoPorCTe.SetQuantidadeLinhasPorPagina(10);

    _relatorioFreteTerceirizadoPorCTe = new RelatorioGlobal("Relatorios/FreteTerceirizadoPorCTe/BuscarDadosRelatorio", _gridFreteTerceirizadoPorCTe, function () {
        _relatorioFreteTerceirizadoPorCTe.loadRelatorio(function () {
            KoBindings(_pesquisaFreteTerceirizadoPorCTe, "knockoutPesquisaFreteTerceirizadoPorCTe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFreteTerceirizadoPorCTe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFreteTerceirizadoPorCTe", false);

            BuscarClientes(_pesquisaFreteTerceirizadoPorCTe.Terceiro, null, null, [EnumModalidadePessoa.TransportadorTerceiro]);
            BuscarVeiculos(_pesquisaFreteTerceirizadoPorCTe.Veiculo);
            BuscarModelosVeicularesCarga(_pesquisaFreteTerceirizadoPorCTe.ModeloVeicular);
            BuscarTiposOperacao(_pesquisaFreteTerceirizadoPorCTe.TipoOperacao);
            BuscarTransportadores(_pesquisaFreteTerceirizadoPorCTe.Empresa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFreteTerceirizadoPorCTe);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFreteTerceirizadoPorCTe.gerarRelatorio("Relatorios/FreteTerceirizadoPorCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFreteTerceirizadoPorCTe.gerarRelatorio("Relatorios/FreteTerceirizadoPorCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
