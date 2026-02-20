$(document).ready(function () {

    $("#btnSalvarCidadesPedagio").click(function () {
        SalvarCidadesPedagio();
    });

    $("#btnCancelarCidadesPedagio").click(function () {
        LimparCamposCidadesPedagio();
    });

    $("#btnExcluirCidadesPedagio").click(function () {
        ExcluirCidadesPedagio();
    });

});

function SalvarCidadesPedagio() {
    if (ValidarCidadesPedagio()) {

        var cidade = {
            Codigo: $("body").data("cidadePedagio") != null ? $("body").data("cidadePedagio") : 0,
            Cidade: $("#txtCidadePedagio").val(),
            Excluir: false
        };

        var cidadesPedagio = $("body").data("cidadesPedagio") == null ? new Array() : $("body").data("cidadesPedagio");

        if (cidade.Codigo == 0)
            cidade.Codigo = (cidadesPedagio.length > 0 ? (cidadesPedagio[0].Codigo > 0 ? -1 : (cidadesPedagio[0].Codigo - 1)) : -1);

        for (var i = 0; i < cidadesPedagio.length; i++) {
            if (cidadesPedagio[i].Codigo == cidade.Codigo) {
                cidadesPedagio.splice(i, 1);
                break;
            }
        }

        cidadesPedagio.push(cidade);

        $("body").data("cidadesPedagio", cidadesPedagio);

        RenderizarCidadesPedagio();
        LimparCamposCidadesPedagio();

    }
    else
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "messages-placeholder-emissaoCIOT");
}

function RenderizarCidadesPedagio(disabled) {

    var cidades = $("body").data("cidadesPedagio") == null ? new Array() : $("body").data("cidadesPedagio");

    $("#tblCidadesPedagio tbody").html("");

    for (var i = 0; i < cidades.length; i++) {
        if (!cidades[i].Excluir)
            $("#tblCidadesPedagio tbody").append("<tr><td>" + cidades[i].Cidade + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarCidadePedagio(" + JSON.stringify(cidades[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblCidadesPedagio tbody").html() == "")
        $("#tblCidadesPedagio tbody").html("<tr><td colspan='5'>Nenhum registro encontrado.</td></tr>");

}

function LimparCamposCidadesPedagio() {
    $("body").data("codigoLocalidadePedagio", null);
    $("#txtCidadePedagio").val('');
    $("#btnExcluirCidadesPedagio").hide();
}

function EditarCidadePedagio(cidadePedagio) {
    $("body").data("cidadePedagio", cidadePedagio);
    $("#txtCidadePedagio").val(cidadePedagio.Cidade);

    $("#btnExcluirCidadesPedagio").show();
}

function ExcluirCidadesPedagio() {
    var cidade = $("body").data("cidadePedagio");

    var cidades = $("body").data("cidadesPedagio") == null ? new Array() : $("body").data("cidadesPedagio");

    for (var i = 0; i < cidades.length; i++) {
        if (cidades[i].Codigo == cidade.Codigo) {
            if (cidade.Codigo <= 0)
                cidades.splice(i, 1);
            else
                cidades[i].Excluir = true;
            break;
        }
    }

    $("body").data("cidadesPedagio", cidades);

    RenderizarCidadesPedagio();
    LimparCamposCidadesPedagio();
}

function ValidarCidadesPedagio() {
    var cidadePedagio = $("#txtCidadePedagio").val();
    var valido = true;

    if (cidadePedagio == null || cidadePedagio == "") {
        CampoComErro("#txtCidadePedagio");
        valido = false;
    } else {
        CampoSemErro("#txtCidadePedagio");
    }

    return valido;
}

