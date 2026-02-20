$(document).ready(function () {
    $("#btnEnviarEmailXMLDacteCTe").click(function () {
        EnviarEmailXMLDacteCTe();
    });
    $("#btnCancelarEnvioEmailXMLDacteCTe").click(function () {
        FecharTelaEnvioEmail();
    });
});
function EnviarEmailXMLDacteCTe() {
    if (ValidarEnvioEmailCTe()) {
        executarRest("/ConhecimentoDeTransporteEletronico/EnviarPorEmail?callback=?", { CodigoCTe: $("#hddCodigoCTE").val(), Emails: $("#txtEmailsEnvioXMLDacteCTe").val() }, function (r) {
            if (r.Sucesso) {
                jAlert("E-mail enviado com sucesso!", "Sucesso");
                FecharTelaEnvioEmail();
            } else {
                ExibirMensagemErro(r.Erro, "Erro!", "placeholder-msgEnvioEmail");
            }
        });
    }
}
function EnviarDacteEXMLParaEmailsCadastrados(cte) {
    jConfirm("Deseja realmente enviar a DACTE e o XML do CT-e nº <b>" + cte.data.Numero + "</b> para os e-mails cadastrados?", "Atenção", function (ret) {
        if (ret) {
            executarRest("/ConhecimentoDeTransporteEletronico/EnviarEmailParaTodos?callback=?", { CodigoCTe: cte.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    jAlert("E-mail enviado com sucesso!", "Sucesso");
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
    });
}
function ValidarEnvioEmailCTe() {
    var emails = $("#txtEmailsEnvioXMLDacteCTe").val().split(';');
    var valido = true;
    for (var i = 0; i < emails.length; i++) {
        if (!ValidarEmail(emails[i])) {
            valido = false;
            break;
        }
    }
    if (!valido) {
        ExibirMensagemAlerta("E-mail inválido. Confira o(s) e-mail(s) digitado(s) e tente novamente.", "Atenção!", "placeholder-msgEnvioEmail");
        CampoComErro("#txtEmailsEnvioXMLDacteCTe");
    } else {
        CampoSemErro("#txtEmailsEnvioXMLDacteCTe");
    }
    return valido;
}
function AbrirTelaEnvioEmail(cte) {
    $("#hddCodigoCTE").val(cte.data.Codigo);
    $("#divEnvioEmailCTe").modal("show");
}
function FecharTelaEnvioEmail() {
    $("#divEnvioEmailCTe").modal("hide");
    $("#txtEmailsEnvioXMLDacteCTe").val('');
    CampoSemErro("#txtEmailsEnvioXMLDacteCTe");
}