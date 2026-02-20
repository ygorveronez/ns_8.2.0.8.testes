
var ResumoPeriodo = function (carregarDadosAssinatura = false) {
    this.DescricaoTransportador = PropertyEntity({ type: types.map, text: "Transportador: " });
    this.NumeroCarta = PropertyEntity({ type: types.map, text: "Número Carta: " });
    this.Periodo = PropertyEntity({ type: types.map, text: "Período: " });
    this.Periodicidade = PropertyEntity({ type: types.map, text: "Periodicidade: " });
    this.DataAnuenciaDisponivel = PropertyEntity({ type: types.map, text: "Anuência disponível em: " });
    this.ValorTotalDevido = PropertyEntity({ type: types.map, text: "Valor dos Documentos: " })
    this.Acrescimos = PropertyEntity({ type: types.map, text: "Acréscimos: " });
    this.Decrescimos = PropertyEntity({ type: types.map, text: "Decréscimos: " });
    this.SaldoTotal = PropertyEntity({ type: types.map, text: "Valor Devido: " });
    this.ValorPago = PropertyEntity({ type: types.map, text: "Valor pago: " });
    this.ValorEmAberto = PropertyEntity({ type: types.map, text: "Saldo a Pagar: " });
    this.NumeroDocumentos = PropertyEntity({ type: types.map, text: "Número de documentos: " });
    this.NumeroDocumentosPendentes = PropertyEntity({ type: types.map, text: "Número de documentos pendentes: " });

    // Dados da lateral
    this.CarregarDadosAssinatura = PropertyEntity({ type: types.map, val: ko.observable(carregarDadosAssinatura) });
    this.DataELocal = PropertyEntity({ type: types.map, text: "DataELocal: " });
    this.NomeTransportador = PropertyEntity({ type: types.map, text: "NomeTransportador: " });
    this.CnpjTransportador = PropertyEntity({ type: types.map, text: "CnpjTransportador: " });
};

async function CarregarResumoPeriodo(codigoConciliacaoTransportador, idHtml, carregarDadosAssinatura = false) {
    var dadosResumoPeriodo = await obterResumoPeriodo(codigoConciliacaoTransportador);

    $.get('Content/Static/Financeiro/ConciliacaoTransportador/ResumoPeriodo.html?dyn=' + guid(), function (html) {
        $("#" + idHtml).html(html);

        var resumoPeriodo = new ResumoPeriodo(carregarDadosAssinatura);
        KoBindings(resumoPeriodo, idHtml);
        PreencherObjetoKnout(resumoPeriodo, dadosResumoPeriodo);
    });
}

function obterResumoPeriodo(codigoConciliacaoTransportador) {
    return new Promise(resolve => {
        executarReST("ConciliacaoTransportador/ObterResumoPeriodo", { Codigo: codigoConciliacaoTransportador}, function (arg) {
            if (arg.Success) {
                resolve(arg);
            } else {
                resolve(null);
            }
        });
    });
}