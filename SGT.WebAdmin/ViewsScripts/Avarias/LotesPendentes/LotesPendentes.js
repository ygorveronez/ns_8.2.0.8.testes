/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLote.js" />
/// <reference path="../../Enumeradores/EnumEtapaLote.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Anexos.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _lote;
var _pesquisaLotes;
var _valores;
var _autorizacao;
var _gridLote;

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Aceitar = PropertyEntity({ eventClick: aprovarLoteClick, type: types.event, text: "Integrar", visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarLoteClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(true) });
};

var Lote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorLote = PropertyEntity({ text: "Valor do Lote: ", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroLote = PropertyEntity({ text: "Número do Lote: ", visible: ko.observable(true), val: ko.observable("") });
    this.DataGeracao = PropertyEntity({ text: "Data Geração: ", visible: ko.observable(true), val: ko.observable("") });
    this.SituacaoLote = PropertyEntity({ text: "Situação: ", visible: ko.observable(true), val: ko.observable("") });
    this.Situacao = PropertyEntity({ visible: false, val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: "Transportador: ", visible: ko.observable(true), val: ko.observable("") });
    this.Responsavel = PropertyEntity({ text: "Responsável: ", visible: ko.observable(true), val: ko.observable("") });
};

var PesquisaLotes = function () {
    this.NumeroLote = PropertyEntity({ text: "Número do Lote:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLote.AgAprovacaoIntegracao), options: EnumSituacaoLote.obterOpcoesPesquisa(), def: EnumSituacaoLote.AgAprovacaoIntegracao, text: "Situação: " });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaLote.Todas), options: EnumEtapaLote.obterOpcoesPesquisa(), def: EnumEtapaLote.Todas, text: "Etapa: " });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarLotes();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: finalizarMultiplosLotesClick, text: "Finalizar Lotes", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: integrarMultiplosLotesClick, text: "Integrar Lotes", visible: ko.observable(false) });
    this.Delegar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: delegarMultiplosLotesClick, text: "Delegar Lotes", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadLotesPendentes() {
    _lote = new Lote();
    KoBindings(_lote, "knockoutLote");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _pesquisaLotes = new PesquisaLotes();
    KoBindings(_pesquisaLotes, "knockoutPesquisaLotes");

    // Busca componentes pesquisa
    new BuscarTransportadores(_pesquisaLotes.Transportador);

    // Load modulos
    loadAnexos();
    loadDelegar();
    loadRegras();

    // Busca as ocorrencias 
    BuscarLotes();
}

//*******MÉTODOS*******

function BuscarLotes() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharLote,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaLotes.SelecionarTodos.val(false);
    _pesquisaLotes.Finalizar.visible(false);
    _pesquisaLotes.AprovarTodas.visible(false);
    _pesquisaLotes.Delegar.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaLotes.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "LotesPendentes/ExportarPesquisa",
        titulo: "LotesPendentes"
    };

    _gridLote = new GridView(_pesquisaLotes.Pesquisar.idGrid, "LotesPendentes/Pesquisa", _pesquisaLotes, menuOpcoes, null, 20, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridLote.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var multiplosSelecionados = _gridLote.ObterMultiplosSelecionados();
    var possuiSelecionado = multiplosSelecionados.length > 0;
    var selecionadoTodos = _pesquisaLotes.SelecionarTodos.val();

    if (possuiSelecionado || selecionadoTodos) {
        _pesquisaLotes.Finalizar.visible(!multiplosSelecionados.some(x => x.Etapa !== EnumEtapaLote.CriacaoLote));
        _pesquisaLotes.AprovarTodas.visible(true);
        _pesquisaLotes.Delegar.visible(true);
    } else {
        _pesquisaLotes.Finalizar.visible(false);
        _pesquisaLotes.AprovarTodas.visible(false);
        _pesquisaLotes.Delegar.visible(false);
    }
}

function finalizarMultiplosLotesClick(e, sender) {
    exibirConfirmacao("Confirmação", "Você realmente deseja finalizar o(s) lote(s) selecionado(s)?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaLotes);

        dados.SelecionarTodos = _pesquisaLotes.SelecionarTodos.val();
        dados.LotesSelecionados = JSON.stringify(_gridLote.ObterMultiplosSelecionados());

        executarReST("Lotes/FinalizarMultiplosLotes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Lotes finalizados com sucesso.");
                    BuscarLotes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function integrarMultiplosLotesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja integrar todas os lotes selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaLotes);

        dados.SelecionarTodos = _pesquisaLotes.SelecionarTodos.val();
        dados.LotesSelecionados = JSON.stringify(_gridLote.ObterMultiplosSelecionados());
        dados.LotesNaoSelecionados = JSON.stringify(_gridLote.ObterMultiplosNaoSelecionados());

        executarReST("Lotes/IntegrarMultiplosLotes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integrado com sucesso.");
                    BuscarLotes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function delegarMultiplosLotesClick() {
    Global.abrirModal('divModalDelegarLote');
}

function detalharLote(itemGrid) {
    LimparCamposLote();
    _lote.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_lote, "Lotes/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _regraLote.Codigo.val(_lote.Codigo.val());
                // Anexos da ocorrencia
                CarregarAnexos(_lote.Codigo.val());
                AbaAutorizacao();

                // Abre modal da ocorrencia
                Global.abrirModal('divModalLote');
                $("#divModalLote").one('hidden.bs.modal', function () {
                    LimparCamposLote();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function LimparCamposLote() {
    resetarTabs();
    limparAnexos();
    LimparDelegar();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function AbaAutorizacao() {
    if (_lote.Situacao.val() == EnumSituacaoLote.AgAprovacaoIntegracao)
        $("#liAutorizacao").show();
    else
        $("#liAutorizacao").hide();
}