/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoGatilhoPedidoOcorrenciaColetaEntrega.js" />
/// <reference path="TipoOperacao.js" />

// #region Objetos Globais do Arquivo

var _crudTipoOperacaoControleColetaEntregaOcorrenciaCadastro;
var _gridTipoOperacaoControleColetaEntregaOcorrencia;
var _tipoOperacaoControleColetaEntregaOcorrencia;
var _tipoOperacaoControleColetaEntregaOcorrenciaCadastro;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDTipoOperacaoControleColetaEntregaOcorrenciaCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarTipoOperacaoControleColetaEntregaOcorrenciaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTipoOperacaoControleColetaEntregaOcorrenciaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirTipoOperacaoControleColetaEntregaOcorrenciaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var TipoOperacaoControleColetaEntregaOcorrencia = function () {
    this.ListaOcorrencia = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarTipoOperacaoControleColetaEntregaOcorrenciaModalClick, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarConfiguracaoDeOcorrenciaAutomatica });
}

var TipoOperacaoControleColetaEntregaOcorrenciaCadastro = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOcorrencia.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Gatilho = PropertyEntity({ val: ko.observable(""), options: EnumTipoGatilhoPedidoOcorrenciaColetaEntrega.obterOpcoes(), def: "", text: Localization.Resources.Pedidos.TipoOperacao.Gatilho.getRequiredFieldDescription(), required: true });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Observacao.getFieldDescription(), maxlength: 3000 });

    this.TagDestinatario = PropertyEntity({ eventClick: function () { InserirTag(self.Observacao.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Destinatario });
    this.TagDestino = PropertyEntity({ eventClick: function () { InserirTag(self.Observacao.id, "#Destino"); }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Destino });
    this.TagOrigem = PropertyEntity({ eventClick: function () { InserirTag(self.Observacao.id, "#Origem"); }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Origem });
    this.TagRemetente = PropertyEntity({ eventClick: function () { InserirTag(self.Observacao.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Remetente });
}

// #endregion Classes

// #region Funções de Inicialização

function loadTipoOperacaoControleColetaEntregaOcorrencia() {
    _tipoOperacaoControleColetaEntregaOcorrencia = new TipoOperacaoControleColetaEntregaOcorrencia();
    KoBindings(_tipoOperacaoControleColetaEntregaOcorrencia, "knockoutTipoOperacaoControleColetaEntregaOcorrencia");

    _tipoOperacaoControleColetaEntregaOcorrenciaCadastro = new TipoOperacaoControleColetaEntregaOcorrenciaCadastro();
    KoBindings(_tipoOperacaoControleColetaEntregaOcorrenciaCadastro, "knockoutTipoOperacaoControleColetaEntregaOcorrenciaCadastro");

    _crudTipoOperacaoControleColetaEntregaOcorrenciaCadastro = new CRUDTipoOperacaoControleColetaEntregaOcorrenciaCadastro();
    KoBindings(_crudTipoOperacaoControleColetaEntregaOcorrenciaCadastro, "knockoutCRUDTipoOperacaoControleColetaEntregaOcorrenciaCadastro");

    loadGridTipoOperacaoControleColetaEntregaOcorrencia();

    new BuscarTipoOcorrencia(_tipoOperacaoControleColetaEntregaOcorrenciaCadastro.TipoOcorrencia, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true);
}

function loadGridTipoOperacaoControleColetaEntregaOcorrencia() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarTipoOperacaoControleColetaEntregaOcorrenciaClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoOcorrencia", title: Localization.Resources.Pedidos.TipoOperacao.TipoDeOcorrencia, width: "60%" },
        { data: "Gatilho", title: Localization.Resources.Pedidos.TipoOperacao.Gatilho, width: "20%" }
    ];

    _gridTipoOperacaoControleColetaEntregaOcorrencia = new BasicDataTable(_tipoOperacaoControleColetaEntregaOcorrencia.ListaOcorrencia.idGrid, header, menuOpcoes);
    _gridTipoOperacaoControleColetaEntregaOcorrencia.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarTipoOperacaoControleColetaEntregaOcorrenciaClick() {
    if (!validarTipoOperacaoControleColetaEntregaOcorrencia())
        return;

    _tipoOperacaoControleColetaEntregaOcorrencia.ListaOcorrencia.val().push(obterTipoOperacaoControleColetaEntregaOcorrenciaSalvar());

    recarregarGridTipoOperacaoControleColetaEntregaOcorrencia();
    fecharModalCadastroTipoOperacaoControleColetaEntregaOcorrencia();
}

function adicionarTipoOperacaoControleColetaEntregaOcorrenciaModalClick() {
    _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Codigo.val(guid());

    controlarBotoesTipoOperacaoControleColetaEntregaOcorrenciaCadastroHabilitados(false);
    exibirModalCadastroTipoOperacaoControleColetaEntregaOcorrencia();
}

function atualizarTipoOperacaoControleColetaEntregaOcorrenciaClick() {
    if (!validarTipoOperacaoControleColetaEntregaOcorrencia())
        return;

    var listaOcorrencia = obterListaTipoOperacaoControleColetaEntregaOcorrencia();

    for (var i = 0; i < listaOcorrencia.length; i++) {
        if (_tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Codigo.val() == listaOcorrencia[i].Codigo) {
            listaOcorrencia.splice(i, 1, obterTipoOperacaoControleColetaEntregaOcorrenciaSalvar());
            break;
        }
    }

    _tipoOperacaoControleColetaEntregaOcorrencia.ListaOcorrencia.val(listaOcorrencia)

    recarregarGridTipoOperacaoControleColetaEntregaOcorrencia();
    fecharModalCadastroTipoOperacaoControleColetaEntregaOcorrencia();
}

function editarTipoOperacaoControleColetaEntregaOcorrenciaClick(registroSelecionado) {
    var tipoOperacaoControleColetaEntregaOcorrencia = obterTipoOperacaoControleColetaEntregaOcorrenciaPorCodigo(registroSelecionado.Codigo);

    if (!tipoOperacaoControleColetaEntregaOcorrencia)
        return;
    
    PreencherObjetoKnout(_tipoOperacaoControleColetaEntregaOcorrenciaCadastro, { Data: tipoOperacaoControleColetaEntregaOcorrencia });
    controlarBotoesTipoOperacaoControleColetaEntregaOcorrenciaCadastroHabilitados(true);
    exibirModalCadastroTipoOperacaoControleColetaEntregaOcorrencia();
}

function excluirTipoOperacaoControleColetaEntregaOcorrenciaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.TipoOperacao.RealmenteDesejaExcluirOcorrencia, function () {
        var listaOcorrencia = obterListaTipoOperacaoControleColetaEntregaOcorrencia();
        
        for (var i = 0; i < listaOcorrencia.length; i++) {
            if (_tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Codigo.val() == listaOcorrencia[i].Codigo)
                listaOcorrencia.splice(i, 1);
        }

        _tipoOperacaoControleColetaEntregaOcorrencia.ListaOcorrencia.val(listaOcorrencia);

        recarregarGridTipoOperacaoControleColetaEntregaOcorrencia();
        fecharModalCadastroTipoOperacaoControleColetaEntregaOcorrencia();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherTipoOperacaoControleColetaEntregaOcorrencia(listaOcorrencia) {
    _tipoOperacaoControleColetaEntregaOcorrencia.ListaOcorrencia.val(listaOcorrencia);

    recarregarGridTipoOperacaoControleColetaEntregaOcorrencia();
}

function preencherTipoOperacaoControleColetaEntregaOcorrenciaSalvar(tipoOperacao) {
    tipoOperacao["ListaGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega"] = obterListaTipoOperacaoControleColetaEntregaOcorrenciaSalvar();
}

function limparCamposTipoOperacaoControleColetaEntregaOcorrencia() {
    _tipoOperacaoControleColetaEntregaOcorrencia.ListaOcorrencia.val([]);

    recarregarGridTipoOperacaoControleColetaEntregaOcorrencia();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesTipoOperacaoControleColetaEntregaOcorrenciaCadastroHabilitados(isEdicao) {
    _crudTipoOperacaoControleColetaEntregaOcorrenciaCadastro.Atualizar.visible(isEdicao);
    _crudTipoOperacaoControleColetaEntregaOcorrenciaCadastro.Excluir.visible(isEdicao);
    _crudTipoOperacaoControleColetaEntregaOcorrenciaCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroTipoOperacaoControleColetaEntregaOcorrencia() {
    Global.abrirModal('divModalCadastroTipoOperacaoControleColetaEntregaOcorrencia');
    $("#divModalCadastroTipoOperacaoControleColetaEntregaOcorrencia").one('hidden.bs.modal', function () {
        limparCamposTipoOperacaoControleColetaEntregaOcorrenciaCadastro();
    });
}

function fecharModalCadastroTipoOperacaoControleColetaEntregaOcorrencia() {
    Global.fecharModal('divModalCadastroTipoOperacaoControleColetaEntregaOcorrencia');
}

function limparCamposTipoOperacaoControleColetaEntregaOcorrenciaCadastro() {
    LimparCampos(_tipoOperacaoControleColetaEntregaOcorrenciaCadastro);
}

function obterTipoOperacaoControleColetaEntregaOcorrenciaPorCodigo(codigo) {
    var listaOcorrencia = obterListaTipoOperacaoControleColetaEntregaOcorrencia();

    for (var i = 0; i < listaOcorrencia.length; i++) {
        var tipoOperacaoControleColetaEntregaOcorrencia = listaOcorrencia[i];

        if (codigo == tipoOperacaoControleColetaEntregaOcorrencia.Codigo)
            return tipoOperacaoControleColetaEntregaOcorrencia;
    }

    return undefined;
}

function obterTipoOperacaoControleColetaEntregaOcorrenciaSalvar() {
    return {
        Codigo: _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Codigo.val(),
        Gatilho: _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Gatilho.val(),
        Observacao: _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Observacao.val(),
        TipoOcorrencia: {
            Codigo: _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.TipoOcorrencia.codEntity(),
            Descricao: _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.TipoOcorrencia.val()
        }
    };
}

function obterListaTipoOperacaoControleColetaEntregaOcorrencia() {
    return _tipoOperacaoControleColetaEntregaOcorrencia.ListaOcorrencia.val().slice();
}

function obterListaTipoOperacaoControleColetaEntregaOcorrenciaSalvar() {
    var listaOcorrencia = obterListaTipoOperacaoControleColetaEntregaOcorrencia();
    var listaOcorrenciaSalvar = new Array();

    for (var i = 0; i < listaOcorrencia.length; i++) {
        var tipoOperacaoControleColetaEntregaOcorrencia = listaOcorrencia[i];

        listaOcorrenciaSalvar.push({
            Codigo: tipoOperacaoControleColetaEntregaOcorrencia.Codigo,
            Gatilho: tipoOperacaoControleColetaEntregaOcorrencia.Gatilho,
            Observacao: tipoOperacaoControleColetaEntregaOcorrencia.Observacao,
            TipoOcorrencia: tipoOperacaoControleColetaEntregaOcorrencia.TipoOcorrencia.Codigo
        });
    }

    return JSON.stringify(listaOcorrenciaSalvar);
}

function recarregarGridTipoOperacaoControleColetaEntregaOcorrencia() {
    var listaOcorrencia = obterListaTipoOperacaoControleColetaEntregaOcorrencia();
    var listaOcorrenciaCarregar = new Array();

    for (var i = 0; i < listaOcorrencia.length; i++) {
        var tipoOperacaoControleColetaEntregaOcorrencia = listaOcorrencia[i];

        listaOcorrenciaCarregar.push({
            Codigo: tipoOperacaoControleColetaEntregaOcorrencia.Codigo,
            Gatilho: EnumTipoGatilhoPedidoOcorrenciaColetaEntrega.obterDescricao(tipoOperacaoControleColetaEntregaOcorrencia.Gatilho),
            Observacao: tipoOperacaoControleColetaEntregaOcorrencia.Observacao,
            TipoOcorrencia: tipoOperacaoControleColetaEntregaOcorrencia.TipoOcorrencia.Descricao
        });
    }

    _gridTipoOperacaoControleColetaEntregaOcorrencia.CarregarGrid(listaOcorrenciaCarregar);
}

function validarTipoOperacaoControleColetaEntregaOcorrencia() {
    if (!ValidarCamposObrigatorios(_tipoOperacaoControleColetaEntregaOcorrenciaCadastro)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    var listaOcorrencia = obterListaTipoOperacaoControleColetaEntregaOcorrencia();

    for (var i = 0; i < listaOcorrencia.length; i++) {
        var tipoOperacaoControleColetaEntregaOcorrencia = listaOcorrencia[i];

        if (
            (tipoOperacaoControleColetaEntregaOcorrencia.Codigo != _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Codigo.val()) &&
            (tipoOperacaoControleColetaEntregaOcorrencia.TipoOcorrencia.Codigo == _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.TipoOcorrencia.codEntity()) &&
            (tipoOperacaoControleColetaEntregaOcorrencia.Gatilho == _tipoOperacaoControleColetaEntregaOcorrenciaCadastro.Gatilho.val())
        ) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pedidos.TipoOperacao.RegistroDuplicado, Localization.Resources.Pedidos.TipoOperacao.JaExisteUmCadastradoComAsConfiguracaesInformadas);
            return false;
        }
    }

    return true;
}

// #endregion Funções Privadas
