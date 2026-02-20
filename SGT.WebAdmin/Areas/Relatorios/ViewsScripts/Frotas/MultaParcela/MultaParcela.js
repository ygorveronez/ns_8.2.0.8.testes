/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoHistoricoInfracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumResponsavelPagamentoInfracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumNivelInfracaoTransito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoInfracaoTransito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoOcorrenciaInfracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMultaParcela, _pesquisaMultaParcela, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioMultaParcela;

var PesquisaMultaParcela = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.NumeroAtuacao = PropertyEntity({ getType: typesKnockout.string, text: "Nº Autuação:" });
    this.NumeroMulta = PropertyEntity({ getType: typesKnockout.int, text: "Nº Multa:" });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial:", val: ko.observable("") });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Final:", val: ko.observable("") });

    this.PagoPor = PropertyEntity({ val: ko.observable(EnumResponsavelPagamentoInfracao.Todos), options: EnumResponsavelPagamentoInfracao.obterOpcoesPesquisa(), def: EnumResponsavelPagamentoInfracao.Todos, text: "Pago Por: " });
    this.Cidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.TipoInfracao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Infração:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.DataVencimentoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Vencimento Inicial:", val: ko.observable("") });
    this.DataVencimentoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Vencimento Final:", val: ko.observable("") });
    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Título:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.StatusMulta = PropertyEntity({ val: ko.observable(EnumSituacaoInfracao.Todos), options: EnumSituacaoInfracao.obterOpcoes(), def: EnumSituacaoInfracao.Todos, text: "Situação da Multa: " });
    this.TipoTipoInfracao = PropertyEntity({ val: ko.observable(EnumTipoInfracaoTransito.Todos), options: EnumTipoInfracaoTransito.obterOpcoesPesquisa(), def: EnumSituacaoInfracao.Todos, text: "Tipo do Tipo de Infração: " });
    this.NivelInfracao = PropertyEntity({ val: ko.observable(EnumNivelInfracaoTransito.Todos), options: EnumNivelInfracaoTransito.obterOpcoesPesquisa(), def: EnumSituacaoInfracao.Todos, text: "Nível da Infração: " });

    this.DataVencimentoInicialPagar = PropertyEntity({ getType: typesKnockout.date, text: "Vcto. Inicial a Pagar:", val: ko.observable("") });
    this.DataVencimentoFinalPagar = PropertyEntity({ getType: typesKnockout.date, text: "Vcto. Final a Pagar:", val: ko.observable("") });
    this.DataLancamentoInicial = PropertyEntity({ getType: typesKnockout.date, text: "Lançamento Inicial:", val: ko.observable("") });
    this.DataLancamentoFinal = PropertyEntity({ getType: typesKnockout.date, text: "Lançamento Final:", val: ko.observable("") });
    this.FornecedorPagar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DataLimiteInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data Limite Ind. Condutor Inicial:", val: ko.observable("") });
    this.DataLimiteFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data Limite Ind. Condutor Final:", val: ko.observable("") });
    this.TipoOcorrenciaInfracao = PropertyEntity({ val: ko.observable(""), options: EnumTipoOcorrenciaInfracao.ObterOpcoesPesquisa(), def: EnumTipoOcorrenciaInfracao.Todos, text: "Tipo de Ocorrência: " });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataLimiteInicial.dateRangeLimit = this.DataLimiteFinal;
    this.DataLimiteFinal.dateRangeInit = this.DataLimiteInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMultaParcela.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaMultaParcela.Visible.visibleFade()) {
                _pesquisaMultaParcela.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaMultaParcela.Visible.visibleFade(true);
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

function LoadMultaParcela() {
    _pesquisaMultaParcela = new PesquisaMultaParcela();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMultaParcela = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MultaParcela/Pesquisa", _pesquisaMultaParcela);

    _gridMultaParcela.SetPermitirEdicaoColunas(true);
    _gridMultaParcela.SetQuantidadeLinhasPorPagina(10);

    _relatorioMultaParcela = new RelatorioGlobal("Relatorios/MultaParcela/BuscarDadosRelatorio", _gridMultaParcela, function () {
        _relatorioMultaParcela.loadRelatorio(function () {
            KoBindings(_pesquisaMultaParcela, "knockoutPesquisaMultaParcela", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMultaParcela", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMultaParcela", false);

            new BuscarVeiculos(_pesquisaMultaParcela.Veiculo);
            new BuscarLocalidades(_pesquisaMultaParcela.Cidade);
            new BuscarTipoInfracao(_pesquisaMultaParcela.TipoInfracao);
            new BuscarMotoristas(_pesquisaMultaParcela.Motorista);
            new BuscarClientes(_pesquisaMultaParcela.Pessoa);
            new BuscarClientes(_pesquisaMultaParcela.FornecedorPagar);
            new BuscarTitulo(_pesquisaMultaParcela.Titulo, null, null, RetornoTitulo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMultaParcela);
}

function RetornoTitulo(data) {
    _pesquisaMultaParcela.Titulo.codEntity(data.Codigo);
    _pesquisaMultaParcela.Titulo.val(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMultaParcela.gerarRelatorio("Relatorios/MultaParcela/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMultaParcela.gerarRelatorio("Relatorios/MultaParcela/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
