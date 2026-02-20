/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusAverbacaoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAverbacaoFechamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCTesAverbados, _pesquisaCTesAverbados, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioCTesAverbados;

var PesquisaCTesAverbados = function () {
    var filtroPorTransp = _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe;
    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", issue: 2, val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", issue: 2, val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataServicoInicial = PropertyEntity({ text: "Data Serviço Inicial: ", issue: 2, val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataServicoFinal = PropertyEntity({ text: "Data Serviço Final: ", issue: 2, val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;
    this.DataServicoInicial.dateRangeLimit = this.DataServicoFinal;
    this.DataServicoFinal.dateRangeInit = this.DataServicoInicial;

    this.Status = PropertyEntity({ text: "Situação:", options: EnumStatusAverbacaoCTe.obterOpcoesPesquisa(), val: ko.observable(EnumStatusAverbacaoCTe.Sucesso), def: EnumStatusAverbacaoCTe.Sucesso, visible: ko.observable(true) });
    this.SituacaoFechamento = PropertyEntity({ text: "Situação Fechamento:", options: EnumSituacaoAverbacaoFechamento.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoAverbacaoFechamento.Todas), def: EnumSituacaoAverbacaoFechamento.Todas, visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: filtroPorTransp });
    this.Seguradora = PropertyEntity({ text: "Seguradora:", issue: 262, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ClienteProvedorOS = PropertyEntity({ text: "Provedor:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ModeloDocumentoFiscal = PropertyEntity({ text: "Modelo Documento Fiscal:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoPropriedadeVeiculo = PropertyEntity({ text: "Tipo de Propriedade: ", options: EnumTipoPropriedadeVeiculo.obterOpcoesPesquisa(), val: ko.observable(EnumTipoPropriedadeVeiculo.Todos), def: EnumTipoPropriedadeVeiculo.Todos });
    this.GrupoTomador = PropertyEntity({ text: "Grupo do Tomador:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCTesAverbados.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCTesAverbados.Visible.visibleFade()) {
                _pesquisaCTesAverbados.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCTesAverbados.Visible.visibleFade(true);
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

function loadRelatorioCTesAverbados() {
    _pesquisaCTesAverbados = new PesquisaCTesAverbados();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCTesAverbados = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CTesAverbados/Pesquisa", _pesquisaCTesAverbados);

    _gridCTesAverbados.SetPermitirEdicaoColunas(true);
    _gridCTesAverbados.SetQuantidadeLinhasPorPagina(25);

    _relatorioCTesAverbados = new RelatorioGlobal("Relatorios/CTesAverbados/BuscarDadosRelatorio", _gridCTesAverbados, function () {
        _relatorioCTesAverbados.loadRelatorio(function () {
            KoBindings(_pesquisaCTesAverbados, "knockoutPesquisaCTes", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCTes", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCTe", false);

            new BuscarTransportadores(_pesquisaCTesAverbados.Transportador);
            new BuscarSeguradoras(_pesquisaCTesAverbados.Seguradora);
            new BuscarClientes(_pesquisaCTesAverbados.ClienteProvedorOS);
            new BuscarModeloDocumentoFiscal(_pesquisaCTesAverbados.ModeloDocumentoFiscal);
            new BuscarGruposPessoas(_pesquisaCTesAverbados.GrupoTomador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCTesAverbados);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCTesAverbados.gerarRelatorio("Relatorios/CTesAverbados/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCTesAverbados.gerarRelatorio("Relatorios/CTesAverbados/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}