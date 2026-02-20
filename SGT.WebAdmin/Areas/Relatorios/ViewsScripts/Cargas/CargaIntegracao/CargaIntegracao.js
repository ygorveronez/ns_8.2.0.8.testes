/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoIntegracaoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaIntegracao, _pesquisaCargaIntegracao, _CRUDRelatorio, _CRUDFiltrosRelatorio, _tipoIntegracao;

var _relatorioCargaIntegracao;

var PesquisaCargaIntegracao = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoRelatorio.getFieldDescription(), issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicialCarga = PropertyEntity({ text: Localization.Resources.Cargas.CargaIntegracao.DataInicialCarga.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalCarga = PropertyEntity({ text: Localization.Resources.Cargas.CargaIntegracao.DataFinalCarga.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialCarga.dateRangeLimit = this.DataFinalCarga;
    this.DataFinalCarga.dateRangeInit = this.DataInicialCarga;

    this.DataInicioIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.CargaIntegracao.DataIntegracaoInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.CargaIntegracao.DataIntegracaoFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicioIntegracao.dateRangeLimit = this.DataFinalIntegracao;
    this.DataFinalIntegracao.dateRangeInit = this.DataInicioIntegracao;

    this.SituacaoCarga = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.Todas), def: EnumSituacoesCarga.Todas, text: "Situação Carga:", options: EnumSituacoesCarga.obterOpcoesIntegracaoPesquisa(), visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoIntegracaoCarga.ProblemaIntegracao), def: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, issue: 0, visible: ko.observable(true) });
    this.TipoIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.CargaIntegracao.TipoIntegracao.getFieldDescription(), options: _tipoIntegracao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Carga.getFieldDescription(), idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoOperacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Transportador.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaIntegracao.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaIntegracao.Visible.visibleFade()) {
                _pesquisaCargaIntegracao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaIntegracao.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function LoadCargaIntegracao() {
    ObterTiposIntegracao().then(function () {
        _pesquisaCargaIntegracao = new PesquisaCargaIntegracao();
        _CRUDRelatorio = new CRUDRelatorio();
        _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

        _gridCargaIntegracao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaIntegracao/Pesquisa", _pesquisaCargaIntegracao);

        _gridCargaIntegracao.SetPermitirEdicaoColunas(true);
        _gridCargaIntegracao.SetQuantidadeLinhasPorPagina(10);

        _relatorioCargaIntegracao = new RelatorioGlobal("Relatorios/CargaIntegracao/BuscarDadosRelatorio", _gridCargaIntegracao, function () {
            _relatorioCargaIntegracao.loadRelatorio(function () {
                KoBindings(_pesquisaCargaIntegracao, "knockoutPesquisaCargaIntegracao", false);
                KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaIntegracao", false);
                KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaIntegracao", false);

                new BuscarCargas(_pesquisaCargaIntegracao.Carga);
                new BuscarTiposOperacao(_pesquisaCargaIntegracao.TipoOperacao);
                new BuscarTransportadores(_pesquisaCargaIntegracao.Transportador, null, null, true);
                new BuscarFilial(_pesquisaCargaIntegracao.Filial);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
                    _pesquisaCargaIntegracao.Filial.visible(true);
                    _pesquisaCargaIntegracao.SituacaoCarga.visible(true);
                }
                else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                    _pesquisaCargaIntegracao.Transportador.text("Empresa/Filial:");
                }

                $("#divConteudoRelatorio").show();
            });
        }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaIntegracao);
    });
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaIntegracao.gerarRelatorio("Relatorios/CargaIntegracao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaIntegracao.gerarRelatorio("Relatorios/CargaIntegracao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ObterTiposIntegracao() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {}, function (r) {
        if (r.Success) {
            _tipoIntegracao = [{ value: "", text: Localization.Resources.Gerais.Geral.Todos }];

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}