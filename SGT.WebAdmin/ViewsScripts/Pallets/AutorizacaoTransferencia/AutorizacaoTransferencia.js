/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTransferenciaPallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTransferencias;
var _pesquisaTransferencia;
var _rejeicao;
var _situacaoTransferenciaUltimaPesquisa = EnumSituacaoTransferenciaPallet.AguardandoAprovacao;
var _transferencia;
var $modalDetalhesTransferencia;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarTransferenciasSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var TransferenciaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: "Quantidade: ", visible: ko.observable(true) });
    this.Setor = PropertyEntity({ text: "Setor: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ text: "Solicitante: ", visible: ko.observable(true) });
    this.Turno = PropertyEntity({ text: "Turno: ", visible: ko.observable(true) });
}

var PesquisaTransferenciaPallet = function () {
    this.Codigo = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTransferenciaPallet.AguardandoAprovacao), options: EnumSituacaoTransferenciaPallet.obterOpcoes(), def: EnumSituacaoTransferenciaPallet.AguardandoAprovacao, text: "Situação: " });
    this.Turno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Turno:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasTransferenciasClick, text: "Aprovar Transferências", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridTransferencias, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasTransferenciasClick, text: "Rejeitar Transferências", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Transferências", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _transferencia = new TransferenciaPallet();
    KoBindings(_transferencia, "knockoutTransferenciaPallet");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoTransferenciaPallet");

    _pesquisaTransferencia = new PesquisaTransferenciaPallet();
    KoBindings(_pesquisaTransferencia, "knockoutPesquisaTransferenciaPallet");

    loadGridTransferencias();
    loadRegras();
    loadDelegar();

    $modalDetalhesTransferencia = $("#divModalTransferenciaPallet");

    new BuscarFilial(_pesquisaTransferencia.Filial);
    new BuscarSetorFuncionario(_pesquisaTransferencia.Setor);
    new BuscarTurno(_pesquisaTransferencia.Turno);
    new BuscarFuncionario(_pesquisaTransferencia.Usuario);

    loadDadosUsuarioLogado(atualizarGridTransferencias);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaTransferencia.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaTransferencia.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridTransferencias() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharTransferencia,
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
        SelecionarTodosKnout: _pesquisaTransferencia.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoTransferencia/ExportarPesquisa",
        titulo: "Autorização Transferência Pallets"
    };

    _gridTransferencias = new GridView(_pesquisaTransferencia.Pesquisar.idGrid, "AutorizacaoTransferencia/Pesquisa", _pesquisaTransferencia, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasTransferenciasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as transferências de pallets selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTransferencia);

        dados.SelecionarTodos = _pesquisaTransferencia.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTransferencias.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTransferencias.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTransferencia/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridTransferencias();
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

    Global.fecharModal('divModalRejeitarTransferenciaPallet');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasTransferenciasClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarTransferenciaPallet');
}

function rejeitarTransferenciasSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as transferências de pallets selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTransferencia);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaTransferencia.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTransferencias.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTransferencias.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTransferencia/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridTransferencias();
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

function atualizarGridTransferencias() {
    _pesquisaTransferencia.SelecionarTodos.val(false);
    _pesquisaTransferencia.AprovarTodas.visible(false);
    _pesquisaTransferencia.DelegarTodas.visible(false);
    _pesquisaTransferencia.RejeitarTodas.visible(false);

    _gridTransferencias.CarregarGrid();

    _situacaoTransferenciaUltimaPesquisa = _pesquisaTransferencia.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaTransferencia.AprovarTodas.visible(false);
    _pesquisaTransferencia.DelegarTodas.visible(false);
    _pesquisaTransferencia.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridTransferencias.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaTransferencia.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoTransferenciaUltimaPesquisa == EnumSituacaoTransferenciaPallet.AguardandoAprovacao) {
            _pesquisaTransferencia.AprovarTodas.visible(true);
            _pesquisaTransferencia.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaTransferencia.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharTransferencia(registroSelecionado) {
    limparCamposTransferencia();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaTransferencia);

    _transferencia.Codigo.val(registroSelecionado.Codigo);
    _transferencia.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_transferencia, "AutorizacaoTransferencia/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoTransferenciaPallet.AguardandoAprovacao);

                Global.abrirModal("divModalTransferenciaPallet");
                $modalDetalhesTransferencia.one('hidden.bs.modal', function () {
                    limparCamposTransferencia();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposTransferencia() {
    $("#myTab a:first").tab("show");

    limparRegras();
}