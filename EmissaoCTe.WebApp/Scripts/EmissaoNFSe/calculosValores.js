$(document).ready(function () {

    $("#txtValorServicoItem").focusout(function () {
        CalcularValores();
    });

    $("#txtQuantidadeItem").focusout(function () {
        CalcularValores();
    });

    $("#txtValorDescontoIncondicionadoItem").focusout(function () {
        CalcularValores();
    });

    $("#txtValorDeducoesItem").focusout(function () {
        CalcularValores();
    });

    $("#txtValorTotalItem").focusout(function () {
        $("#txtBaseCalculoISSItem").val(Globalize.format($("#txtValorTotalItem").val(), "n2"));
        CalculaValoresISS();
    });

    $("#txtAliquotaISSItem").focusout(function () {
        CalcularValores();
    });

    $("#selISSRetido").change(function () {
        CalcularISSRetido();
    });

    $("#chkIncluirISSNoFrete").click(function () {
        if (this.checked) {
            CalcularISSIncluso();
        } else {
            CalcularValores();
        }
    });
});

function CalcularValores() {
    var iss = Globalize.parseFloat($("#txtAliquotaISSItem").val() != null ? $("#txtAliquotaISSItem").val() : "0");
    var valorServico = Globalize.parseFloat($("#txtValorServicoItem").val() != null ? $("#txtValorServicoItem").val() : "0");
    var quantidade = Globalize.parseFloat($("#txtQuantidadeItem").val() != null ? $("#txtQuantidadeItem").val() : "0");
    var descontoCondicionado = Globalize.parseFloat($("#txtValorDescontoCondicionadoItem").val() != null ? $("#txtValorDescontoCondicionadoItem").val() : "0");
    var descontoIncondicionado = Globalize.parseFloat($("#txtValorDescontoIncondicionadoItem").val() != null ? $("#txtValorDescontoIncondicionadoItem").val() : "0");
    var valorDeducoes = Globalize.parseFloat($("#txtValorDeducoesItem").val() != null ? $("#txtValorDeducoesItem").val() : "0");
    var valorTotal = (valorServico * quantidade) - descontoIncondicionado - valorDeducoes;

    $("#txtValorTotalItem").val(Globalize.format(valorTotal, "n2"));
    $("#txtBaseCalculoISSItem").val(Globalize.format(valorTotal, "n2"));

    CalculaValoresISS();
}

function CalculaValoresISS() {
    var iss = Globalize.parseFloat($("#txtAliquotaISSItem").val() != null ? $("#txtAliquotaISSItem").val() : "0");  
    var baseISS = Globalize.parseFloat($("#txtBaseCalculoISSItem").val() != null ? $("#txtBaseCalculoISSItem").val() : "0");
    var valorISS = 0;
    
    if (iss > 0 && baseISS > 0)
        valorISS = baseISS * (iss / 100);

    $("#txtBaseCalculoISSItem").val(Globalize.format(baseISS, "n2"));
    $("#txtValorISSItem").val(Globalize.format(valorISS, "n2"));

    if ($("#chkIncluirISSNoFrete")[0].checked && iss > 0)
        $("#txtValorTotalItem").val(Globalize.format(baseISS, "n2"));

    CalcularTotais();

    CalcularISSRetido();

    CalcularISSIncluso();
}

function CalcularISSIncluso() {
    if ($("#chkIncluirISSNoFrete")[0].checked) {
        var iss = Globalize.parseFloat($("#txtAliquotaISSItem").val() != null ? $("#txtAliquotaISSItem").val() : "0");
        var baseISS = Globalize.parseFloat($("#txtBaseCalculoISSItem").val() != null ? $("#txtBaseCalculoISSItem").val() : "0");
        var valorISS = 0;

        if ($("#chkIncluirISSNoFrete")[0].checked)
            baseISS += (iss > 0 ? ((baseISS / ((100 - iss) / 100)) - baseISS) : 0);

        if (iss > 0 && baseISS > 0)
            valorISS = baseISS * (iss / 100);

        $("#txtBaseCalculoISSItem").val(Globalize.format(baseISS, "n2"));
        $("#txtValorISSItem").val(Globalize.format(valorISS, "n2"));

        if ($("#chkIncluirISSNoFrete")[0].checked && iss > 0)
            $("#txtValorTotalItem").val(Globalize.format(baseISS, "n2"));

        CalcularTotais();

        CalcularISSRetido();
    }
}

function CalcularTotais() {
    $("#txtValorServicos").val($("#txtValorTotalItem").val());

    $("#txtValorDeducoes").val(Globalize.format($("#txtValorDeducoesItem").val(), "n2"));
    $("#txtValorDescontoCondicionado").val(Globalize.format($("#txtValorDescontoCondicionadoItem").val(), "n2"));
    $("#txtValorDescontoIncondicionado").val(Globalize.format($("#txtValorDescontoIncondicionadoItem").val(), "n2"));

    $("#txtAliquotaISS").val($("#txtAliquotaISSItem").val());
    $("#txtBaseCalculoISS").val($("#txtBaseCalculoISSItem").val());
    $("#txtValorISS").val($("#txtValorISSItem").val());
}

function CalcularISSRetido() {
    var issRetido = $("#selISSRetido").val();
    var valorISSRetido = 0;
    if (issRetido == "true")
        valorISSRetido = Globalize.parseFloat($("#txtValorISS").val() != null ? $("#txtValorISS").val() : "0");

    $("#txtValorISSRetido").val(Globalize.format(valorISSRetido, "n2"));
}