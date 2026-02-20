/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFreteTerceirizadoValePedagio, _pesquisaFreteTerceirizadoValePedagio, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioFreteTerceirizadoValePedagio;

var PesquisaFreteTerceirizadoValePedagio = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid() });
    this.Terceiro = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), issue: 0 });

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

    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable([EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada]), def: [EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada], getType: typesKnockout.selectMultiple, options: EnumSituacaoContratoFrete.ObterOpcoes() });

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
            _gridFreteTerceirizadoValePedagio.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFreteTerceirizadoValePedagio.Visible.visibleFade()) {
                _pesquisaFreteTerceirizadoValePedagio.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFreteTerceirizadoValePedagio.Visible.visibleFade(true);
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

function LoadFreteTerceirizadoValePedagio() {
    _pesquisaFreteTerceirizadoValePedagio = new PesquisaFreteTerceirizadoValePedagio();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFreteTerceirizadoValePedagio = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FreteTerceirizadoValePedagio/Pesquisa", _pesquisaFreteTerceirizadoValePedagio);

    _gridFreteTerceirizadoValePedagio.SetPermitirEdicaoColunas(true);
    _gridFreteTerceirizadoValePedagio.SetQuantidadeLinhasPorPagina(10);

    _relatorioFreteTerceirizadoValePedagio = new RelatorioGlobal("Relatorios/FreteTerceirizadoValePedagio/BuscarDadosRelatorio", _gridFreteTerceirizadoValePedagio, function () {
        _relatorioFreteTerceirizadoValePedagio.loadRelatorio(function () {
            KoBindings(_pesquisaFreteTerceirizadoValePedagio, "knockoutPesquisaFreteTerceirizadoValePedagio", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFreteTerceirizadoValePedagio", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFreteTerceirizadoValePedagio", false);

            new BuscarClientes(_pesquisaFreteTerceirizadoValePedagio.Terceiro, null, null, [EnumModalidadePessoa.TransportadorTerceiro]);
            new BuscarVeiculos(_pesquisaFreteTerceirizadoValePedagio.Veiculo);
            new BuscarModelosVeicularesCarga(_pesquisaFreteTerceirizadoValePedagio.ModeloVeicular);
            new BuscarTiposOperacao(_pesquisaFreteTerceirizadoValePedagio.TipoOperacao);
            new BuscarTransportadores(_pesquisaFreteTerceirizadoValePedagio.Empresa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFreteTerceirizadoValePedagio);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFreteTerceirizadoValePedagio.gerarRelatorio("Relatorios/FreteTerceirizadoValePedagio/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFreteTerceirizadoValePedagio.gerarRelatorio("Relatorios/FreteTerceirizadoValePedagio/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
