/// <reference path="../../../../../ViewsScripts/Consultas/Conhecimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Fatura.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Bordero.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Justificativa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoJustificativa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoAcrescimoDescontoTituloDocumento.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoFinalidadeJustificativa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTituloAcrescimoDesconto, _pesquisaTituloAcrescimoDesconto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTituloAcrescimoDesconto;

var opcoesTipoJustificativa = [
    { text: "Todos", value: "" },
    { text: "Acréscimo", value: EnumTipoJustificativa.Acrescimo },
    { text: "Desconto", value: EnumTipoJustificativa.Desconto },
];

var opcoesOrigemAcrescimoDesconto = [
    { text: "Todos", value: "" },
    { text: "Geração", value: EnumTipoAcrescimoDescontoTituloDocumento.Geracao },
    { text: "Baixa", value: EnumTipoAcrescimoDescontoTituloDocumento.Baixa },
];

var opcoesTipoTitulo = [
    { text: "Todos", value: "" },
    { text: "A Receber", value: EnumTipoTitulo.AReceber },
    { text: "A Pagar", value: EnumTipoTitulo.APagar },
];



var PesquisaTituloAcrescimoDesconto = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoTitulo = PropertyEntity({ val: ko.observable(EnumSituacaoTitulo.Todos), options: EnumSituacaoTitulo.obterOpcoesPesquisa(), def: EnumSituacaoTitulo.Todos, text: "Situação do Título:" });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: opcoesOrigemAcrescimoDesconto, def: "", text: "Origem:" });
    this.TipoJustificativa = PropertyEntity({ val: ko.observable(""), options: opcoesTipoJustificativa, def: "", text: "Tipo da Justificativa:" });

    this.Justificativa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Justificativa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Bordero = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Borderô:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo de Pessoa:" });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.DataEmissaoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data de Emissão Inicial:" });
    this.DataEmissaoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data de Emissão Final:" });
    this.DataLiquidacaoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data de Liquidação Inicial:" });
    this.DataLiquidacaoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data de Liquidação Final:" });
    this.DataBaseLiquidacaoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Base de Liquidação Inicial:" });
    this.DataBaseLiquidacaoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Base de Liquidação Final:" });

    this.TipoDeTitulo = PropertyEntity({ val: ko.observable(""), options: opcoesTipoTitulo, def: "", text: "Tipo de Título:" });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
            _pesquisaTituloAcrescimoDesconto.Pessoa.visible(true);
            _pesquisaTituloAcrescimoDesconto.GrupoPessoas.visible(false);
            LimparCampoEntity(_pesquisaTituloAcrescimoDesconto.GrupoPessoas);
        } else {
            _pesquisaTituloAcrescimoDesconto.GrupoPessoas.visible(true);
            _pesquisaTituloAcrescimoDesconto.Pessoa.visible(false);
            LimparCampoEntity(_pesquisaTituloAcrescimoDesconto.Pessoa);
        }
    });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTituloAcrescimoDesconto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTituloAcrescimoDesconto.Visible.visibleFade()) {
                _pesquisaTituloAcrescimoDesconto.Visible.visibleFade(false);
                e.BuscaAvancada.icon("faç fa-plus");
            } else {
                _pesquisaTituloAcrescimoDesconto.Visible.visibleFade(true);
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

function LoadTituloAcrescimoDesconto() {
    _pesquisaTituloAcrescimoDesconto = new PesquisaTituloAcrescimoDesconto();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTituloAcrescimoDesconto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TituloAcrescimoDesconto/Pesquisa", _pesquisaTituloAcrescimoDesconto);

    _gridTituloAcrescimoDesconto.SetPermitirEdicaoColunas(true);
    _gridTituloAcrescimoDesconto.SetQuantidadeLinhasPorPagina(10);

    _relatorioTituloAcrescimoDesconto = new RelatorioGlobal("Relatorios/TituloAcrescimoDesconto/BuscarDadosRelatorio", _gridTituloAcrescimoDesconto, function () {
        _relatorioTituloAcrescimoDesconto.loadRelatorio(function () {
            KoBindings(_pesquisaTituloAcrescimoDesconto, "knockoutPesquisaTituloAcrescimoDesconto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTituloAcrescimoDesconto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTituloAcrescimoDesconto", false);

            new BuscarBorderos(_pesquisaTituloAcrescimoDesconto.Bordero);
            new BuscarJustificativas(_pesquisaTituloAcrescimoDesconto.Justificativa);
            new BuscarFatura(_pesquisaTituloAcrescimoDesconto.Fatura, function (r) { _pesquisaTituloAcrescimoDesconto.Fatura.val(r.Numero); _pesquisaTituloAcrescimoDesconto.Fatura.codEntity(r.Codigo); });
            new BuscarClientes(_pesquisaTituloAcrescimoDesconto.Pessoa);
            new BuscarGruposPessoas(_pesquisaTituloAcrescimoDesconto.GrupoPessoas);
            new BuscarConhecimentoNotaReferencia(_pesquisaTituloAcrescimoDesconto.CTe, function (data) {
                _pesquisaTituloAcrescimoDesconto.CTe.val(data.Numero + "-" + data.Serie);
                _pesquisaTituloAcrescimoDesconto.CTe.codEntity(data.Codigo);
            });

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTituloAcrescimoDesconto);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTituloAcrescimoDesconto.gerarRelatorio("Relatorios/TituloAcrescimoDesconto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTituloAcrescimoDesconto.gerarRelatorio("Relatorios/TituloAcrescimoDesconto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
