/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _periodoCarregamento;

/*
 * Declaração das Classes
 */

var PeriodoCarregamento = function () {
    this._dataCarregamento = PropertyEntity({});
    this._filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0)});
    this._tipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this._buscaPeriodoCarregamento;

    this._init();
}

PeriodoCarregamento.prototype = {
    buscar: function () {
        if (_preenchendoDadosCarregamento)
            return;

        var pedidos = PEDIDOS_SELECIONADOS();
        var dataCarregamento = _carregamento.DataCarregamento.val();
        var codigoFilial = (pedidos.length > 0) ? pedidos[0].CodigoFilial : 0;
        var codigoTipoCarga = _carregamentoTransporte.TipoDeCarga.codEntity();

        if ((this._dataCarregamento.val() == dataCarregamento) && (this._filial.codEntity() == codigoFilial) && (this._tipoCarga.codEntity() == codigoTipoCarga))
            return;

        this._dataCarregamento.val(dataCarregamento);
        this._filial.codEntity(codigoFilial);
        this._filial.val((pedidos.length > 0) ? pedidos[0].Filial : "");
        this._tipoCarga.codEntity(codigoTipoCarga);
        this._tipoCarga.val(_carregamentoTransporte.TipoDeCarga.val());

        if (!Boolean(dataCarregamento) || !Boolean(codigoFilial) || !Boolean(codigoTipoCarga)) {
            this._dataCarregamento.val("");
            _carregamento.DataCarregamento.val("");
            buscarCapacidadeJanelaCarregamento();
            return;
        }

        this._buscaPeriodoCarregamento.buscarPrimeiro(_carregamento.DataCarregamento.val());
    },
    limpar: function () {
        LimparCampos(this);
    },
    _init: function () {
        var self = this;

        var botoesDataCarregamento = document
            .getElementById(_carregamento.DataCarregamento.id)
            .parentElement
            .getElementsByClassName("input-group-append")

        if (botoesDataCarregamento.length > 1)
            botoesDataCarregamento[0].classList.add("d-none");

        self._buscaPeriodoCarregamento = new BuscarPeriodoCarregamento(
            _carregamento.DataCarregamento,
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
            _carregamento.DataCarregamento.val(retorno.InicioCarregamento);
        }
        else {
            this._dataCarregamento.val("");
            _carregamento.DataCarregamento.val("");
        }

        buscarCapacidadeJanelaCarregamento();
    }
};

/*
 * Declaração das Funções de Inicialização
 */

function loadPeriodoCarregamento() {
    if (_carregamento.InformarPeriodoCarregamento.val())
        _periodoCarregamento = new PeriodoCarregamento();
}

/*
 * Declaração das Funções Públicas
 */

function buscarPeriodoCarregamento() {
    if (_periodoCarregamento)
        _periodoCarregamento.buscar();
}

function limparCamposPeriodoCarregamento() {
    if (_periodoCarregamento)
        _periodoCarregamento.limpar();
}
