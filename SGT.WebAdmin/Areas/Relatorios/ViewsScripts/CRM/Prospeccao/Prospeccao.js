/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioProspeccoes, _gridProspeccoes, _pesquisaProspeccoes, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoContato = [
    { text: "Todos", value: "" },
    { text: "Telefone", value: EnumTipoContatoAtendimento.Telefone },
    { text: "Email", value: EnumTipoContatoAtendimento.Email },
    { text: "Skype", value: EnumTipoContatoAtendimento.Skype },
    { text: "Chat Web", value: EnumTipoContatoAtendimento.ChatWeb }
];

var _satisfacao = [
    { text: "Todos", value: "" },
    { text: "Não Avaliado", value: EnumNivelSatisfacao.NaoAvaliado },
    { text: "Ótimo", value: EnumNivelSatisfacao.Otimo },
    { text: "Bom", value: EnumNivelSatisfacao.Bom },
    { text: "Ruim", value: EnumNivelSatisfacao.Ruim }
];

var _faturado = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var PesquisaProspeccoes = function () {
    this.DataLancamentoInicial = PropertyEntity({ text: "Data Lançamento Inicial: ", getType: typesKnockout.date });
    this.DataLancamentoFinal = PropertyEntity({ text: "Data Lançamento Final: ", getType: typesKnockout.date });
    this.DataLancamentoInicial.dateRangeLimit = this.DataLancamentoFinal;
    this.DataLancamentoFinal.dateRangeInit = this.DataLancamentoInicial;

    this.DataRetornoInicial = PropertyEntity({ text: "Data Retorno Inicial: ", getType: typesKnockout.date });
    this.DataRetornoFinal = PropertyEntity({ text: "Data Retorno Final: ", getType: typesKnockout.date });
    this.DataRetornoInicial.dateRangeLimit = this.DataRetornoFinal;
    this.DataRetornoFinal.dateRangeInit = this.DataRetornoInicial;

    this.Usuario = PropertyEntity({ text: "Usuário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Produto = PropertyEntity({ text: "Produto:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ text: "Cliente:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Cidade = PropertyEntity({ text: "Cidade:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.OrigemContato = PropertyEntity({ text: "Origem do Contato:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.CNPJ = PropertyEntity({ text: "CNPJ:", type: types.map, getType: typesKnockout.cnpj });

    this.TipoContato = PropertyEntity({ val: ko.observable(""), options: _tipoContato, text: "Tipo do Contato: " });
    this.Satisfacao = PropertyEntity({ val: ko.observable(""), options: _satisfacao, text: "Satisfação: " });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoProspeccao.Todos), options: EnumSituacaoProspeccao.obterOpcoesPesquisa(), def: EnumSituacaoProspeccao.Todos });
    this.Faturado = PropertyEntity({ val: ko.observable(""), options: _faturado, text: "Faturado: " });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridProspeccoes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioProspeccoes() {
    _pesquisaProspeccoes = new PesquisaProspeccoes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridProspeccoes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Prospeccao/Pesquisa", _pesquisaProspeccoes, null, null, 10);
    _gridProspeccoes.SetPermitirEdicaoColunas(true);

    _relatorioProspeccoes = new RelatorioGlobal("Relatorios/Prospeccao/BuscarDadosRelatorio", _gridProspeccoes, function () {
        _relatorioProspeccoes.loadRelatorio(function () {
            KoBindings(_pesquisaProspeccoes, "knockoutPesquisaProspeccao");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaProspeccao");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaProspeccao");

            // Buscas
            new BuscarFuncionario(_pesquisaProspeccoes.Usuario);
            new BuscarClienteProspect(_pesquisaProspeccoes.Cliente);
            new BuscarLocalidades(_pesquisaProspeccoes.Cidade);
            new BuscarProdutoProspect(_pesquisaProspeccoes.Produto);
            new BuscarOrigemContatoClienteProspect(_pesquisaProspeccoes.OrigemContato);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaProspeccoes);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioProspeccoes.gerarRelatorio("Relatorios/Prospeccao/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioProspeccoes.gerarRelatorio("Relatorios/Prospeccao/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}