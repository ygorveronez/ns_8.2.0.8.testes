/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentrosDescarregamento.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAgendamentoPallet.js" />
/// <reference path="CapacidadeDescarregamentoDados.js" />
/// <reference path="ComposicaoHorarioDescarregamento.js" />
/// <reference path="HorarioDescarregamento.js" />
/// <reference path="HorarioDescarregamentoPeriodo.js" />
/// <reference path="TabelaDescarregamento.js" />
/// <reference path="Observacoes.js" />
/// <reference path="DisponibilidadeVeiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _centroDescarregamentoAtual;
var _dadosPesquisaDescarregamento;
var _legendaJanelaDescarregamento;
var _pesquisaJanelasDescarregamento;
var _situacoesCadastradas = [];
var _tabelaDescarregamento;
var _tiposIntegracao;
var _motivoCancelamentoAgendamento;
/*
 * Declaração das Classes
 */

var LegendaJanelaDescarregamento = function () {
    this.ExibirLegendas = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.UtilizarSituacaoNaJanelaDescarregamento), getType: typesKnockout.bool });
    this.ExibirLegendasPorSituacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });

    this.AguardandoConfirmacaoAgendamento = PropertyEntity({ text: ko.observable("Ag. Confirmação de Agendamento") });
    this.AguardandoDescarregamento = PropertyEntity({ text: "Ag. Descarregamento" });
    this.DescarregamentoFinalizado = PropertyEntity({ text: "Descarregamento Finalizado" });
    this.NaoComparecimento = PropertyEntity({ text: "Não Comparecido" });
    this.DevolvidaParcialmente = PropertyEntity({ text: "Devolvida Parcialmente" });
    this.CargaDevolvida = PropertyEntity({ text: "Carga Devolvida" });
    this.EntregueParcialmente = PropertyEntity({ text: "Entregue Parcialmente" });
    this.AguardandoGeracaoSenha = PropertyEntity({ text: "Ag. Geração Senha" });
    this.ChegadaConfirmada = PropertyEntity({ text: "Chegada Confirmada" });
    this.SaidaVeiculoConfirmada = PropertyEntity({ text: "Saída do Veículo Confirmada" });
    this.Excedente = PropertyEntity({ text: "Excedente", cssClass: ko.observable(""), visible: ko.observable(false) });
    this.Desagendado = PropertyEntity({ text: "Desagendado", cssClass: 'legenda-desagendado', visible: _ConfiguracoesJanelaDescarga.PermiteExcluirAgendamentoDaCargaJanelaDescarga });
    this.Cancelado = PropertyEntity({ text: "Cancelado" });
    this.ValidacaoFiscal = PropertyEntity({ text: "Validação Fiscal" });
    this.Nucleo = PropertyEntity({ text: "Validação Núcleo" });
    this.ListaLegendaDinamica = ko.observableArray([]);
    this.ListaLegendaPorSituacao = ko.observableArray([]);
}

var PesquisaJanelaDescarregamento = function () {
    this.CentroDescarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Descarregamento:", idBtnSearch: guid(), required: true });
    this.DataDescarregamento = PropertyEntity({ text: "*Data de Descarregamento: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), required: !_CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo, visible: ko.observable(!_CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo) });
    this.DataDescarregamentoInicial = PropertyEntity({ text: "*Data Descarregamento Inicial: ", getType: typesKnockout.date, dateRangeInit: this.DataDescarregamentoFinal, val: ko.observable(""), required: _CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo, visible: ko.observable(_CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo) });
    this.DataDescarregamentoFinal = PropertyEntity({ text: "*Data Descarregamento Final: ", getType: typesKnockout.date, dateRangeLimit: this.DataDescarregamentoInicial, val: ko.observable(""), required: _CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo, visible: ko.observable(_CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaDescarregamento.Todas), options: EnumSituacaoCargaJanelaDescarregamento.obterOpcoesPesquisa(true), def: EnumSituacaoCargaJanelaDescarregamento.Todas, text: "Situação: ", visible: ko.observable(true), getType: typesKnockout.selectMultiple });
    this.SituacaoCadastrada = PropertyEntity({ options: ko.observable([]), text: "Situação: ", visible: ko.observable(false) });
    this.SituacaoAgendamentoPallet = PropertyEntity({ val: ko.observable(EnumSituacaoAgendamentoPallet.Todas), options: EnumSituacaoAgendamentoPallet.obterOpcoesPesquisa(), def: EnumSituacaoAgendamentoPallet.Todas, text: "Tipo de Agendamento: ", visible: ko.observable(true) });
    this.SenhaAgendamento = PropertyEntity({ text: "Senha do Agendamento:" });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga:" });
    this.NumeroPedido = PropertyEntity({ text: "Número do Pedido:" });
    this.DataLancamento = PropertyEntity({ text: "Data do Lançamento:", getType: typesKnockout.date });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veiculo:", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.NumeroNF = PropertyEntity({ text: "Número da NF:" });
    this.NumeroCTe = PropertyEntity({ text: "Número do CT-e:" });
    this.NumeroLacre = PropertyEntity({ text: "Número do Lacre:" });
    this.ExcedenteDescarregamento = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos2), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), def: EnumSimNaoPesquisa.Todos2, text: "Excedente: ", visible: ko.observable(true) });
    this.DataDescarregamentoInicial.dateRangeLimit = this.DataDescarregamentoFinal;
    this.DataDescarregamentoFinal.dateRangeInit = this.DataDescarregamentoInicial;

    this.Adicionar = PropertyEntity({
        eventClick: function () {
            if (_CONFIGURACAO_TMS.AlterarDataCarregamentoEDescarregamentoPorPeriodo)
                exibirModalAdicionarJanelaDescarregamentoPeriodo();
            else
                exibirModalAdicionarJanelaDescarregamento();
        }, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(false)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            carregarDadosPesquisa();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var MotivoCancelamentoAgendamento = function () {
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), def: "" });

    this.Cancelar = PropertyEntity({ text: "Cancelar", type: types.event, eventClick: function () { } });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadJanelaDescarregamento() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarConfiguracaoJanelaDescarregamento(function (configuracaoJanelaDescarregamento) {
                _pesquisaJanelasDescarregamento = new PesquisaJanelaDescarregamento();
                KoBindings(_pesquisaJanelasDescarregamento, "knockoutPesquisaJanelaDescarregamento", false, _pesquisaJanelasDescarregamento.Pesquisar.id);

                BuscarCentrosDescarregamento(_pesquisaJanelasDescarregamento.CentroDescarregamento);
                BuscarClientesFactory(_pesquisaJanelasDescarregamento.Fornecedor);
                BuscarVeiculos(_pesquisaJanelasDescarregamento.Veiculo);
                BuscarTiposdeCarga(_pesquisaJanelasDescarregamento.TipoCarga);
                BuscarTransportadores(_pesquisaJanelasDescarregamento.Transportador);

                _legendaJanelaDescarregamento = new LegendaJanelaDescarregamento();
                KoBindings(_legendaJanelaDescarregamento, "knockoutLegendaJanelaDescarregamento");
                PreencherObjetoKnoutLegenda(_legendaJanelaDescarregamento, configuracaoJanelaDescarregamento.Legendas);
                carregarLegendas(configuracaoJanelaDescarregamento);

                _motivoCancelamentoAgendamento = new MotivoCancelamentoAgendamento();
                KoBindings(_motivoCancelamentoAgendamento, "knockoutMotivoCancelamentoAgendamento");

                _tabelaDescarregamento = new TabelaDescarregamento();
                obterTiposIntegracao();

                loadHorarioDescarregamento();
                loadAlterarAgendamento();
                loadHorarioDescarregamentoPeriodo();
                loadCapacidadeDescarregamentoDados();
                loadInformarSenhaAgendamento();
                loadCargaAcaoParcial();
                loadAdicionarJanelaDescarregamento();
                loadAdicionarJanelaDescarregamentoPeriodo();
                loadSenhasAgendamento();
                loadComposicaoHorarioDescarregamento();
                loadDetalhesMonitoramento();
                loadHistoricoMonitoramento();
                loadObservacoes();
                loadDisponibilidadeVeiculo();
                loadConfirmarChegada();
                loadDescargaArmazemExterno();
                loadIntegracaoAVIPED();
            });
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function carregarDadosPesquisa() {
    if (!ValidarCamposObrigatorios(_pesquisaJanelasDescarregamento)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return
    }

    _pesquisaJanelasDescarregamento.Adicionar.visible(!_FormularioSomenteLeitura);

    _dadosPesquisaDescarregamento = RetornarObjetoPesquisa(_pesquisaJanelasDescarregamento);

    carregarInformacoesCentroDescarregamento(function () {
        $("#container-descarregamento").show();

        _legendaJanelaDescarregamento.Excedente.visible(!_centroDescarregamentoAtual.BloquearJanelaDescarregamentoExcedente);

        _pesquisaJanelasDescarregamento.ExibirFiltros.visibleFade(false);
        _tabelaDescarregamento.Load();
        buscarCapacidadeDescarregamentoDados();

        if (_CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo) {
            setTimeout(function () {
                $(".btn-exportar a").addClass("pull-right margin-top-5");
            }, 6000);
        }
    });
}

/*
 * Declaração das Funções Privadas
 */

function carregarConfiguracaoJanelaDescarregamento(callback) {
    executarReST("JanelaDescarga/ObterConfiguracao", undefined, function (retorno) {
        if (retorno.Success)
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function carregarInformacoesCentroDescarregamento(callback) {
    executarReST("JanelaDescarga/ObterInformacoesCentroDescarregamento", _dadosPesquisaDescarregamento, function (retorno) {
        if (retorno.Success) {
            _centroDescarregamentoAtual = retorno.Data;
            callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function carregarLegendas(configuracaoJanelaDescarregamento) {
    carregarLegendasDinamicas(configuracaoJanelaDescarregamento.LegendasDinamicas);

    if (configuracaoJanelaDescarregamento.LegendasPorSituacao.length > 0) {
        carregarLegendasPorSituacao(configuracaoJanelaDescarregamento.LegendasPorSituacao);
        
        _legendaJanelaDescarregamento.ExibirLegendas.val(false);
        _legendaJanelaDescarregamento.ExibirLegendasPorSituacao.val(_CONFIGURACAO_TMS.UtilizarSituacaoNaJanelaDescarregamento);
        _pesquisaJanelasDescarregamento.Situacao.visible(false);
        _pesquisaJanelasDescarregamento.SituacaoCadastrada.visible(true);
    }
}

function carregarLegendasDinamicas(legendasDinamicas) {
    for (var i = 0; i < legendasDinamicas.length; i++) {
        var legendaDinamica = legendasDinamicas[i];

        _legendaJanelaDescarregamento.ListaLegendaDinamica.push({
            text: legendaDinamica.Descricao,
            cssClass: Global.ObterClasseDinamica(legendaDinamica.Cores)
        });
    }
}

function carregarLegendasPorSituacao(legendasPorSituacao) {
    var situacoes = [{ text: "Todas", value: "" }];

    for (var i = 0; i < legendasPorSituacao.length; i++) {
        var legendaPorSituacao = legendasPorSituacao[i];

        _situacoesCadastradas.push({ Codigo: legendaPorSituacao.Codigo, Situacao: legendaPorSituacao.Situacao });
        situacoes.push({ text: legendaPorSituacao.Descricao, value: legendaPorSituacao.Codigo });

        _legendaJanelaDescarregamento.ListaLegendaPorSituacao.push({
            text: legendaPorSituacao.Descricao,
            cssClass: Global.ObterClasseDinamica(legendaPorSituacao.Cores)
        });
    }

    _pesquisaJanelasDescarregamento.SituacaoCadastrada.options(situacoes);
}

function obterTiposIntegracao() {
    _tiposIntegracao = new Array();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.SAD
        ])
    }, function (r) {
        if (r.Success) {
            for (var i = 0; i < r.Data.length; i++)
                _tiposIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}
