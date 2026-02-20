$(document).ready(function () {

    $("#txtParcelas").mask("9?9");
    $("#txtIntervaloDias").mask("9?9");
    FormatarCampoDate("txtDataVcto");
    FormatarCampoDate("txtDataVctoParcela");
    
    $("#btnGerarParcelas").click(function () {
        GerarParcelas();
    });
        
    $("#btnSalvarParcela").click(function () {
        SalvarParcela();
    });

    LimparCamposParcelas();
});

function LimparCamposParcelas() {
    $("body").data("parcelas", null);

    $("#txtParcelas").val('');
    $("#txtIntervaloDias").val('');
    $("#txtDataVcto").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#selArredondar").val($("#selArredondar option:first").val()).change();

    $("body").data("parcelas", null);
}

function GerarParcelas() {
    if (ValidarCamposParcelas() && !VerificaParcelaPaga()) {
        if ($("body").data("parcelas") != null && $("body").data("parcelas").length > 0)
            $("body").data("parcelas", null);

        var parcelas = $("body").data("parcelas") == null ? new Array() : $("body").data("parcelas");

        var qtdparcelas = Globalize.parseInt($("#txtParcelas").val());
        var intervaloDias = Globalize.parseInt($("#txtIntervaloDias").val());
        var dataPrimeiroVencimento = Globalize.parseDate($("#txtDataVcto").val(), "dd/MM/yyyy");
        var valor = Globalize.parseFloat($("#txtValor").val());
        var valorascrescimo = Globalize.parseFloat($("#txtAcrescimo").val());
        var valordesconto = Globalize.parseFloat($("#txtDesconto").val());
        var valorTotal = valor + valorascrescimo - valordesconto;
        var arredondarPrimeiraParcela = $("#selArredondar").val() == "0" ? true : false;

        if (isNaN(qtdparcelas) || qtdparcelas <= 0)
            qtdparcelas = 1;

        if (dataPrimeiroVencimento == null)
            dataPrimeiroVencimento = new Date();

        var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / qtdparcelas, "n2"));
        var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * qtdparcelas), "n2"));

        if (valorTotal > 0) {
            for (var i = 1; i <= qtdparcelas; i++) {
                if (i != 1) {
                    if (isNaN(intervaloDias) || intervaloDias <= 0)
                        dataPrimeiroVencimento.setMonth(dataPrimeiroVencimento.getMonth() + 1);
                    else
                        dataPrimeiroVencimento.setDate(dataPrimeiroVencimento.getDate() + intervaloDias);
                }

                var parcela = {
                    Parcela: i,
                    Valor: valorParcela,
                    DataVcto: Globalize.format(dataPrimeiroVencimento, "dd/MM/yyyy"),
                    ValorPgto: "0",
                    DataPgto: "",
                    Status: "Pendente",
                    ObservacaoBaixa: ""
                };

                parcelas.push(parcela);
            }

            if (arredondarPrimeiraParcela)
                parcelas[0].Valor += valorDiferenca;
            else
                parcelas[duplicatas.length - 1].Valor += valorDiferenca;

            $("body").data("parcelas", parcelas);

            RenderizarParcelas();
        }
    }
}

function ValidarCamposParcelas() {
    var qtdparcelas = Globalize.parseInt($("#txtParcelas").val());
    var intervaloDias = Globalize.parseInt($("#txtIntervaloDias").val());
    var dataPrimeiroVencimento = Globalize.parseDate($("#txtDataVcto").val(), "dd/MM/yyyy");

    var valor = Globalize.parseFloat($("#txtValor").val());
    var valorascrescimo = Globalize.parseFloat($("#txtAcrescimo").val());
    var valordesconto = Globalize.parseFloat($("#txtDesconto").val());
    var valorTotal = valor + valorascrescimo - valordesconto;
    var valido = true;

    if (isNaN(valorTotal) || valorTotal <= 0) {
        CampoComErro("#txtValor");
        ExibirMensagemAlerta("Valor total (Valor + Acresc. - Desc.) menor ou igual a zero, favor verificar!", "Atenção!", "placeholder-mensagem-parcelas");
        valido = false;
    } else {
        CampoSemErro("#txtValor");
    }

    if (isNaN(qtdparcelas) || qtdparcelas <= 0) {
        CampoComErro("#txtParcelas");
        ExibirMensagemAlerta("Quantidade de Parcelas não informada, favor verificar!", "Atenção!", "placeholder-mensagem-parcelas");
        valido = false;
    } else {
        CampoSemErro("#txtParcelas");
    }

    return valido;
}

function RenderizarParcelas() {
    var parcelas = $("body").data("parcelas") == null ? new Array() : $("body").data("parcelas");

    $("#tblParcelas tbody").html("");

    for (var i = 0; i < parcelas.length; i++) {
        $("#tblParcelas tbody").append("<tr><td>" + parcelas[i].Parcela + "</td><td>" + Globalize.format(parcelas[i].Valor, "n2") + "</td><td>" + parcelas[i].DataVcto + "</td><td>" + Globalize.format(parcelas[i].ValorPgto, "n2") + "</td><td>" + parcelas[i].DataPgto + "</td><td>" + parcelas[i].ObservacaoBaixa + "<td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarParcela(" + JSON.stringify(parcelas[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblParcelas tbody").html() == "")
        $("#tblParcelas tbody").html("<tr><td colspan='6'>Nenhum registro encontrado.</td></tr>");
}

function EditarParcela(parcela) {
    $("body").data("parcela", parcela);

    $("#hddCodigoParcela").val(parcela.Parcela);
    $("#txtDataVctoParcela").val(Globalize.format(parcela.DataVcto, "dd/MM/yyyy"));
    $("#txtValorParcela").val(Globalize.format(parcela.Valor, "n2"));

    $("#divAlterarParcela").show();
}

function SalvarParcela() {
    var dataVencimento = $("#txtDataVctoParcela").val();
    var valorParcela = Globalize.parseFloat($("#txtValorParcela").val());
    if (dataVencimento == null || dataVencimento.length != 10) {
        CampoComErro("#txtDataVctoParcela");
    } else if (valorParcela == null || valorParcela == 0) {
        CampoComErro("#txtValorParcela");
    } else{
        CampoSemErro("#txtDataVctoParcela");
        var parcelas = $("body").data("parcelas") == null ? new Array() : $("body").data("parcelas");
        for (var i = 0; i < parcelas.length; i++) {
            if (parcelas[i].Parcela == $("#hddCodigoParcela").val()) {
                parcelas[i].DataVcto = $("#txtDataVctoParcela").val();
                parcelas[i].Valor = valorParcela;
                break;
            }
        }
        RenderizarParcelas();
        $("#divAlterarParcela").hide();
    }
}

function BuscarDadosParcelas() {
    var parcelas = $("body").data("parcelas") == null ? new Array() : $("body").data("parcelas");
    var qtdTotal = parcelas.length;
    var intervaloDias = 30;
    var dataPrimeiroVencimento = new Date();

    if (parcelas.length > 0)
        dataPrimeiroVencimento = parcelas[0].DataVcto;

    if (parcelas.length > 1) {
        var data_inicial = parcelas[0].DataVcto;
        var data_final = parcelas[1].DataVcto;

        if (data_inicial != '' && data_final != '') {
            var data3 = new Date(data_inicial.substr(6, 4), data_inicial.substr(3, 2) - 1, data_inicial.substr(0, 2));
            var data4 = new Date(data_final.substr(6, 4), data_final.substr(3, 2) - 1, data_final.substr(0, 2));
            var intervaloDias = Math.ceil((data4.getTime() - data3.getTime()) / 1000 / 60 / 60 / 24);
        }
    }

    $("#txtIntervaloDias").val(intervaloDias);
    $("#txtParcelas").val(qtdTotal);
    $("#txtDataVcto").val(dataPrimeiroVencimento);
}

function VerificaParcelaPaga() {
    var parcelas = $("body").data("parcelas") == null ? new Array() : $("body").data("parcelas");
    var valido = false;

    for (var i = 0; i < parcelas.length; i++) {
        if (parcelas[i].ValorPgto > 0) {
            valido = true;
            ExibirMensagemAlerta("Não é possível fazer alterações, duplicata possui parcelas pagas.", "Atenção!", "messages-placeholder");
            break;
        }
    }
    return valido;
}


