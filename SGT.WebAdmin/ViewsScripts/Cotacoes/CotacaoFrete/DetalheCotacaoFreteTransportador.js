/// <reference path="Transportador.js" />

var _detalheCotacaoFreteTransportador = null;

var DetalheCotacaoFreteTransportadorModel = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Transportador = PropertyEntity({ text: "Transportador: " });
    this.DataColetaPrevista = PropertyEntity({ text: "Data Coleta Prevista: " });
    this.DataPrazoEntrega = PropertyEntity({ text: "Data Prazo Entrega: " });
    this.PrazoEntrega = PropertyEntity({ text: "Prazo de Entrega: " });

    this.ValorFrete = PropertyEntity({ text: "Valor do Frete: " });
    this.ValorFreteTotal = PropertyEntity({ text: "Valor do Frete Total: " });

    this.BaseCalculo = PropertyEntity({ text: "Base de Cálculo: " });
    this.Aliquota = PropertyEntity({ text: "Alíquota: " });
    this.ValorICMS = PropertyEntity({ text: "Valor do ICMS: " });
    this.ValorISS = PropertyEntity({ text: "Valor do ISS: " });
    this.ValorISSRetido = PropertyEntity({ text: "Valor do ISS Retido: " });
    this.ValorTotalCotacao = PropertyEntity({ text: "Valor Total da Cotação: " });
    this.DistanciaRaioKM = PropertyEntity({ text: "Distância Raio KM: " });
    this.CodigoIntegracaoTabelaFreteCliente = PropertyEntity({ text: "Código da Tabela de Frete: " });

    this.Componentes = ko.observableArray([]);
};

function LoadDetalheCotacaoFreteTransportador() {
    _detalheCotacaoFreteTransportador = new DetalheCotacaoFreteTransportadorModel();

    KoBindings(_detalheCotacaoFreteTransportador, "knockoutDetalheCotacaoFreteTransportador");
}

function DetalhesCotacaoFreteTransportadorClick(cotacaoTransportador) {

    let dadosCotacaoTransportador;
    for (let i = 0; i < _transportador.Transportadores.list.length; i++) {
        if (_transportador.Transportadores.list[i].Transportador.CNPJ == cotacaoTransportador.Codigo) {
            dadosCotacaoTransportador = _transportador.Transportadores.list[i];
            break;
        }
    }

    if (dadosCotacaoTransportador == null) {
        exibirMensagem(tipoMensagem.aviso, "Atenção!", "Detalhes não encontrados.");
        return;
    }

    _detalheCotacaoFreteTransportador.Codigo.val(cotacaoTransportador.Codigo);
    _detalheCotacaoFreteTransportador.Transportador.val(dadosCotacaoTransportador.Transportador.Descricao);
    _detalheCotacaoFreteTransportador.DataColetaPrevista.val(dadosCotacaoTransportador.DataPrevisaoColeta);
    _detalheCotacaoFreteTransportador.DataPrazoEntrega.val(dadosCotacaoTransportador.DataPrazoEntrega);
    _detalheCotacaoFreteTransportador.PrazoEntrega.val(dadosCotacaoTransportador.PrazoEntrega);
    _detalheCotacaoFreteTransportador.ValorFreteTotal.val(dadosCotacaoTransportador.ValorFrete);
    _detalheCotacaoFreteTransportador.BaseCalculo.val(Globalize.format(dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.BaseCalculo, "n2"));
    _detalheCotacaoFreteTransportador.Aliquota.val(Globalize.format(dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.Aliquota, "n2"));
    _detalheCotacaoFreteTransportador.ValorICMS.val(Globalize.format(dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.ValorICMS, "n2"));
    _detalheCotacaoFreteTransportador.ValorISS.val(Globalize.format(dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.ValorISS, "n2"));
    _detalheCotacaoFreteTransportador.ValorISSRetido.val(Globalize.format(dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.ValorISSRetido, "n2"));
    _detalheCotacaoFreteTransportador.ValorTotalCotacao.val(dadosCotacaoTransportador.ValorCotacao);
    _detalheCotacaoFreteTransportador.DistanciaRaioKM.val(dadosCotacaoTransportador.DistanciaRaioKM);
    _detalheCotacaoFreteTransportador.CodigoIntegracaoTabelaFreteCliente.val(dadosCotacaoTransportador.RetornoCompleto.CodigoIntegracaoTabelaFreteCliente);

    _detalheCotacaoFreteTransportador.Componentes.removeAll();

    let valorTotalComponentes = 0;

    for (let i = 0; i < dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.Componentes.length; i++) {
        let componente = dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.Componentes[i];
        console.log(componente);
        _detalheCotacaoFreteTransportador.Componentes.push({ Descricao: componente.Descricao + ": ", Valor: Globalize.format(componente.Valor, "n2") });
        if (componente.IncluirBaseCalculoICMS)
            valorTotalComponentes += componente.Valor;
    }

    let impostos = dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.ValorICMS - dadosCotacaoTransportador.RetornoCompleto.ValorCotacao.ValorISS;

    let numeroString = dadosCotacaoTransportador.ValorFrete;
    let numeroSemPontoMilhar = numeroString.replace(/\./g, '');
    let numeroFormatado = numeroSemPontoMilhar.replace(',', '.');
    let numeroFinal = parseFloat(numeroFormatado);

    _detalheCotacaoFreteTransportador.ValorFrete.val(Globalize.format((numeroFinal - valorTotalComponentes - impostos), "n2"));

    AbrirModalDetalhesCotacaoFreteTransportador();
}

function AbrirModalDetalhesCotacaoFreteTransportador() {    
    Global.abrirModal('knockoutDetalheCotacaoFreteTransportador');
}