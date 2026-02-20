/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _capacidadeJanelaCarregamento;
var _capacidadeDocasJanelaCarregamento;

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

    // Produtividade
    this.PossuiProdutividade = PropertyEntity({ getType: typesKnockout.bool });
    this.HorasProdutividade = PropertyEntity({});
    this.ProdutividadeUtilizada = PropertyEntity({});

    PreencherObjetoKnout(this, { Data: dados });
}

var CapacidadeJanelaCarregamento = function () {
    this.ExibirCapacidadeCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ListaCapacidadeCarregamentoPeriodo = ko.observableArray();

    this.TipoVisualizacaoCapacidadeJanela = PropertyEntity({ text: ko.observable("Tipo Visualização:"), val: ko.observable(1), def: 1, options: EnumTipoVisualizacaoCapacidadeJanela.obterOpcoes(), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoVisualizacaoCapacidadeJanela.val.subscribe(buscarCapacidadeCarregamentoPorPeriodo);
}

var CapacidadeDocasJanelaCarregamento = function () {
    this.ExibirCapacidadeCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PercentualAgendado = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PorcentagemAgendado.getFieldDescription() });
    this.ToneladasAlocadas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ToneladasAlocadas.getFieldDescription() });
    this.VeiculosInformados = PropertyEntity({ text: Localization.Resources.Cargas.Carga.VeiculosInformados.getFieldDescription() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCapacidadeCarregamentoDados() {
    _capacidadeJanelaCarregamento = new CapacidadeJanelaCarregamento();
    KoBindings(_capacidadeJanelaCarregamento, "knockoutCapacidadeJanelaCarregamento");

    _capacidadeDocasJanelaCarregamento = new CapacidadeDocasJanelaCarregamento();
    KoBindings(_capacidadeDocasJanelaCarregamento, "knockoutCapacidadeDocasJanelaCarregamento");
}

/*
 * Declaração das Funções Públicas
 */

function buscarCapacidadeCarregamentoPorDocas() {
    var dados = {
        CentroCarregamento: _centroCarregamentoAtual.Codigo,
        DataCarregamento: _listaCarregamento.ObterData(),
        Transportador: _dadosPesquisaCarregamento.Transportador,
        TipoOperacao: _dadosPesquisaCarregamento.TipoOperacao
    };

    executarReST("JanelaCarregamento/ObterCapacidadeCarregamentoDocas", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data === false) {
                _capacidadeDocasJanelaCarregamento.ExibirCapacidadeCarregamento.val(false);
                return;
            }

            _capacidadeDocasJanelaCarregamento.ExibirCapacidadeCarregamento.val(true);
            PreencherObjetoKnout(_capacidadeDocasJanelaCarregamento, retorno);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            _capacidadeDocasJanelaCarregamento.ExibirCapacidadeCarregamento.val(false);
        }
    });
}

function buscarCapacidadeCarregamentoPorPeriodo() {
    executarReST("JanelaCarregamento/ObterCapacidadeCarregamentoDados", { CentroCarregamento: _centroCarregamentoAtual.Codigo, DataCarregamento: _listaCarregamento.ObterData(), TipoVisualizacaoCapacidadeJanela: _capacidadeJanelaCarregamento.TipoVisualizacaoCapacidadeJanela.val() }, function (retorno) {
        if (retorno.Success) {
            if (Boolean(retorno.Data) && (retorno.Data.length > 0)) {
                _capacidadeJanelaCarregamento.ListaCapacidadeCarregamentoPeriodo.removeAll();

                for (var i = 0; i < retorno.Data.length; i++)
                    _capacidadeJanelaCarregamento.ListaCapacidadeCarregamentoPeriodo.push(new CapacidadeCarregamentoPeriodo(retorno.Data[i]));

                var carouselCapacidadeCarregamento = document.querySelector('#carousel-capacidade-carregamento');
                new bootstrap.Carousel(carouselCapacidadeCarregamento, { interval: false });

                _capacidadeJanelaCarregamento.ExibirCapacidadeCarregamento.val(true);

                if (_centroCarregamentoAtual.Configuracao.ExibirComboVolumeCubagem)
                    _capacidadeJanelaCarregamento.TipoVisualizacaoCapacidadeJanela.visible(true);
                else
                    _capacidadeJanelaCarregamento.TipoVisualizacaoCapacidadeJanela.visible(false);
            }
            else
                _capacidadeJanelaCarregamento.ExibirCapacidadeCarregamento.val(false);
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            _capacidadeJanelaCarregamento.ExibirCapacidadeCarregamento.val(false);
        }
    });
}

function buscarCapacidadeCarregamentoDados() {
    buscarCapacidadeCarregamentoPorPeriodo();
    if (_centroCarregamentoAtual.LimiteCarregamentos == EnumLimiteCarregamentosCentroCarregamento.QuantidadeDocas)
        buscarCapacidadeCarregamentoPorDocas();
}
