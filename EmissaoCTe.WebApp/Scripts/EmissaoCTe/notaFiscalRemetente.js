$(document).ready(function () {
    $("#txtNumeroNotaFiscalRemetente").mask("9?99999999999999");
    $("#txtCFOPNotaFiscalRemetente").mask("9999");

    FormatarCampoDate("txtDataEmissaoNotaFiscalRemetente");
    
    $("#txtBaseCalculoICMSNotaFiscalRemetente").priceFormat({ prefix: '' });
    $("#txtValorICMSNotaFiscalRemetente").priceFormat({ prefix: '' });
    $("#txtBaseCalculoICMSSTNotaFiscalRemetente").priceFormat({ prefix: '' });
    $("#txtValorICMSSTNotaFiscalRemetente").priceFormat({ prefix: '' });
    $("#txtPesoTotalNotaFiscalRemetente").priceFormat({ prefix: '' });
    $("#txtValorProdutosNotaFiscalRemetente").priceFormat({ prefix: '' });
    $("#txtValorNotaNotaFiscalRemetente").priceFormat({ prefix: '' });

    $("#btnSalvarNotaFiscalRemetente").click(function () {
        SalvarNotaFiscalRemetente();
    });

    $("#btnExcluirNotaFiscalRemetente").click(function () {
        ExcluirNotaFiscalRemetente();
    });

    $("#btnCancelarNotaFiscalRemetente").click(function () {
        LimparCamposNotasFiscaisRemetente();
    });
});
function ValidarCamposNotaFiscalRemetente() {
    var valido = true;
    serie = $("#txtSerieNotaFiscalRemetente").val();
    numero = Globalize.parseInt($("#txtNumeroNotaFiscalRemetente").val());
    dataEmissao = $("#txtDataEmissaoNotaFiscalRemetente").val();
    cfop = $("#txtCFOPNotaFiscalRemetente").val();
    peso = Globalize.parseFloat($("#txtPesoTotalNotaFiscalRemetente").val());
    valor = Globalize.parseFloat($("#txtValorNotaNotaFiscalRemetente").val());
    if (serie != "") {
        CampoSemErro("#txtSerieNotaFiscalRemetente");
    } else {
        CampoComErro("#txtSerieNotaFiscalRemetente");
        valido = false;
    }
    if (numero > 0) {
        CampoSemErro("#txtNumeroNotaFiscalRemetente");
    } else {
        CampoComErro("#txtNumeroNotaFiscalRemetente");
        valido = false;
    }
    if (dataEmissao != "") {
        CampoSemErro("#txtDataEmissaoNotaFiscalRemetente");
    } else {
        CampoComErro("#txtDataEmissaoNotaFiscalRemetente");
        valido = false;
    }
    if ($("#txtCFOPNotaFiscalRemetente").prop("disabled") == false) {
        if (cfop != "") {
            var primeiroDigito = Globalize.parseInt(cfop.substring(0, 1));
            if (cfop.length == 4 && cfop != "0000" && primeiroDigito <= 7 && primeiroDigito != 4) {
                CampoSemErro("#txtCFOPNotaFiscalRemetente");
            } else {
                jAlert("A CFOP deve possuir 4 dígitos, ser diferente de '0000' e o primeiro dígito deve ser menor ou igual a 7 e diferente de 4.", "Atenção");
                CampoComErro("#txtCFOPNotaFiscalRemetente");
                valido = false;
            }
        } else {
            CampoComErro("#txtCFOPNotaFiscalRemetente");
            valido = false;
        }
    }
    //if (peso > 0) {
    //    CampoSemErro("#txtPesoTotalNotaFiscalRemetente");
    //} else {
    //    CampoComErro("#txtPesoTotalNotaFiscalRemetente");
    //    valido = false;
    //}
    if (valor > 0) {
        CampoSemErro("#txtValorNotaNotaFiscalRemetente");
    } else {
        CampoComErro("#txtValorNotaNotaFiscalRemetente");
        valido = false;
    }
    return valido;
}
function LimparCamposNotasFiscaisRemetente() {
    $("#hddNotaFiscalRemetenteEmEdicao").val('0');
    $("#ddlModeloNotaFiscaiRemetente").val($("#ddlModeloNotaFiscaiRemetente option:first").val());
    $("#txtSerieNotaFiscalRemetente").val('');
    $("#txtNumeroNotaFiscalRemetente").val('');
    $("#txtDataEmissaoNotaFiscalRemetente").val('');
    $("#txtCFOPNotaFiscalRemetente").val('');
    $("#txtBaseCalculoICMSNotaFiscalRemetente").val('0,00');
    $("#txtValorICMSNotaFiscalRemetente").val('0,00');
    $("#txtBaseCalculoICMSSTNotaFiscalRemetente").val('0,00');
    $("#txtValorICMSSTNotaFiscalRemetente").val('0,00');
    $("#txtPesoTotalNotaFiscalRemetente").val('0,00');
    $("#txtPINNotaFiscalRemetente").val('');
    $("#txtValorProdutosNotaFiscalRemetente").val('0,00');
    $("#txtValorNotaNotaFiscalRemetente").val('0,00');
    $("#btnExcluirNotaFiscalRemetente").hide();
}
function SalvarNotaFiscalRemetente() {
    if (ValidarCamposNotaFiscalRemetente()) {
        var notaFiscal = {
            Codigo: $("#hddNotaFiscalRemetenteEmEdicao").val(),
            Modelo: $("#ddlModeloNotaFiscaiRemetente").val(),
            Serie: $("#txtSerieNotaFiscalRemetente").val(),
            Numero: Globalize.parseInt($("#txtNumeroNotaFiscalRemetente").val()),
            DataEmissao: $("#txtDataEmissaoNotaFiscalRemetente").val(),
            CFOP: $("#txtCFOPNotaFiscalRemetente").val(),
            BaseCalculoICMS: Globalize.parseFloat($("#txtBaseCalculoICMSNotaFiscalRemetente").val()),
            ValorICMS: Globalize.parseFloat($("#txtValorICMSNotaFiscalRemetente").val()),
            BaseCalculoICMSST: Globalize.parseFloat($("#txtBaseCalculoICMSSTNotaFiscalRemetente").val()),
            ValorICMSST: Globalize.parseFloat($("#txtValorICMSSTNotaFiscalRemetente").val()),
            Peso: Globalize.parseFloat($("#txtPesoTotalNotaFiscalRemetente").val()),
            PIN: $("#txtPINNotaFiscalRemetente").val(),
            ValorProdutos: Globalize.parseFloat($("#txtValorProdutosNotaFiscalRemetente").val()),
            ValorTotal: Globalize.parseFloat($("#txtValorNotaNotaFiscalRemetente").val()),
            Excluir: false
        };
        var notasFiscais = $("#hddNotasFiscaisRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisRemetente").val());
        if (notaFiscal.Codigo == 0)
            notaFiscal.Codigo = -(notasFiscais.length + 1);
        if (notasFiscais.length > 0) {
            for (var i = 0; i < notasFiscais.length; i++) {
                if (notasFiscais[i].Codigo == notaFiscal.Codigo) {
                    notasFiscais.splice(i, 1);
                    break;
                }
            }
        }
        notasFiscais.push(notaFiscal);
        notasFiscais.sort();
        $("#hddNotasFiscaisRemetente").val(JSON.stringify(notasFiscais));
        RenderizarNotasFiscaisRemetente();
        LimparCamposNotasFiscaisRemetente();
        AtualizarValorTotalDaCargaNF();
        //BuscarFretePorValor();
    }
}
function EditarNotaFiscalRemetente(notaFiscal) {
    $("#hddNotaFiscalRemetenteEmEdicao").val(notaFiscal.Codigo);
    $("#ddlModeloNotaFiscaiRemetente").val(notaFiscal.Modelo);
    $("#txtSerieNotaFiscalRemetente").val(notaFiscal.Serie);
    $("#txtNumeroNotaFiscalRemetente").val(notaFiscal.Numero);
    $("#txtDataEmissaoNotaFiscalRemetente").val(notaFiscal.DataEmissao);
    $("#txtCFOPNotaFiscalRemetente").val(notaFiscal.CFOP);
    $("#txtBaseCalculoICMSNotaFiscalRemetente").val(Globalize.format(notaFiscal.BaseCalculoICMS, "n2"));
    $("#txtValorICMSNotaFiscalRemetente").val(Globalize.format(notaFiscal.ValorICMS, "n2"));
    $("#txtBaseCalculoICMSSTNotaFiscalRemetente").val(Globalize.format(notaFiscal.BaseCalculoICMSST, "n2"));
    $("#txtValorICMSSTNotaFiscalRemetente").val(Globalize.format(notaFiscal.ValorICMSST, "n2"));
    $("#txtPesoTotalNotaFiscalRemetente").val(Globalize.format(notaFiscal.Peso, "n2"));
    $("#txtPINNotaFiscalRemetente").val(notaFiscal.PIN);
    $("#txtValorProdutosNotaFiscalRemetente").val(Globalize.format(notaFiscal.ValorProdutos, "n2"));
    $("#txtValorNotaNotaFiscalRemetente").val(Globalize.format(notaFiscal.ValorTotal, "n2"));
    $("#btnExcluirNotaFiscalRemetente").show();
}
function ExcluirNotaFiscalRemetente() {
    jConfirm("Deseja realmente excluir esta nota fiscal?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddNotaFiscalRemetenteEmEdicao").val());
            var notasFiscais = $("#hddNotasFiscaisRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisRemetente").val());
            for (var i = 0; i < notasFiscais.length; i++) {
                if (notasFiscais[i].Codigo == codigo) {
                    if (codigo > 0) {
                        notasFiscais[i].Excluir = true;
                    } else {
                        notasFiscais.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddNotasFiscaisRemetente").val(JSON.stringify(notasFiscais));
            RenderizarNotasFiscaisRemetente();
            LimparCamposNotasFiscaisRemetente();
            AtualizarValorTotalDaCargaNF();
            //BuscarFretePorValor();
        }
    });
}
function RenderizarNotasFiscaisRemetente() {
    $("#tblNotasFiscaisRemetente tbody").html("");
    var notasFiscais = $("#hddNotasFiscaisRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisRemetente").val());
    for (var i = 0; i < notasFiscais.length; i++) {
        if (!notasFiscais[i].Excluir) {
            $("#tblNotasFiscaisRemetente tbody").append("<tr><td>" + notasFiscais[i].Numero + "</td><td>" + notasFiscais[i].Serie + "</td><td>" + notasFiscais[i].DataEmissao + "</td><td>" + Globalize.format(notasFiscais[i].ValorICMS, "n2") + "</td><td>" + Globalize.format(notasFiscais[i].ValorICMSST, "n2") + "</td><td>" + Globalize.format(notasFiscais[i].ValorTotal, "n2") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarNotaFiscalRemetente(" + JSON.stringify(notasFiscais[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblNotasFiscaisRemetente tbody").html() == "") {
        $("#tblNotasFiscaisRemetente tbody").html("<tr><td colspan='7'>Nenhum registro encontrado!</td></tr>");
    }
}
function AtualizarValorTotalDaCargaNF() {
    var NotasFiscaisRemetente = $("#hddNotasFiscaisRemetente").val() == "" ? new Array() : JSON.parse($("#hddNotasFiscaisRemetente").val());
    var valorTotalCarga = 0;
    for (var i = 0; i < NotasFiscaisRemetente.length; i++)
        if (!NotasFiscaisRemetente[i].Excluir && NotasFiscaisRemetente[i].ValorTotal > 0)
            valorTotalCarga += NotasFiscaisRemetente[i].ValorTotal;

    var valorTotal = Globalize.format(valorTotalCarga, "n2");

    $("#txtValorTotalCarga").val(valorTotal);
    $("#txtValorCargaAverbacao").val(valorTotal);
}