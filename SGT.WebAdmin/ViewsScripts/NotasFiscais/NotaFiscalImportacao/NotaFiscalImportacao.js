var _gridNotaFiscalImportacao;
var _pesquisaNotaFiscalImportacao;

var PesquisaNotaFiscalImportacao = function () {
    this.DataInicial = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: "Data da Importação Inicial:" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: "Data da Importação Final:" });

    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataInicial.dateRangeLimit = this.DataFinal;

    this.NumeroNotaFiscal = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), text: "Número NF-e:", configInt: { precision: 0, allowZero: false, thousands: '' } });

    this.ImportarNotaFiscal = PropertyEntity({
        type: types.local,
        text: "Importar Nota Fiscal",
        visible: true,
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "NotaFiscalImportacao/ImportarNotaFiscal",
        UrlConfiguracao: "NotaFiscalImportacao/ConfiguracaoImportacaoNotaFiscal",
        CodigoControleImportacao: EnumCodigoControleImportacao.O040_NotaFiscal,
        CallbackImportacao: function () {

        }
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridNotaFiscalImportacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

function loadNotaFiscalImportacao() {
    _pesquisaNotaFiscalImportacao = new PesquisaNotaFiscalImportacao();
    KoBindings(_pesquisaNotaFiscalImportacao, "knockoutPesquisaNotaFiscalImportacao");

    loadGridNotaFiscalImportacao();
}

function loadGridNotaFiscalImportacao() {
    //var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", exibirDetalhesImportacaoNotaFiscal, tamanho: "10", icone: "" };

    //var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [detalhes], tamanho: 10 };

    _gridNotaFiscalImportacao = new GridView(_pesquisaNotaFiscalImportacao.Pesquisar.idGrid, "NotaFiscalImportacao/Pesquisa", _pesquisaNotaFiscalImportacao, null, null, 20, null, null, null, null, 60);
    _gridNotaFiscalImportacao.CarregarGrid();
}

function exibirDetalhesImportacaoNotaFiscal() {

}