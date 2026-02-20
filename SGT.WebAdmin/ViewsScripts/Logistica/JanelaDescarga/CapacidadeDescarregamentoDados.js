/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _capacidadeJanelaDescarregamento;

/*
 * Declaração das Classes
 */

var CapacidadeDescarregamentoPeriodo = function (dados) {
    this.CapacidadeAdicional = PropertyEntity({});
    this.CapacidadeDescarregamento = PropertyEntity({});
    this.CapacidadeDisponivel = PropertyEntity({});
    this.CapacidadeUtilizada = PropertyEntity({});
    this.Periodo = PropertyEntity({});
    this.PeriodoDescricao = PropertyEntity({});
    this.PeriodoAtivo = PropertyEntity({ getType: typesKnockout.bool });

    PreencherObjetoKnout(this, { Data: dados });
}

var CapacidadeJanelaDescarregamento = function () {
    this.ExibirCapacidadeDescarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ListaCapacidadeDescarregamentoPeriodo = ko.observableArray();
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCapacidadeDescarregamentoDados() {
    _capacidadeJanelaDescarregamento = new CapacidadeJanelaDescarregamento();
    KoBindings(_capacidadeJanelaDescarregamento, "knockoutCapacidadeJanelaDescarregamento");
}

/*
 * Declaração das Funções Públicas
 */

function buscarCapacidadeDescarregamentoDados() {
    executarReST("JanelaDescarga/ObterCapacidadeDescarregamentoDados", { CentroDescarregamento: _dadosPesquisaDescarregamento.CentroDescarregamento, DataDescarregamento: _tabelaDescarregamento.ObterData() }, function (retorno) {
        if (retorno.Success) {
            if (Boolean(retorno.Data) && (retorno.Data.length > 0)) {
                _capacidadeJanelaDescarregamento.ListaCapacidadeDescarregamentoPeriodo.removeAll();

                for (var i = 0; i < retorno.Data.length; i++)
                    _capacidadeJanelaDescarregamento.ListaCapacidadeDescarregamentoPeriodo.push(new CapacidadeDescarregamentoPeriodo(retorno.Data[i]));

                var carouselCapacidadeDescarregamento = document.querySelector('#carousel-capacidade-descarregamento');
                new bootstrap.Carousel(carouselCapacidadeDescarregamento, { interval: false });

                _capacidadeJanelaDescarregamento.ExibirCapacidadeDescarregamento.val(true);
            }
            else
                _capacidadeJanelaDescarregamento.ExibirCapacidadeDescarregamento.val(false);
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            _capacidadeJanelaDescarregamento.ExibirCapacidadeDescarregamento.val(false);
        }
    });
}
