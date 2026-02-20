$(document).ready(function () {
    $("#btnSalvarMunicipioCarregamento").click(function () {
        SalvarMunicipioCarregamento();
    });

    $("#btnExcluirMunicipioCarregamento").click(function () {
        ExcluirMunicipioCarregamento();
    });

    $("#btnCancelarMunicipioCarregamento").click(function () {
        LimparCamposMunicipioCarregamento();
    });
});

function ValidarCamposMunicipioCarregamento() {
    var municipio = $("#selMunicipioCarregamento").val();
    var valido = true;

    if (municipio != null && municipio != "") {
        CampoSemErro("#selMunicipioCarregamento");
    } else {
        CampoComErro("#selMunicipioCarregamento");
        valido = false;
    }

    return valido;
}

function SalvarMunicipioCarregamento() {
    if (ValidarCamposMunicipioCarregamento()) {
        var municipio = {
            Codigo: $("body").data("municipioCarregamento") != null ? $("body").data("municipioCarregamento").Codigo : 0,
            CodigoMunicipio: $("#selMunicipioCarregamento").val(),
            DescricaoMunicipio: $("#selMunicipioCarregamento option:selected").text(),
            Excluir: false
        };

        var municipios = $("body").data("municipiosCarregamento") == null ? new Array() : $("body").data("municipiosCarregamento");

        for (var i = 0; i < municipios.length; i++) {
            if (municipios[i].CodigoMunicipio == municipio.CodigoMunicipio && municipios[i].Codigo != municipio.Codigo && municipios[i].Excluir == false) {
                ExibirMensagemAlerta("Este município já foi utilizado.", "Atenção!", "placeholder-msgEmissaoMDFe");
                return;
            }
        }

        municipios.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (municipio.Codigo == 0)
            municipio.Codigo = (municipios.length > 0 ? (municipios[0].Codigo > 0 ? -1 : (municipios[0].Codigo - 1)) : -1);

        for (var i = 0; i < municipios.length; i++) {
            if (municipios[i].Codigo == municipio.Codigo) {
                municipios.splice(i, 1);
                break;
            }
        }

        municipios.push(municipio);

        municipios.sort(function (a, b) { return a.DescricaoMunicipio < b.DescricaoMunicipio ? -1 : 1; });

        $("body").data("municipiosCarregamento", municipios);

        RenderizarMunicipiosCarregamento();
        LimparCamposMunicipioCarregamento();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
    }
}

function EditarMunicipioCarregamento(municipio) {
    $("body").data("municipioCarregamento", municipio);
    $("#selMunicipioCarregamento").val(municipio.CodigoMunicipio);
    $("#btnExcluirMunicipioCarregamento").show();
}

function ExcluirMunicipioCarregamento() {
    var municipio = $("body").data("municipioCarregamento");

    var municipios = $("body").data("municipiosCarregamento") == null ? new Array() : $("body").data("municipiosCarregamento");

    for (var i = 0; i < municipios.length; i++) {
        if (municipios[i].Codigo == municipio.Codigo) {
            if (municipio.Codigo <= 0)
                municipios.splice(i, 1);
            else
                municipios[i].Excluir = true;
            break;
        }
    }

    $("body").data("municipiosCarregamento", municipios);

    RenderizarMunicipiosCarregamento();
    LimparCamposMunicipioCarregamento();
}

function RenderizarMunicipiosCarregamento(disabled) {
    var municipios = $("body").data("municipiosCarregamento") == null ? new Array() : $("body").data("municipiosCarregamento");

    $("#tblMunicipiosCarregamento tbody").html("");

    for (var i = 0; i < municipios.length; i++) {
        if (!municipios[i].Excluir)
            $("#tblMunicipiosCarregamento tbody").append("<tr><td>" + municipios[i].DescricaoMunicipio + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarMunicipioCarregamento(" + JSON.stringify(municipios[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblMunicipiosCarregamento tbody").html() == "")
        $("#tblMunicipiosCarregamento tbody").html("<tr><td colspan='2'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposMunicipioCarregamento() {
    $("body").data("municipioCarregamento", null);
    $("#selMunicipioCarregamento").val($("#selMunicipioCarregamento option:first").val());
    $("#btnExcluirMunicipioCarregamento").hide();
}