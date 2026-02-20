/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/TipoInfracao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoInfracao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridInfracoes;
var _infracao;
var _pesquisaInfracao;
var _rejeicao;
var _situacaoInfracaoUltimaPesquisa = EnumSituacaoInfracao.AguardandoAprovacao;
var $modalDetalhesInfracao;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarInfracoesSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var Infracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Cidade = PropertyEntity({ text: "Cidade: ", visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.TipoInfracao = PropertyEntity({ text: "Tipo Infração: ", visible: ko.observable(true) });
}

var PesquisaInfracaoAutorizacao = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoInfracao.AguardandoAprovacao), options: EnumSituacaoInfracao.obterOpcoes(), def: EnumSituacaoInfracao.AguardandoAprovacao, text: "Situação: " });
    this.TipoInfracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Infração:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasInfracoesClick, text: "Aprovar Infrações", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridInfracoes, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasInfracoesClick, text: "Rejeitar Infrações", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Infrações", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _infracao = new Infracao();
    KoBindings(_infracao, "knockoutInfracao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoInfracao");

    _pesquisaInfracao = new PesquisaInfracaoAutorizacao();
    KoBindings(_pesquisaInfracao, "knockoutPesquisaInfracaoAutorizacao");

    loadGridInfracoes();
    loadRegras();
    loadDelegar();

    $modalDetalhesInfracao = $("#divModalInfracao");

    new BuscarTipoInfracao(_pesquisaInfracao.TipoInfracao);
    new BuscarFuncionario(_pesquisaInfracao.Usuario);

    loadDadosUsuarioLogado(atualizarGridInfracoes);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaInfracao.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaInfracao.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridInfracoes() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharInfracao,
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
        SelecionarTodosKnout: _pesquisaInfracao.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoInfracao/ExportarPesquisa",
        titulo: "Autorização Infração"
    };

    _gridInfracoes = new GridView(_pesquisaInfracao.Pesquisar.idGrid, "AutorizacaoInfracao/Pesquisa", _pesquisaInfracao, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasInfracoesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as infrações selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaInfracao);

        dados.SelecionarTodos = _pesquisaInfracao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridInfracoes.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridInfracoes.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoInfracao/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridInfracoes();
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

    Global.fecharModal('divModalRejeitarInfracao');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasInfracoesClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarInfracao');
}

function rejeitarInfracoesSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as infrações selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaInfracao);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaInfracao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridInfracoes.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridInfracoes.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoInfracao/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridInfracoes();
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

function atualizarGridInfracoes() {
    _pesquisaInfracao.SelecionarTodos.val(false);
    _pesquisaInfracao.AprovarTodas.visible(false);
    _pesquisaInfracao.DelegarTodas.visible(false);
    _pesquisaInfracao.RejeitarTodas.visible(false);

    _gridInfracoes.CarregarGrid();

    _situacaoInfracaoUltimaPesquisa = _pesquisaInfracao.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaInfracao.AprovarTodas.visible(false);
    _pesquisaInfracao.DelegarTodas.visible(false);
    _pesquisaInfracao.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridInfracoes.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaInfracao.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoInfracaoUltimaPesquisa == EnumSituacaoInfracao.AguardandoAprovacao) {
            _pesquisaInfracao.AprovarTodas.visible(true);
            _pesquisaInfracao.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaInfracao.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharInfracao(registroSelecionado) {
    limparCamposInfracao();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaInfracao);

    _infracao.Codigo.val(registroSelecionado.Codigo);
    _infracao.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_infracao, "AutorizacaoInfracao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoInfracao.AguardandoAprovacao);

                Global.abrirModal("divModalInfracao");
                $modalDetalhesInfracao.one('hidden.bs.modal', function () {
                    limparCamposInfracao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposInfracao() {
    $("#myTab a:first").tab("show");

    limparRegras();
}