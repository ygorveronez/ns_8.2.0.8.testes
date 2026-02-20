/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAprovacaoSolicitacaoGas.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridSolicitacaoGas;
var _pesquisaSolicitacaoGas;
var _rejeicao;
var _situacaoSolicitacaoGasUltimaPesquisa = EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao;
var _solicitacaoGas;
var $modalDetalhesSolicitacaoGas;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarSolicitacoesGasSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var SolicitacaoGas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    
    this.Base = PropertyEntity({ text: "Base: ", visible: ko.observable(true) });
    this.DataMedicao = PropertyEntity({ text: "Data Medição: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.VolumeRodoviario = PropertyEntity({ text: "Volume Rodoviário: ", visible: ko.observable(true) });
    this.DisponibilidadeTransferencia = PropertyEntity({ text: "Disponibilidade Transferência: ", visible: ko.observable(true) });
}

var PesquisaSolicitacaoGas = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao), options: EnumSituacaoAprovacaoSolicitacaoGas.obterOpcoesPesquisaSolicitacaoGas(), def: EnumSituacaoAprovacaoSolicitacaoGas.AguardandoAprovacao, text: "Situação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.Base = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Base:", idBtnSearch: guid() });
    
    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasSolicitacoesDeGasClick, text: "Aprovar Solicitações de Gás", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridSolicitacaoGas, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasTabelasFreteClick, text: "Rejeitar Solicitações de Gás", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Solicitações de Gás", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAprovacaoSolicitacaoGas() {
    _solicitacaoGas = new SolicitacaoGas();
    KoBindings(_solicitacaoGas, "knockoutSolicitacaoGas");
    
    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoSolicitacaoGas");

    _pesquisaSolicitacaoGas = new PesquisaSolicitacaoGas();
    KoBindings(_pesquisaSolicitacaoGas, "knockoutPesquisaSolicitacaoGas");

    loadGridSolicitacaoGas();
    loadRegras();
    loadDelegar();
    
    $modalDetalhesSolicitacaoGas = $("#divModalSolicitacaoGas");
    
    new BuscarFuncionario(_pesquisaSolicitacaoGas.Usuario);
    new BuscarClientes(_pesquisaSolicitacaoGas.Base, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    
    loadDadosUsuarioLogado(atualizarGridSolicitacaoGas);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaSolicitacaoGas.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaSolicitacaoGas.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridSolicitacaoGas() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharSolicitacaoGas,
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
        SelecionarTodosKnout: _pesquisaSolicitacaoGas.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AprovacaoSolicitacaoGas/ExportarPesquisa",
        titulo: "Aprovação de Solicitação de Gás"
    };

    _gridSolicitacaoGas = new GridView(_pesquisaSolicitacaoGas.Pesquisar.idGrid, "AprovacaoSolicitacaoGas/Pesquisa", _pesquisaSolicitacaoGas, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasSolicitacoesDeGasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as solicitações de gás selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaSolicitacaoGas);

        dados.SelecionarTodos = _pesquisaSolicitacaoGas.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridSolicitacaoGas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridSolicitacaoGas.ObterMultiplosNaoSelecionados());

        executarReST("AprovacaoSolicitacaoGas/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridSolicitacaoGas();
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

    Global.fecharModal('divModalRejeitarSolicitacaoGas');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasTabelasFreteClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarSolicitacaoGas');
}

function rejeitarSolicitacoesGasSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as solicitações de gás selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaSolicitacaoGas);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaSolicitacaoGas.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridSolicitacaoGas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridSolicitacaoGas.ObterMultiplosNaoSelecionados());

        executarReST("AprovacaoSolicitacaoGas/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridSolicitacaoGas();
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

function atualizarGridSolicitacaoGas() {
    _pesquisaSolicitacaoGas.SelecionarTodos.val(false);
    _pesquisaSolicitacaoGas.AprovarTodas.visible(false);
    _pesquisaSolicitacaoGas.DelegarTodas.visible(false);
    _pesquisaSolicitacaoGas.RejeitarTodas.visible(false);

    _gridSolicitacaoGas.CarregarGrid();

    _situacaoSolicitacaoGasUltimaPesquisa = _pesquisaSolicitacaoGas.Situacao.val()
}

function atualizarSolicitacaoGas() {
    BuscarPorCodigo(_solicitacaoGas, "AprovacaoSolicitacaoGas/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao);
                atualizarGridRegras();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function exibirMultiplasOpcoes() {
    _pesquisaSolicitacaoGas.AprovarTodas.visible(false);
    _pesquisaSolicitacaoGas.DelegarTodas.visible(false);
    _pesquisaSolicitacaoGas.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridSolicitacaoGas.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaSolicitacaoGas.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoSolicitacaoGasUltimaPesquisa == EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao) {
            _pesquisaSolicitacaoGas.AprovarTodas.visible(true);
            _pesquisaSolicitacaoGas.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaSolicitacaoGas.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharSolicitacaoGas(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaSolicitacaoGas);

    _solicitacaoGas.Codigo.val(registroSelecionado.Codigo);
    _solicitacaoGas.Usuario.val(pesquisa.Usuario);
    
    BuscarPorCodigo(_solicitacaoGas, "AprovacaoSolicitacaoGas/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAlteracaoTabelaFrete.AguardandoAprovacao);
                atualizarGridRegras();

                Global.abrirModal("divModalSolicitacaoGas");
                $modalDetalhesSolicitacaoGas.one('hidden.bs.modal', function () {
                    limparCamposSolicitacaoGas();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposSolicitacaoGas() {
    Global.ResetarAbas();

    LimparCampos(_solicitacaoGas);
    limparRegras();
}