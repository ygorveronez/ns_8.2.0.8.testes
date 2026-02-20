//*******MAPEAMENTO KNOUCKOUT*******

var _gridPosicaoDocumentoReceber, _pesquisaPosicaoDocumentoReceber, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPosicaoDocumentoReceber;

var _opcoesTipoFaturamentoPosicao = [
    { text: "Todos", value: "" },
    { text: "Faturado", value: EnumTipoFaturamentoPosicao.Faturado },
    { text: "Não Faturado", value: EnumTipoFaturamentoPosicao.NaoFaturado }
];

var PesquisaPosicaoDocumentoReceber = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.GrupoPessoasTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.TipoFaturamento = PropertyEntity({ text: "Faturamento:", options: _opcoesTipoFaturamentoPosicao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.DataPosicao = PropertyEntity({ text: "*Data da Posição:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
     
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPosicaoDocumentoReceber.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPosicaoDocumentoReceber.Visible.visibleFade() == true) {
                _pesquisaPosicaoDocumentoReceber.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPosicaoDocumentoReceber.Visible.visibleFade(true);
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

function LoadPosicaoDocumentoReceber() {
    _pesquisaPosicaoDocumentoReceber = new PesquisaPosicaoDocumentoReceber();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPosicaoDocumentoReceber = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/PosicaoDocumentoReceber/Pesquisa", _pesquisaPosicaoDocumentoReceber);

    _gridPosicaoDocumentoReceber.SetPermitirEdicaoColunas(true);
    _gridPosicaoDocumentoReceber.SetQuantidadeLinhasPorPagina(10);

    _relatorioPosicaoDocumentoReceber = new RelatorioGlobal("Relatorios/PosicaoDocumentoReceber/BuscarDadosRelatorio", _gridPosicaoDocumentoReceber, function () {
        _relatorioPosicaoDocumentoReceber.loadRelatorio(function () {
            KoBindings(_pesquisaPosicaoDocumentoReceber, "knockoutPesquisaPosicaoDocumentoReceber", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPosicaoDocumentoReceber", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPosicaoDocumentoReceber", false);

            new BuscarClientes(_pesquisaPosicaoDocumentoReceber.Remetente);
            new BuscarClientes(_pesquisaPosicaoDocumentoReceber.Destinatario);
            new BuscarClientes(_pesquisaPosicaoDocumentoReceber.Tomador);
            new BuscarTransportadores(_pesquisaPosicaoDocumentoReceber.Empresa);
            new BuscarLocalidades(_pesquisaPosicaoDocumentoReceber.Origem);
            new BuscarLocalidades(_pesquisaPosicaoDocumentoReceber.Destino);
            new BuscarGruposPessoas(_pesquisaPosicaoDocumentoReceber.GrupoPessoasTomador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPosicaoDocumentoReceber);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPosicaoDocumentoReceber.gerarRelatorio("Relatorios/PosicaoDocumentoReceber/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPosicaoDocumentoReceber.gerarRelatorio("Relatorios/PosicaoDocumentoReceber/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
