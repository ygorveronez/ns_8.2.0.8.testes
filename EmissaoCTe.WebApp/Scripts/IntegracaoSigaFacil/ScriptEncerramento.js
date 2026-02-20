$(document).ready(function () {
    //$("#txtQuantidade").priceFormat({ centsLimit: 0, centsSeparator: '' });
    $("#txtEncerramentoValorFrete").priceFormat();
    $("#txtEncerramentoValorTarifa").priceFormat();
    $("#txtEncerramentoPesoTotal").priceFormat();
    $("#txtEncerramentoValorMercadoria").priceFormat();
    $("#txtEncerramentoValorMercadoriaPorKG").priceFormat();
    $("#txtEncerramentoPercentualTolerancia").priceFormat();
    $("#txtEncerramentoValorAdiantamento").priceFormat();
    $("#txtEncerramentoValorSeguro").priceFormat();
    $("#txtEncerramentoValorPedagio").priceFormat();
    $("#txtEncerramentoValorIRRF").priceFormat();
    $("#txtEncerramentoValorINSS").priceFormat();
    $("#txtEncerramentoValorSEST").priceFormat();
    $("#txtEncerramentoValorSENAT").priceFormat();
    $("#txtEncerramentoTotalOperacao").priceFormat();
    $("#txtEncerramentoTotalQuitacao").priceFormat();
    $("#txtEncerramentoValorAbastecimento").priceFormat();

    $("#btnEncerramentoCarregarPorCTe").click(function () {
        CalcularPorCTe();
        CalcularValorOpercao();
        CalcularValorQuitacao();
        CalcularValorLiquidoEBruto();
    });

    $("#btnEncerramentoCalcularImpostos").click(function () {
        CalcularImpostos();
        CalcularValorOpercao();
        CalcularValorQuitacao();
        CalcularValorLiquidoEBruto();
    });

    $([
        "#txtEncerramentoValorFrete",
        "#txtEncerramentoValorTarifa",
        "#txtEncerramentoValorAdiantamento",
        "#txtEncerramentoValorSeguro",
        "#txtEncerramentoValorPedagio",
        "#txtEncerramentoValorIRRF",
        "#txtEncerramentoValorINSS",
        "#txtEncerramentoValorSEST",
        "#txtEncerramentoValorSENAT",
    ].join(", ")).change(function () {
        CalcularValorOpercao();
        CalcularValorQuitacao();
        CalcularValorLiquidoEBruto();
    });

    LimparCamposEncerramento();
});

function CalculaOperacaoEQuitacao() {
    var camposSomar = ["ValorFrete", "ValorPedagio", "ValorSeguro"];
    var camposSubtrair = ["ValorINSS", "ValorIRRF", "ValorSENAT", "ValorSEST"];
    var totalSoma = 0;
    var totalSubtracao = 0;

    for (var i in camposSomar) {
        totalSoma += Globalize.parseFloat($("#txtEncerramento" + camposSomar[i]).val());
    }

    for (var i in camposSubtrair) {
        totalSubtracao += Globalize.parseFloat($("#txtEncerramento" + camposSubtrair[i]).val());
    }

    return totalSoma - totalSubtracao;
}
function CalcularValorOpercao() {
    total = CalculaOperacaoEQuitacao();

    $("#txtEncerramentoTotalOperacao").val(Globalize.format(total, "n2"));
}
function CalcularValorQuitacao() {
    total = CalculaOperacaoEQuitacao();

    // Quitacao e o valor total da operacao menos o adiantamento
    total = total - Globalize.parseFloat($("#txtEncerramentoValorAdiantamento").val());

    $("#txtEncerramentoTotalQuitacao").val(Globalize.format(total, "n2"));
}

function CalcularValorLiquidoEBruto() {
    CalcularValorBruto();
    CalcularValorLiquido();
}

function CalcularValorBruto() {
    var camposSomar = ["ValorFrete", /*"ValorAdiantamento",*/ "ValorINSS", "ValorIRRF", "ValorSENAT", "ValorSEST"];
    var totalSoma = 0;

    for (var i in camposSomar) {
        totalSoma += Globalize.parseFloat($("#txtEncerramento" + camposSomar[i]).val());
    }

    $("#txtEncerramentoValorBruto").val(Globalize.format(totalSoma, "n2"));
}

function CalcularValorLiquido() {
    var valorFrete = Globalize.parseFloat($("#txtEncerramentoValorFrete").val());
    var camposSomar = ["ValorAdiantamento", "ValorINSS", "ValorIRRF", "ValorSENAT", "ValorSEST"];
    var totalSoma = 0;

    for (var i in camposSomar) 
        totalSoma += Globalize.parseFloat($("#txtEncerramento" + camposSomar[i]).val());

    var valorLiquido = valorFrete - totalSoma;

    $("#txtEncerramentoValorLiquido").val(Globalize.format(valorLiquido, "n2"));
}

function CalcularPorCTe() {
    var documento = $("body").data("documentos");

    var somatorio = {
        ValorFrete: 0,
        ValorTarifaFrete: 0,
        PesoBruto: 0,
        ValorTotalMercadoria: 0,
        ValorMercadoriaKG: 0,
    };

    // Em cada documento
    for (var doc in documento)
        // Em cada atributo
        for (som in somatorio)
            // Incremente o referido atributo
            somatorio[som] += documento[doc][som];

    // Em cada atributo
    for (som in somatorio)
        // Formate os valores
        somatorio[som] = Globalize.format(somatorio[som], "n2");

    var evt = "keyup";
    // Colocar o valor e engatilha o evento para formatar corretamente
    $("#txtEncerramentoValorFrete").val(somatorio.ValorFrete).trigger(evt);
    $("#txtEncerramentoValorTarifa").val(somatorio.ValorTarifaFrete).trigger(evt);
    $("#txtEncerramentoPesoTotal").val(somatorio.PesoBruto).trigger(evt);
    $("#txtEncerramentoValorMercadoria").val(somatorio.ValorTotalMercadoria).trigger(evt);
    $("#txtEncerramentoValorMercadoriaPorKG").val(somatorio.ValorMercadoriaKG).trigger(evt);
}
function LimparCamposEncerramento() {
    $("#txtEncerramentoValorFrete").val("0,00");
    $("#txtEncerramentoValorTarifa").val("0,00");
    $("#txtEncerramentoPesoTotal").val("0,00");
    $("#txtEncerramentoValorMercadoria").val("0,00");
    $("#txtEncerramentoValorMercadoriaPorKG").val("0,00");
    $("#txtEncerramentoValorAdiantamento").val("0,00");
    $("#txtEncerramentoValorSeguro").val("0,00");
    $("#txtEncerramentoValorPedagio").val("0,00");
    $("#txtEncerramentoPercentualTolerancia").val("0,00");
    $("#txtEncerramentoValorIRRF").val("0,00");
    $("#txtEncerramentoValorINSS").val("0,00");
    $("#txtEncerramentoValorSEST").val("0,00");
    $("#txtEncerramentoValorSENAT").val("0,00");
    $("#txtEncerramentoTotalOperacao").val("0,00");
    $("#txtEncerramentoTotalQuitacao").val("0,00");
    $("#txtEncerramentoValorAbastecimento").val("0,00");
    $("#selEncerramentoTipoQuebra").val("3");
    $("#selEncerramentoTipoTolerancia").val("1");
}
