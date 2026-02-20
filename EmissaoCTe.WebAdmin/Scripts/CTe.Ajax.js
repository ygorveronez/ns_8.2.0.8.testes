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

function executarDownload(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading) {
    if ($.fileDownload) {
        if (exibirLoading == null)
            exibirLoading = true;
        $.fileDownload(ObterPath() + relativeUrl, {
            httpMethod: 'GET',
            data: dados,
            prepareCallback: function (url) {
                iniciarRequisicao();
            },
            successCallback: function (url) {
                finalizarRequisicao();
                if (sucessCallback) {
                    sucessCallback(url);
                }
            },
            failCallback: function (html, url) {
                finalizarRequisicao();
                if (errorCallback) {
                    errorCallback(html, url);
                } else {
                    try {
                        var retorno = JSON.parse(html.replace("(", "").replace(");", ""));
                        ExibirMensagemErro(retorno.Erro, "Atenção!");
                    } catch (ex) {
                        ExibirMensagemErro("Não foi possível realizar o download do arquivo. Atualize a página e tente novamente.", "Atenção!");
                    }
                }
            }
        });
    }
}

function ObterPath() {
    var path = "";
    if (document.location.pathname.split("/").length > 1) {
        var paths = document.location.pathname.split("/");
        for (var i = 0; (paths.length - 1) > i; i++) {
            if (paths[i] != "") {
                path += "/" + paths[i];
            }
        }
    }
    return path;
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