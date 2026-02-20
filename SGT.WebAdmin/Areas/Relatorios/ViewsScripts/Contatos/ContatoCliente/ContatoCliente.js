//*******MAPEAMENTO KNOUCKOUT*******

var _gridContatoCliente, _pesquisaContatoCliente, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioContatoCliente;

var _tipoPessoa = [
    { text: "Pessoa (CNPJ/CPF)", value: 1 },
    { text: "Grupo de Pessoas", value: 2 }
];


var PesquisaContatoCliente = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo de Contato:", options: ko.observable([]), url: "TipoContato/BuscarTodos", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Situação do Contato:", options: ko.observable([]), url: "SituacaoContato/BuscarTodos", visible: ko.observable(true) });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: "", text: "Tipo de Pessoa:" });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Bordero = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Borderô:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Título:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _pesquisaContatoCliente.Pessoa.visible(true);
            _pesquisaContatoCliente.Pessoa.required = true;
            _pesquisaContatoCliente.GrupoPessoas.required = false;
            _pesquisaContatoCliente.GrupoPessoas.visible(false);
            _pesquisaContatoCliente.GrupoPessoas.codEntity(0);
            _pesquisaContatoCliente.GrupoPessoas.val('');
        } else {
            _pesquisaContatoCliente.GrupoPessoas.visible(true);
            _pesquisaContatoCliente.GrupoPessoas.required = true;
            _pesquisaContatoCliente.Pessoa.required = false;
            _pesquisaContatoCliente.Pessoa.visible(false);
            _pesquisaContatoCliente.Pessoa.codEntity(0);
            _pesquisaContatoCliente.Pessoa.val('');
        }
    });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridContatoCliente.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaContatoCliente.Visible.visibleFade() == true) {
                _pesquisaContatoCliente.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaContatoCliente.Visible.visibleFade(true);
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

function LoadContatoCliente() {
    _pesquisaContatoCliente = new PesquisaContatoCliente();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridContatoCliente = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ContatoCliente/Pesquisa", _pesquisaContatoCliente);

    _gridContatoCliente.SetPermitirEdicaoColunas(true);
    _gridContatoCliente.SetQuantidadeLinhasPorPagina(10);

    _relatorioContatoCliente = new RelatorioGlobal("Relatorios/ContatoCliente/BuscarDadosRelatorio", _gridContatoCliente, function () {
        _relatorioContatoCliente.loadRelatorio(function () {
            KoBindings(_pesquisaContatoCliente, "knockoutPesquisaContatoCliente", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaContatoCliente", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaContatoCliente", false);


            new BuscarClientes(_pesquisaContatoCliente.Pessoa);
            new BuscarGruposPessoas(_pesquisaContatoCliente.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarTitulo(_pesquisaContatoCliente.Titulo, null, null, function (r) {
                _pesquisaContatoCliente.Titulo.val(r.Codigo);
                _pesquisaContatoCliente.Titulo.codEntity(r.Codigo);
            });
            new BuscarBorderos(_pesquisaContatoCliente.Bordero);
            new BuscarFatura(_pesquisaContatoCliente.Fatura, function (r) {
                _pesquisaContatoCliente.Fatura.val(r.Numero);
                _pesquisaContatoCliente.Fatura.codEntity(r.Codigo);
            });

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaContatoCliente);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioContatoCliente.gerarRelatorio("Relatorios/ContatoCliente/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioContatoCliente.gerarRelatorio("Relatorios/ContatoCliente/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
