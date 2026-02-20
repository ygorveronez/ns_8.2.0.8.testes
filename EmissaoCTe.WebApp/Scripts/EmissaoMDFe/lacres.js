$(document).ready(function () {
    $("#btnSalvarLacre").click(function () {
        SalvarLacre();
    });

    $("#btnExcluirLacre").click(function () {
        ExcluirLacre();
    });

    $("#btnCancelarLacre").click(function () {
        LimparCamposLacre();
    });
});

function ValidarCamposLacre() {
    var lacre = $("#txtNumeroLacre").val();
    var valido = true;

    if (lacre != "") {
        CampoSemErro("#txtNumeroLacre");
    } else {
        CampoComErro("#txtNumeroLacre");
        valido = false;
    }

    return valido;
}

function SalvarLacre() {
    if (ValidarCamposLacre()) {
        var lacre = {
            Codigo: $("body").data("lacre") != null ? $("body").data("lacre").Codigo : 0,
            Numero: $("#txtNumeroLacre").val(),
            Excluir: false
        };

        var lacres = $("body").data("lacres") == null ? new Array() : $("body").data("lacres");

        lacres.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (lacre.Codigo == 0)
            lacre.Codigo = (lacres.length > 0 ? (lacres[0].Codigo > 0 ? -1 : (lacres[0].Codigo - 1)) : -1);

        for (var i = 0; i < lacres.length; i++) {
            if (lacres[i].Codigo == lacre.Codigo) {
                lacres.splice(i, 1);
                break;
            }
        }

        lacres.push(lacre);

        lacres.sort(function (a, b) { return a.Numero < b.Numero ? -1 : 1; });

        $("body").data("lacres", lacres);

        RenderizarLacres();
        LimparCamposLacre();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
    }
}

function EditarLacre(lacre) {
    $("body").data("lacre", lacre);
    $("#txtNumeroLacre").val(lacre.Numero);
    $("#btnExcluirLacre").show();
}

function ExcluirLacre() {
    var lacre = $("body").data("lacre");

    var lacres = $("body").data("lacres") == null ? new Array() : $("body").data("lacres");

    for (var i = 0; i < lacres.length; i++) {
        if (lacres[i].Codigo == lacre.Codigo) {
            if (lacre.Codigo <= 0)
                lacres.splice(i, 1);
            else
                lacres[i].Excluir = true;
            break;
        }
    }

    $("body").data("lacres", lacres);

    RenderizarLacres();
    LimparCamposLacre();
}

function RenderizarLacres(disabled) {
    var lacres = $("body").data("lacres") == null ? new Array() : $("body").data("lacres");

    $("#tblLacres tbody").html("");

    for (var i = 0; i < lacres.length; i++) {
        if (!lacres[i].Excluir)
            $("#tblLacres tbody").append("<tr><td>" + lacres[i].Numero + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarLacre(" + JSON.stringify(lacres[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblLacres tbody").html() == "")
        $("#tblLacres tbody").html("<tr><td colspan='2'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposLacre() {
    $("body").data("lacre", null);
    $("#txtNumeroLacre").val('');
    $("#btnExcluirLacre").hide();
}