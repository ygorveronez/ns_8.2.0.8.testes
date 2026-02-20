/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumCausadorSinistro.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumIndicadorPagador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoHistoricoInfracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSinistro, _pesquisaSinistro, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioSinistro;

var PesquisaSinistro = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });


    this.NumeroSinistro = PropertyEntity({ text: "Número Sinistro:", getType: typesKnockout.int });
    this.CausadorSinistro = PropertyEntity({ text: "Causador do Sinistro:", val: ko.observable(EnumCausadorSinistro.Todos), options: EnumCausadorSinistro.obterOpcoesPesquisa(), def: EnumCausadorSinistro.Todos });
    this.TipoSinistro = PropertyEntity({ text: "Tipo Sinistro:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.NumeroBoletimOcorrencia = PropertyEntity({ text: "Número do B.O:", getType: typesKnockout.string });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Cidade = PropertyEntity({ text: "Cidade:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ text: "Placa Cavalo:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.VeiculoReboque = PropertyEntity({ text: "Placa Carreta/Reboque:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ text: "Motorista:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid() });
    this.NumeroOrdemServico = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.IndicacaoPagador = PropertyEntity({ text: "Indicação do Pagador:", val: ko.observable(EnumIndicadorPagador.Todos), options: EnumIndicadorPagador.obterOpcoesPesquisa(), def: EnumIndicadorPagador.Todos });
    this.SituacaoSinistro = PropertyEntity({ text: "Situação do Sinistro:", val: ko.observable(EnumTipoHistoricoInfracao.Todos), options: EnumTipoHistoricoInfracao.obterOpcoesFluxoSinistroPesquisa(), def: EnumTipoHistoricoInfracao.Todos });
};


var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridSinistro.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaSinistro.Visible.visibleFade()) {
                _pesquisaSinistro.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaSinistro.Visible.visibleFade(true);
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

function loadRelatorioSinistro() {
    _pesquisaSinistro = new PesquisaSinistro();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridSinistro = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Sinistro/Pesquisa", _pesquisaSinistro, null, null, 10);
    _gridSinistro.SetPermitirEdicaoColunas(true);

    _relatorioSinistro = new RelatorioGlobal("Relatorios/Sinistro/BuscarDadosRelatorio", _gridSinistro, function () {
        _relatorioSinistro.loadRelatorio(function () {
            KoBindings(_pesquisaSinistro, "knockoutPesquisaSinistro");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaSinistro");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaSinistro");

            new BuscarTipoSinistro(_pesquisaSinistro.TipoSinistro);
            new BuscarLocalidades(_pesquisaSinistro.Cidade);
            new BuscarTracaoManobra(_pesquisaSinistro.Veiculo);
            new BuscarReboques(_pesquisaSinistro.VeiculoReboque);
            new BuscarMotoristas(_pesquisaSinistro.Motorista);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaSinistro);

    
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioSinistro.gerarRelatorio("Relatorios/Sinistro/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}
function GerarRelatorioExcelClick(e, sender) {
    _relatorioSinistro.gerarRelatorio("Relatorios/Sinistro/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
