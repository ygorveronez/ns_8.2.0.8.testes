//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoOcorrencia, _pesquisaTipoOcorrencia, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioTipoOcorrencia;

var _tipoPessoa = [
    { text: "Pessoa (CNPJ/CPF)", value: 1 },
    { text: "Grupo de Pessoas", value: 2 }
];

var _opcoesSituacao = [
    { text: "Todos", value: "" },
    { text: "Ativo", value: true },
    { text: "Inativo", value: false }
];

var PesquisaTipoOcorrencia = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Descricao = PropertyEntity({ text: "Descrição:", maxlength: 150 });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: 1, text: "Tipo de Pessoa:" });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "Situação:", options: _opcoesSituacao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _pesquisaTipoOcorrencia.Pessoa.visible(true);
            _pesquisaTipoOcorrencia.GrupoPessoas.visible(false);
            _pesquisaTipoOcorrencia.GrupoPessoas.codEntity(0);
            _pesquisaTipoOcorrencia.GrupoPessoas.val('');
        } else {
            _pesquisaTipoOcorrencia.GrupoPessoas.visible(true);
            _pesquisaTipoOcorrencia.Pessoa.visible(false);
            _pesquisaTipoOcorrencia.Pessoa.codEntity(0);
            _pesquisaTipoOcorrencia.Pessoa.val('');
        }
    });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTipoOcorrencia.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaTipoOcorrencia.Visible.visibleFade() == true) {
                _pesquisaTipoOcorrencia.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaTipoOcorrencia.Visible.visibleFade(true);
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

function LoadTipoOcorrencia() {
    _pesquisaTipoOcorrencia = new PesquisaTipoOcorrencia();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTipoOcorrencia = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/TipoOcorrencia/Pesquisa", _pesquisaTipoOcorrencia);

    _gridTipoOcorrencia.SetPermitirEdicaoColunas(true);
    _gridTipoOcorrencia.SetQuantidadeLinhasPorPagina(10);

    _relatorioTipoOcorrencia = new RelatorioGlobal("Relatorios/TipoOcorrencia/BuscarDadosRelatorio", _gridTipoOcorrencia, function () {
        _relatorioTipoOcorrencia.loadRelatorio(function () {
            KoBindings(_pesquisaTipoOcorrencia, "knockoutPesquisaTipoOcorrencia", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTipoOcorrencia", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTipoOcorrencia", false);

            new BuscarClientes(_pesquisaTipoOcorrencia.Pessoa);
            new BuscarGruposPessoas(_pesquisaTipoOcorrencia.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTipoOcorrencia);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTipoOcorrencia.gerarRelatorio("Relatorios/TipoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTipoOcorrencia.gerarRelatorio("Relatorios/TipoOcorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
