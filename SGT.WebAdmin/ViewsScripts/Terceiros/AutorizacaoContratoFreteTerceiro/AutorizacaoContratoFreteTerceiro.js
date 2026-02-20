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
/// <reference path="../../Enumeradores/EnumSituacaoContratoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _contratoFreteTerceiro;
var _pesquisaContratoFrete;
var _rejeicao;
var _autorizacao;
var _gridContratoFreteTerceiro;
var $modalContratoFreteTerceiro;

var _situacaoContratoFrete = [
    { text: "Todas", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoContratoFrete.AgAprovacao },
    { text: "Aprovado", value: EnumSituacaoContratoFrete.Aprovado },
    { text: "Finalizado", value: EnumSituacaoContratoFrete.Finalizado },
    { text: "Cancelado", value: EnumSituacaoContratoFrete.Cancelado },
    { text: "Rejeitado", value: EnumSituacaoContratoFrete.Rejeitado },
];

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarContratoFreteSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var ContratoFreteTerceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número:", visible: ko.observable(true) });
    this.DataEmissaoContrato = PropertyEntity({ text: "Data do Contrato:", visible: ko.observable(true) });
    this.Transbordo = PropertyEntity({ text: "Transbordo:", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ text: "Carga:", visible: ko.observable(true) });
    this.SituacaoContratoFrete = PropertyEntity({ text: "Situação:", visible: ko.observable(true) });
    this.TipoFreteEscolhido = PropertyEntity({ text: "Frete Escolhido:", visible: ko.observable(true) });
    this.ValorAdiantamento = PropertyEntity({ text: "Valor Adiantamento:", visible: ko.observable(true) });
    this.Descontos = PropertyEntity({ text: "Descontos:", visible: ko.observable(true) });
    this.PercentualAdiantamento = PropertyEntity({ text: "Adiantamento:", visible: ko.observable(true) });
    this.Terceiro = PropertyEntity({ text: "Terceiro:", visible: ko.observable(true) });
    this.ValorFreteSubcontratacao = PropertyEntity({ text: "Valor Frete:", visible: ko.observable(true) });
    this.ValorOutrosAdiantamento = PropertyEntity({ text: "Valor Outros Adiantamento:", visible: ko.observable(true) });
    this.ValorPedagio = PropertyEntity({ text: "Valor Pedágio:", visible: ko.observable(true) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaContratoFrete = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoContratoFrete.AgAprovacao), options: _situacaoContratoFrete, def: EnumSituacaoContratoFrete.AgAprovacao, text: "Situação: " });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarContratoFretes();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosContratoFretesClick, text: "Aprovar Contratos", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosContratoFretesClick, text: "Rejeitar Contratos", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAutorizacao() {
    _contratoFreteTerceiro = new ContratoFreteTerceiro();
    KoBindings(_contratoFreteTerceiro, "knockoutContratoFreteTerceiro");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoContratoFreteTerceiro");

    _pesquisaContratoFrete = new PesquisaContratoFrete();
    KoBindings(_pesquisaContratoFrete, "knockoutPesquisaContratoFreteTerceiro", false, _pesquisaContratoFrete.Pesquisar.id);

    $modalContratoFreteTerceiro = $("#divModalContratoFreteTerceiro");
    // Busca componentes pesquisa
    new BuscarFuncionario(_pesquisaContratoFrete.Usuario);
    new BuscarTransportadores(_pesquisaContratoFrete.Empresa);
    new BuscarCargas(_pesquisaContratoFrete.Carga);

    // Load modulos
    loadRegras();

    // Busca 
    BuscarContratoFretes();
}

function downloadPlanilhaClick() {
    var dados = {
        Codigo: _contratoFreteTerceiro.Codigo.val()
    };
    if (dados.Codigo > 0) {
        executarDownload("ContratoFreteTerceiro/DownloadPlanilha", dados);
    }
}

function rejeitarContratoFreteSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os contratos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContratoFrete);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaContratoFrete.SelecionarTodos.val();
        dados.ObjetosSelecionados = JSON.stringify(_gridContratoFreteTerceiro.ObterMultiplosSelecionados());
        dados.ObjetosNaoSelecionados = JSON.stringify(_gridContratoFreteTerceiro.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoFreteTerceiro/ReprovarMultiplasLinhas", dados, function (arg) {
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
                    BuscarContratoFretes();
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
    Global.fecharModal('divModalRejeitarContratoFreteTerceiro');
}

function rejeitarMultiplosContratoFretesClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarContratoFreteTerceiro');
}




//*******MÉTODOS*******


function BuscarContratoFretes() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharContratoFrete,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    //-- Reseta
    _pesquisaContratoFrete.SelecionarTodos.val(false);
    _pesquisaContratoFrete.AprovarTodas.visible(false);
    _pesquisaContratoFrete.RejeitarTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaContratoFrete.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoContratoFreteTerceiro/ExportarPesquisa",
        titulo: "Autorização ContratoFrete"
    };

    _gridContratoFreteTerceiro = new GridView(_pesquisaContratoFrete.Pesquisar.idGrid, "AutorizacaoContratoFreteTerceiro/Pesquisa", _pesquisaContratoFrete, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridContratoFreteTerceiro.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaContratoFrete.Situacao.val();
    var possuiSelecionado = _gridContratoFreteTerceiro.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaContratoFrete.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoContratoFrete.AgAprovacao);

    // Esconde todas opções
    _pesquisaContratoFrete.AprovarTodas.visible(false);
    _pesquisaContratoFrete.RejeitarTodas.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaContratoFrete.AprovarTodas.visible(true);
            _pesquisaContratoFrete.RejeitarTodas.visible(true);
        }
    }
}

function aprovarMultiplosContratoFretesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os itens selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaContratoFrete);

        dados.SelecionarTodos = _pesquisaContratoFrete.SelecionarTodos.val();
        dados.ObjetosSelecionados = JSON.stringify(_gridContratoFreteTerceiro.ObterMultiplosSelecionados());
        dados.ObjetosNaoSelecionados = JSON.stringify(_gridContratoFreteTerceiro.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoContratoFreteTerceiro/AprovarMultiplasLinhas", dados, function (arg) {
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

                    if (arg.Data.Msg != "" && arg.Data.Msg != null)
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Data.Msg);

                    BuscarContratoFretes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function detalharContratoFrete(itemGrid) {
    limparCamposContratoFrete();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaContratoFrete);
    _contratoFreteTerceiro.Codigo.val(itemGrid.Codigo);
    _contratoFreteTerceiro.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_contratoFreteTerceiro, "AutorizacaoContratoFreteTerceiro/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                AtualizarGridRegras();

                // Abre modal 
                Global.abrirModal("divModalContratoFreteTerceiro");
                $modalContratoFreteTerceiro.one('hidden.bs.modal', function () {
                    limparCamposContratoFrete();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, null);
}

function limparCamposContratoFrete() {
    resetarTabs();
    limparRegras();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}
