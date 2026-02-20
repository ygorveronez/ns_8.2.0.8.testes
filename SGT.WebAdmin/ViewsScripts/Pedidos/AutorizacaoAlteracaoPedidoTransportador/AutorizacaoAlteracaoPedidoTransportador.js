/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoRejeicaoAlteracaoPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlcadaRegra.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoMotivoRejeicaoAlteracaoPedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _alteracaoPedido;
var _CRUDAlteracaoPedido;
var _gridAlteracaoPedido;
var _gridCamposAlterados;
var _pesquisaAlteracaoPedido;
var _rejeicao;
var _situacaoAprovacaoUltimaPesquisa = EnumSituacaoAlcadaRegra.Pendente;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAlteracoesPedidosSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var AlteracaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº do Pedido:" });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga: " });
    this.TipoOperacao = PropertyEntity({ text: "Operação: " });
    this.Cargas = PropertyEntity({ text: "Cargas: " });
};

var CRUDAlteracaoPedido = function () {
    this.Aprovar = PropertyEntity({ type: types.event, eventClick: aprovarAlteracaoPedidoClick, text: "Aprovar", visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ type: types.event, eventClick: rejeitarAlteracaoPedidoClick, text: "Rejeitar", visible: ko.observable(true) });
}

var PesquisaAlteracaoPedido = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido:", val: ko.observable(""), def: "" });
    this.SituacaoAprovacao = PropertyEntity({ val: ko.observable(EnumSituacaoAlcadaRegra.Pendente), options: EnumSituacaoAlcadaRegra.obterOpcoesPesquisa(), def: EnumSituacaoAlcadaRegra.Pendente, text: "Situação da Aprovação: " });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasAlteracoesPedidosClick, text: "Aprovar Alterações de Pedidos", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridAlteracaoPedido, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasAlteracoesPedidosClick, text: "Rejeitar Alterações de Pedidos", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _alteracaoPedido = new AlteracaoPedido();
    KoBindings(_alteracaoPedido, "knockoutAlteracaoPedido");

    _CRUDAlteracaoPedido = new CRUDAlteracaoPedido();
    KoBindings(_CRUDAlteracaoPedido, "knockoutCRUDAlteracaoPedido");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoAlteracaoPedido");

    _pesquisaAlteracaoPedido = new PesquisaAlteracaoPedido();
    KoBindings(_pesquisaAlteracaoPedido, "knockoutPesquisaAlteracaoPedido");

    new BuscarMotivoRejeicaoAlteracaoPedido(_rejeicao.Motivo, undefined, EnumTipoMotivoRejeicaoAlteracaoPedido.Transportador);

    loadGridAlteracaoPedido();
    loadGridCamposAlterados();
    atualizarGridAlteracaoPedido();
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
        url: "AutorizacaoAlteracaoPedidoTransportador/ExportarPesquisa",
        titulo: "Autorização de Alteração de Pedido"
    };

    _gridAlteracaoPedido = new GridView(_pesquisaAlteracaoPedido.Pesquisar.idGrid, "AutorizacaoAlteracaoPedidoTransportador/Pesquisa", _pesquisaAlteracaoPedido, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

function loadGridCamposAlterados() {
    _gridCamposAlterados = new GridView("grid-pedido-campos-alterados", "AlteracaoPedido/PesquisaCamposAlterados", _alteracaoPedido, null, null, 20);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarAlteracaoPedidoClick() {
    executarReST("AutorizacaoAlteracaoPedidoTransportador/Aprovar", { Codigo: _alteracaoPedido.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aprovado com sucesso.");

                atualizarGridAlteracaoPedido();
                atualizarAlteracaoPedido();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function aprovarMultiplasAlteracoesPedidosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as alterações de pedidos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAlteracaoPedido);

        dados.SelecionarTodos = _pesquisaAlteracaoPedido.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAlteracaoPedidoTransportador/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alterações foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alteração foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "")
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alteração pendente.");

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
    fecharModalRejeitarAlteracaoPedido();
}

function rejeitarAlteracaoPedidoClick() {
    _rejeicao.Codigo.val(_alteracaoPedido.Codigo.val());

    exibirModalRejeitarAlteracaoPedido();
}

function rejeitarMultiplasAlteracoesPedidosClick() {
    exibirModalRejeitarAlteracaoPedido();
}

function rejeitarAlteracoesPedidosSelecionadasClick() {
    if (_rejeicao.Codigo.val() > 0)
        rejeitarAlteracaoPedido();
    else
        rejeitarAlteracoesPedidosSelecionadas();
}

/*
 * Declaração das Funções
 */

function atualizarAlteracaoPedido() {
    BuscarPorCodigo(_alteracaoPedido, "AutorizacaoAlteracaoPedido/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarExibicaoBotoesAlteracaoPedido(retorno.Data.AprovacaoPendente);
            controlarExibicaoAbaCamposAlterados(retorno.Data.SituacaoAlteracaoPedido !== EnumSituacaoAlteracaoPedido.Aprovada);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function atualizarGridAlteracaoPedido() {
    _pesquisaAlteracaoPedido.SelecionarTodos.val(false);
    _pesquisaAlteracaoPedido.AprovarTodas.visible(false);
    _pesquisaAlteracaoPedido.RejeitarTodas.visible(false);

    _gridAlteracaoPedido.CarregarGrid();

    _situacaoAprovacaoUltimaPesquisa = _pesquisaAlteracaoPedido.SituacaoAprovacao.val();
}

function atualizarGridCamposAlterados() {
    _gridCamposAlterados.CarregarGrid();
}

function exibirMultiplasOpcoes() {
    _pesquisaAlteracaoPedido.AprovarTodas.visible(false);
    _pesquisaAlteracaoPedido.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridAlteracaoPedido.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaAlteracaoPedido.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoAprovacaoUltimaPesquisa == EnumSituacaoAlcadaRegra.Pendente) {
            _pesquisaAlteracaoPedido.AprovarTodas.visible(true);
            _pesquisaAlteracaoPedido.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaCamposAlterados(exibirCamposAlterados) {
    if (exibirCamposAlterados)
        $("#liCamposAlterados").show();
    else {
        $("a[href='#knockoutAlteracaoPedido']").click();
        $("#liCamposAlterados").hide();
    }
}

function controlarExibicaoBotoesAlteracaoPedido(exibirBotoes) {
    if (exibirBotoes)
        $("#knockoutCRUDAlteracaoPedido").show();
    else
        $("#knockoutCRUDAlteracaoPedido").hide();
}

function detalharAlteracaoPedido(registroSelecionado) {
    _alteracaoPedido.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_alteracaoPedido, "AutorizacaoAlteracaoPedidoTransportador/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                controlarExibicaoBotoesAlteracaoPedido(retorno.Data.AprovacaoPendente);
                atualizarGridCamposAlterados();
                controlarExibicaoAbaCamposAlterados(retorno.Data.SituacaoAlteracaoPedido !== EnumSituacaoAlteracaoPedido.Aprovada);
                exibirModalAlteracaoPedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function exibirModalAlteracaoPedido() {
    Global.abrirModal('divModalAlteracaoPedido');
    $("#divModalAlteracaoPedido").one('hidden.bs.modal', function () {
        limparCamposAlteracaoPedido();
    });
}

function exibirModalRejeitarAlteracaoPedido() {
    Global.abrirModal('divModalRejeitarAlteracaoPedido');
    $("#divModalRejeitarAlteracaoPedido").one('hidden.bs.modal', function () {
        LimparCampos(_rejeicao);
    });
}

function fecharModalRejeitarAlteracaoPedido() {
    Global.fecharModal("divModalRejeitarAlteracaoPedido");
}

function limparCamposAlteracaoPedido() {
    Global.ResetarAbas();
}

function rejeitarAlteracaoPedido() {
    if (!ValidarCampoObrigatorioEntity(_rejeicao.Motivo))
        return exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Para reprovar a alteração de pedido é necessário informar o motivo.");

    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar a alteração de pedido?", function () {
        var dados = {
            Codigo: _rejeicao.Codigo.val(),
            Motivo: _rejeicao.Motivo.codEntity(),
        };

        executarReST("AutorizacaoAlteracaoPedidoTransportador/Reprovar", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Reprovado com sucesso.");
                    atualizarGridAlteracaoPedido();
                    fecharModalRejeitarAlteracaoPedido();
                    atualizarAlteracaoPedido();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function rejeitarAlteracoesPedidosSelecionadas() {
    if (!ValidarCampoObrigatorioEntity(_rejeicao.Motivo))
        return exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Para reprovar todas as alterações de pedidos selecionadas é necessário informar o motivo.");

    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as alterações de pedidos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAlteracaoPedido);

        dados.Motivo = _rejeicao.Motivo.codEntity();
        dados.SelecionarTodos = _pesquisaAlteracaoPedido.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAlteracaoPedido.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAlteracaoPedidoTransportador/ReprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alterações foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alteração foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alteração pendente.");

                    atualizarGridAlteracaoPedido();
                    fecharModalRejeitarAlteracaoPedido();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}
