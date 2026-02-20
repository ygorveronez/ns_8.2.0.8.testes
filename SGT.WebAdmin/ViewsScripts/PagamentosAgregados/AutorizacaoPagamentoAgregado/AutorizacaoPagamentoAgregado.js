/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDescarteLoteProdutoEmbarcador.js" />
/// <reference path="AutorizarRegras.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _pagamentoAgregado;
var _pesquisaPagamentos;
var _rejeicao;
var _valores;
var _autorizacao;
var _gridPagamento;
var $modalPagamentoAgregado;

var _situacaoPagamento = [
    { text: "Todas", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao },
    { text: "Finalizado", value: EnumSituacaoDescarteLoteProdutoEmbarcador.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoDescarteLoteProdutoEmbarcador.Rejeitada }
];

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPagamentoSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var PagamentoAgregado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.DataPagamento = PropertyEntity({ text: "Data Pagamento: ", visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor: ", visible: ko.observable(true) });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", visible: ko.observable(true) });

    this.Cliente = PropertyEntity({ text: "Agregado: ", visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "Motivo: ", visible: ko.observable(true) });   

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaPagamentos = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao), options: _situacaoPagamento, def: EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarPagamentos();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosPagamentosClick, text: "Aprovar Pagamentos", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosPagamentosClick, text: "Rejeitar Pagamentos", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAutorizacao() {
    _pagamentoAgregado = new PagamentoAgregado();
    KoBindings(_pagamentoAgregado, "knockoutPagamento");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoPagamentoAgregado");

    _pesquisaPagamentos = new PesquisaPagamentos();
    KoBindings(_pesquisaPagamentos, "knockoutPesquisaPagamentos");

    $modalPagamentoAgregado = $("#divModalPagamentoAgregado");
    // Busca componentes pesquisa
    new BuscarFuncionario(_pesquisaPagamentos.Usuario);
    new BuscarClientes(_pesquisaPagamentos.Cliente);

    // Load modulos    
    loadRegras();

    // Busca 
    BuscarPagamentos();
}

function rejeitarPagamentoSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os pagamentos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamentos);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaPagamentos.SelecionarTodos.val();
        dados.PagamentosSelecionados = JSON.stringify(_gridPagamento.ObterMultiplosSelecionados());
        dados.PagamentosNaoSelecionados = JSON.stringify(_gridPagamento.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamentoAgregado/ReprovarMultiplosPagamentos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de pagamentos foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de pagamentos foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    BuscarPagamentos();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    Global.fecharModal('divModalRejeitarPagamentoAgregado');
}

function rejeitarMultiplosPagamentosClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarPagamentoAgregado');
}

//*******MÉTODOS*******

function BuscarPagamentos() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharPagamento,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaPagamentos.SelecionarTodos.val(false);
    _pesquisaPagamentos.AprovarTodas.visible(false);
    _pesquisaPagamentos.RejeitarTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaPagamentos.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoPagamentoAgregado/ExportarPesquisa",
        titulo: "Autorização Pagamento"
    };

    _gridPagamento = new GridView(_pesquisaPagamentos.Pesquisar.idGrid, "AutorizacaoPagamentoAgregado/Pesquisa", _pesquisaPagamentos, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridPagamento.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaPagamentos.Situacao.val();
    var possuiSelecionado = _gridPagamento.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaPagamentos.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao);

    // Esconde todas opções
    _pesquisaPagamentos.AprovarTodas.visible(false);
    _pesquisaPagamentos.RejeitarTodas.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaPagamentos.AprovarTodas.visible(true);
            _pesquisaPagamentos.RejeitarTodas.visible(true);
        }
    }
}

function aprovarMultiplosPagamentosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os pagamentos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamentos);

        dados.SelecionarTodos = _pesquisaPagamentos.SelecionarTodos.val();
        dados.PagamentosSelecionados = JSON.stringify(_gridPagamento.ObterMultiplosSelecionados());
        dados.PagamentosNaoSelecionados = JSON.stringify(_gridPagamento.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamentoAgregado/AprovarMultiplosPagamentos", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        var msg = "";
                        if (arg.Data.RegrasModificadas > 1) msg = arg.Data.RegrasModificadas + " alçadas de descarte foram aprovadas.";
                        else msg = arg.Data.RegrasModificadas + " alçada de descarte foi aprovada.";

                        exibirMensagem(tipoMensagem.ok, "Sucesso", msg);
                    }
                    else if (arg.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    if (arg.Data.Msg != "")
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Data.Msg);

                    BuscarPagamentos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        })
    });
}

function detalharPagamento(itemGrid) {
    limparCamposPagamento();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaPagamentos);
    _pagamentoAgregado.Codigo.val(itemGrid.Codigo);
    _pagamentoAgregado.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_pagamentoAgregado, "AutorizacaoPagamentoAgregado/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                AtualizarGridRegras();

                // Abre modal 
                Global.abrirModal("divModalPagamentoAgregado");
                $modalPagamentoAgregado.one('hidden.bs.modal', function () {
                    limparCamposPagamento();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, null);
}

function limparCamposPagamento() {
    resetarTabs();    
    limparRegras();
}

function resetarTabs() {
    Global.ResetarAbas();
}
