/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoConferenciaContainer.js" />

// #region Objetos Globais do Arquivo

var _conferenciaContainerAprovacao;
var _gridConferenciaContainer;
var _pesquisaConferenciaContainer;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConferenciaContainerAprovacao = function () {
    this.Codigo = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 1000 });

    this.Aprovar = PropertyEntity({ eventClick: aprovarClick, type: types.event, text: "Aprovar" });
}

var PesquisaConferenciaContainer = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", def: "", val: ko.observable(""), maxlength: 50 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoConferenciaContainer.AguardandoConferencia), options: EnumSituacaoConferenciaContainer.obterOpcoesPesquisa(), def: EnumSituacaoConferenciaContainer.AguardandoConferencia, text: "Situação:" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(false);
            recarregarConferenciaContainer();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

// #endregion Classes

// #region Funções de Inicialização

function loadConferenciaContainer() {
    _pesquisaConferenciaContainer = new PesquisaConferenciaContainer();
    KoBindings(_pesquisaConferenciaContainer, "knockoutPesquisaConferenciaContainer", false);

    _conferenciaContainerAprovacao = new ConferenciaContainerAprovacao();
    KoBindings(_conferenciaContainerAprovacao, "knockoutConferenciaContainerAprovacao");

    loadGridConferenciaContainer();
}

function loadGridConferenciaContainer() {
    var opcaoAprovar = { descricao: "Aprovar", id: guid(), metodo: exibirModalAprovacaoClick, icone: "", visibilidade: isExibirOpcaoAprovarConferenciaContainer };
    var opcaoAuditoria = { descricao: "Auditoria", id: guid(), metodo: exibirAuditoriaClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [opcaoAprovar, opcaoAuditoria] };

    _gridConferenciaContainer = new GridView(_pesquisaConferenciaContainer.Pesquisar.idGrid, "ConferenciaContainer/Pesquisa", _pesquisaConferenciaContainer, menuOpcoes);
    _gridConferenciaContainer.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarClick() {
    executarReST("ConferenciaContainer/Aprovar", RetornarObjetoPesquisa(_conferenciaContainerAprovacao), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A conferência do container foi aprovada com sucesso!");
                fecharModalConferenciaContainerAprovacao();
                recarregarConferenciaContainer();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirModalAprovacaoClick(registroSelecionado) {
    executarReST("ConferenciaContainer/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            PreencherObjetoKnout(_conferenciaContainerAprovacao, retorno);
            exibirModalConferenciaContainerAprovacao();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirAuditoriaClick(registroSelecionado) {
    var data = { Codigo: registroSelecionado.Codigo };
    var closureAuditoria = OpcaoAuditoria("ConferenciaContainer", null);

    closureAuditoria(data);
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function exibirModalConferenciaContainerAprovacao() {
    Global.abrirModal('divModalConferenciaContainerAprovacao');
    $("#divModalConferenciaContainerAprovacao").one('hidden.bs.modal', function () {
        LimparCampos(_conferenciaContainerAprovacao);
    });
}

function fecharModalConferenciaContainerAprovacao() {
    Global.fecharModal('divModalConferenciaContainerAprovacao');
}

function isExibirOpcaoAprovarConferenciaContainer(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoConferenciaContainer.AguardandoConferencia;
}

function recarregarConferenciaContainer() {
    _gridConferenciaContainer.CarregarGrid();
}

// #endregion Funções Privadas
