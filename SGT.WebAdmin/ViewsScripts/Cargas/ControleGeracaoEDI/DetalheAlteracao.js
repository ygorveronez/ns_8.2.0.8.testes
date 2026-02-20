/*
 * Declaração de Objetos Globais do Arquivo
 */

var _detalheAlteracao;

/*
 * Declaração das Classes
 */

var DetalheAlteracao = function () {
    this.IDOC = PropertyEntity({ text: Localization.Resources.Cargas.ControleGeracaoEDI.IDOC.getFieldDescription() });
    this.DetalhesCarga = ko.observableArray();
}

var DetalheAlteracaoCarga = function (detalheCarga) {
    this.Carga = PropertyEntity({ val: detalheCarga.Carga, text: Localization.Resources.Cargas.ControleGeracaoEDI.Carga.getFieldDescription() });
    this.MeioTransporteAnterior = PropertyEntity({ val: detalheCarga.MeioTransporteAnterior });
    this.MeioTransporteAtual = PropertyEntity({ val: detalheCarga.MeioTransporteAtual });
    this.ModeloVeicularAnterior = PropertyEntity({ val: detalheCarga.ModeloVeicularAnterior });
    this.ModeloVeicularAtual = PropertyEntity({ val: detalheCarga.ModeloVeicularAtual });
    this.PlacaAnterior = PropertyEntity({ val: detalheCarga.PlacaAnterior });
    this.PlacaAtual = PropertyEntity({ val: detalheCarga.PlacaAtual });
    this.QuantidadeNfsAnterior = PropertyEntity({ val: detalheCarga.QuantidadeNfsAnterior });
    this.QuantidadeNfsAtual = PropertyEntity({ val: detalheCarga.QuantidadeNfsAtual });
    this.RoteiroAnterior = PropertyEntity({ val: detalheCarga.RoteiroAnterior });
    this.RoteiroAtual = PropertyEntity({ val: detalheCarga.RoteiroAtual });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDetalheAlteracao() {
    _detalheAlteracao = new DetalheAlteracao();
    KoBindings(_detalheAlteracao,"knockoutDetalheAlteracao");
}

/*
 * Declaração das Funções Privadas
 */

function carregarDetalhesCarga(detalhesCarga) {
    for (var i = 0; i < detalhesCarga.length; i++) {
        _detalheAlteracao.DetalhesCarga.push(new DetalheAlteracaoCarga(detalhesCarga[i]));
    }
}

function configurarSlideDetalhesCarga() {
    $("#detalhesControleGeracaoEDISlide").carousel("pause").removeData();
    $("#detalhesControleGeracaoEDISlide").carousel({ interval: false });
    $("#detalhesControleGeracaoEDISlide").carousel(0);
}

/*
 * Declaração das Funções Públicas
 */

function limparDetalheAlteracao() {
    LimparCampos(_detalheAlteracao);

    _detalheAlteracao.DetalhesCarga.removeAll();
}

function preencherDetalheAlteracao(detalheAlteracao) {
    PreencherObjetoKnout(_detalheAlteracao, { Data: detalheAlteracao });
    carregarDetalhesCarga(detalheAlteracao.DetalhesCarga);
    configurarSlideDetalhesCarga(); 
}