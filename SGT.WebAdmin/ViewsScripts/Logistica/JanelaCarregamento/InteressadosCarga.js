/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />

// #region Objetos Globais do Arquivo

var _interessadosCarga;
var _gridInteressadosCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var InteressadosCarga = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Transportador = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.SelecionarTransportador = PropertyEntity({ eventClick: selecionarTransportadorInteressadoClick, type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarTransportador, idGrid: guid(), enable: ko.observable(false) });
    this.ImprimirCotacao = PropertyEntity({ eventClick: imprimirCotacaoTransportadorInteressadoClick, type: types.event, text: Localization.Resources.Cargas.Carga.ImprimirCotacao, idGrid: guid(), visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadInteressadosCarga() {
    _interessadosCarga = new InteressadosCarga();
    KoBindings(_interessadosCarga, "divModalSelecaoTransportador");

    loadGridInteressadosCarga();
}

function loadGridInteressadosCarga() {
    var multiplaescolha = {
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        permitirSelecionarSomenteUmRegistro: true,
        somenteLeitura: false,
        callbackSelecionado: function (e, registroSelecionado) {
            _interessadosCarga.Transportador.val(registroSelecionado.CodigoTransportador);
            _interessadosCarga.SelecionarTransportador.enable(true);
            _interessadosCarga.ImprimirCotacao.visible(true);
        },
        callbackNaoSelecionado: function () {
            _interessadosCarga.Transportador.val(0);
            _interessadosCarga.SelecionarTransportador.enable(false);
            _interessadosCarga.ImprimirCotacao.visible(false);
        }
    };

    _gridInteressadosCarga = new GridView("grid-interessados-carga", "JanelaCarregamento/ObterInteressadosCarga", _interessadosCarga, null, null, 10, null, null, null, multiplaescolha, 9999999);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function imprimirCotacaoTransportadorInteressadoClick() {
    executarDownload("JanelaCarregamento/ImprimirValoresCotacao", { Carga: _interessadosCarga.Carga.val(), Transportador: _interessadosCarga.Transportador.val() });
}

function selecionarTransportadorInteressadoClick() {
    executarReST("JanelaCarregamento/SalvarInteressadoCarga", { Carga: _interessadosCarga.Carga.val(), Transportador: _interessadosCarga.Transportador.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.TransportadorSelecionadoComSucesso);
                Global.fecharModal('divModalSelecaoTransportador');
                LimparCampos(_interessadosCarga);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function AbrirTelaInteressadosCarga(carga, sender) {
    if (sender)
        sender.stopPropagation();

    _interessadosCarga.Carga.val(carga);
    _gridInteressadosCarga.CarregarGrid();

    Global.abrirModal("divModalSelecaoTransportador");
}

// #endregion Funções Públicas