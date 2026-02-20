/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="DetalhesPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas;
var _gridNotasCompativeisApenasPorCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAdicionarNovosPedidosPorNotasAvulsas = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Numero.getFieldDescription(), getType: typesKnockout.int });
    this.ChaveNotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Chave.getFieldDescription(), val: ko.observable(""), def: "" });

    this.Pesquisar = PropertyEntity({ eventClick: function () { _gridNotasCompativeisApenasPorCarga.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar });

    this.GridNotasAvulsasCompativeis = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Cargas.Carga.MarcarDesmarcarTodos, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNovosPedidosPorNotasAvulsasClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, enable: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAdicionarNovosPedidosPorNotasAvulsas() {
    _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas = new DetalhePedidoAdicionarNovosPedidosPorNotasAvulsas();
    KoBindings(_detalhePedidoAdicionarNovosPedidosPorNotasAvulsas, "divModalDetalhesPedidoAdicionarNovosPedidosPorNotasAvulsas");

    loadGridAdicionarNovosPedidosPorNotasAvulsas();
}

function loadGridAdicionarNovosPedidosPorNotasAvulsas() {

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        somenteLeitura: false,
        SelecionarTodosKnout: _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.SelecionarTodos
    };

    _gridNotasCompativeisApenasPorCarga = new GridView(_detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.GridNotasAvulsasCompativeis.idGrid, "DocumentoNF/PesquisaNotasCompativeisApenasPorCarga", _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas, null, null, null, null, null, null, multiplaescolha);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarNovosPedidosPorNotasAvulsasClick() {
    var documentosSelecionados = _gridNotasCompativeisApenasPorCarga.ObterMultiplosSelecionados();

    if (documentosSelecionados.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.SelecioneAoMenosUmDocumentoParaRealizarVinculoCarga);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteGerarNovosPedidosAPartirDeNotasAvulsas, function () {
        if (_detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.SelecionarTodos.val())
            documentosSelecionados = _gridNotasCompativeisApenasPorCarga.ObterMultiplosNaoSelecionados();
        else
            documentosSelecionados = _gridNotasCompativeisApenasPorCarga.ObterMultiplosSelecionados();

        var codigosDocumentosSelecionados = new Array();

        for (var i = 0; i < documentosSelecionados.length; i++)
            codigosDocumentosSelecionados.push(documentosSelecionados[i].Codigo);

        var data = {
            CodigoCarga: _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.CodigoCarga.val(),
            NumeroNotaFiscal: _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.NumeroNotaFiscal.val(),
            ChaveNotaFiscal: _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.ChaveNotaFiscal.val(),
            SelecionarTodos: _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.SelecionarTodos.val(),
            Documentos: JSON.stringify(codigosDocumentosSelecionados)
        };

        adicionarNovosPedidosPorNotasAvulsas(data);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarNovosPedidosPorNotasAvulsasDetalhePedido(codigoCarga) {
    _detalhePedidoAdicionarNovosPedidosPorNotasAvulsas.CodigoCarga.val(codigoCarga);

    _gridNotasCompativeisApenasPorCarga.CarregarGrid(function () {
        exibirModalAdicionarNovosPedidosPorNotasAvulsasDetalhePedido();
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function adicionarNovosPedidosPorNotasAvulsas(data) {
    executarReST("Carga/AdicionarNovosPedidosPorNotasAvulsas", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                fecharModalAdicionarNovosPedidosPorNotasAvulsasDetalhePedido();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.PedidosAdicionadosComSucesso);
                exibirDetalhesPedidos(data.CodigoCarga);
                IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirModalAdicionarNovosPedidosPorNotasAvulsasDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAdicionarNovosPedidosPorNotasAvulsas');
    $("#divModalDetalhesPedidoAdicionarNovosPedidosPorNotasAvulsas").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAdicionarNovosPedidosPorNotasAvulsas);
    });
}

function fecharModalAdicionarNovosPedidosPorNotasAvulsasDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAdicionarNovosPedidosPorNotasAvulsas');
}

// #endregion Funções Privadas
