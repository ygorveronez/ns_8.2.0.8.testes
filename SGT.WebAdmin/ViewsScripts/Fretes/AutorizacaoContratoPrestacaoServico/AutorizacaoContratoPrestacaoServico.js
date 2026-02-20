/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoPrestacaoServico.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _contratoPrestacaoServico;
var _gridContratoPrestacaoServico;
var _pesquisaContratoPrestacaoServico;
var _rejeicao;
var _situacaoContratoPrestacaoServicoUltimaPesquisa = EnumSituacaoContratoPrestacaoServico.AguardandoAprovacao;
var $modalDetalhesContratoPrestacaoServico;
var _modalContratoPrestacaoServico;
var _modalRejeitarContratoPrestacaoServico;
var _modalDelegarSelecionados;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarContratosPrestacaoServicoSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var ContratoPrestacaoServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.DataFinal = PropertyEntity({ text: "Data Final: ", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "Descrição: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação da Aprovação: ", visible: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.ValorTeto = PropertyEntity({ text: "Valor Teto: ", visible: ko.observable(true) });
}

var PesquisaContratoPrestacaoServico = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoContratoPrestacaoServico.AguardandoAprovacao), options: EnumSituacaoContratoPrestacaoServico.obterOpcoesPesquisa(), def: EnumSituacaoContratoPrestacaoServico.AguardandoAprovacao, text: "Situação da Aprovação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosContratosPrestacaoServicoClick, text: "Aprovar Contratos", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridContratoPrestacaoServico, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosContratosPrestacaoServicoClick, text: "Rejeitar Contratos", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Contratos", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _contratoPrestacaoServico = new ContratoPrestacaoServico();
    KoBindings(_contratoPrestacaoServico, "knockoutContratoPrestacaoServico");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoContratoPrestacaoServico");

    _pesquisaContratoPrestacaoServico = new PesquisaContratoPrestacaoServico();
    KoBindings(_pesquisaContratoPrestacaoServico, "knockoutPesquisaContratoPrestacaoServico");

    loadGridContratoPrestacaoServico();
    loadRegras();
    loadDelegar();

    $modalDetalhesContratoPrestacaoServico = $("#divModalContratoPrestacaoServico");

    new BuscarFuncionario(_pesquisaContratoPrestacaoServico.Usuario);

    loadDadosUsuarioLogado(atualizarGridContratoPrestacaoServico);
    _modalContratoPrestacaoServico = new bootstrap.Modal(document.getElementById("divModalContratoPrestacaoServico"), { backdrop: true, keyboard: true });
    _modalRejeitarContratoPrestacaoServico = new bootstrap.Modal(document.getElementById("divModalRejeitarContratoPrestacaoServico"), { backdrop: true, keyboard: true });
    _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaContratoPrestacaoServico.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaContratoPrestacaoServico.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridContratoPrestacaoServico() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharContratoPrestacaoServico,
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
        SelecionarTodosKnout: _pesquisaContratoPrestacaoServico.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoContratoPrestacaoServico/ExportarPesquisa",
        titulo: "Autorização de Contrato de Prestação de Serviço"
    };

    _gridContratoPrestacaoServico = new GridView(_pesquisaContratoPrestacaoServico.Pesquisar.idGrid, "AutorizacaoContratoPrestacaoServico/Pesquisa", _pesquisaContratoPrestacaoServico, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplosContratosPrestacaoServicoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os contratos de prestação de serviço selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContratoPrestacaoServico);

        dados.SelecionarTodos = _pesquisaContratoPrestacaoServico.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridContratoPrestacaoServico.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridContratoPrestacaoServico.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoPrestacaoServico/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridContratoPrestacaoServico();
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

    _modalRejeitarContratoPrestacaoServico.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplosContratosPrestacaoServicoClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarContratoPrestacaoServico.show();
}

function rejeitarContratosPrestacaoServicoSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os contratos de prestação de serviço selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContratoPrestacaoServico);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaContratoPrestacaoServico.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridContratoPrestacaoServico.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridContratoPrestacaoServico.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoPrestacaoServico/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridContratoPrestacaoServico();
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

function atualizarGridContratoPrestacaoServico() {
    _pesquisaContratoPrestacaoServico.SelecionarTodos.val(false);
    _pesquisaContratoPrestacaoServico.AprovarTodas.visible(false);
    _pesquisaContratoPrestacaoServico.DelegarTodas.visible(false);
    _pesquisaContratoPrestacaoServico.RejeitarTodas.visible(false);

    _gridContratoPrestacaoServico.CarregarGrid();

    _situacaoContratoPrestacaoServicoUltimaPesquisa = _pesquisaContratoPrestacaoServico.Situacao.val()
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function exibirMultiplasOpcoes() {
    _pesquisaContratoPrestacaoServico.AprovarTodas.visible(false);
    _pesquisaContratoPrestacaoServico.DelegarTodas.visible(false);
    _pesquisaContratoPrestacaoServico.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridContratoPrestacaoServico.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaContratoPrestacaoServico.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoContratoPrestacaoServicoUltimaPesquisa == EnumSituacaoContratoPrestacaoServico.AguardandoAprovacao) {
            _pesquisaContratoPrestacaoServico.AprovarTodas.visible(true);
            _pesquisaContratoPrestacaoServico.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaContratoPrestacaoServico.RejeitarTodas.visible(true);
        }
    }
}

function detalharContratoPrestacaoServico(registroSelecionado) {
    limparCamposContratoPrestacaoServico();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaContratoPrestacaoServico);

    _contratoPrestacaoServico.Codigo.val(registroSelecionado.Codigo);
    _contratoPrestacaoServico.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_contratoPrestacaoServico, "AutorizacaoContratoPrestacaoServico/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoContratoPrestacaoServico.AguardandoAprovacao);

                _modalContratoPrestacaoServico.show();
                $modalDetalhesContratoPrestacaoServico.one('hidden.bs.modal', function () {
                    limparCamposContratoPrestacaoServico();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposContratoPrestacaoServico() {
    $("#myTab a:first").tab("show");

    limparRegras();
}