$(document).ready(function () {
    $("#txtCNPJFornecedorValePedagio").mask("99999999999999");
    $("#txtCNPJResponsavelValePedagio").mask("99999999999999");
    $("#txtNumeroComprovanteValePedagio").mask("9?9999999999999999999");
    $("#txtValorValePedagio").priceFormat({ prefix: '' });
    $("#txtValorValePedagio").val("0,00");
    $("#txtQuantidadeEixosValePedagio").priceFormat({ limit: 2, centsLimit: 0, centsSeparator: '' });

    CarregarConsultadeClientes("btnBuscarFornecedorValePedagio", "btnBuscarFornecedorValePedagio", RetornoConsultaFornecedorValePedagio, true, false, "J");
    CarregarConsultadeClientes("btnBuscarResponsavelValePedagio", "btnBuscarResponsavelValePedagio", RetornoConsultaResponsavelValePedagio, true, false, "J");

    $("#btnSalvarValePedagio").click(function () {
        SalvarValePedagio();
    });

    $("#btnExcluirValePedagio").click(function () {
        ExcluirValePedagio();
    });

    $("#btnCancelarValePedagio").click(function () {
        LimparCamposValePedagio();
    });
});

function RetornoConsultaFornecedorValePedagio(forn) {
    $("#txtCNPJFornecedorValePedagio").val(forn.CPFCNPJ.replace(/[^0-9]/g, ""));
}

function RetornoConsultaResponsavelValePedagio(resp) {
    $("#txtCNPJResponsavelValePedagio").val(resp.CPFCNPJ.replace(/[^0-9]/g, ""));
}

function ValidarCamposValePedagio() {
    var numeroComprovante = $("#txtNumeroComprovanteValePedagio").val();
    var cnpjFornecedor = $("#txtCNPJFornecedorValePedagio").val();
    var valido = true;

    if (numeroComprovante != "") {
        CampoSemErro("#txtNumeroComprovanteValePedagio");
    } else {
        CampoComErro("#txtNumeroComprovanteValePedagio");
        valido = false;
    }

    if (cnpjFornecedor != "") {
        CampoSemErro("#txtCNPJFornecedorValePedagio");
    } else {
        CampoComErro("#txtCNPJFornecedorValePedagio");
        valido = false;
    }

    //if ($("#txtValorValePedagio").is(":visible") && parseFloat($("#txtValorValePedagio").val()) > 0) {
    //    CampoSemErro("#txtValorValePedagio");
    //} else {
    //    CampoComErro("#txtValorValePedagio");
    //    valido = false;
    //}

    return valido;
}

function SalvarValePedagio() {
    if (ValidarCamposValePedagio()) {
        var valePedagio = {
            Codigo: $("body").data("valePedagio") != null ? $("body").data("valePedagio").Codigo : 0,
            CNPJFornecedor: $("#txtCNPJFornecedorValePedagio").val(),
            CNPJResponsavel: $("#txtCNPJResponsavelValePedagio").val(),
            NumeroComprovante: $("#txtNumeroComprovanteValePedagio").val(),
            CodigoAgendamentoPorto: $("#txtCodigoAgendamentoPortoValePedagio").val(),
            ValorValePedagio: Globalize.parseFloat($("#txtValorValePedagio").val()),
            TipoCompra: $("#selTipoCompraValePedagio").val(),
            QuantidadeEixos: Globalize.parseInt($("#txtQuantidadeEixosValePedagio").val()),
            Excluir: false
        };

        var valesPedagio = $("body").data("valesPedagio") == null ? new Array() : $("body").data("valesPedagio");

        valesPedagio.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (valePedagio.Codigo == 0)
            valePedagio.Codigo = (valesPedagio.length > 0 ? (valesPedagio[0].Codigo > 0 ? -1 : (valesPedagio[0].Codigo - 1)) : -1);

        for (var i = 0; i < valesPedagio.length; i++) {
            if (valesPedagio[i].Codigo == valePedagio.Codigo) {
                valesPedagio.splice(i, 1);
                break;
            }
        }

        valesPedagio.push(valePedagio);

        valesPedagio.sort(function (a, b) { return a.NumeroComprovante < b.NumeroComprovante ? -1 : 1; });

        $("body").data("valesPedagio", valesPedagio);

        RenderizarValesPedagio();
        LimparCamposValePedagio();

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
    }
}

function EditarValePedagio(valePedagio) {
    $("body").data("valePedagio", valePedagio);
    $("#txtCNPJFornecedorValePedagio").val(valePedagio.CNPJFornecedor);
    $("#txtCNPJResponsavelValePedagio").val(valePedagio.CNPJResponsavel);
    $("#txtNumeroComprovanteValePedagio").val(valePedagio.NumeroComprovante);
    $("#txtCodigoAgendamentoPortoValePedagio").val(valePedagio.CodigoAgendamentoPorto);
    $("#txtValorValePedagio").val(Globalize.format(valePedagio.ValorValePedagio, "n2"));
    $("#txtQuantidadeEixosValePedagio").val(Globalize.format(valePedagio.QuantidadeEixos, "n0"));
    $("#selTipoCompraValePedagio").val(valePedagio.TipoCompra);
    
    $("#btnExcluirValePedagio").show();
}

function ExcluirValePedagio() {
    var valePedagio = $("body").data("valePedagio");

    var valesPedagio = $("body").data("valesPedagio") == null ? new Array() : $("body").data("valesPedagio");

    for (var i = 0; i < valesPedagio.length; i++) {
        if (valesPedagio[i].Codigo == valePedagio.Codigo) {
            if (valePedagio.Codigo <= 0)
                valesPedagio.splice(i, 1);
            else
                valesPedagio[i].Excluir = true;
            break;
        }
    }

    $("body").data("valesPedagio", valesPedagio);

    RenderizarValesPedagio();
    LimparCamposValePedagio();
}

function RenderizarValesPedagio(disabled) {
    var valesPedagio = $("body").data("valesPedagio") == null ? new Array() : $("body").data("valesPedagio");

    $("#tblValePedagio tbody").html("");

    for (var i = 0; i < valesPedagio.length; i++) {
        if (!valesPedagio[i].Excluir)
            $("#tblValePedagio tbody").append("<tr>" +
                "<td>" + valesPedagio[i].CNPJFornecedor + "</td>" +
                "<td>" + valesPedagio[i].CNPJResponsavel + "</td>" +
                "<td>" + valesPedagio[i].NumeroComprovante + "</td>" +
                ($("body").hasClass("mdfe-300") ? "<td>" + Globalize.format(valesPedagio[i].ValorValePedagio, "n2") + "</td>" : "") +
                "<td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarValePedagio(" + JSON.stringify(valesPedagio[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblValePedagio tbody").html() == "")
        $("#tblValePedagio tbody").html("<tr><td colspan='4'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposValePedagio() {
    $("body").data("valePedagio", null);
    $("#txtCNPJFornecedorValePedagio").val('');
    $("#txtCNPJResponsavelValePedagio").val('');
    $("#txtNumeroComprovanteValePedagio").val('');
    $("#txtCodigoAgendamentoPortoValePedagio").val('');
    $("#txtValorValePedagio").val('0,00');
    $("#btnExcluirValePedagio").hide();
    $("#txtQuantidadeEixosValePedagio").val('2');    
    $("#selTipoCompraValePedagio").val($("#selTipoCompraValePedagio option:first").val());
}