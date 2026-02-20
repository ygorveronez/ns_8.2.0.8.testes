/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTermoQuitacao;
var _pesquisaTermoQuitacao;
var _rejeicao;
var _situacaoTermoQuitacaoUltimaPesquisa = EnumSituacaoTermoQuitacao.AguardandoAprovacao;
var _termoQuitacao;
var $modalDetalhesTermoQuitacao;

/*
 * Declaração das Classes
 */

var TermoQuitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.DataBase = PropertyEntity({ text: "Data Base: " });
    this.Numero = PropertyEntity({ text: "Número: " });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: " });
    this.Transportador = PropertyEntity({ text: "Transportador: " });
}

var PesquisaTermoQuitacao = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.DataBaseInicial = PropertyEntity({ text: "Data Base Inicial: ", getType: typesKnockout.date });
    this.DataBaseLimite = PropertyEntity({ text: "Data Base Limite: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTermoQuitacao.AguardandoAprovacao), options: EnumSituacaoTermoQuitacao.obterOpcoesPesquisa(), def: EnumSituacaoTermoQuitacao.AguardandoAprovacao, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataBaseInicial.dateRangeLimit = this.DataBaseLimite;
    this.DataBaseLimite.dateRangeInit = this.DataBaseInicial;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosTermosQuitacaoClick, text: "Aprovar Termos de Quitação", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridTermoQuitacao, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosTermosQuitacaoClick, text: "Rejeitar Termos de Quitação", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Termos de Quitação", visible: ko.observable(false) });
}

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarTermosQuitacaoSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _termoQuitacao = new TermoQuitacao();
    KoBindings(_termoQuitacao, "knockoutTermoQuitacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoTermoQuitacao");

    _pesquisaTermoQuitacao = new PesquisaTermoQuitacao();
    KoBindings(_pesquisaTermoQuitacao, "knockoutPesquisaTermoQuitacao");

    loadGridTermoQuitacao();
    loadRegras();
    loadDelegar();

    $modalDetalhesTermoQuitacao = $("#divModalTermoQuitacao");

    new BuscarTransportadores(_pesquisaTermoQuitacao.Transportador);
    new BuscarFuncionario(_pesquisaTermoQuitacao.Usuario);

    loadDadosUsuarioLogado(atualizarGridTermoQuitacao);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaTermoQuitacao.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaTermoQuitacao.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridTermoQuitacao() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharTermoQuitacao,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoDetalhes]
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaTermoQuitacao.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoTermoQuitacao/ExportarPesquisa",
        titulo: "Autorização de Termos de Quitação"
    };

    _gridTermoQuitacao = new GridView(_pesquisaTermoQuitacao.Pesquisar.idGrid, "AutorizacaoTermoQuitacao/Pesquisa", _pesquisaTermoQuitacao, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplosTermosQuitacaoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os termos de quitação selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTermoQuitacao);

        dados.SelecionarTodos = _pesquisaTermoQuitacao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTermoQuitacao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTermoQuitacao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTermoQuitacao/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "")
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridTermoQuitacao();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    Global.fecharModal('divModalRejeitarTermoQuitacao');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplosTermosQuitacaoClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarTermoQuitacao');
}

function rejeitarTermosQuitacaoSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os termos de quitação selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTermoQuitacao);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaTermoQuitacao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTermoQuitacao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTermoQuitacao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTermoQuitacao/ReprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridTermoQuitacao();
                    cancelarRejeicaoSelecionadosClick();
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
 * Declaração das Funções Públicas
 */

function atualizarGridTermoQuitacao() {
    _pesquisaTermoQuitacao.SelecionarTodos.val(false);
    _pesquisaTermoQuitacao.AprovarTodas.visible(false);
    _pesquisaTermoQuitacao.DelegarTodas.visible(false);
    _pesquisaTermoQuitacao.RejeitarTodas.visible(false);

    _gridTermoQuitacao.CarregarGrid();

    _situacaoTermoQuitacaoUltimaPesquisa = _pesquisaTermoQuitacao.Situacao.val()
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalDetalhesTermoQuitacao() {
    Global.abrirModal("divModalTermoQuitacao");
    $modalDetalhesTermoQuitacao.one('hidden.bs.modal', function () {
        limparCamposTermoQuitacao();
    });
}

function exibirMultiplasOpcoes() {
    _pesquisaTermoQuitacao.AprovarTodas.visible(false);
    _pesquisaTermoQuitacao.DelegarTodas.visible(false);
    _pesquisaTermoQuitacao.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridTermoQuitacao.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaTermoQuitacao.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoTermoQuitacaoUltimaPesquisa == EnumSituacaoTermoQuitacao.AguardandoAprovacao) {
            _pesquisaTermoQuitacao.AprovarTodas.visible(true);
            _pesquisaTermoQuitacao.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaTermoQuitacao.RejeitarTodas.visible(true);
        }
    }
}

function detalharTermoQuitacao(registroSelecionado) {
    limparCamposTermoQuitacao();

    _termoQuitacao.Usuario.val(_pesquisaTermoQuitacao.Usuario.val());

    executarReST("AutorizacaoTermoQuitacao/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_termoQuitacao, retorno);
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoTermoQuitacao.AguardandoAprovacao);
                exibirModalDetalhesTermoQuitacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposTermoQuitacao() {
    Global.ResetarAbas();

    limparRegras();
}
