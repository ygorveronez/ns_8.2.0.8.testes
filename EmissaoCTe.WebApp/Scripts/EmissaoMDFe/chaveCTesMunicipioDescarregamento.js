$(document).ready(function () {
    $("#txtChaveCTeMunicipioDescarregamento").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");

    $("#btnSalvarChaveCTeMunicipioDescarregamento").click(function () {
        SalvarChaveCTeMunicipioDescarregamento();
    });

    $("#btnExcluirChaveCTeMunicipioDescarregamento").click(function () {
        ExcluirChaveCTeMunicipioDescarregamento();
    });

    $("#btnCancelarChaveCTeMunicipioDescarregamento").click(function () {
        LimparCamposChaveCTeMunicipioDescarregamento();
    });

    $("#btnSalvarAlteracoesChaveCTesMunicipioDescarregamento").click(function () {
        SalvarAlteracoesChaveCTesMunicipioDescarregamento();
    });

    $("#btnCancelarAlteracoesChaveCTesMunicipioDescarregamento").click(function () {
        if ($("body").data("ChaveCTeMunicipioDescarregamentoAlterado") != null && $("body").data("ChaveCTeMunicipioDescarregamentoAlterado") == true) {
            jConfirm("Deseja realmente descartar as alterações realizadas nas chaves de CT-es deste município?", "Atenção!", function (r) {
                if (r)
                    FecharTelaChaveCTesMunicipioDescarregamento();
            });
        } else {
            FecharTelaChaveCTesMunicipioDescarregamento();
        }
    });

    $("#btnFecharTelaChaveCTesMunicipioDescarregamento").click(function () {
        $("#btnCancelarAlteracoesChaveCTesMunicipioDescarregamento").trigger("click");
    });
});

function AbrirTelaChaveCTesMunicipioDescarregamento(municipio) {
    LimparCamposChaveCTeMunicipioDescarregamento();

    $("body").data("municipioDescarregamentoChaveCTe", municipio);

    $("body").data("chaveCTeMunicipioDescarregamentoAlterado", false);

    RenderizarChaveCTesMunicipioDescarregamento();

    $("#tituloMunicipioDescarregamentoChaveCTe").text("Município: " + municipio.DescricaoMunicipio);

    $("#divChaveCTesMunicipioDescarregamento").modal({ keyboard: false, backdrop: 'static' });
}

function ValidarCamposChaveCTeMunicipioDescarregamento() {
    var chave = $("#txtChaveCTeMunicipioDescarregamento").val().replace(/[^0-9]/g, '');
    var valido = true;

    if (chave != null && chave.length == 44) {
        CampoSemErro("#txtChaveCTeMunicipioDescarregamento");
    } else {
        CampoComErro("#txtChaveCTeMunicipioDescarregamento");
        valido = false;
    }

    return valido;
}

function SalvarChaveCTeMunicipioDescarregamento() {
    if (ValidarCamposChaveCTeMunicipioDescarregamento()) {
        $("body").data("ChaveCTeMunicipioDescarregamentoAlterado", true);

        var chaveCTe = {
            Codigo: $("body").data("municipioDescarregamentoChaveCTeEmEdicao") != null ? $("body").data("municipioDescarregamentoChaveCTeEmEdicao").Codigo : 0,
            Chave: $("#txtChaveCTeMunicipioDescarregamento").val().replace(/[^0-9]/g, ''),
            Excluir: false
        };

        var municipioDescarregamento = $("body").data("municipioDescarregamentoChaveCTe");

        for (var i = 0; i < municipioDescarregamento.ChaveCTes.length; i++) {
            if (municipioDescarregamento.ChaveCTes[i].Chave == chaveCTe.Chave && municipioDescarregamento.ChaveCTes[i].Codigo != chaveCTe.Codigo && municipioDescarregamento.ChaveCTes[i].Excluir == false) {
                ExibirMensagemAlerta("Esta chave de CT-e já foi utilizada.", "Atenção!", "placeholder-msgChaveCTesMunicipioDescarregamento");
                return;
            }
        }

        municipioDescarregamento.ChaveCTes.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (chaveCTe.Codigo == 0)
            chaveCTe.Codigo = (municipioDescarregamento.ChaveCTes.length > 0 ? (municipioDescarregamento.ChaveCTes[0].Codigo > 0 ? -1 : (municipioDescarregamento.ChaveCTes[0].Codigo - 1)) : -1);

        for (var i = 0; i < municipioDescarregamento.ChaveCTes.length; i++) {
            if (municipioDescarregamento.ChaveCTes[i].Codigo == chaveCTe.Codigo) {
                municipioDescarregamento.ChaveCTes.splice(i, 1);
                break;
            }
        }

        municipioDescarregamento.ChaveCTes.push(chaveCTe);

        municipioDescarregamento.ChaveCTes.sort(function (a, b) { return a.Chave < b.Chave ? -1 : 1; });

        $("body").data("municipioDescarregamentoChaveCTe", municipioDescarregamento);

        RenderizarChaveCTesMunicipioDescarregamento();
        LimparCamposChaveCTeMunicipioDescarregamento();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgChaveCTesMunicipioDescarregamento");
    }
}

function EditarChaveCTeMunicipioDescarregamento(chaveCTe) {
    $("body").data("municipioDescarregamentoChaveCTeEmEdicao", chaveCTe);
    $("#txtChaveCTeMunicipioDescarregamento").val(chaveCTe.Chave);
    $("#btnExcluirChaveCTeMunicipioDescarregamento").show();
}

function ExcluirChaveCTeMunicipioDescarregamento() {
    $("body").data("ChaveCTeMunicipioDescarregamentoAlterado", true);

    var chaveCTe = $("body").data("municipioDescarregamentoChaveCTeEmEdicao");

    var municipioDescarregamento = $("body").data("municipioDescarregamentoChaveCTe");

    for (var i = 0; i < municipioDescarregamento.ChaveCTes.length; i++) {
        if (municipioDescarregamento.ChaveCTes[i].Codigo == chaveCTe.Codigo) {
            if (chaveCTe.Codigo <= 0)
                municipioDescarregamento.ChaveCTes.splice(i, 1);
            else
                municipioDescarregamento.ChaveCTes[i].Excluir = true;
            break;
        }
    }

    $("body").data("municipioDescarregamentoChaveCTe", municipioDescarregamento);

    RenderizarChaveCTesMunicipioDescarregamento();
    LimparCamposChaveCTeMunicipioDescarregamento();
}

function RenderizarChaveCTesMunicipioDescarregamento() {
    var municipioDescarregamento = $("body").data("municipioDescarregamentoChaveCTe");

    $("#tblChaveCTesMunicipioDescarregamento tbody").html("");

    if (municipioDescarregamento != null && municipioDescarregamento.ChaveCTes != null) {
        for (var i = 0; i < municipioDescarregamento.ChaveCTes.length; i++) {
            if (!municipioDescarregamento.ChaveCTes[i].Excluir)
                $("#tblChaveCTesMunicipioDescarregamento tbody").append("<tr><td>" + municipioDescarregamento.ChaveCTes[i].Chave + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarChaveCTeMunicipioDescarregamento(" + JSON.stringify(municipioDescarregamento.ChaveCTes[i]) + ")'>Editar</button></td></tr>");
        }
    }

    if ($("#tblChaveCTesMunicipioDescarregamento tbody").html() == "")
        $("#tblChaveCTesMunicipioDescarregamento tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function LimparCamposChaveCTeMunicipioDescarregamento() {
    $("body").data("municipioDescarregamentoChaveCTeEmEdicao", null);
    $("#txtChaveCTeMunicipioDescarregamento").val('');
    $("#btnExcluirChaveCTeMunicipioDescarregamento").hide();
}

function SalvarAlteracoesChaveCTesMunicipioDescarregamento() {
    var municipioDescarregamento = $("body").data("municipioDescarregamentoChaveCTe");
    var municipiosDescarregamento = $("body").data("municipiosDescarregamento");

    for (var i = 0; i < municipiosDescarregamento.length; i++) {
        if (municipiosDescarregamento[i].Codigo == municipioDescarregamento.Codigo) {
            municipiosDescarregamento[i] = municipioDescarregamento;
            break;
        }
    }

    $("body").data("municipiosDescarregamento", municipiosDescarregamento);

    RenderizarMunicipiosDescarregamento();

    FecharTelaChaveCTesMunicipioDescarregamento();
}

function FecharTelaChaveCTesMunicipioDescarregamento() {
    LimparCamposChaveCTeMunicipioDescarregamento();

    $("body").data("municipioDescarregamentoChaveCTe", null);

    $("body").data("ChaveCTeMunicipioDescarregamentoAlterado", false);

    $("#tituloMunicipioDescarregamentoChaveCTe").text('');

    $("#divChaveCTesMunicipioDescarregamento").modal('hide');
}