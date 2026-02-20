/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _capacidadeJanelaCarregamento;

/*
 * Declaração das Classes
 */

var CapacidadeCarregamentoPeriodo = function (dados) {
    this.CapacidadeAdicional = PropertyEntity({});
    this.CapacidadeCarregamento = PropertyEntity({});
    this.CapacidadeDisponivel = PropertyEntity({});
    this.CapacidadeUtilizada = PropertyEntity({});
    this.Periodo = PropertyEntity({});
    this.PeriodoAtivo = PropertyEntity({ getType: typesKnockout.bool });

    PreencherObjetoKnout(this, { Data: dados });
}

var CapacidadeJanelaCarregamento = function () {
    var self = this;

    this.DataCarregamento = PropertyEntity({});
    this.ExibirCapacidadeCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Filial = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ListaCapacidadeCarregamentoPeriodo = ko.observableArray();

    this.DataCarregamento.formatted = ko.computed(function () { return self.DataCarregamento.val() ? moment(Global.criarData(self.DataCarregamento.val())).locale('pt-br').format('LL') : ""; });

    this.Anterior = PropertyEntity({ eventClick: function () { self._dataCarregamentoAnterior(); }, type: types.event });
    this.Proxima = PropertyEntity({ eventClick: function () { self._proximaDataCarregamento(); }, type: types.event });
}

CapacidadeJanelaCarregamento.prototype = {
    buscar: function (callback) {
        var pedidos = PEDIDOS_SELECIONADOS();
        var dataCarregamento = _carregamento.DataCarregamento.val();
        var filial = (pedidos.length > 0) ? pedidos[0].CodigoFilial : 0;
        var tipoCarga = _carregamentoTransporte.TipoDeCarga.codEntity();

        if ((this.DataCarregamento.val() == dataCarregamento) && (this.Filial.val() == filial) && (this.TipoCarga.val() == tipoCarga)) {
            if (callback instanceof Function)
                callback();

            return;
        }

        this.DataCarregamento.val(dataCarregamento);
        this.Filial.val(filial);
        this.TipoCarga.val(tipoCarga);

        if (!Boolean(dataCarregamento) || !Boolean(filial) || !Boolean(tipoCarga)) {
            _capacidadeJanelaCarregamento.ExibirCapacidadeCarregamento.val(false);

            if (callback instanceof Function)
                callback();

            return;
        }

        this._buscar(callback);
    },
    limpar: function () {
        LimparCampos(this);
    },
    _buscar: function (callback) {
        executarReST("JanelaCarregamento/ObterCapacidadeCarregamentoDados", { Filial: this.Filial.val(), TipoCarga: this.TipoCarga.val(), DataCarregamento: this.DataCarregamento.val() }, function (retorno) {
            if (retorno.Success) {
                if (Boolean(retorno.Data) && (retorno.Data.length > 0)) {
                    _capacidadeJanelaCarregamento.ListaCapacidadeCarregamentoPeriodo.removeAll();

                    for (var i = 0; i < retorno.Data.length; i++)
                        _capacidadeJanelaCarregamento.ListaCapacidadeCarregamentoPeriodo.push(new CapacidadeCarregamentoPeriodo(retorno.Data[i]));

                    var carouselCapacidadeCarregamento = document.querySelector('#carousel-capacidade-carregamento');
                    new bootstrap.Carousel(carouselCapacidadeCarregamento, { interval: false });

                    _capacidadeJanelaCarregamento.ExibirCapacidadeCarregamento.val(true);
                }
                else
                    _capacidadeJanelaCarregamento.ExibirCapacidadeCarregamento.val(false);
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                _capacidadeJanelaCarregamento.ExibirCapacidadeCarregamento.val(false);
            }

            if (callback instanceof Function)
                callback();
        });
    },
    _dataCarregamentoAnterior: function () {
        var dataCarregamento = moment(Global.criarData(this.DataCarregamento.val())).add(-1, 'days');

        this.DataCarregamento.val(dataCarregamento ? dataCarregamento.format('DD/MM/YYYY') : "");

        this._buscar();
    },
    _proximaDataCarregamento: function () {
        var dataCarregamento = moment(Global.criarData(this.DataCarregamento.val())).add(1, 'days');

        this.DataCarregamento.val(dataCarregamento ? dataCarregamento.format('DD/MM/YYYY') : "");

        this._buscar();
    }
};

/*
 * Declaração das Funções de Inicialização
 */

function loadCapacidadeJanelaCarregamento() {
    _capacidadeJanelaCarregamento = new CapacidadeJanelaCarregamento();
    KoBindings(_capacidadeJanelaCarregamento, "knockoutCapacidadeJanelaCarregamento");
}

/*
 * Declaração das Funções Públicas
 */

function buscarCapacidadeJanelaCarregamento(callback) {
    if (_capacidadeJanelaCarregamento)
        _capacidadeJanelaCarregamento.buscar(callback);
}

function limparCamposCapacidadeJanelaCarregamento() {
    if (_capacidadeJanelaCarregamento)
        _capacidadeJanelaCarregamento.limpar();
}
