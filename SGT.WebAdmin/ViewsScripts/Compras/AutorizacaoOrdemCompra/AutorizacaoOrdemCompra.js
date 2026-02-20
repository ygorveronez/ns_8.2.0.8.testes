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
/// <reference path="../../Enumeradores/EnumSituacaoOrdemCompra.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Mercadorias.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _autorizacaoOrdemCompra;
var _pesquisaAutorizacaoOrdemCompra;
var _rejeicaoAutorizacaoOrdemCompra;
var _gridAutorizacaoOrdemCompra;
var $modalOrdemCompra;

var RejeitarAutorizacaoOrdemCompra = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarAutorizacaoOrdemCompraClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAutorizacaoOrdemCompraClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var AutorizacaoOrdemCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.DataPrevisao = PropertyEntity({ text: "Data Prev. Retorno: ", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador: ", visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ text: "Fornecedor: ", visible: ko.observable(true) });
    this.CondicaoPagamento = PropertyEntity({ text: "Condição de Pagamento: ", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observações: ", visible: ko.observable(true) });
    this.MotivoAprovacao = PropertyEntity({ text: "Observações Aprovação: ", visible: ko.observable(true) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
};

var PesquisaAutorizacaoOrdemCompra = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoOrdemCompra.AgAprovacao), options: EnumSituacaoOrdemCompra.obterOpcoesPesquisa(), def: EnumSituacaoOrdemCompra.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarOrdemAutorizacao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasAutorizacaoOrdemCompraClick, text: "Aprovar Requisições", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasAutorizacaoOrdemCompraClick, text: "Rejeitar Requisições", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAutorizacaoOrdemCompra() {
    _rejeicaoAutorizacaoOrdemCompra = new RejeitarAutorizacaoOrdemCompra();
    KoBindings(_rejeicaoAutorizacaoOrdemCompra, "knockoutRejeicaoOrdemCompra");

    _pesquisaAutorizacaoOrdemCompra = new PesquisaAutorizacaoOrdemCompra();
    KoBindings(_pesquisaAutorizacaoOrdemCompra, "knockoutPesquisaAutorizacaoOrdemCompra");

    new BuscarFuncionario(_pesquisaAutorizacaoOrdemCompra.Usuario);
    new BuscarFuncionario(_pesquisaAutorizacaoOrdemCompra.Operador);
    new BuscarClientes(_pesquisaAutorizacaoOrdemCompra.Fornecedor);

    carregarModalDetalhesOrdemCompra("ModalDetalhesOrdemCompra", BuscarOrdemAutorizacao);
}

function carregarModalDetalhesOrdemCompra(idDivConteudo, callback) {
    $.get("Content/Static/Compras/AutorizacaoOrdemCompra.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        _autorizacaoOrdemCompra = new AutorizacaoOrdemCompra();
        KoBindings(_autorizacaoOrdemCompra, "knockoutAutorizacaoOrdemCompra");

        $modalOrdemCompra = $("#divModalOrdemCompra");
        
        loadMercadoriasAutorizacaoOrdemCompra();
        loadRegrasAutorizacaoOrdemCompra();

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function rejeitarAutorizacaoOrdemCompraClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as requisições selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAutorizacaoOrdemCompra);
        var rejeicao = RetornarObjetoPesquisa(_rejeicaoAutorizacaoOrdemCompra);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaAutorizacaoOrdemCompra.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAutorizacaoOrdemCompra.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAutorizacaoOrdemCompra.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoOrdemCompra/ReprovarMultiplosItens", dados, function (arg) {
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
                    BuscarOrdemAutorizacao();
                    cancelarAutorizacaoOrdemCompraClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function cancelarAutorizacaoOrdemCompraClick() {
    LimparCampos(_rejeicaoAutorizacaoOrdemCompra);
    Global.fecharModal('divModalRejeitarOrdemCompra');
}

function rejeitarMultiplasAutorizacaoOrdemCompraClick() {
    LimparCampos(_rejeicaoAutorizacaoOrdemCompra);
    Global.abrirModal('divModalRejeitarOrdemCompra');
}

//*******MÉTODOS*******

function RecarregarGridOrdem() {
    if (!string.IsNullOrWhiteSpace( _gridAutorizacaoOrdemCompra ))
        _gridAutorizacaoOrdemCompra.CarregarGrid();
    else if (_gridAprovacaoOrdemCompra != null) 
        CarregarAprovacaoOrdemCompra();
}

function BuscarOrdemAutorizacao() {
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharAutorizacaoOrdemCompra,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaAutorizacaoOrdemCompra.SelecionarTodos.val(false);
    _pesquisaAutorizacaoOrdemCompra.AprovarTodas.visible(false);
    _pesquisaAutorizacaoOrdemCompra.RejeitarTodas.visible(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaAutorizacaoOrdemCompra.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoOrdemCompra/ExportarPesquisa",
        titulo: "Autorização Requisição"
    };

    _gridAutorizacaoOrdemCompra = new GridView(_pesquisaAutorizacaoOrdemCompra.Pesquisar.idGrid, "AutorizacaoOrdemCompra/Pesquisa", _pesquisaAutorizacaoOrdemCompra, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridAutorizacaoOrdemCompra.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaAutorizacaoOrdemCompra.Situacao.val();
    var possuiSelecionado = _gridAutorizacaoOrdemCompra.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaAutorizacaoOrdemCompra.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoOrdemCompra.AgAprovacao);

    // Esconde todas opções
    _pesquisaAutorizacaoOrdemCompra.AprovarTodas.visible(false);
    _pesquisaAutorizacaoOrdemCompra.RejeitarTodas.visible(false);

    //if (possuiSelecionado || selecionadoTodos) {
    //    if (situacaoPermiteSelecao) {
    //        _pesquisaAutorizacaoOrdemCompra.AprovarTodas.visible(true);
    //        _pesquisaAutorizacaoOrdemCompra.RejeitarTodas.visible(true);
    //    }
    //}
}

function aprovarMultiplasAutorizacaoOrdemCompraClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as requisições selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAutorizacaoOrdemCompra);

        dados.SelecionarTodos = _pesquisaAutorizacaoOrdemCompra.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridAutorizacaoOrdemCompra.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridAutorizacaoOrdemCompra.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoOrdemCompra/AprovarMultiplosItens", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        var msg = "";
                        if (arg.Data.RegrasModificadas > 1) msg = arg.Data.RegrasModificadas + " alçadas foram aprovadas.";
                        else msg = arg.Data.RegrasModificadas + " alçada foi aprovada.";

                        exibirMensagem(tipoMensagem.ok, "Sucesso", msg);
                        Global.fecharModal("divModalOrdemCompra");
                    }
                    else if (arg.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    BuscarOrdemAutorizacao();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharAutorizacaoOrdemCompra(itemGrid) {
    limparCamposAutorizacaoOrdemCompra();

    _autorizacaoOrdemCompra.Codigo.val(itemGrid.Codigo);

    if (_pesquisaAutorizacaoOrdemCompra != null) {
        var pesquisa = RetornarObjetoPesquisa(_pesquisaAutorizacaoOrdemCompra);
        _autorizacaoOrdemCompra.Usuario.val(pesquisa.Usuario);
    }

    BuscarPorCodigo(_autorizacaoOrdemCompra, "AutorizacaoOrdemCompra/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                CarregarMercadoriasAutorizacaoOrdemCompra(_autorizacaoOrdemCompra.Codigo.val());

                AtualizarGridRegrasAutorizacaoOrdemCompra();

                Global.abrirModal("divModalOrdemCompra");
                $modalOrdemCompra.one('hidden.bs.modal', function () {
                    limparCamposAutorizacaoOrdemCompra();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function limparCamposAutorizacaoOrdemCompra() {
    limparMercadoriasAutorizacaoOrdemCompra();
    limparRegrasAutorizacaoOrdemCompra();

    Global.ResetarAbas();
}