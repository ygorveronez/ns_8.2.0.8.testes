//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaCTeIntegracao, _pesquisaCargaCTeIntegracao, _CRUDRelatorio, _CRUDFiltrosRelatorio, _tipoIntegracao;

var _relatorioCargaCTeIntegracao;

var PesquisaCargaCTeIntegracao = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TipoIntegracao = PropertyEntity({ text: "Tipo de Integração:", options: _tipoIntegracao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 58, visible: ko.observable(true) });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.DataIntegracaoInicial = PropertyEntity({ text: "Data Integração Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataIntegracaoFinal = PropertyEntity({ text: "Data Integração Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataIntegracaoInicial.dateRangeLimit = this.DataIntegracaoFinal;
    this.DataIntegracaoFinal.dateRangeInit = this.DataIntegracaoInicial;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCTeIntegracao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCargaCTeIntegracao.Visible.visibleFade() == true) {
                _pesquisaCargaCTeIntegracao.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCargaCTeIntegracao.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadCargaCTeIntegracao() {
    ObterTiposIntegracao().then(function () {
        _pesquisaCargaCTeIntegracao = new PesquisaCargaCTeIntegracao();
        _CRUDRelatorio = new CRUDRelatorio();
        _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

        _gridCargaCTeIntegracao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CargaCTeIntegracao/Pesquisa", _pesquisaCargaCTeIntegracao);

        _gridCargaCTeIntegracao.SetPermitirEdicaoColunas(true);
        _gridCargaCTeIntegracao.SetQuantidadeLinhasPorPagina(10);

        _relatorioCargaCTeIntegracao = new RelatorioGlobal("Relatorios/CargaCTeIntegracao/BuscarDadosRelatorio", _gridCargaCTeIntegracao, function () {
            _relatorioCargaCTeIntegracao.loadRelatorio(function () {
                KoBindings(_pesquisaCargaCTeIntegracao, "knockoutPesquisaCargaCTeIntegracao", false);
                KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCargaCTeIntegracao", false);
                KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCargaCTeIntegracao", false);

                new BuscarCargas(_pesquisaCargaCTeIntegracao.Carga);

                new BuscarConhecimentoNotaReferencia(_pesquisaCargaCTeIntegracao.CTe, function (r) {
                    _pesquisaCargaCTeIntegracao.CTe.val(r.Numero + " - " + r.Serie);
                    _pesquisaCargaCTeIntegracao.CTe.codEntity(r.Codigo);
                });

                new BuscarGruposPessoas(_pesquisaCargaCTeIntegracao.GrupoPessoas);

                $("#divConteudoRelatorio").show();
            });
        }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCargaCTeIntegracao);
    });
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCargaCTeIntegracao.gerarRelatorio("Relatorios/CargaCTeIntegracao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCargaCTeIntegracao.gerarRelatorio("Relatorios/CargaCTeIntegracao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
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