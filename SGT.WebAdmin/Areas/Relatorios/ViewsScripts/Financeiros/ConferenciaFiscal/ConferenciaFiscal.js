/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusCTe.js"/>

//********MAPEAMENTO KNOCKOUT********

var _relatorioConferenciaFiscal, _gridConferenciaFiscal, _pesquisaConferenciaFiscal, _CRUDRelatorio, _CRUDFiltrosRelatorio;

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

var PesquisaConferenciaFiscal = function () {
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report })
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
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridConferenciaFiscal.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaConferenciaFiscal.Visible.visibleFade() === true) {
                _pesquisaConferenciaFiscal.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaConferenciaFiscal.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadConferenciaFiscal() {
    _pesquisaConferenciaFiscal = new PesquisaConferenciaFiscal();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConferenciaFiscal = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConferenciaFiscal/Pesquisa", _pesquisaConferenciaFiscal, null, null, 10);
    _gridConferenciaFiscal.SetPermitirEdicaoColunas(true);

    _relatorioConferenciaFiscal = new RelatorioGlobal("Relatorios/ConferenciaFiscal/BuscarDadosRelatorio", _gridConferenciaFiscal, function () {
        _relatorioConferenciaFiscal.loadRelatorio(function () {
            KoBindings(_pesquisaConferenciaFiscal, "knockoutPesquisaConferenciaFiscal", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConferenciaFiscal", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConferenciaFiscal", false);

            BuscarClientes(_pesquisaConferenciaFiscal.Terceiro, null, null, [EnumModalidadePessoa.TransportadorTerceiro]);
            BuscarVeiculos(_pesquisaConferenciaFiscal.Veiculo);
            BuscarModelosVeicularesCarga(_pesquisaConferenciaFiscal.ModeloVeicular);
            BuscarTiposOperacao(_pesquisaConferenciaFiscal.TipoOperacao);
            BuscarTransportadores(_pesquisaConferenciaFiscal.Empresa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConferenciaFiscal);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioConferenciaFiscal.gerarRelatorio("Relatorios/ConferenciaFiscal/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioConferenciaFiscal.gerarRelatorio("Relatorios/ConferenciaFiscal/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}