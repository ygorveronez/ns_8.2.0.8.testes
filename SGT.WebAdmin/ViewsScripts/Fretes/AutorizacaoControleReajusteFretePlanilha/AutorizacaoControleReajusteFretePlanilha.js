/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoControleReajusteFretePlanilha.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />

// #region Objetos Globais do Arquivo

var _controleReajusteFretePlanilha;
var _pesquisaControle;
var _rejeicao;
var _gridControleReajuste;
var _situacaoControleReajusteFretePlanilhaUltimaPesquisa = EnumSituacaoControleReajusteFretePlanilha.AgAprovacao;
var $modalControleReajuste;
var _modalControleReajusteFretePlanilha;
var _modalRejeitarControleReajusteFretePlanilha;
var _modalDelegarSelecionados;


var _situacaoControle = [
    { text: "Todas", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoControleReajusteFretePlanilha.AgAprovacao },
    { text: "Aprovado", value: EnumSituacaoControleReajusteFretePlanilha.Aprovado },
    { text: "Finalizado", value: EnumSituacaoControleReajusteFretePlanilha.Finalizado },
    { text: "Cancelado", value: EnumSituacaoControleReajusteFretePlanilha.Cancelado },
    { text: "Rejeitado", value: EnumSituacaoControleReajusteFretePlanilha.Rejeitado },
];

// #endregion Objetos Globais do Arquivo

// #region Classes

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarControleSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var ControleReajuste = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumSituacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação: ", visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ text: "Solicitante: ", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: "Transportador: ", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observacao: ", visible: ko.observable(true) });

    this.Download = PropertyEntity({ eventClick: downloadPlanilhaClick, type: types.event, text: "Download", visible: ko.observable(true) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
}

var PesquisaControle = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoControleReajusteFretePlanilha.AgAprovacao), options: _situacaoControle, def: EnumSituacaoControleReajusteFretePlanilha.AgAprovacao, text: "Situação: " });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            atualizarGridControleReajuste();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosControlesClick, text: "Aprovar Reajustes", visible: ko.observable(false) });
    this.DelegarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Reajustes", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosControlesClick, text: "Rejeitar Reajustes", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadAutorizacao() {
    _controleReajusteFretePlanilha = new ControleReajuste();
    KoBindings(_controleReajusteFretePlanilha, "knockoutControle");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoControleReajusteFretePlanilha");

    _pesquisaControle = new PesquisaControle();
    KoBindings(_pesquisaControle, "knockoutPesquisaControleReajusteFretePlanilha");

    $modalControleReajuste = $("#divModalControleReajusteFretePlanilha");

    new BuscarFuncionario(_pesquisaControle.Usuario);
    new BuscarTransportadores(_pesquisaControle.Empresa);
    new BuscarFilial(_pesquisaControle.Filial);
    new BuscarTiposOperacao(_pesquisaControle.TipoOperacao);

    loadRegras();
    loadDelegar();
    loadGridControleReajuste();
    loadDadosUsuarioLogado(atualizarGridControleReajuste);
    _modalControleReajusteFretePlanilha = new bootstrap.Modal(document.getElementById("divModalControleReajusteFretePlanilha"), { backdrop: true, keyboard: true });
    _modalRejeitarControleReajusteFretePlanilha = new bootstrap.Modal(document.getElementById("divModalRejeitarControleReajusteFretePlanilha"), { backdrop: true, keyboard: true });
    _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaControle.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaControle.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridControleReajuste() {
    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharControleClick,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaControle.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "AutorizacaoControleReajusteFretePlanilha/ExportarPesquisa",
        titulo: "Autorização Controle"
    };

    _gridControleReajuste = new GridView(_pesquisaControle.Pesquisar.idGrid, "AutorizacaoControleReajusteFretePlanilha/Pesquisa", _pesquisaControle, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplosControlesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os reajustes selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaControle);

        dados.SelecionarTodos = _pesquisaControle.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridControleReajuste.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridControleReajuste.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoControleReajusteFretePlanilha/AprovarMultiplasLinhas", dados, function (arg) {
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

                    atualizarGridControleReajuste();
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
    _modalRejeitarControleReajusteFretePlanilha.hide();
}

function detalharControleClick(itemGrid) {
    limparCamposControle();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaControle);
    _controleReajusteFretePlanilha.Codigo.val(itemGrid.Codigo);
    _controleReajusteFretePlanilha.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_controleReajusteFretePlanilha, "AutorizacaoControleReajusteFretePlanilha/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != null) {
                AtualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoControleReajusteFretePlanilha.AgAprovacao);

                _modalControleReajusteFretePlanilha.show();
                $modalControleReajuste.one('hidden.bs.modal', function () {
                    limparCamposControle();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function downloadPlanilhaClick() {
    var dados = {
        Codigo: _controleReajusteFretePlanilha.Codigo.val()
    };
    if (dados.Codigo > 0) {
        executarDownload("ControleReajusteFretePlanilha/DownloadPlanilha", dados);
    }
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarControleSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os reajustes selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaControle);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaControle.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridControleReajuste.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridControleReajuste.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoControleReajusteFretePlanilha/ReprovarMultiplasLinhas", dados, function (arg) {
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
                    atualizarGridControleReajuste();
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

function rejeitarMultiplosControlesClick() {
    LimparCampos(_rejeicao);
    _modalRejeitarControleReajusteFretePlanilha.show();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function atualizarGridControleReajuste() {
    _pesquisaControle.SelecionarTodos.val(false);
    _pesquisaControle.AprovarTodas.visible(false);
    _pesquisaControle.DelegarTodos.visible(false);
    _pesquisaControle.RejeitarTodas.visible(false);

    _gridControleReajuste.CarregarGrid();

    _situacaoControleReajusteFretePlanilhaUltimaPesquisa = _pesquisaControle.Situacao.val();
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirMultiplasOpcoes(e) {
    var possuiSelecionado = _gridControleReajuste.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaControle.SelecionarTodos.val();
    var situacaoPermiteSelecao = (_situacaoControleReajusteFretePlanilhaUltimaPesquisa == EnumSituacaoControleReajusteFretePlanilha.AgAprovacao);

    // Esconde todas opções
    _pesquisaControle.AprovarTodas.visible(false);
    _pesquisaControle.DelegarTodos.visible(false);
    _pesquisaControle.RejeitarTodas.visible(false);

    if (situacaoPermiteSelecao && (possuiSelecionado || selecionadoTodos)) {
        _pesquisaControle.AprovarTodas.visible(true);
        _pesquisaControle.DelegarTodos.visible(true);
        _pesquisaControle.RejeitarTodas.visible(true);
    }
}

function limparCamposControle() {
    resetarTabs();
    limparRegras();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

// #endregion Funções Privadas
