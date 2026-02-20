/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/PeriodoCarregamento.js" />
/// <reference path="CapacidadeJanelaCarregamento.js" />
/// <reference path="CarregamentoFilial.js" />

// #region Objetos Globais do Arquivo

var _periodoCarregamento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PeriodoCarregamento = function () {
    this._dataCarregamento = PropertyEntity({ validaEscritaBusca: false });
    this._filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0)});
    this._tipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this._buscaPeriodoCarregamento;

    this._init();
}

PeriodoCarregamento.prototype = {
    buscar: function () {
        if (_preenchendoDadosCarregamento)
            return;

        var dadosPorFilial = obterDadosCarregamentoPorFilial();
        var dataCarregamento = dadosPorFilial.DataCarregamento;
        var codigoFilial = dadosPorFilial.CodigoFilial;
        var codigoTipoCarga = _carregamentoTransporte.TipoDeCarga.codEntity();

        if ((this._dataCarregamento.val() == dataCarregamento) && (this._filial.codEntity() == codigoFilial) && (this._tipoCarga.codEntity() == codigoTipoCarga))
            return;

        this._dataCarregamento.val(dataCarregamento);
        this._filial.codEntity(codigoFilial);
        this._filial.val(dadosPorFilial.DescricaoFilial);
        this._tipoCarga.codEntity(codigoTipoCarga);
        this._tipoCarga.val(_carregamentoTransporte.TipoDeCarga.val());

        if (!Boolean(dataCarregamento) || !Boolean(codigoFilial) || !Boolean(codigoTipoCarga)) {
            this._dataCarregamento.val("");
            definirDataCarregamentoPorFilial("");
            buscarCapacidadeJanelaCarregamento();
            return;
        }

        this._buscaPeriodoCarregamento.buscarPrimeiro(dataCarregamento);
    },
    limpar: function () {
        LimparCampos(this);
    },
    _init: function () {
        var self = this;

        self._buscaPeriodoCarregamento = new BuscarPeriodoCarregamento(
            self._dataCarregamento,
            function (retorno) { self._retornoConsultaPeriodoCarregamento(retorno); },
            self._dataCarregamento,
            undefined,
            self._filial,
            self._tipoCarga,
            true
        );
    },
    _retornoConsultaPeriodoCarregamento: function (retorno) {
        if (retorno) {
            this._dataCarregamento.val(retorno.InicioCarregamento);
            definirDataCarregamentoPorFilial(retorno.InicioCarregamento);
        }
        else {
            this._dataCarregamento.val("");
            definirDataCarregamentoPorFilial("");
        }

        buscarCapacidadeJanelaCarregamento();
    }
};

// #endregion Classes

// #region Funções de Inicialização

function loadPeriodoCarregamento() {
    if (_carregamento.InformarPeriodoCarregamento.val())
        _periodoCarregamento = new PeriodoCarregamento();
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function buscarPeriodoCarregamento() {
    if (_periodoCarregamento)
        _periodoCarregamento.buscar();
}

function limparCamposPeriodoCarregamento() {
    if (_periodoCarregamento)
        _periodoCarregamento.limpar();
}

// #endregion Funções Públicas
