$(document).ready(function () {
    InicializarCampoConsulta("TecnologiaRastreador");
    InicializarCampoConsulta("TipoComunicacaoRastreador");
    $("#chkPossuiRastreador").click(function () {
        MostrarOcultarCamposRastreador();
    });
});

function LoadRastreadorVeiculo(veiculo) {
    if (veiculo) {
        $("#chkPossuiRastreador").prop("checked", veiculo.PossuiRastreador);
        if (veiculo.TecnologiaRastreador) {
            $("#hddCodigoTecnologiaRastreador").val(veiculo.TecnologiaRastreador.Codigo);
            $("#txtTecnologiaRastreador").val(veiculo.TecnologiaRastreador.Descricao);
            $("#hddCodigoTipoComunicacaoRastreador").val(veiculo.TipoComunicacaoRastreador.Codigo);
            $("#txtTipoComunicacaoRastreador").val(veiculo.TipoComunicacaoRastreador.Descricao);
        }
        $("#txtNumeroEquipamentoRastreador").val(veiculo.NumeroEquipamentoRastreador);
    } else {
        $("#chkPossuiRastreador").prop("checked", false);
    }
    MostrarOcultarCamposRastreador();
}

function LimparRastreadorVeiculo() {
    $("#chkPossuiRastreador").prop("checked", false);
    MostrarOcultarCamposRastreador();
    $("#txtTecnologiaRastreador").val("");
    $("#hddCodigoTecnologiaRastreador").val("");
    $("#txtTipoComunicacaoRastreador").val("");
    $("#hddCodigoTipoComunicacaoRastreador").val("");
    $("#txtNumeroEquipamentoRastreador").val("");
}

function ValidarRastreadorVeiculo() {
    if ($("#chkPossuiRastreador").is(":checked")) {
        return $("#hddCodigoTecnologiaRastreador").val() != "" && $("#hddCodigoTipoComunicacaoRastreador").val() != "" && $("#txtNumeroEquipamentoRastreador").val() != "";
    }
    return true;
}
function MostrarOcultarCamposRastreador() {
    if ($("#chkPossuiRastreador").is(":checked")) {
        $("#infoRastreador").show();
    } else {
        $("#infoRastreador").hide();
    }
}

function InicializarCampoConsulta(nome, functionConsulta) {
    var campoBtn = "btnBuscar" + nome;
    var campoFn = "CarregarConsultaDe" + nome;
    var campoTxtObj = $("#txt" + nome);
    var campoHddObj = $("#hddCodigo" + nome);

    if (campoTxtObj && campoHddObj) {
        var fn = window[campoFn];
        if (typeof fn === 'function') {
            fn(campoBtn, campoBtn, function (row) {
                campoTxtObj.val(row.Descricao);
                campoHddObj.val(row.Codigo);
            }, true, false);
        }

        campoTxtObj.keydown(function (e) {
            if (e.which != 9 && e.which != 16) {
                if (e.which == 8 || e.which == 46) {
                    $(this).val("");
                    campoHddObj.val("");
                } else {
                    e.preventDefault();
                }
            }
        });
    }
}