/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoHistoricoInfracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumResponsavelPagamentoInfracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoInfracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumNivelInfracaoTransito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoInfracaoTransito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoOcorrenciaInfracao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMulta, _pesquisaMulta, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioMulta;

var PesquisaMulta = function () {
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
    this.TipoMotorista = PropertyEntity({ val: ko.observable(EnumTipoMotorista.Todos), options: EnumTipoMotorista.obterOpcoesPesquisa(), def: EnumTipoMotorista.Todos, text: "Tipo Motorista: ", issue: 640, required: true });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicialEmissaoInfracao = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial da Emissão da Infração:", val: ko.observable("") });
    this.DataFinalEmissaoInfracao = PropertyEntity({ getType: typesKnockout.date, text: "Data Final da Emissão da Infração:", val: ko.observable("") });

    this.DataInicialEmissaoInfracao.dateRangeLimit = this.DataLimiteFinal;
    this.DataLimiteFinal.dateRangeInit = this.DataLimiteInicial;
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMulta.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaMulta.Visible.visibleFade() === true) {
                _pesquisaMulta.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaMulta.Visible.visibleFade(true);
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

function LoadMulta() {
    _pesquisaMulta = new PesquisaMulta();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMulta = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Multa/Pesquisa", _pesquisaMulta);

    _gridMulta.SetPermitirEdicaoColunas(true);
    _gridMulta.SetQuantidadeLinhasPorPagina(10);

    _relatorioMulta = new RelatorioGlobal("Relatorios/Multa/BuscarDadosRelatorio", _gridMulta, function () {
        _relatorioMulta.loadRelatorio(function () {
            KoBindings(_pesquisaMulta, "knockoutPesquisaMulta", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMulta", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMulta", false);

            BuscarVeiculos(_pesquisaMulta.Veiculo);
            BuscarLocalidades(_pesquisaMulta.Cidade);
            BuscarTipoInfracao(_pesquisaMulta.TipoInfracao);
            BuscarMotoristas(_pesquisaMulta.Motorista);
            BuscarClientes(_pesquisaMulta.Pessoa);
            BuscarClientes(_pesquisaMulta.FornecedorPagar);
            BuscarTitulo(_pesquisaMulta.Titulo, null, null, RetornoTitulo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMulta);
}

function RetornoTitulo(data) {
    _pesquisaMulta.Titulo.codEntity(data.Codigo);
    _pesquisaMulta.Titulo.val(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMulta.gerarRelatorio("Relatorios/Multa/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMulta.gerarRelatorio("Relatorios/Multa/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
