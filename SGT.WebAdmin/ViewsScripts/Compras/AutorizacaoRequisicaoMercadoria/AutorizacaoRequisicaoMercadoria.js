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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRequisicaoMercadoria.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Mercadorias.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _requisicaoMercadoria;
var _pesquisaRequisicaoMercadoria;
var _rejeicao;
var _valores;
var _gridRequisicao;
var $modalRequisicaoMercadoria;

var _situacaoRequisicao = [
    { text: "Todas", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoRequisicaoMercadoria.AgAprovacao },
    { text: "Finalizado", value: EnumSituacaoRequisicaoMercadoria.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoRequisicaoMercadoria.Rejeitada }
];

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarRequisicaoelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var RequisicaoMercadoria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Colaborador = PropertyEntity({ text: "Colaborador: ", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "Motivo: ", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", visible: ko.observable(true) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
};

var PesquisaRequisicao = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoRequisicaoMercadoria.AgAprovacao), options: _situacaoRequisicao, def: EnumSituacaoRequisicaoMercadoria.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid() });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarRequisicao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosRequisicaoClick, text: "Aprovar Requisições", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosRequisicaoClick, text: "Rejeitar Requisições", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAutorizacao() {
    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoRequisicaoMercadoria");

    _pesquisaRequisicaoMercadoria = new PesquisaRequisicao();
    KoBindings(_pesquisaRequisicaoMercadoria, "knockoutPesquisaRequisicao");

    new BuscarFuncionario(_pesquisaRequisicaoMercadoria.Usuario);
    new BuscarTransportadores(_pesquisaRequisicaoMercadoria.Filial);
    new BuscarMotivoCompra(_pesquisaRequisicaoMercadoria.Motivo);

    carregarModalDetalhesRequisicao("ModalDetalhesRequisicao", BuscarRequisicao);
}

function carregarModalDetalhesRequisicao(idDivConteudo, callback) {
    $.get("Content/Static/Compras/AutorizacaoRequisicaoMercadoria.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        _requisicaoMercadoria = new RequisicaoMercadoria();
        KoBindings(_requisicaoMercadoria, "knockoutRequisicao");

        $modalRequisicaoMercadoria = $("#divModalRequisicaoMercadoria");

        loadMercadoriasAutorizacao();
        loadRegras();

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function rejeitarRequisicaoelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as requisições selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaRequisicaoMercadoria);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaRequisicaoMercadoria.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridRequisicao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridRequisicao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoRequisicaoMercadoria/ReprovarMultiplosItens", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    BuscarRequisicao();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    Global.fecharModal('divModalRejeitarRequisicaoMercadoria');
}

function rejeitarMultiplosRequisicaoClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarRequisicaoMercadoria');
}

//*******MÉTODOS*******

function RecarregarGridRequisicao() {
    if (_gridRequisicao != null)
        _gridRequisicao.CarregarGrid();
    else if (_gridAprovacaoRequisicoesMercadoria != null)
        CarregarAprovacaoFluxoCompra();
}

function BuscarRequisicao() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharRequisicao,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaRequisicaoMercadoria.SelecionarTodos.val(false);
    _pesquisaRequisicaoMercadoria.AprovarTodas.visible(false);
    _pesquisaRequisicaoMercadoria.RejeitarTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaRequisicaoMercadoria.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoRequisicaoMercadoria/ExportarPesquisa",
        titulo: "Autorização Requisição"
    };

    _gridRequisicao = new GridView(_pesquisaRequisicaoMercadoria.Pesquisar.idGrid, "AutorizacaoRequisicaoMercadoria/Pesquisa", _pesquisaRequisicaoMercadoria, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridRequisicao.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaRequisicaoMercadoria.Situacao.val();
    var possuiSelecionado = _gridRequisicao.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaRequisicaoMercadoria.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoRequisicaoMercadoria.AgAprovacao);

    // Esconde todas opções
    _pesquisaRequisicaoMercadoria.AprovarTodas.visible(false);
    _pesquisaRequisicaoMercadoria.RejeitarTodas.visible(false);

    //if (possuiSelecionado || selecionadoTodos) {
    //    if (situacaoPermiteSelecao) {
    //        _pesquisaRequisicaoMercadoria.AprovarTodas.visible(true);
    //        _pesquisaRequisicaoMercadoria.RejeitarTodas.visible(true);
    //    }
    //}
}

function aprovarMultiplosRequisicaoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as requisições selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaRequisicaoMercadoria);

        dados.SelecionarTodos = _pesquisaRequisicaoMercadoria.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridRequisicao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridRequisicao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoRequisicaoMercadoria/AprovarMultiplosItens", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        var msg = "";
                        if (arg.Data.RegrasModificadas > 1) msg = arg.Data.RegrasModificadas + " alçadas foram aprovadas.";
                        else msg = arg.Data.RegrasModificadas + " alçada foi aprovada.";

                        exibirMensagem(tipoMensagem.ok, "Sucesso", msg);
                    }
                    else if (arg.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    BuscarRequisicao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharRequisicao(itemGrid) {
    limparCamposRequisicao();

    _requisicaoMercadoria.Codigo.val(itemGrid.Codigo);

    if (_pesquisaRequisicaoMercadoria != null) {
        var pesquisa = RetornarObjetoPesquisa(_pesquisaRequisicaoMercadoria);
        _requisicaoMercadoria.Usuario.val(pesquisa.Usuario);
    }

    BuscarPorCodigo(_requisicaoMercadoria, "AutorizacaoRequisicaoMercadoria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                CarregarMercadorias(_requisicaoMercadoria.Codigo.val());

                AtualizarGridRegras();

                Global.abrirModal("divModalRequisicaoMercadoria");
                $modalRequisicaoMercadoria.one('hidden.bs.modal', function () {
                    limparCamposRequisicao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function limparCamposRequisicao() {
    resetarTabs();
    limparMercadorias();
    limparRegras();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
