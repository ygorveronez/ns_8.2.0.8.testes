/// <reference path="AprovacaoTransportador.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoRejeicaoAlteracaoPedido.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoMotivoRejeicaoAlteracaoPedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _alteracaoPedido;
var _gridAlteracaoPedido;
var _gridCamposAlterados;
var _pesquisaAlteracaoPedido;
var _rejeicao;
var _situacaoAlteracaoPedidoUltimaPesquisa = EnumSituacaoAlteracaoPedido.AguardandoAprovacao;
var $modalDetalhesAlteracaoPedido;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAlteracoesPedidosSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var AlteracaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº do Pedido:" });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga: " });
    this.TipoOperacao = PropertyEntity({ text: "Operação: " });
};

var PesquisaAlteracaoPedido = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido:", val: ko.observable(""), def: "" });
    this.SituacaoAlteracaoPedido = PropertyEntity({ val: ko.observable(EnumSituacaoAlteracaoPedido.AguardandoAprovacao), options: EnumSituacaoAlteracaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoAlteracaoPedido.AguardandoAprovacao, text: "Situação da Alteração do Pedido: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasAlteracoesPedidosClick, text: "Aprovar Alterações de Pedidos", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridAlteracaoPedido, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasAlteracoesPedidosClick, text: "Rejeitar Alterações de Pedidos", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Alterações de Pedidos", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _alteracaoPedido = new AlteracaoPedido();
    KoBindings(_alteracaoPedido, "knockoutAlteracaoPedido");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoAlteracaoPedido");

    _pesquisaAlteracaoPedido = new PesquisaAlteracaoPedido();
    KoBindings(_pesquisaAlteracaoPedido, "knockoutPesquisaAlteracaoPedido");

    new BuscarMotivoRejeicaoAlteracaoPedido(_rejeicao.Motivo, undefined, EnumTipoMotivoRejeicaoAlteracaoPedido.Embarcador);

    loadGridAlteracaoPedido();
    loadGridCamposAlterados();
    loadRegras();
    loadAprovacaoTransportador();
    loadDelegar();

    $modalDetalhesAlteracaoPedido = $("#divModalAlteracaoPedido");

    new BuscarFuncionario(_pesquisaAlteracaoPedido.Usuario);

    loadDadosUsuarioLogado(atualizarGridAlteracaoPedido);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaAlteracaoPedido.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaAlteracaoPedido.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridAlteracaoPedido() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharAlteracaoPedido,
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
        SelecionarTodosKnout: _pesquisaAlteracaoPedido.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoAlteracaoPedido/ExportarPesquisa",
        titulo: "Autorização de Alteração de Pedido"
    };

    _gridAlteracaoPedido = new GridView(_pesquisaAlteracaoPedido.Pesquisar.idGrid, "AutorizacaoAlteracaoPedido/Pesquisa", _pesquisaAlteracaoPedido, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

function loadGridCamposAlterados() {
    _gridCamposAlterados = new GridView("grid-pedido-campos-alterados", "AlteracaoPedido/PesquisaCamposAlterados", _alteracaoPedido, null, null, 20);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasAlteracoesPedidosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as alterações de pedidos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAlteracaoPedido);

        dados.SelecionarTodos = _pesquisaAlteracaoPedido.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAlteracaoPedido/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridAlteracaoPedido();
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

    Global.fecharModal('divModalRejeitarAlteracaoPedido');
}

function exibirDelegarSelecionadosClick() {    
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasAlteracoesPedidosClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarAlteracaoPedido');
}

function rejeitarAlteracoesPedidosSelecionadasClick() {
    if (!ValidarCampoObrigatorioEntity(_rejeicao.Motivo))
        return exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Para reprovar todas as alterações de pedidos selecionadas é necessário informar o motivo.");

    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as alterações de pedidos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAlteracaoPedido);

        dados.Motivo = _rejeicao.Motivo.codEntity();
        dados.SelecionarTodos = _pesquisaAlteracaoPedido.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAlteracaoPedido/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridAlteracaoPedido();
                    cancelarRejeicaoSelecionadosClick();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções
 */

function atualizarGridAlteracaoPedido() {
    _pesquisaAlteracaoPedido.SelecionarTodos.val(false);
    _pesquisaAlteracaoPedido.AprovarTodas.visible(false);
    _pesquisaAlteracaoPedido.DelegarTodas.visible(false);
    _pesquisaAlteracaoPedido.RejeitarTodas.visible(false);

    _gridAlteracaoPedido.CarregarGrid();

    _situacaoAlteracaoPedidoUltimaPesquisa = _pesquisaAlteracaoPedido.SituacaoAlteracaoPedido.val()
}

function atualizarGridCamposAlterados() {
    _gridCamposAlterados.CarregarGrid();
}

function exibirMultiplasOpcoes() {
    _pesquisaAlteracaoPedido.AprovarTodas.visible(false);
    _pesquisaAlteracaoPedido.DelegarTodas.visible(false);
    _pesquisaAlteracaoPedido.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridAlteracaoPedido.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaAlteracaoPedido.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoAlteracaoPedidoUltimaPesquisa == EnumSituacaoAlteracaoPedido.AguardandoAprovacao) {
            _pesquisaAlteracaoPedido.AprovarTodas.visible(true);
            _pesquisaAlteracaoPedido.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaAlteracaoPedido.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaAprovacaoTransportador(exibirAprovacaoTransportador) {
    if (exibirAprovacaoTransportador)
        $("#liAutorizacaoTransportador").show();
    else
        $("#liAutorizacaoTransportador").hide();
}

function controlarExibicaoAbaCamposAlterados(exibirCamposAlterados) {
    if (exibirCamposAlterados)
        $("#liCamposAlterados").show();
    else
        $("#liCamposAlterados").hide();
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharAlteracaoPedido(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaAlteracaoPedido);

    _alteracaoPedido.Codigo.val(registroSelecionado.Codigo);
    _alteracaoPedido.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_alteracaoPedido, "AutorizacaoAlteracaoPedido/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridCamposAlterados();
                atualizarGridRegras();
                atualizarGridAprovacaoTransportador();
                controlarExibicaoAbaCamposAlterados(retorno.Data.SituacaoAlteracaoPedido !== EnumSituacaoAlteracaoPedido.Aprovada);
                controlarExibicaoAbaDelegar(retorno.Data.SituacaoAlteracaoPedido === EnumSituacaoAlteracaoPedido.AguardandoAprovacao);

                $modalDetalhesAlteracaoPedido.modal("show");                
                $modalDetalhesAlteracaoPedido.one('hidden.bs.modal', function () {
                    limparCamposAlteracaoPedido();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    }, null);
}

function limparCamposAlteracaoPedido() {
    $("#myTab a:first").tab("show");

    limparRegras();
    controlarExibicaoAbaCamposAlterados(false);
    controlarExibicaoAbaAprovacaoTransportador(false);
    controlarExibicaoAbaDelegar(false);
}