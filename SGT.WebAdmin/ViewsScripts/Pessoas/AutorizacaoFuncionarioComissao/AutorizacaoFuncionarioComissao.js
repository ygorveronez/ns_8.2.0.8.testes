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
/// <reference path="../../Enumeradores/EnumSituacaoFuncionarioComissao.js" />
/// <reference path="AutorizacaoFuncionarioComissaoAutorizarRegras.js" />
/// <reference path="AutorizacaoFuncionarioComissaoTitulos.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _funcionarioComissao;
var _pesquisaFuncionarioComissao;
var _rejeicao;
var _valores;
var _autorizacao;
var _gridFuncionarioComissao;
var $modalFuncionarioComissao;

var _situacaoFuncionarioComissao = [
    { text: "Todas", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoFuncionarioComissao.AgAprovacao },
    { text: "Finalizado", value: EnumSituacaoFuncionarioComissao.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoFuncionarioComissao.Rejeitada }
];

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarFuncionarioComissaoSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var FuncionarioComissao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário: ", visible: ko.observable(true) });

    this.ValorFinal = PropertyEntity({ text: "Valor Total Final: ", visible: ko.observable(true) });
    this.QuantidadeTitulos = PropertyEntity({ text: "Quantidade Títulos: ", visible: ko.observable(true) });
    this.PercentualComissao = PropertyEntity({ text: "% Comissão: ", visible: ko.observable(true) });
    this.PercentualComissaoAcrescimo = PropertyEntity({ text: "+ Comissão: ", visible: ko.observable(true) });
    this.PercentualComissaoTotal = PropertyEntity({ text: "% Total Comissão: ", visible: ko.observable(true) });
    this.ValorComissao = PropertyEntity({ text: "Valor Comissão: ", visible: ko.observable(true) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaFuncionarioComissao = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFuncionarioComissao.AgAprovacao), options: _situacaoFuncionarioComissao, def: EnumSituacaoFuncionarioComissao.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarFuncionarioComissao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosFuncionarioComissaoClick, text: "Aprovar Requisições", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosFuncionarioComissaoClick, text: "Rejeitar Requisições", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAutorizacaoFuncionarioComissao() {
    _funcionarioComissao = new FuncionarioComissao();
    KoBindings(_funcionarioComissao, "knockoutFuncionarioComissao");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoFuncionarioComissao");

    _pesquisaFuncionarioComissao = new PesquisaFuncionarioComissao();
    KoBindings(_pesquisaFuncionarioComissao, "knockoutPesquisaFuncionarioComissao");

    $modalFuncionarioComissao = $("#divModalFuncionarioComissao");
    // Busca componentes pesquisa
    new BuscarFuncionario(_pesquisaFuncionarioComissao.Usuario);
    new BuscarFuncionario(_pesquisaFuncionarioComissao.Operador);
    new BuscarFuncionario(_pesquisaFuncionarioComissao.Funcionario);

    // Load modulos
    loadTitulos();
    loadRegras();

    // Busca 
    BuscarFuncionarioComissao();
}

function rejeitarFuncionarioComissaoSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as requisições selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaFuncionarioComissao);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaFuncionarioComissao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridFuncionarioComissao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridFuncionarioComissao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoFuncionarioComissao/ReprovarMultiplosItens", dados, function (arg) {
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
                    BuscarFuncionarioComissao();
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
    Global.fecharModal('divModalRejeitarFuncionarioComissao');
}

function rejeitarMultiplosFuncionarioComissaoClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarFuncionarioComissao');
}




//*******MÉTODOS*******


function BuscarFuncionarioComissao() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharFuncionarioComissao,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaFuncionarioComissao.SelecionarTodos.val(false);
    _pesquisaFuncionarioComissao.AprovarTodas.visible(false);
    _pesquisaFuncionarioComissao.RejeitarTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaFuncionarioComissao.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoFuncionarioComissao/ExportarPesquisa",
        titulo: "Autorização Comissão Funcionário"
    };

    _gridFuncionarioComissao = new GridView(_pesquisaFuncionarioComissao.Pesquisar.idGrid, "AutorizacaoFuncionarioComissao/Pesquisa", _pesquisaFuncionarioComissao, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridFuncionarioComissao.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaFuncionarioComissao.Situacao.val();
    var possuiSelecionado = _gridFuncionarioComissao.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaFuncionarioComissao.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoFuncionarioComissao.AgAprovacao);

    // Esconde todas opções
    _pesquisaFuncionarioComissao.AprovarTodas.visible(false);
    _pesquisaFuncionarioComissao.RejeitarTodas.visible(false);
}

function aprovarMultiplosFuncionarioComissaoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as requisições selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaFuncionarioComissao);

        dados.SelecionarTodos = _pesquisaFuncionarioComissao.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridFuncionarioComissao.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridFuncionarioComissao.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoFuncionarioComissao/AprovarMultiplosItens", dados, function (arg) {
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

                    BuscarFuncionarioComissao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharFuncionarioComissao(itemGrid) {
    limparCamposFuncionarioComissao();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaFuncionarioComissao);
    _funcionarioComissao.Codigo.val(itemGrid.Codigo);
    _funcionarioComissao.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_funcionarioComissao, "AutorizacaoFuncionarioComissao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                // Titulos
                CarregarTitulos(_funcionarioComissao.Codigo.val());

                AtualizarGridRegras();

                // Abre modal 
                Global.abrirModal("divModalFuncionarioComissao");
                $modalFuncionarioComissao.one('hidden.bs.modal', function () {
                    limparCamposFuncionarioComissao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function limparCamposFuncionarioComissao() {
    resetarTabs();
    limparTitulos();
    limparRegras();
}

function resetarTabs() {
    Global.ResetarAbas();
}
