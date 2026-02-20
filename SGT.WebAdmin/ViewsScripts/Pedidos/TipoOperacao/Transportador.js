/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoUltimoPontoRoteirizacao.js" />
/// <reference path="TipoOperacao.js" />

// #region Objetos Globais do Arquivo

var _crudTipoOperacaoTransportadorCadastro;
var _gridTipoOperacaoTransportador;
var _tipoOperacaoTransportador;
var _tipoOperacaoTransportadorCadastro;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDTipoOperacaoTransportadorCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarTipoOperacaoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTipoOperacaoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirTipoOperacaoTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var TipoOperacaoTransportador = function () {
    this.ListaTransportador = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarTipoOperacaoTransportadorModalClick, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarTransportador });
}

var TipoOperacaoTransportadorCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Pedidos.TipoOperacao.Transportador.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Transportadores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Pedidos.TipoOperacao.Transportadores.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.Retornando), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: EnumTipoUltimoPontoRoteirizacao.Retornando, text: Localization.Resources.Pedidos.TipoOperacao.UltimoPonto.getFieldDescription() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadTipoOperacaoTransportador() {
    _tipoOperacaoTransportador = new TipoOperacaoTransportador();
    KoBindings(_tipoOperacaoTransportador, "knockoutTipoOperacaoTransportador");

    _tipoOperacaoTransportadorCadastro = new TipoOperacaoTransportadorCadastro();
    KoBindings(_tipoOperacaoTransportadorCadastro, "knockoutTipoOperacaoTransportadorCadastro");

    _crudTipoOperacaoTransportadorCadastro = new CRUDTipoOperacaoTransportadorCadastro();
    KoBindings(_crudTipoOperacaoTransportadorCadastro, "knockoutCRUDTipoOperacaoTransportadorCadastro");

    loadGridTipoOperacaoTransportador();

    new BuscarTransportadores(_tipoOperacaoTransportadorCadastro.Transportador);
    new BuscarTransportadores(_tipoOperacaoTransportadorCadastro.Transportadores);
}

function loadGridTipoOperacaoTransportador() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarTipoOperacaoTransportadorClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Transportador", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%" },
        { data: "TipoUltimoPontoRoteirizacao", title: Localization.Resources.Pedidos.TipoOperacao.UltimoPonto, width: "20%" }
    ];

    _gridTipoOperacaoTransportador = new BasicDataTable(_tipoOperacaoTransportador.ListaTransportador.idGrid, header, menuOpcoes);
    _gridTipoOperacaoTransportador.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarTipoOperacaoTransportadorClick() {
    let transportadores = _tipoOperacaoTransportadorCadastro.Transportadores.multiplesEntities();

    transportadores.forEach((transportador) => {
        if (validarTipoOperacaoTransportador(transportador)) {
            _tipoOperacaoTransportador.ListaTransportador.val().push(obterTipoOperacaoTransportadorSalvar(transportador));
        }
    });

    recarregarGridTipoOperacaoTransportador();
    fecharModalCadastroTipoOperacaoTransportador();
}

function adicionarTipoOperacaoTransportadorModalClick() {
    _tipoOperacaoTransportadorCadastro.Transportadores.required(true);
    _tipoOperacaoTransportadorCadastro.Transportadores.visible(true);

    _tipoOperacaoTransportadorCadastro.Transportador.required(false);
    _tipoOperacaoTransportadorCadastro.Transportador.visible(false);

    controlarBotoesTipoOperacaoTransportadorCadastroHabilitados(false);
    exibirModalCadastroTipoOperacaoTransportador();
}

function atualizarTipoOperacaoTransportadorClick() {
    if (!validarTipoOperacaoTransportador())
        return;

    var listaTransportador = obterListaTipoOperacaoTransportador();
    let transportador = {
        Codigo: _tipoOperacaoTransportadorCadastro.Transportador.codEntity(),
        Descricao: _tipoOperacaoTransportadorCadastro.Transportador.val()
    }

    for (var i = 0; i < listaTransportador.length; i++) {
        if (_tipoOperacaoTransportadorCadastro.Codigo.val() == listaTransportador[i].Codigo) {
            listaTransportador.splice(i, 1, obterTipoOperacaoTransportadorSalvar(transportador));
            break;
        }
    }

    _tipoOperacaoTransportador.ListaTransportador.val(listaTransportador)

    recarregarGridTipoOperacaoTransportador();
    fecharModalCadastroTipoOperacaoTransportador();
}

function editarTipoOperacaoTransportadorClick(registroSelecionado) {
    _tipoOperacaoTransportadorCadastro.Transportadores.required(false);
    _tipoOperacaoTransportadorCadastro.Transportadores.visible(false);

    _tipoOperacaoTransportadorCadastro.Transportador.required(true);
    _tipoOperacaoTransportadorCadastro.Transportador.visible(true);

    var tipoOperacaoTransportador = obterTipoOperacaoTransportadorPorCodigo(registroSelecionado.Codigo);

    if (!tipoOperacaoTransportador)
        return;

    PreencherObjetoKnout(_tipoOperacaoTransportadorCadastro, { Data: tipoOperacaoTransportador });
    controlarBotoesTipoOperacaoTransportadorCadastroHabilitados(true);
    exibirModalCadastroTipoOperacaoTransportador();
}

function excluirTipoOperacaoTransportadorClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.TipoOperacao.RealmenteDesejaExcluirTransportador, function () {
        var listaTransportador = obterListaTipoOperacaoTransportador();

        for (var i = 0; i < listaTransportador.length; i++) {
            if (_tipoOperacaoTransportadorCadastro.Codigo.val() == listaTransportador[i].Codigo)
                listaTransportador.splice(i, 1);
        }

        _tipoOperacaoTransportador.ListaTransportador.val(listaTransportador);

        recarregarGridTipoOperacaoTransportador();
        fecharModalCadastroTipoOperacaoTransportador();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherTipoOperacaoTransportador(listaTransportador) {
    _tipoOperacaoTransportador.ListaTransportador.val(listaTransportador);

    recarregarGridTipoOperacaoTransportador();
}

function preencherTipoOperacaoTransportadorSalvar(tipoOperacao) {
    tipoOperacao["ListaTransportador"] = obterListaTipoOperacaoTransportadorSalvar();
}

function limparCamposTipoOperacaoTransportador() {
    _tipoOperacaoTransportador.ListaTransportador.val([]);

    recarregarGridTipoOperacaoTransportador();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesTipoOperacaoTransportadorCadastroHabilitados(isEdicao) {
    _crudTipoOperacaoTransportadorCadastro.Atualizar.visible(isEdicao);
    _crudTipoOperacaoTransportadorCadastro.Excluir.visible(isEdicao);
    _crudTipoOperacaoTransportadorCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroTipoOperacaoTransportador() {
    Global.abrirModal('divModalCadastroTipoOperacaoTransportador');
    $("#divModalCadastroTipoOperacaoTransportador").one('hidden.bs.modal', function () {
        limparCamposTipoOperacaoTransportadorCadastro();
    });
}

function fecharModalCadastroTipoOperacaoTransportador() {
    Global.fecharModal('divModalCadastroTipoOperacaoTransportador');
}

function limparCamposTipoOperacaoTransportadorCadastro() {
    LimparCampos(_tipoOperacaoTransportadorCadastro);
}

function obterTipoOperacaoTransportadorPorCodigo(codigo) {
    var listaTransportador = obterListaTipoOperacaoTransportador();

    for (var i = 0; i < listaTransportador.length; i++) {
        var tipoOperacaoTransportador = listaTransportador[i];

        if (codigo == tipoOperacaoTransportador.Codigo)
            return tipoOperacaoTransportador;
    }

    return undefined;
}

function obterTipoOperacaoTransportadorSalvar(transportador) {
    return {
        Codigo: _tipoOperacaoTransportadorCadastro.Codigo.val() || guid(),
        TipoUltimoPontoRoteirizacao: _tipoOperacaoTransportadorCadastro.TipoUltimoPontoRoteirizacao.val(),
        Transportador: {
            Codigo: transportador.Codigo,
            Descricao: transportador.Descricao
        }
    };
}

function obterListaTipoOperacaoTransportador() {
    return _tipoOperacaoTransportador.ListaTransportador.val().slice();
}

function obterListaTipoOperacaoTransportadorSalvar() {
    var listaTransportador = obterListaTipoOperacaoTransportador();
    var listaTransportadorSalvar = new Array();

    for (var i = 0; i < listaTransportador.length; i++) {
        var tipoOperacaoTransportador = listaTransportador[i];

        listaTransportadorSalvar.push({
            Codigo: tipoOperacaoTransportador.Codigo,
            TipoUltimoPontoRoteirizacao: tipoOperacaoTransportador.TipoUltimoPontoRoteirizacao,
            Transportador: tipoOperacaoTransportador.Transportador.Codigo
        });
    }

    return JSON.stringify(listaTransportadorSalvar);
}

function recarregarGridTipoOperacaoTransportador() {
    var listaTransportador = obterListaTipoOperacaoTransportador();
    var listaTransportadorCarregar = new Array();

    for (var i = 0; i < listaTransportador.length; i++) {
        var tipoOperacaoTransportador = listaTransportador[i];

        listaTransportadorCarregar.push({
            Codigo: tipoOperacaoTransportador.Codigo,
            Transportador: tipoOperacaoTransportador.Transportador.Descricao,
            TipoUltimoPontoRoteirizacao: EnumTipoUltimoPontoRoteirizacao.obterDescricao(tipoOperacaoTransportador.TipoUltimoPontoRoteirizacao)
        });
    }

    _gridTipoOperacaoTransportador.CarregarGrid(listaTransportadorCarregar);
}

function validarTipoOperacaoTransportador(transportador) {
    if (!ValidarCamposObrigatorios(_tipoOperacaoTransportadorCadastro)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    var listaTransportador = obterListaTipoOperacaoTransportador();
    let codigoTransportador = transportador ? transportador.Codigo : _tipoOperacaoTransportadorCadastro.Transportador.codEntity();

    for (var i = 0; i < listaTransportador.length; i++) {
        var tipoOperacaoTransportador = listaTransportador[i];

        if (
            (tipoOperacaoTransportador.Codigo != _tipoOperacaoTransportadorCadastro.Codigo.val()) &&
            (tipoOperacaoTransportador.Transportador.Codigo == codigoTransportador)
        ) {
            let mensagemRegistroDuplicado = transportador
                ? Localization.Resources.Pedidos.TipoOperacao.NomeTransportadorInformadoJaEstaCadastrado.format(transportador.Descricao)
                : Localization.Resources.Pedidos.TipoOperacao.TransportadorInformadoJaEstaCadastrado;

            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pedidos.TipoOperacao.RegistroDuplicado, mensagemRegistroDuplicado);
            return false;
        }
    }

    return true;
}

// #endregion Funções Privadas
