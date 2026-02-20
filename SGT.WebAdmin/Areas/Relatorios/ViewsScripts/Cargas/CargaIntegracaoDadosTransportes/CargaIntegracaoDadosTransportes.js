/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoIntegracaoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaIntegracaoDadosTransportes, _pesquisaCargaIntegracaoDadosTransportes, _CRUDRelatorio, _CRUDFiltrosRelatorio, _tipoIntegracao;

var _relatorioCargaIntegracaoDadosTransportes;

var PesquisaCargaIntegracaoDadosTransportes = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicialCarga = PropertyEntity({ text: "Data Inicial Carga: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalCarga = PropertyEntity({ text: "Data Final Carga: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialCarga.dateRangeLimit = this.DataFinalCarga;
    this.DataFinalCarga.dateRangeInit = this.DataInicialCarga;

    this.DataInicioIntegracao = PropertyEntity({ text: "Data Integração Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalIntegracao = PropertyEntity({ text: "Data Integração Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicioIntegracao.dateRangeLimit = this.DataFinalIntegracao;
    this.DataFinalIntegracao.dateRangeInit = this.DataInicioIntegracao;

    this.Situacao = PropertyEntity({ text: "Situação:", options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.TipoIntegracao = PropertyEntity({ text: "Tipo de Integração:", options: _tipoIntegracao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicioEncerramento = PropertyEntity({ text: "Data Inicial Encerramento: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEncerramento = PropertyEntity({ text: "Data Final Encerramento: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicioEncerramento.dateRangeLimit = this.DataInicioEncerramento;
    this.DataFinalEncerramento.dateRangeInit = this.DataFinalEncerramento;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaIntegracaoDadosTransportes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaIntegracaoDadosTransportes.Visible.visibleFade()) {
                _pesquisaCargaIntegracaoDadosTransportes.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaIntegracaoDadosTransportes.Visible.visibleFade(true);
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

function LoadCargaIntegracaoDadosTransportes() {
    ObterTiposIntegracao().then(function () {
        _pesquisaCargaIntegracaoDadosTransportes = new PesquisaCargaIntegracaoDadosTransportes();
        _CRUDRelatorio = new CRUDRelatorio();
        _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

        _gridCargaIntegracaoDadosTransportes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaIntegracaoDadosTransportes/Pesquisa", _pesquisaCargaIntegracaoDadosTransportes);

        _gridCargaIntegracaoDadosTransportes.SetPermitirEdicaoColunas(true);
        _gridCargaIntegracaoDadosTransportes.SetPermitirRedimencionarColunas(true);
        _gridCargaIntegracaoDadosTransportes.SetQuantidadeLinhasPorPagina(10);

        _relatorioCargaIntegracaoDadosTransportes = new RelatorioGlobal("Relatorios/CargaIntegracaoDadosTransportes/BuscarDadosRelatorio", _gridCargaIntegracaoDadosTransportes, function () {
            _relatorioCargaIntegracaoDadosTransportes.loadRelatorio(function () {
                KoBindings(_pesquisaCargaIntegracaoDadosTransportes, "knockoutPesquisaCargaIntegracaoDadosTransportes", false);
                KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaIntegracaoDadosTransportes", false);
                KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaIntegracaoDadosTransportes", false);

                new BuscarCargas(_pesquisaCargaIntegracaoDadosTransportes.Carga);
                new BuscarTiposOperacao(_pesquisaCargaIntegracaoDadosTransportes.TipoOperacao);
                new BuscarTransportadores(_pesquisaCargaIntegracaoDadosTransportes.Transportador, null, null, true);
                new BuscarFilial(_pesquisaCargaIntegracaoDadosTransportes.Filial);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
                    _pesquisaCargaIntegracaoDadosTransportes.Filial.visible(true);
                }
                else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                    _pesquisaCargaIntegracaoDadosTransportes.Transportador.text("Empresa/Filial:");
                }

                $("#divConteudoRelatorio").show();
            });
        }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaIntegracaoDadosTransportes);
    });
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaIntegracaoDadosTransportes.gerarRelatorio("Relatorios/CargaIntegracaoDadosTransportes/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaIntegracaoDadosTransportes.gerarRelatorio("Relatorios/CargaIntegracaoDadosTransportes/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ObterTiposIntegracao() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {}, function (r) {
        if (r.Success) {
            _tipoIntegracao = [{ value: "", text: "Todos" }];

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }

        p.done();
    });

    return p;
}