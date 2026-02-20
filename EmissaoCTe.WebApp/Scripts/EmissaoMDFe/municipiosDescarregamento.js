$(document).ready(function () {
    $("#btnSalvarMunicipioDescarregamento").click(function () {
        SalvarMunicipioDescarregamento();
    });

    $("#btnExcluirMunicipioDescarregamento").click(function () {
        ExcluirMunicipioDescarregamento();
    });

    $("#btnCancelarMunicipioDescarregamento").click(function () {
        LimparCamposMunicipioDescarregamento();
    });
});

function ValidarCamposMunicipioDescarregamento() {
    var municipio = $("#selMunicipioDescarregamento").val();
    var valido = true;

    if (municipio != null && municipio != "") {
        CampoSemErro("#selMunicipioDescarregamento");
    } else {
        CampoComErro("#selMunicipioDescarregamento");
        valido = false;
    }

    return valido;
}

function SalvarMunicipioDescarregamento() {
    if (ValidarCamposMunicipioDescarregamento()) {
        var municipio = {
            Codigo: $("body").data("municipioDescarregamento") != null ? $("body").data("municipioDescarregamento").Codigo : 0,
            CodigoMunicipio: $("#selMunicipioDescarregamento").val(),
            DescricaoMunicipio: $("#selMunicipioDescarregamento option:selected").text(),
            Documentos: $("body").data("municipioDescarregamento") != null ? $("body").data("municipioDescarregamento").Documentos : new Array(),
            NFes: $("body").data("municipioDescarregamento") != null ? $("body").data("municipioDescarregamento").NFes : new Array(),
            ChaveCTes: $("body").data("municipioDescarregamento") != null ? $("body").data("municipioDescarregamento").ChaveCTes : new Array(),
            Excluir: false
        };

        var municipios = $("body").data("municipiosDescarregamento") == null ? new Array() : $("body").data("municipiosDescarregamento");

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

        $("body").data("municipiosDescarregamento", municipios);

        RenderizarMunicipiosDescarregamento();
        LimparCamposMunicipioDescarregamento();

        if (municipio.Documentos.length == 0) {
            if ($("#selTipoEmitente").val() == "1") {
                jConfirm("Deseja vincular os CT-es a este município de descarregamento?", "Atenção!", function (r) {
                    if (r)
                        AbrirTelaDocumentosMunicipioDescarregamento(municipio);
                });
            } else if ($("#selTipoEmitente").val() == "2" || $("#selTipoEmitente").val() == "3") {
                jConfirm("Deseja vincular as NF-es a este município de descarregamento?", "Atenção!", function (r) {
                    if (r)
                        AbrirTelaNFesMunicipioDescarregamento(municipio);
                });
            }
        }

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
    }
}

function EditarMunicipioDescarregamento(municipio) {
    $("body").data("municipioDescarregamento", municipio);
    $("#selMunicipioDescarregamento").val(municipio.CodigoMunicipio);
    $("#btnExcluirMunicipioDescarregamento").show();
}

function ExcluirMunicipioDescarregamento() {
    var municipio = $("body").data("municipioDescarregamento");

    var municipios = $("body").data("municipiosDescarregamento") == null ? new Array() : $("body").data("municipiosDescarregamento");

    for (var i = 0; i < municipios.length; i++) {
        if (municipios[i].Codigo == municipio.Codigo) {
            if (municipio.Codigo <= 0)
                municipios.splice(i, 1);
            else
                municipios[i].Excluir = true;
            break;
        }
    }

    $("body").data("municipiosDescarregamento", municipios);

    RenderizarMunicipiosDescarregamento();
    LimparCamposMunicipioDescarregamento();
}

function RenderizarMunicipiosDescarregamento(disabled) {
    var municipios = $("body").data("municipiosDescarregamento") == null ? new Array() : $("body").data("municipiosDescarregamento");
    var tipoEmitente = $("#selTipoEmitente").val();
    var $tbody = $("#tblMunicipiosDescarregamento tbody");
    var $rows = [];

    $tbody.html("");

    municipios.forEach(function (info) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.DescricaoMunicipio + "</td>" +
                "<td class='btns-municipios-descarregamento'>" +
                    "<button class='btn btn-default btn-xs editar' type='button' " + (disabled ? "disabled" : "") + " title='Editar'><span class='glyphicon glyphicon-edit'></span></button>" +
                    "<button class='btn btn-default btn-xs documentos' type='button' " + (disabled ? "disabled" : "") + " title='Documentos'><span class='glyphicon glyphicon-file'></span></button>" +
                "</td>" +
            "</tr>");

            $row.on("click", ".editar", function () {
                EditarMunicipioDescarregamento(info);
            });

            $row.on("click", ".documentos", function () {
                if (tipoEmitente == "1")
                    AbrirTelaDocumentosMunicipioDescarregamento(info);
                else if (tipoEmitente == "2")
                    AbrirTelaNFesMunicipioDescarregamento(info);
                else if (tipoEmitente == "3")
                    AbrirTelaNFesMunicipioDescarregamento(info);
                else if (tipoEmitente == "9")
                    AbrirTelaChaveCTesMunicipioDescarregamento(info);
            });

            $rows.push($row);
        }
    });

    $tbody.append.apply($tbody, $rows);

    if ($tbody.find("tr").length == 0)
        $tbody.html("<tr><td colspan='" + $("#tblMunicipiosDescarregamento thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposMunicipioDescarregamento() {
    $("body").data("municipioDescarregamento", null);
    $("#selMunicipioDescarregamento").val($("#selMunicipioDescarregamento option:first").val());
    $("#btnExcluirMunicipioDescarregamento").hide();
}