/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDevolucaoValePallet.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _devolucaoValePallet;
var _gridDevolucaoValePallet;
var _pesquisaDevolucaoValePallet;
var $modalDetalhesDevolucaoValePallet;
var _rejeicao;
var _situacaoDevolucaoValePalletUltimaPesquisa = EnumSituacaoDevolucaoValePallet.AguardandoAprovacao;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarDevolucoesValePalletSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var DevolucaoValePallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.NumeroValePallet = PropertyEntity({ text: "Número do Vale Pallet: ", visible: ko.observable(true) });
    this.QuantidadePallets = PropertyEntity({ text: "Quantidade: ", visible: ko.observable(true) });
    this.Setor = PropertyEntity({ text: "Setor: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) })
    this.Transportador = PropertyEntity({ text: "Transportador: ", visible: ko.observable(true) })
}

var PesquisaDevolucaoValePallet = function () {
    this.Codigo = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoDevolucaoValePallet.AguardandoAprovacao), options: EnumSituacaoDevolucaoValePallet.obterOpcoes(), def: EnumSituacaoDevolucaoValePallet.AguardandoAprovacao, text: "Situação: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasDevolucoesValePalletClick, text: "Aprovar Devoluções", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridDevolucaoValePallet, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasDevolucoesValePalletClick, text: "Rejeitar Devoluções", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Devoluções", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _devolucaoValePallet = new DevolucaoValePallet();
    KoBindings(_devolucaoValePallet, "knockoutDevolucaoValePalletPallet");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoDevolucaoValePalletPallet");

    _pesquisaDevolucaoValePallet = new PesquisaDevolucaoValePallet();
    KoBindings(_pesquisaDevolucaoValePallet, "knockoutPesquisaDevolucaoValePalletPallet");

    loadGridDevolucaoValePallet();
    loadRegras();
    loadDelegar();

    $modalDetalhesDevolucaoValePallet = $("#divModalDevolucaoValePalletPallet");

    new BuscarFilial(_pesquisaDevolucaoValePallet.Filial);
    new BuscarFuncionario(_pesquisaDevolucaoValePallet.Usuario);
    new BuscarSetorFuncionario(_pesquisaDevolucaoValePallet.Setor);
    new BuscarTransportadores(_pesquisaDevolucaoValePallet.Transportador);

    loadDadosUsuarioLogado(atualizarGridDevolucaoValePallet);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaDevolucaoValePallet.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaDevolucaoValePallet.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridDevolucaoValePallet() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharDevolucaoValePallet,
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
        SelecionarTodosKnout: _pesquisaDevolucaoValePallet.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoDevolucaoValePallet/ExportarPesquisa",
        titulo: "Autorização Devolução de Vale Pallets"
    };

    _gridDevolucaoValePallet = new GridView(_pesquisaDevolucaoValePallet.Pesquisar.idGrid, "AutorizacaoDevolucaoValePallet/Pesquisa", _pesquisaDevolucaoValePallet, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasDevolucoesValePalletClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as devoluções de vale pallets selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaDevolucaoValePallet);

        dados.SelecionarTodos = _pesquisaDevolucaoValePallet.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridDevolucaoValePallet.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridDevolucaoValePallet.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoDevolucaoValePallet/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridDevolucaoValePallet();
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

    Global.fecharModal('divModalRejeitarDevolucaoValePalletPallet');
}

function exibirDelegarSelecionadosClick() {    
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasDevolucoesValePalletClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarDevolucaoValePalletPallet');
}

function rejeitarDevolucoesValePalletSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as devoluções de vale pallets selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaDevolucaoValePallet);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaDevolucaoValePallet.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridDevolucaoValePallet.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridDevolucaoValePallet.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoDevolucaoValePallet/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridDevolucaoValePallet();
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

function atualizarGridDevolucaoValePallet() {
    _pesquisaDevolucaoValePallet.SelecionarTodos.val(false);
    _pesquisaDevolucaoValePallet.AprovarTodas.visible(false);
    _pesquisaDevolucaoValePallet.DelegarTodas.visible(false);
    _pesquisaDevolucaoValePallet.RejeitarTodas.visible(false);

    _gridDevolucaoValePallet.CarregarGrid();

    _situacaoDevolucaoValePalletUltimaPesquisa = _pesquisaDevolucaoValePallet.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaDevolucaoValePallet.AprovarTodas.visible(false);
    _pesquisaDevolucaoValePallet.DelegarTodas.visible(false);
    _pesquisaDevolucaoValePallet.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridDevolucaoValePallet.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaDevolucaoValePallet.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoDevolucaoValePalletUltimaPesquisa == EnumSituacaoDevolucaoValePallet.AguardandoAprovacao) {
            _pesquisaDevolucaoValePallet.AprovarTodas.visible(true);
            _pesquisaDevolucaoValePallet.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaDevolucaoValePallet.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharDevolucaoValePallet(registroSelecionado) {
    limparCamposDevolucaoValePallet();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaDevolucaoValePallet);

    _devolucaoValePallet.Codigo.val(registroSelecionado.Codigo);
    _devolucaoValePallet.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_devolucaoValePallet, "AutorizacaoDevolucaoValePallet/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoDevolucaoValePallet.AguardandoAprovacao);

                Global.abrirModal("divModalDevolucaoValePalletPallet");
                $modalDetalhesDevolucaoValePallet.one('hidden.bs.modal', function () {
                    limparCamposDevolucaoValePallet();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposDevolucaoValePallet() {
    Global.ResetarAbas();
    limparRegras();
}