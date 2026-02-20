/// <reference path="Avaliacao.js" />
/// <reference path="Resumo.js" />
/// <reference path="..\..\Consultas\TabelaFrete.js" />
/// <reference path="..\..\Consultas\Tranportador.js" />
/// <reference path="..\..\Enumeradores\EnumSituacaoLicitacaoParticipacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridLicitacaoParticipacao;
var _pesquisaLicitacaoParticipacao;
var _situacaoLicitacaoParticipacao = EnumSituacaoLicitacaoParticipacao.AguardandoRetornoOferta;

/*
 * Declaração das Classes
 */

var PesquisaLicitacaoParticipacao = function () {
    this.DescricaoLicitacao = PropertyEntity({ text: "Descrição: ", maxlength: 200 });
    this.NumeroLicitacao = PropertyEntity({ text: "Licitação: ", getType: typesKnockout.int });
    this.NumeroLicitacaoParticipacao = PropertyEntity({ text: "Proposta: ", getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLicitacaoParticipacao.AguardandoRetornoOferta), options: EnumSituacaoLicitacaoParticipacao.obterOpcoesPesquisaAprovacao(), def: EnumSituacaoLicitacaoParticipacao.AguardandoRetornoOferta, text: "Situação: " });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tabela de Frete: "), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador: "), idBtnSearch: guid() });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasLicitacaoParticipacaoClick, text: "Aprovar Propostas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function () {
            _situacaoLicitacaoParticipacao = _pesquisaLicitacaoParticipacao.Situacao.val();
            recarregarGridLicitacaoParticipacao();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprovarMultiplasLicitacaoParticipacaoClick, text: "Rejeitar Propostas", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLicitacaoParticipacaoAvaliacao() {
    _pesquisaLicitacaoParticipacao = new PesquisaLicitacaoParticipacao();
    KoBindings(_pesquisaLicitacaoParticipacao, "knockoutPesquisaLicitacaoParticipacao");

    new BuscarTabelasDeFrete(_pesquisaLicitacaoParticipacao.TabelaFrete);
    new BuscarTransportadores(_pesquisaLicitacaoParticipacao.Transportador);

    loadGridLicitacaoParticipacao();
    loadAvaliacao();
    loadResumoLicitacaoParticipacao();
}

function loadGridLicitacaoParticipacao() {
    var opcaoAvaliar = { descricao: "Avaliar", id: "clasEditar", evento: "onclick", metodo: exibirModalAvaliarOfertaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoAvaliar] };
    var quantidadePorPagina = 25;
    var configuracaoExportacao = { url: "LicitacaoParticipacaoAvaliacao/ExportarPesquisa", titulo: "Propostas Disponíveis para Avaliação" };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaLicitacaoParticipacao.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    _gridLicitacaoParticipacao = new GridView(_pesquisaLicitacaoParticipacao.Pesquisar.idGrid, "LicitacaoParticipacaoAvaliacao/Pesquisa", _pesquisaLicitacaoParticipacao, menuOpcoes, null, quantidadePorPagina, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
    _gridLicitacaoParticipacao.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasLicitacaoParticipacaoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as propostas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaLicitacaoParticipacao);

        dados.SelecionarTodos = _pesquisaLicitacaoParticipacao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridLicitacaoParticipacao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridLicitacaoParticipacao.ObterMultiplosNaoSelecionados());

        executarReST("LicitacaoParticipacaoAvaliacao/AprovarMultiplas", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.PropostasModificadas > 0) {
                        if (retorno.Data.PropostasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.PropostasModificadas + " proposta foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 proposta foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "")
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma proposta pendente para aprovação.");

                    recarregarGridLicitacaoParticipacao();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function exibirModalAvaliarOfertaClick(registroSelecionado) {
    exibirAvaliacao(registroSelecionado.Codigo);
}

function reprovarMultiplasLicitacaoParticipacaoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja rejeitar todas as propostas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaLicitacaoParticipacao);

        dados.SelecionarTodos = _pesquisaLicitacaoParticipacao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridLicitacaoParticipacao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridLicitacaoParticipacao.ObterMultiplosNaoSelecionados());

        executarReST("LicitacaoParticipacaoAvaliacao/ReprovarMultiplas", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.PropostasModificadas > 0) {
                        if (retorno.Data.PropostasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.PropostasModificadas + " proposta foram rejeitadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 proposta foi rejeitada.");
                    }
                    else if (retorno.Data.Msg == "")
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma proposta pendente para rejeição.");

                    recarregarGridLicitacaoParticipacao();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

/*
 * Declaração das Funções
 */

function exibirMultiplasOpcoes() {
    _pesquisaLicitacaoParticipacao.AprovarTodas.visible(false);
    _pesquisaLicitacaoParticipacao.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridLicitacaoParticipacao.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaLicitacaoParticipacao.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoLicitacaoParticipacao == EnumSituacaoLicitacaoParticipacao.AguardandoRetornoOferta) {
            _pesquisaLicitacaoParticipacao.AprovarTodas.visible(true);
            _pesquisaLicitacaoParticipacao.RejeitarTodas.visible(true);
        }
    }
}

function recarregarGridLicitacaoParticipacao() {
    _gridLicitacaoParticipacao.CarregarGrid();
}