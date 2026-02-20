/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoAvariaPallet.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvariaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _avaria;
var _gridAvarias;
var _isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;
var _pesquisaAvaria;
var _rejeicao;
var _situacaoAvariaUltimaPesquisa = EnumSituacaoAvariaPallet.AguardandoAprovacao;
var $modalDetalhesAvaria;

/*
 * Declaração das Classes
 */

var AvariaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: !_isTMS });
    this.MotivoAvaria = PropertyEntity({ text: "Motivo Avaria: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.Setor = PropertyEntity({ text: "Setor: ", visible: !_isTMS });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ text: "Solicitante: ", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Empresa/Filial: ", visible: _isTMS });

    this.QuantidadesAvariadas = ko.observableArray();
}

var PesquisaAvariaPallet = function () {
    this.Codigo = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: !_isTMS });
    this.MotivoAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Avaria:", idBtnSearch: guid() });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), visible: !_isTMS });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAvariaPallet.AguardandoAprovacao), options: EnumSituacaoAvariaPallet.obterOpcoes(), def: EnumSituacaoAvariaPallet.AguardandoAprovacao, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: _isTMS  });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid()});

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasAvariasClick, text: "Aprovar Avarias", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridAvarias, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasAvariasClick, text: "Rejeitar Avarias", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Avarias", visible: ko.observable(false) });
}

var AvariaQuantidade = function (situacao) {
    this.Quantidade = PropertyEntity({ val: situacao.Quantidade, text: "Quantidade " + situacao.Descricao + ":" });
    this.Valor = PropertyEntity({ val: Globalize.format(situacao.Valor, "n2"), text: "Valor " + situacao.Descricao + ":" });
}

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAvariasSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _avaria = new AvariaPallet();
    KoBindings(_avaria, "knockoutAvariaPallet");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoAvariaPallet");

    _pesquisaAvaria = new PesquisaAvariaPallet();
    KoBindings(_pesquisaAvaria, "knockoutPesquisaAvariaPallet");

    loadGridAvarias();
    loadRegras();
    loadDelegar();

    $modalDetalhesAvaria = $("#divModalAvariaPallet");

    new BuscarFilial(_pesquisaAvaria.Filial);
    new BuscarMotivoAvariaPallet(_pesquisaAvaria.MotivoAvaria);
    new BuscarSetorFuncionario(_pesquisaAvaria.Setor);
    new BuscarTransportadores(_pesquisaAvaria.Transportador);
    new BuscarFuncionario(_pesquisaAvaria.Usuario);

    loadDadosUsuarioLogado(atualizarGridAvarias);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaAvaria.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaAvaria.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridAvarias() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharAvaria,
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
        SelecionarTodosKnout: _pesquisaAvaria.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoAvariaPallet/ExportarPesquisa",
        titulo: "Autorização Avaria Pallets"
    };

    _gridAvarias = new GridView(_pesquisaAvaria.Pesquisar.idGrid, "AutorizacaoAvariaPallet/Pesquisa", _pesquisaAvaria, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasAvariasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as avarias de pallets selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAvaria);

        dados.SelecionarTodos = _pesquisaAvaria.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAvarias.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAvarias.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAvariaPallet/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridAvarias();
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

    Global.fecharModal('divModalRejeitarAvariaPallet');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasAvariasClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarAvariaPallet');
}

function rejeitarAvariasSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as avarias de pallets selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAvaria);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaAvaria.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAvarias.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAvarias.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAvariaPallet/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridAvarias();
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

function atualizarGridAvarias() {
    _pesquisaAvaria.SelecionarTodos.val(false);
    _pesquisaAvaria.AprovarTodas.visible(false);
    _pesquisaAvaria.DelegarTodas.visible(false);
    _pesquisaAvaria.RejeitarTodas.visible(false);

    _gridAvarias.CarregarGrid();

    _situacaoAvariaUltimaPesquisa = _pesquisaAvaria.Situacao.val()
}

function carregarQuantidadesAvariadas(dadosQuantidadesAvariadas) {
    for (var i = 0; i < dadosQuantidadesAvariadas.length; i++) {
        var quantidadeAvariada = new AvariaQuantidade(dadosQuantidadesAvariadas[i]);

        _avaria.QuantidadesAvariadas.push(quantidadeAvariada);
    }
}   

function exibirModalDetalhesAvaria() {
    Global.abrirModal("divModalAvariaPallet");
    $modalDetalhesAvaria.one('hidden.bs.modal', function () {
        limparCamposAvaria();
    });
}

function exibirMultiplasOpcoes() {
    _pesquisaAvaria.AprovarTodas.visible(false);
    _pesquisaAvaria.DelegarTodas.visible(false);
    _pesquisaAvaria.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridAvarias.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaAvaria.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoAvariaUltimaPesquisa == EnumSituacaoAvariaPallet.AguardandoAprovacao) {
            _pesquisaAvaria.AprovarTodas.visible(true);
            _pesquisaAvaria.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaAvaria.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharAvaria(registroSelecionado) {
    limparCamposAvaria();

    _avaria.Usuario.val(_pesquisaAvaria.Usuario.val());

    executarReST("AutorizacaoAvariaPallet/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_avaria, retorno);
                carregarQuantidadesAvariadas(retorno.Data.QuantidadesAvariadas);
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAvariaPallet.AguardandoAprovacao);
                exibirModalDetalhesAvaria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposAvaria() {
    $("#myTab a:first").tab("show");

    _avaria.QuantidadesAvariadas.removeAll();

    limparRegras();
}