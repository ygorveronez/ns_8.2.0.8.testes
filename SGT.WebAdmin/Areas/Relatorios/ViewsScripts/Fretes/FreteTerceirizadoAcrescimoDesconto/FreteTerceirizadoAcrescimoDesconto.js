/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFreteTerceirizadoAcrescimoDesconto, _pesquisaFreteTerceirizadoAcrescimoDesconto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioFreteTerceirizadoAcrescimoDesconto;

var PesquisaFreteTerceirizadoAcrescimoDesconto = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroContrato = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "", text: "Número do Contrato:" });
    this.NumeroCarga = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da Carga:" });

    this.DataEmissaoContratoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data do Contrato Inicial:" });
    this.DataEmissaoContratoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data do Contrato Final:" });
    this.DataEmissaoContratoInicial.dateRangeLimit = this.DataEmissaoContratoFinal;
    this.DataEmissaoContratoFinal.dateRangeInit = this.DataEmissaoContratoInicial;

    this.Situacao = PropertyEntity({ val: ko.observable([EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada]), def: [EnumSituacaoContratoFrete.Aprovado, EnumSituacaoContratoFrete.Finalizada], getType: typesKnockout.selectMultiple, options: EnumSituacaoContratoFrete.ObterOpcoes(), text: "Situação:", issue: 0, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFreteTerceirizadoAcrescimoDesconto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFreteTerceirizadoAcrescimoDesconto.Visible.visibleFade()) {
                _pesquisaFreteTerceirizadoAcrescimoDesconto.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFreteTerceirizadoAcrescimoDesconto.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadFreteTerceirizadoAcrescimoDesconto() {
    _pesquisaFreteTerceirizadoAcrescimoDesconto = new PesquisaFreteTerceirizadoAcrescimoDesconto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFreteTerceirizadoAcrescimoDesconto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FreteTerceirizadoAcrescimoDesconto/Pesquisa", _pesquisaFreteTerceirizadoAcrescimoDesconto);

    _gridFreteTerceirizadoAcrescimoDesconto.SetPermitirEdicaoColunas(true);
    _gridFreteTerceirizadoAcrescimoDesconto.SetQuantidadeLinhasPorPagina(10);

    _relatorioFreteTerceirizadoAcrescimoDesconto = new RelatorioGlobal("Relatorios/FreteTerceirizadoAcrescimoDesconto/BuscarDadosRelatorio", _gridFreteTerceirizadoAcrescimoDesconto, function () {
        _relatorioFreteTerceirizadoAcrescimoDesconto.loadRelatorio(function () {
            KoBindings(_pesquisaFreteTerceirizadoAcrescimoDesconto, "knockoutPesquisaFreteTerceirizadoAcrescimoDesconto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFreteTerceirizadoAcrescimoDesconto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFreteTerceirizadoAcrescimoDesconto", false);

            new BuscarClientes(_pesquisaFreteTerceirizadoAcrescimoDesconto.Terceiro, null, null, [EnumModalidadePessoa.TransportadorTerceiro]);
            new BuscarVeiculos(_pesquisaFreteTerceirizadoAcrescimoDesconto.Veiculo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFreteTerceirizadoAcrescimoDesconto);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFreteTerceirizadoAcrescimoDesconto.gerarRelatorio("Relatorios/FreteTerceirizadoAcrescimoDesconto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFreteTerceirizadoAcrescimoDesconto.gerarRelatorio("Relatorios/FreteTerceirizadoAcrescimoDesconto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
