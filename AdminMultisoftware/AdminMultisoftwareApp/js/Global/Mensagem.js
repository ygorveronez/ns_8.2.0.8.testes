/// <reference path="notification/SmartNotification.min.js" />

var tipoMensagem = {ok : "ok", atencao : "atencao", aviso : "aviso", falha : "falha"};

function exibirAlertaNotificacao(titulo, icone, data, timeout) {
    if (titulo.length > 50) {
        titulo = titulo.substring(0, 50) + "...";
    }
    $.sound_on = false;
    $.smallBox({
        title: titulo,
        content: "<i class='fa fa-clock-o'></i> <i>" + data + "</i>",
        color: "#3B8686",
        iconSmall: icone != "" ? "fa " + icone + " bounce animated" : "",
        timeout: timeout != null ? timeout : 4000,
    });
}

function exibirMensagem(tipo, titulo, mensagem, timeout) {
    $.sound_on = true;
    if (tipo == tipoMensagem.ok) {
        $.smallBox({
            title: titulo,
            content: mensagem,
            color: "#739E73",
            timeout: timeout != null ? timeout : 3000,
            icon: "fa fa-check"
        });
    }

    if (tipo == tipoMensagem.atencao) {
        $.bigBox({
            title: titulo,
            content: mensagem,
            color: "#C79121",
            timeout: timeout != null ? timeout : 6000,
            icon: "fa fa-shield fadeInLeft animated"
        });

    }

    if (tipo == tipoMensagem.aviso) {
        $.bigBox({
            title: titulo,
            content: mensagem,
            timeout: timeout != null ? timeout : 6000,
            color: "#3276B1",
            icon: "fa fa-bell swing animated"
        });

    }

    if (tipo == tipoMensagem.falha) {
        $.bigBox({
            title: titulo,
            content: mensagem,
            color: "#C46A69",
            icon: "fa fa-warning shake animated"
        });
    }
}

function exibirConfirmacao(titulo, mensagem, callbackConfirma, callbackCancela) {
    $.SmartMessageBox({
        title: titulo,
        content: mensagem,
        buttons: "[Não][Sim]"
    }, function (ButtonPress, Value) {
        if (ButtonPress == "Não") {
            if (callbackCancela != null) {
                callbackCancela();
            } else {
                return 0;
            }
        } else {
            if (ButtonPress == "Sim") {
                callbackConfirma();
            }
        }
    });

}
var _RequisicaoIniciada = false;
function iniciarRequisicao() {
    _RequisicaoIniciada = true;
    $.blockUI({
        message: '<div class="progress progress-striped active"><div class="progress-bar" role="progressbar" aria-valuenow="100" aria-valuemin="100" aria-valuemax="100" style="width: 100%"><span>Processando, aguarde...</span></div></div>',
        baseZ: 99999,
        css: {
            border: 'none',
            padding: '0',
            opacity: 1,
            height: 0,
            cursor: 'default'
        },
        overlayCSS: {
            cursor: 'default',
            opacity: 0.2
        }
    });
}

function finalizarRequisicao() {
    _RequisicaoIniciada = false;
    $.unblockUI();

}