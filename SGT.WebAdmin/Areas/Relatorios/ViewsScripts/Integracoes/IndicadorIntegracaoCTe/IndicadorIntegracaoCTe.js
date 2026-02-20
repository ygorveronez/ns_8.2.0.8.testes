/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _gridIndicadorIntegracaoCTe;
var _pesquisaIndicadorIntegracaoCTe;
var _relatorioIndicadorIntegracaoCTe;

/*
 * Declaração das Classes
 */

var PesquisaIndicadorIntegracaoCTe = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: ", maxlength: 50 });
    this.DataEmissaoInicio = PropertyEntity({ text: "Data Emissão Início: ", getType: typesKnockout.date });
    this.DataEmissaoLimite = PropertyEntity({ text: "Data Emissão Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Filial:"), idBtnSearch: guid() });
    this.NumeroCTe = PropertyEntity({ text: "Número do CT-e: ", getType: typesKnockout.int, maxlength: 11 });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid() });
    this.Integrado1 = PropertyEntity({ text: ko.observable(""), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.Integrado2 = PropertyEntity({ text: ko.observable(""), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.Integrado3 = PropertyEntity({ text: ko.observable(""), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.Integrado4 = PropertyEntity({ text: ko.observable(""), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.Integrado5 = PropertyEntity({ text: ko.observable(""), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });

    this.DataEmissaoInicio.dateRangeLimit = this.DataEmissaoLimite;
    this.DataEmissaoLimite.dateRangeInit = this.DataEmissaoInicio;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridIndicadorIntegracaoCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadIndicadorIntegracaoCTe() {
    buscarIntegradoras(function (integradoras) {
        _pesquisaIndicadorIntegracaoCTe = new PesquisaIndicadorIntegracaoCTe();
        _CRUDRelatorio = new CRUDRelatorio();
        _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
        _gridIndicadorIntegracaoCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/IndicadorIntegracaoCTe/Pesquisa", _pesquisaIndicadorIntegracaoCTe);
        _gridIndicadorIntegracaoCTe.SetPermitirEdicaoColunas(true);
        _gridIndicadorIntegracaoCTe.SetQuantidadeLinhasPorPagina(20);

        _relatorioIndicadorIntegracaoCTe = new RelatorioGlobal("Relatorios/IndicadorIntegracaoCTe/BuscarDadosRelatorio", _gridIndicadorIntegracaoCTe, function () {
            _relatorioIndicadorIntegracaoCTe.loadRelatorio(function () {
                KoBindings(_pesquisaIndicadorIntegracaoCTe, "knockoutPesquisaIndicadorIntegracaoCTe");
                KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaIndicadorIntegracaoCTe");
                KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaIndicadorIntegracaoCTe");

                new BuscarFilial(_pesquisaIndicadorIntegracaoCTe.Filial);
                new BuscarTransportadores(_pesquisaIndicadorIntegracaoCTe.Transportador);

                for (var i = 0; i < integradoras.length; i++) {
                    var integradora = integradoras[i];
                    var filtroPesquisaIntegrado = _pesquisaIndicadorIntegracaoCTe["Integrado" + integradora.Numero];

                    filtroPesquisaIntegrado.text("Integrado " + integradora.Descricao + ":");
                    filtroPesquisaIntegrado.visible(true);
                }
            });
        }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaIndicadorIntegracaoCTe);
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function buscarIntegradoras(callback) {
    executarReST("Relatorios/IndicadorIntegracaoCTe/BuscarIntegradoras", {}, function (retorno) {
        if (retorno.Success && Boolean(retorno.Data))
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function GerarRelatorioPDFClick() {
    _relatorioIndicadorIntegracaoCTe.gerarRelatorio("Relatorios/IndicadorIntegracaoCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioIndicadorIntegracaoCTe.gerarRelatorio("Relatorios/IndicadorIntegracaoCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
