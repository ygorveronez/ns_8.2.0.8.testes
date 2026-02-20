$(document).ajaxStart(function () {
    iniciarRequisicao();
}).ajaxStop(function () {
    finalizarRequisicao();
});


function executarRest(relativeUrl, dados, callback, erroCallback) {
    var path = "";
    if (document.location.pathname.split("/").length > 1) {
        var paths = document.location.pathname.split("/");
        for (var i = 0; (paths.length - 1) > i; i++) {
            if (paths[i] != "") {
                path += "/" + paths[i];
            }
        }
    }
    $.ajax({
        type: "POST",
        url: document.location.protocol + "//" + document.location.host + path + relativeUrl,
        data: dados,
        dataType: 'jsonp',
        success: function (retorno) {
            if (retorno) {
                if (retorno.SessaoExpirada) {
                    location.href = "Logout.aspx";
                }
            }
            callback(retorno);
        },
        error: function (x) {
            if (x.status == 200) {
                location.href = 'Logon.aspx?ReturnUrl=' + document.location.pathname;
            } else {
                erroCallback(x);
            }
        }
    });
}

function iniciarRequisicao() {
    $.blockUI({
        message: "Processando, aguarde...",
        baseZ: 99999,
        css: {
            border: 'none',
            padding: '5px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: .8,
            color: '#fff',
            cursor: 'default'
        },
        overlayCSS: {
            cursor: 'default',
            opacity: 0.4
        }
    });
}

function finalizarRequisicao() {
    $.unblockUI();
}