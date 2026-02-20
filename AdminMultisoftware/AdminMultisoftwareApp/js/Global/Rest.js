/// <reference path="libs/jquery-2.0.2.min.js" />
/// <reference path="Mensagem.js" />
/// <reference path="../libs/jquery.filedownload.js" />

$(document).ajaxStart(function () {
    //iniciarRequisicao();
}).ajaxStop(function () {
    finalizarRequisicao();
});


function executarReST(url, dados, callback, erroCallback) {

    if (!_RequisicaoIniciada) {
        iniciarRequisicao();
    }
    $.ajax({
        type: "POST",
        url: url,
        data: dados,
        dataType: 'jsonp',
        success: function (retorno) {
            if (retorno.Authorized) {
                callback(retorno);
            } else {
                window.location.href = "Login?ReturnUrl=" + retorno.RedirectURL;
            }
        },
        error: function (x) {
            if (erroCallback != null)
                erroCallback(x);
            else
                exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível realizar uma requisição para o servidor. Erro: " + x.status + " - " + x.statusText);
        }
    });
}

function enviarArquivo(url, dados, arquivo, callback, erroCallback) {

    if (!_RequisicaoIniciada) {
        iniciarRequisicao();
    }

    $.each(dados, function (i, data) {
        url += "&" + i + "=" + data;
    })

    $.ajax({
        url: url,
        type: 'POST',
        data: arquivo,
        dataType: 'jsonp',
        success: function (retorno) {
            if (retorno.Authorized) {
                callback(retorno);
            } else {
                window.location.href = "Login?ReturnUrl=" + retorno.RedirectURL;
            }
        },
        error: function (x) {
            if (erroCallback != null)
                erroCallback(x);
            else
                exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível realizar uma requisição para o servidor. Erro: " + x.status + " - " + x.statusText);
        },
        cache: false,
        contentType: false,
        processData: false
    });
}



function executarDownload(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading) {
    if (!_RequisicaoIniciada) {
        iniciarRequisicao();
    }
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
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.atencao, "Atenção!", retorno.Msg);
                        } else {
                            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
                        }
                    } catch (ex) {
                        exibirMensagem(tipoMensagem.falha, "Atenção!", "Não foi possível realizar o download do arquivo. Atualize a página e tente novamente.");
                    }
                }
            }
        });
        return false;
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
    path += "/"
    return path;
}
