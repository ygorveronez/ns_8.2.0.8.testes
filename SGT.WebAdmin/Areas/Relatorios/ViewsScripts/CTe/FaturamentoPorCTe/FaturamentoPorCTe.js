/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFatura.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoFaturamentoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioFaturamentoPorCTe, _gridFaturamentoPorCTe, _pesquisaFaturamentoPorCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _situacaoCTe = [
    { text: "Autorizado", value: "A" },
    { text: "Pendente", value: "P" },
    { text: "Enviado", value: "E" },
    { text: "Rejeitado", value: "R" },
    { text: "Cancelado", value: "C" },
    { text: "Anulado", value: "Z" },
    { text: "Inutilizado", value: "I" },
    { text: "Denegado", value: "D" },
    { text: "Em Digitação", value: "S" },
    { text: "Em Cancelamento", value: "K" },
    { text: "Em Inutilização", value: "L" }
];

var _situacaoFatura = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacoesFatura.EmAndamento },
    { text: "Fechada", value: EnumSituacoesFatura.Fechado }
];

var _tipoTomador = [
    { value: EnumTipoTomador.Destinatario, text: "Destinatário" },
    { value: EnumTipoTomador.Expedidor, text: "Expedidor" },
    { value: EnumTipoTomador.Recebedor, text: "Recebedor" },
    { value: EnumTipoTomador.Remetente, text: "Remetente" },
    { value: EnumTipoTomador.Outros, text: "Outros" }
];

var PesquisaFaturamentoPorCTe = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.DataInicialFatura = PropertyEntity({ text: "Data Fatura Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalFatura = PropertyEntity({ text: "Data Fatura Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialFatura.dateRangeLimit = this.DataFinalFatura;
    this.DataFinalFatura.dateRangeInit = this.DataInicialFatura;

    this.DataInicialVencimentoFatura = PropertyEntity({ text: "Data Vencimento Fatura Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalVencimentoFatura = PropertyEntity({ text: "Data Vencimento Fatura Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialVencimentoFatura.dateRangeLimit = this.DataFinalVencimentoFatura;
    this.DataFinalVencimentoFatura.dateRangeInit = this.DataInicialVencimentoFatura;

    this.DataInicialPrevisaoSaidaNavio = PropertyEntity({ text: "Previsão Saída Navio Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataFinalPrevisaoSaidaNavio = PropertyEntity({ text: "Previsão Saída Navio Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.DataInicialPrevisaoSaidaNavio.dateRangeLimit = this.DataFinalPrevisaoSaidaNavio;
    this.DataFinalPrevisaoSaidaNavio.dateRangeInit = this.DataInicialPrevisaoSaidaNavio;

    this.NumeroInicial = PropertyEntity({ text: "Núm. Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Núm. Final: ", getType: typesKnockout.int });
    this.NFe = PropertyEntity({ text: "Núm. NF-e: " });
    this.NumeroBoleto = PropertyEntity({ text: "Núm. Boleto: " });
    this.NumeroFatura = PropertyEntity({ text: "Núm. Fatura: ", getType: typesKnockout.int });
    this.NumeroTitulo = PropertyEntity({ text: "Núm. Título: ", getType: typesKnockout.int });

    this.SituacaoFatura = PropertyEntity({ text: "Situação da Fatura:", options: _situacaoFatura, val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pessoas:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.SituacaoFaturamentoCte = PropertyEntity({ text: "Sit. do Faturamento do CT-e:", options: EnumSituacaoFaturamentoCTe.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoFaturamentoCTe.Todos), def: EnumSituacaoFaturamentoCTe.Todos, visible: ko.observable(true) });
    this.TipoProposta = PropertyEntity({ text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesPesquisa(), val: ko.observable(EnumTipoPropostaMultimodal.Todos), def: EnumTipoPropostaMultimodal.Todos, visible: ko.observable(true) });
    this.VeioPorImportacao = PropertyEntity({ text: "Veio por importação:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.CTeSubstituido = PropertyEntity({ text: "Somente CT-e Substituído", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo do CT-e:", visible: ko.observable(true) });
    this.TipoServico = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoServicoMultimodal.obterOpcoesSemNumero(), text: "Tipo de Serviço:", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _situacaoCTe, text: "Situação do CT-e:", issue: 120, visible: ko.observable(true) });
    this.TipoTomador = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _tipoTomador, text: "Tipo do Tomador:", visible: ko.observable(true) });
    this.StatusTitulo = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Situação Título:", options: EnumSituacaoTitulo.obterOpcoes() });

    //Emissão Multimodal
    this.NumeroBooking = PropertyEntity({ text: "Número Booking:" });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });
    this.NumeroControle = PropertyEntity({ text: "Número Controle:" });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.Todas), def: EnumSituacoesCarga.Todas, text: "Situação Carga:", options: EnumSituacoesCarga.obterOpcoesPesquisaTMS(), visible: ko.observable(true) });
    this.SituacaoCargaMercante = PropertyEntity({ text: "Situação Carga: ", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridFaturamentoPorCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFaturamentoPorCTe.Visible.visibleFade()) {
                _pesquisaFaturamentoPorCTe.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFaturamentoPorCTe.Visible.visibleFade(true);
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

function LoadFaturamentoPorCTe() {
    _pesquisaFaturamentoPorCTe = new PesquisaFaturamentoPorCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridFaturamentoPorCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/FaturamentoPorCTe/Pesquisa", _pesquisaFaturamentoPorCTe, null, null, 10);
    _gridFaturamentoPorCTe.SetPermitirEdicaoColunas(true);

    _relatorioFaturamentoPorCTe = new RelatorioGlobal("Relatorios/FaturamentoPorCTe/BuscarDadosRelatorio", _gridFaturamentoPorCTe, function () {
        _relatorioFaturamentoPorCTe.loadRelatorio(function () {
            KoBindings(_pesquisaFaturamentoPorCTe, "knockoutPesquisaFaturamentoPorCTe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaFaturamentoPorCTe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaFaturamentoPorCTe", false);

            new BuscarCargas(_pesquisaFaturamentoPorCTe.Carga);
            new BuscarPedidoViagemNavio(_pesquisaFaturamentoPorCTe.Viagem);
            new BuscarPorto(_pesquisaFaturamentoPorCTe.PortoOrigem);
            new BuscarPorto(_pesquisaFaturamentoPorCTe.PortoDestino);
            new BuscarClientes(_pesquisaFaturamentoPorCTe.Tomador);
            new BuscarGruposPessoas(_pesquisaFaturamentoPorCTe.GrupoPessoas);
            new BuscarTiposOperacao(_pesquisaFaturamentoPorCTe.TipoOperacao);

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaFaturamentoPorCTe.SituacaoCarga.visible(false);
                _pesquisaFaturamentoPorCTe.SituacaoCargaMercante.visible(true);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaFaturamentoPorCTe);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioFaturamentoPorCTe.gerarRelatorio("Relatorios/FaturamentoPorCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioFaturamentoPorCTe.gerarRelatorio("Relatorios/FaturamentoPorCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}