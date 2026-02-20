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
/// <reference path="../../Enumeradores/EnumSituacaoDescarteLoteProdutoEmbarcador.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Anexos.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _descarteLote;
var _pesquisaDescartes;
var _rejeicao;
var _valores;
var _autorizacao;
var _gridDescarte;
var $modalDescarteLote;

var _situacaoDescarte = [
    { text: "Todas", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao },
    { text: "Finalizado", value: EnumSituacaoDescarteLoteProdutoEmbarcador.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoDescarteLoteProdutoEmbarcador.Rejeitada }
];

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarDescarteSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var DescarteLote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ text: "Data de Vencimento: ", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.DataDescarte = PropertyEntity({ text: "Data Descarte: ", visible: ko.observable(true) });
    this.QuantidadeLote = PropertyEntity({ text: "Quantidade Lote: ", visible: ko.observable(true) });
    this.QuantidadeAtual = PropertyEntity({ text: "Quantidade Atual: ", visible: ko.observable(true) });
    this.CodigoBarras = PropertyEntity({ text: "Codigo Barras: ", visible: ko.observable(true) });
    this.DepositoPosicao = PropertyEntity({ text: "Local Armazenenamento: ", visible: ko.observable(true) });
    this.ProdutoEmbarcador = PropertyEntity({ text: "Produto do Embarcador: ", visible: ko.observable(true) });
    this.QuantidadeDescartada = PropertyEntity({ text: "Quantidade Descartada: ", visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: "Motivo: ", visible: ko.observable(true) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaDescartes = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao), options: _situacaoDescarte, def: EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarDescartes();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosDescartesClick, text: "Aprovar Descartes", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosDescartesClick, text: "Rejeitar Descartes", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAutorizacao() {
    _descarteLote = new DescarteLote();
    KoBindings(_descarteLote, "knockoutDescarte");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoDescarteLote");

    _pesquisaDescartes = new PesquisaDescartes();
    KoBindings(_pesquisaDescartes, "knockoutPesquisaDescartes");

    $modalDescarteLote = $("#divModalDescarteLote");
    // Busca componentes pesquisa
    new BuscarFuncionario(_pesquisaDescartes.Usuario);
    new BuscarProdutos(_pesquisaDescartes.Produto);

    // Load modulos
    loadAnexos();
    loadRegras();

    // Busca 
    BuscarDescartes();
}

function rejeitarDescarteSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os descartes selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaDescartes);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaDescartes.SelecionarTodos.val();
        dados.DescartesSelecionados = JSON.stringify(_gridDescarte.ObterMultiplosSelecionados());
        dados.DescartesNaoSelecionados = JSON.stringify(_gridDescarte.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoDescarteLote/ReprovarMultiplosDescartes", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçadas de descartes foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.RegrasModificadas + " alçada de descartes foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    BuscarDescartes();
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
    Global.fecharModal("divModalRejeitarDescarteLote");
}

function rejeitarMultiplosDescartesClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal("divModalRejeitarDescarteLote");
}




//*******MÉTODOS*******


function BuscarDescartes() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        metodo: detalharDescarte,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaDescartes.SelecionarTodos.val(false);
    _pesquisaDescartes.AprovarTodas.visible(false);
    _pesquisaDescartes.RejeitarTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaDescartes.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoDescarteLote/ExportarPesquisa",
        titulo: "Autorização Descarte"
    };

    _gridDescarte = new GridView(_pesquisaDescartes.Pesquisar.idGrid, "AutorizacaoDescarteLote/Pesquisa", _pesquisaDescartes, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridDescarte.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaDescartes.Situacao.val();
    var possuiSelecionado = _gridDescarte.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaDescartes.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao);

    // Esconde todas opções
    _pesquisaDescartes.AprovarTodas.visible(false);
    _pesquisaDescartes.RejeitarTodas.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaDescartes.AprovarTodas.visible(true);
            _pesquisaDescartes.RejeitarTodas.visible(true);
        }
    }
}

function aprovarMultiplosDescartesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os descartes selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaDescartes);

        dados.SelecionarTodos = _pesquisaDescartes.SelecionarTodos.val();
        dados.DescartesSelecionados = JSON.stringify(_gridDescarte.ObterMultiplosSelecionados());
        dados.DescartesNaoSelecionados = JSON.stringify(_gridDescarte.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoDescarteLote/AprovarMultiplosDescartes", dados, function (arg) {
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

                    if(arg.Data.Msg != "")
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Data.Msg);

                    BuscarDescartes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharDescarte(itemGrid) {
    limparCamposDescarte();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaDescartes);
    _descarteLote.Codigo.val(itemGrid.Codigo);
    _descarteLote.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_descarteLote, "AutorizacaoDescarteLote/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                // Anexos
                CarregarAnexos(_descarteLote.Codigo.val());

                AtualizarGridRegras();

                // Abre modal 
                Global.abrirModal("divModalDescarteLote");
                $modalDescarteLote.one('hidden.bs.modal', function () {
                    limparCamposDescarte();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function limparCamposDescarte() {
    resetarTabs();
    limparAnexos();
    limparRegras();
}

function resetarTabs() {
    Global.ResetarAbas();
}
