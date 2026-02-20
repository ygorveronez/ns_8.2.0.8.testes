
//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaComparativoMensalFaturamentoGrupoPessoas;

var _propriedadesVeiculo = [
    { value: '', text: 'Todos' },
    { value: 'P', text: 'Próprio' },
    { value: 'T', text: 'Terceiro' },
    { value: 'O', text: 'Outros' }
];

var _opcoesQuantidadeMesesAnteriores = [
    { value: 1, text: '1 mês' },
    { value: 2, text: '2 meses' },
    { value: 3, text: '3 meses' },
    { value: 4, text: '4 meses' },
    { value: 5, text: '5 meses' },
    { value: 6, text: '6 meses' },
    { value: 7, text: '7 meses' },
    { value: 8, text: '8 meses' },
    { value: 9, text: '9 meses' },
    { value: 10, text: '10 meses' },
    { value: 11, text: '11 meses' },
    { value: 12, text: '12 meses' }
];

var PesquisaComparativoMensalFaturamentoGrupoPessoas = function () {
    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.PrimeiraDataDoMesAtual()), def: Global.PrimeiraDataDoMesAtual() });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.ApenasClientes = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), visible: ko.observable(false) })

    this.PropriedadeVeiculo = PropertyEntity({ val: ko.observable(""), options: _propriedadesVeiculo, def: "", text: "Propriedade do Veículo: " });
    this.GruposPessoas = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.selectMultiple, url: "GrupoPessoas/ObterTodos", params: { Tipo: 0, Ativo: _statusPesquisa.Todos }, def: new Array(), text: "Grupo de Pessoas: ", options: ko.observable(new Array()) });

    this.QuantidadeMesesAnteriores = PropertyEntity({ text: "Meses a Comparar:", options: _opcoesQuantidadeMesesAnteriores, val: ko.observable(1), def: 1, issue: 0, visible: ko.observable(true) });

    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioComparativoMensalFaturamentoGrupoPessoas() {
    _pesquisaComparativoMensalFaturamentoGrupoPessoas = new PesquisaComparativoMensalFaturamentoGrupoPessoas();
    KoBindings(_pesquisaComparativoMensalFaturamentoGrupoPessoas, "knockoutPesquisaComparativoMensalFaturamentoGrupoPessoas", false);
}

function GerarRelatorioPDFClick(e, sender) {
    GerarRelatorioComparativoMensalFaturamentoGrupoPessoas(EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    GerarRelatorioComparativoMensalFaturamentoGrupoPessoas(EnumTipoArquivoRelatorio.XLS);
}

function GerarRelatorioComparativoMensalFaturamentoGrupoPessoas(tipoArquivo) {
    var dados = RetornarObjetoPesquisa(_pesquisaComparativoMensalFaturamentoGrupoPessoas);

    dados["TipoArquivo"] = tipoArquivo;

    executarDownload("Relatorios/ComparativoMensalFaturamentoGrupoPessoas/GerarRelatorio", dados);
}