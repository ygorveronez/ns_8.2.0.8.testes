/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoBiddingConvite.js" />
/// <reference path="../../Consultas\TipoDeBidding.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridBiddings;
var _bidding;
var _pesquisaBidding;
var _rejeicao;
var _situacaoBiddingUltimaPesquisa = EnumSituacaoBiddingConvite.AguardandoAprovacao;
var $modalDetalhesBidding;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarBiddingsSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var Bidding = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Cidade = PropertyEntity({ text: "Cidade: ", visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.TotalSpendBidding = PropertyEntity({ text: "Total Spend Bidding: ", visible: ko.observable(true) });
};

var PesquisaBidding = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.TipoBidding = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Bidding: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitante: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasBiddingsClick, text: "Aprovar Biddings", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridBiddings, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasBiddingsClick, text: "Rejeitar Biddings", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Biddings", visible: ko.observable(false) });
    this.Situacao = PropertyEntity({
        val: ko.observable([EnumSituacaoBiddingConvite.AguardandoAprovacao, EnumSituacaoBiddingConvite.AprovacaoRejeitada, EnumSituacaoBiddingConvite.Fechamento]),
        def: [EnumSituacaoBiddingConvite.AguardandoAprovacao, EnumSituacaoBiddingConvite.AprovacaoRejeitada, EnumSituacaoBiddingConvite.Fechamento],
        getType: typesKnockout.selectMultiple, text: "Situação:", options: EnumSituacaoBiddingConvite.ObterOpcoesPesquisa()
    });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _bidding = new Bidding();
    KoBindings(_bidding, "knockoutBidding");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoBidding");

    _pesquisaBidding = new PesquisaBidding();
    KoBindings(_pesquisaBidding, "knockoutPesquisaBidding");

    loadGridBiddings();
    loadRegras();
    loadDelegar();

    $modalDetalhesBidding = $("#divModalBidding");

    BuscarFuncionario(_pesquisaBidding.Usuario);
    BuscarFuncionario(_pesquisaBidding.Solicitante);
    BuscarTipoDeBidding(_pesquisaBidding.TipoBidding)

    loadDadosUsuarioLogado(atualizarGridBiddings);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaBidding.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaBidding.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridBiddings() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharBidding,
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
        SelecionarTodosKnout: _pesquisaBidding.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoBidding/ExportarPesquisa",
        titulo: "Autorização Bidding"
    };

    _gridBiddings = new GridView(_pesquisaBidding.Pesquisar.idGrid, "AutorizacaoBidding/Pesquisa", _pesquisaBidding, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasBiddingsClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as Biddings selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaBidding);

        dados.SelecionarTodos = _pesquisaBidding.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridBiddings.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridBiddings.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoBidding/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    atualizarGridBiddings();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    Global.fecharModal("divModalRejeitarBidding");
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal("divModalDelegarSelecionados");
}

function rejeitarMultiplasBiddingsClick() {
    LimparCampos(_rejeicao);

    $("#divModalRejeitarBidding").modal("show");
}

function rejeitarBiddingsSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as Biddings selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaBidding);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaBidding.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridBiddings.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridBiddings.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoBidding/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridBiddings();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}

/*
 * Declaração das Funções
 */

function atualizarGridBiddings() {
    _pesquisaBidding.SelecionarTodos.val(false);
    _pesquisaBidding.AprovarTodas.visible(false);
    _pesquisaBidding.DelegarTodas.visible(false);
    _pesquisaBidding.RejeitarTodas.visible(false);

    _gridBiddings.CarregarGrid();

    _situacaoBiddingUltimaPesquisa = _pesquisaBidding.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaBidding.AprovarTodas.visible(false);
    _pesquisaBidding.DelegarTodas.visible(false);
    _pesquisaBidding.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridBiddings.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaBidding.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoBiddingUltimaPesquisa == EnumSituacaoBiddingConvite.AguardandoAprovacao) {
            _pesquisaBidding.AprovarTodas.visible(true);
            _pesquisaBidding.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaBidding.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharBidding(registroSelecionado) {
    limparCamposBidding();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaBidding);

    _bidding.Codigo.val(registroSelecionado.Codigo);
    _bidding.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_bidding, "AutorizacaoBidding/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoBidding.AguardandoAprovacao);

                Global.abrirModal("divModalBidding");
                $modalDetalhesBidding.one('hidden.bs.modal', function () {
                    limparCamposBidding();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposBidding() {
    $("#myTab a:first").tab("show");

    limparRegras();
}