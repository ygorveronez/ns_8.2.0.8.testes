$(document).ready(function () {
    $("#txtQuantidadeDeDuplicatas").mask("9?9");
    $("#txtFrequenciaVencimentos").mask("9?9");

    FormatarCampoDate("txtDataPrimeiroVencimento");
    FormatarCampoDate("txtDataVencimentoDuplicata");

    $("#txtValorDuplicata").priceFormat({ prefix: '' });
    $("#btnSalvarDuplicata").click(function () {
        SalvarDuplicata();
    });
    $("#btnExcluirDuplicata").click(function () {
        ExcluirDuplicata();
    });
    $("#btnCancelarDuplicata").click(function () {
        LimparCamposDuplicata();
    });
    $("#btnGerarDuplicatas").click(function () {
        GerarParcelasDaFatura();
    });
});
function GerarParcelasDaFatura() {
    var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());
    if (duplicatas.length <= 0) {
        var quantidadeDuplicatas = Globalize.parseInt($("#txtQuantidadeDeDuplicatas").val());
        if (isNaN(quantidadeDuplicatas))
            quantidadeDuplicatas = 1;
        var frequenciaVencimentos = Globalize.parseInt($("#txtFrequenciaVencimentos").val());
        if (isNaN(frequenciaVencimentos))
            frequenciaVencimentos = 30;
        var dataPrimeiroVencimento = Globalize.parseDate($("#txtDataPrimeiroVencimento").val(), "dd/MM/yyyy");
        if (dataPrimeiroVencimento == null)
            dataPrimeiroVencimento = new Date();
        var arredondarPrimeiraParcela = $("#selArredondamentoDuplicatas").val() == "0" ? true : false;
        var valorTotal = Globalize.parseFloat($("#txtValorFreteContratado").val());
        var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / quantidadeDuplicatas, "n2"));
        var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * quantidadeDuplicatas), "n2"));
        if (valorTotal > 0) {
            for (var i = 1; i <= quantidadeDuplicatas; i++) {
                if (i != 1)
                    dataPrimeiroVencimento.setDate(dataPrimeiroVencimento.getDate() + frequenciaVencimentos);
                var duplicata = {
                    Codigo: -i,
                    DataVencimento: Globalize.format(dataPrimeiroVencimento, "dd/MM/yyyy"),
                    Valor: valorParcela,
                    Numero: $("#txtNumeroDuplicata").val(),
                    Parcela: i,
                    Excluir: false
                };
                duplicatas.push(duplicata);
            }
            if (valorDiferenca > 0) {
                if (arredondarPrimeiraParcela)
                    duplicatas[0].Valor += valorDiferenca;
                else
                    duplicatas[duplicatas.length - 1].Valor += valorDiferenca;
            }
            $("#hddDuplicatas").val(JSON.stringify(duplicatas));
            RenderizarDuplicata();
            LimparCamposDuplicata();
        } else {
            jAlert("O valor do frete deve ser maior do que zero para gerar automaticamente as parcelas.", "Atenção");
        }
    } else {
        jAlert("Não é possível gerar as parcelas pois já existem parcelas geradas.<br/>Exclua as parcelas existentes para realizar a geração automatica.", "Atenção");
    }

}
function BuscarProximoNumeroFatura() {
    executarRest("/CobrancaCTe/BuscarProximoNumeroDeFatura?callback=?", {}, function (r) {
        if (r.Sucesso) {
            $("#txtNumeroDuplicata").val(r.Objeto);
            SetarProximaParcelaFatura();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}
function SetarProximaParcelaFatura() {
    var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());
    var numeroParcela = 1;
    for (var i = 0; i < duplicatas.length; i++)
        if (duplicatas[i].Parcela >= numeroParcela)
            numeroParcela = duplicatas[i].Parcela + 1;
    $("#txtParcelaDuplicata").val(numeroParcela);
}
function LimparCamposDuplicata() {
    SetarProximaParcelaFatura();
    $("#hddDuplicataEmEdicao").val("0");
    $("#txtDataVencimentoDuplicata").val("");
    $("#txtValorDuplicata").val("0,00");
    $("#btnExcluirDuplicata").hide();
}
function LimparCamposFatura() {
    $("#txtQuantidadeDeDuplicatas").val("");
    $("#txtFrequenciaVencimentos").val("");
    $("#txtDataPrimeiroVencimento").val("");
    $("#selArredondamentoDuplicatas").val($("#selArredondamentoDuplicatas option:first").val());
    LimparCamposDuplicata();
}
function ValidarCamposDuplicata() {
    var dataVencimento = $("#txtDataVencimentoDuplicata").val();
    var valor = Globalize.parseFloat($("#txtValorDuplicata").val());
    var valido = true;
    if (dataVencimento != "") {
        CampoSemErro("#txtDataVencimentoDuplicata");
    } else {
        CampoComErro("#txtDataVencimentoDuplicata");
        valido = false;
    }
    if (valor != "") {
        CampoSemErro("#txtValorDuplicata");
    } else {
        CampoComErro("#txtValorDuplicata");
        valido = false;
    }
    return valido;
}
function SalvarDuplicata() {
    if (ValidarCamposDuplicata()) {
        var duplicata = {
            Codigo: Globalize.parseInt($("#hddDuplicataEmEdicao").val()),
            DataVencimento: $("#txtDataVencimentoDuplicata").val(),
            Valor: Globalize.parseFloat($("#txtValorDuplicata").val()),
            Numero: $("#txtNumeroDuplicata").val(),
            Parcela: Globalize.parseInt($("#txtParcelaDuplicata").val()),
            Excluir: false
        };
        var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());
        if (duplicata.Codigo == 0)
            duplicata.Codigo = -(duplicatas.length + 1);
        if (duplicatas.length > 0) {
            for (var i = 0; i < duplicatas.length; i++) {
                if (duplicatas[i].Codigo == duplicata.Codigo) {
                    duplicatas.splice(i, 1);
                    break;
                }
            }
        }
        duplicatas.push(duplicata);
        duplicatas.sort();
        $("#hddDuplicatas").val(JSON.stringify(duplicatas));
        RenderizarDuplicata();
        LimparCamposDuplicata();
    }
}
function EditarDuplicata(duplicata) {
    $("#hddDuplicataEmEdicao").val(duplicata.Codigo);
    $("#txtNumeroDuplicata").val(duplicata.Numero);
    $("#txtParcelaDuplicata").val(duplicata.Parcela);
    $("#txtDataVencimentoDuplicata").val(duplicata.DataVencimento);
    $("#txtValorDuplicata").val(Globalize.format(duplicata.Valor, "n2"));
    $("#btnExcluirDuplicata").show();
}
function ExcluirDuplicata() {
    jConfirm("Deseja realmente excluir esta duplicata?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddDuplicataEmEdicao").val());
            var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());
            for (var i = 0; i < duplicatas.length; i++) {
                if (duplicatas[i].Codigo == codigo) {
                    if (codigo > 0) {
                        duplicatas[i].Excluir = true;
                    } else {
                        duplicatas.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddDuplicatas").val(JSON.stringify(duplicatas));
            RenderizarDuplicata();
            LimparCamposDuplicata();
        }
    });
}
function RenderizarDuplicata() {
    $("#tblDuplicatasCobranca tbody").html("");
    var duplicatas = $("#hddDuplicatas").val() == "" ? new Array() : JSON.parse($("#hddDuplicatas").val());
    for (var i = 0; i < duplicatas.length; i++) {
        if (!duplicatas[i].Excluir) {
            $("#tblDuplicatasCobranca tbody").append("<tr><td>" + duplicatas[i].Numero + "</td><td>" + duplicatas[i].Parcela + "</td><td>" + duplicatas[i].DataVencimento + "</td><td>" + duplicatas[i].Valor + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarDuplicata(" + JSON.stringify(duplicatas[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblDuplicatasCobranca tbody").html() == "") {
        $("#tblDuplicatasCobranca tbody").html("<tr><td colspan='5'>Nenhum registro encontrado!</td></tr>");
    }
}