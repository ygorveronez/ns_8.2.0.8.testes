$(document).ready(function () {
    $("#btnSalvarCancelamentoCIOT").click(function () {
        CancelarCIOT();
    });

    $("#btnCancelarCancelamentoCIOT").click(function () {
        FecharTelaCancelamentoCIOT();
    });
});

function CancelarCIOT() {
    if ($("#txtMotivoCancelamentoCIOT").val().length < 20) {
        ExibirMensagemErro("O motivo do cancelamento do CIOT deve possuir no mínimo 20 caracteres.", "Atenção!", "messages-placeholder-cancelamentoCIOT");
        return;
    }

    executarRest("/IntegracaoSigaFacil/Cancelar?callback=?", { CodigoCIOT: $("body").data("codigoCIOTCancelamento"), MotivoCancelamento: $("#txtMotivoCancelamentoCIOT").val() }, function (r) {

        if (r.Sucesso) {
            ExibirMensagemSucesso("Cancelamento solicitado com sucesso!", "Sucesso!");
            AtualizarGridCIOT();
            FecharTelaCancelamentoCIOT();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder-cancelamentoCIOT");
        }
    });
}

function AbrirTelaCancelamentoCIOT(ciot) {
    var statusCancelamento = ["Aberto", "Autorizado", "Evento Rejeitado", "Rejeitado", "Encerrado"];
    var podeCancelar = $.inArray(ciot.data.Status, statusCancelamento) >= 0;

    if (podeCancelar) {
        $("#tituloCancelamento").text("Cancelamento do CIOT Nº " + ciot.data.Numero);
        $("body").data("codigoCIOTCancelamento", ciot.data.Codigo);

        $("#divCancelamentoCIOT").modal({ keyboard: false, backdrop: 'static' });
    } else {
        jAlert("Não é possivel cancelar o CIOT #" + ciot.data.Numero + "<br>O status do CIOT não permite o cancelamento.", "Atenção!");
    }
}

function FecharTelaCancelamentoCIOT() {
    $("#divCancelamentoCIOT").modal('hide');
    LimparCamposCancelamentoCIOT();
}

function LimparCamposCancelamentoCIOT() {
    $("#tituloCancelamento").text("");
    $("body").data("codigoCIOTCancelamento", null);
}
