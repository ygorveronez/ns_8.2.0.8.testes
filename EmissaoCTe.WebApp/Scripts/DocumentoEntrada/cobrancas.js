$(document).ready(function () {
    $("#txtQuantidadeParcelasDuplicata").mask("9?9");
    $("#txtDiasDuplicata").mask("9?9");

    $("#txtDataVencimento").datepicker();
    $("#txtDataVencimento").mask("99/99/9999");

    $("#txtDataPrimeiroVencimentoDuplicata").datepicker();
    $("#txtDataPrimeiroVencimentoDuplicata").mask("99/99/9999");

    $("#txtValorDuplicata").priceFormat();

    $("#btnSalvarDuplicata").click(function () {
        SalvarCobranca();
    });

    $("#btnExcluirDuplicata").click(function () {
        ExcluirCobranca();
    });

    $("#btnCancelarDuplicata").click(function () {
        LimparCamposCobranca();
    });

    $("#btnGerarDuplicatasAutomaticamente").click(function () {
        GerarDuplicatasAutomaticamente();
    });

    LimparCamposCobranca();
});

function GerarDuplicatasAutomaticamente() {
    if ($("body").data("cobrancas") != null && $("body").data("cobrancas").length > 0) {
        var duplicatas = $("body").data("cobrancas") == null ? new Array() : $("body").data("cobrancas");
        for (var i = 0; i <= duplicatas.length -1; i++) {
            if (duplicatas[i].Excluir == false) {
                ExibirMensagemAlerta("Delete as cobranças existentes para gerar automaticamente as parcelas.", "Atenção!");
                return;
            }
        }
    }

    var duplicatas = $("body").data("cobrancas") == null ? new Array() : $("body").data("cobrancas");

    var quantidadeDuplicatas = Globalize.parseInt($("#txtQuantidadeParcelasDuplicata").val());
    var frequenciaVencimentos = Globalize.parseInt($("#txtDiasDuplicata").val());
    var dataPrimeiroVencimento = Globalize.parseDate($("#txtDataPrimeiroVencimentoDuplicata").val(), "dd/MM/yyyy");
    var arredondarPrimeiraParcela = $("#selArredondamentoDuplicata").val() == "0" ? true : false;
    var valorTotal = Globalize.parseFloat($("#txtValorTotal").val());
    var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / quantidadeDuplicatas, "n2"));
    var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * quantidadeDuplicatas), "n2"));

    if (isNaN(quantidadeDuplicatas) || quantidadeDuplicatas <= 0)
        quantidadeDuplicatas = 1;

    if (isNaN(frequenciaVencimentos) || frequenciaVencimentos <= 0)
        frequenciaVencimentos = 30;

    if (dataPrimeiroVencimento == null)
        dataPrimeiroVencimento = new Date();    

    if (valorTotal > 0) {
        for (var i = 1; i <= quantidadeDuplicatas; i++) {
            if (i != 1)
                dataPrimeiroVencimento.setDate(dataPrimeiroVencimento.getDate() + frequenciaVencimentos);

            var duplicata = {
                Codigo: -i,
                DataVencimento: Globalize.format(dataPrimeiroVencimento, "dd/MM/yyyy"),
                Valor: valorParcela,
                Numero: $("#txtNumero").val() + "/" + i,
                Excluir: false
            };

            duplicatas.push(duplicata);
        }

        if (arredondarPrimeiraParcela) {
            var i = 0;
            while (duplicatas[i].Excluir == true)
                i += 1;
            duplicatas[i].Valor += valorDiferenca;
        }
        else
            duplicatas[duplicatas.length - 1].Valor += valorDiferenca;

        $("body").data("cobrancas", duplicatas);

        RenderizarCobrancas();
    }
}

function LimparCamposCobranca() {
    $("body").data("cobranca", null);

    $("#txtNumeroDuplicata").val("");
    $("#txtValorDuplicata").val("0,00");
    $("#txtDataVencimento").val("");

    $("#txtQuantidadeParcelasDuplicata").val("");
    $("#txtDiasDuplicata").val("");
    $("#txtDataPrimeiroVencimentoDuplicata").val("");
    $("#selArredondamentoDuplicata").val("0");

    $("#btnExcluirDuplicata").hide();
}

function ValidarCamposCobranca() {
    var valor = Globalize.parseFloat($("#txtValorDuplicata").val());
    var numero = $.trim($("#txtNumeroDuplicata").val());
    var dataVencimento = $("#txtDataVencimento").val();
    var valido = true;

    if (isNaN(valor) || valor <= 0) {
        CampoComErro("#txtValorDuplicata");
        valido = false;
    } else {
        CampoSemErro("#txtValorDuplicata");
    }

    if (numero == null || numero.length == 0) {
        CampoComErro("#txtNumeroDuplicata");
        valido = false;
    } else {
        CampoSemErro("#txtNumeroDuplicata");
    }

    if (dataVencimento == null || dataVencimento.length != 10) {
        CampoComErro("#txtDataVencimento");
        valido = false;
    } else {
        CampoSemErro("#txtDataVencimento");
    }

    return valido;
}

function SalvarCobranca() {
    if (ValidarCamposCobranca()) {
        var cobranca = {
            Codigo: $("body").data("cobranca") != null ? $("body").data("cobranca").Codigo : 0,
            Numero: $("#txtNumeroDuplicata").val(),
            Valor: Globalize.parseFloat($("#txtValorDuplicata").val()),
            DataVencimento: $("#txtDataVencimento").val(),
            Excluir: false
        };

        var cobrancas = $("body").data("cobrancas") == null ? new Array() : $("body").data("cobrancas");

        cobrancas.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (cobranca.Codigo == 0)
            cobranca.Codigo = (cobrancas.length > 0 ? (cobrancas[0].Codigo > 0 ? -1 : (cobrancas[0].Codigo - 1)) : -1);

        for (var i = 0; i < cobrancas.length; i++) {
            if (cobrancas[i].Codigo == cobranca.Codigo) {
                cobrancas.splice(i, 1);
                break;
            }
        }

        cobrancas.push(cobranca);

        cobrancas.sort(function (a, b) { return a.Numero < b.Numero ? -1 : 1; });

        $("body").data("cobrancas", cobrancas);

        RenderizarCobrancas();
        LimparCamposCobranca();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-mensagem-cobrancas");
    }
}

function RenderizarCobrancas() {
    var cobrancas = $("body").data("cobrancas") == null ? new Array() : $("body").data("cobrancas");

    $("#tblCobrancas tbody").html("");

    for (var i = 0; i < cobrancas.length; i++) {
        if (!cobrancas[i].Excluir)
            $("#tblCobrancas tbody").append("<tr><td>" + cobrancas[i].Numero + "</td><td>" + Globalize.format(cobrancas[i].Valor, "n2") + "</td><td>" + cobrancas[i].DataVencimento + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarCobranca(" + JSON.stringify(cobrancas[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblCobrancas tbody").html() == "")
        $("#tblCobrancas tbody").html("<tr><td colspan='4'>Nenhum registro encontrado.</td></tr>");
}

function EditarCobranca(cobranca) {
    $("body").data("cobranca", cobranca);

    $("#txtNumeroDuplicata").val(cobranca.Numero);
    $("#txtValorDuplicata").val(Globalize.format(cobranca.Valor, "n2"));
    $("#txtDataVencimento").val(cobranca.DataVencimento);

    $("#btnExcluirDuplicata").show();
}

function ExcluirCobranca() {
    var cobranca = $("body").data("cobranca");

    var cobrancas = $("body").data("cobrancas") == null ? new Array() : $("body").data("cobrancas");

    for (var i = 0; i < cobrancas.length; i++) {
        if (cobrancas[i].Codigo == cobranca.Codigo) {
            if (cobranca.Codigo <= 0)
                cobrancas.splice(i, 1);
            else
                cobrancas[i].Excluir = true;
            break;
        }
    }

    $("body").data("cobrancas", cobrancas);

    RenderizarCobrancas();
    LimparCamposCobranca();
}