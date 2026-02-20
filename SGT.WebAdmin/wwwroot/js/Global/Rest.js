/// <reference path="../libs/jquery-2.1.1.js" />
/// <reference path="Mensagem.js" />
/// <reference path="../libs/jquery.filedownload.js" />

$(document).ajaxStop(function () {
    finalizarRequisicao();
});

function executarReSTGET(url, dados, callback, erroCallback) {

    if (!_RequisicaoIniciada)
        iniciarRequisicao();

    $.ajax({
        type: "GET",
        url: url,
        data: dados,
        dataType: 'jsonp',
        success: callback,
        error: function (x) {
            if (erroCallback != null)
                erroCallback(x);
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelRealizarUmaRequisicaoParaServidorErro.format(x.status, x.statusText));
        }
    });
}

function executarReST(url, dados, callback, erroCallback, exibirLoading) {

    if (exibirLoading !== false) {
        if (!_RequisicaoIniciada) {
            iniciarRequisicao();
        }
    }

    return $.ajax({
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
            if (x.status == 401)
                window.location.href = "Login";

            if (erroCallback != null)
                erroCallback(x);
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelRealizarUmaRequisicaoParaServidorErro.format(x.status, x.statusText));
        }
    });
}

function enviarArquivo(url, dados, arquivo, callback, erroCallback, controlarRequisicao) {

    if (controlarRequisicao == null)
        controlarRequisicao = true;

    if (controlarRequisicao && !_RequisicaoIniciada) {
        iniciarRequisicao();
    }

    if (dados != null) {
        if (!(url.indexOf("?") > -1))
            url += "?";

        $.each(dados, function (i, data) {
            url += "&" + i + "=" + data;
        })
    }

    // Converte # em url code (%23)
    url = url.replace(/#/g, "%23");

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
            if (x.status == 401)
                window.location.href = "Login";

            if (erroCallback != null)
                erroCallback(x);
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelRealizarUmaRequisicaoParaServidorErro.format(x.status, x.statusText));
        },
        cache: false,
        contentType: false,
        processData: false
    });
}

function executarDownloadPost(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading) {
    return executarDownload(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading, true)
}

function executarDownload(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading, executarComoMetodoPost) {
    if (!_RequisicaoIniciada)
        iniciarRequisicao();

    if (!$.fileDownload) return;

    if (exibirLoading == null)
        exibirLoading = true;

    var isGet = !executarComoMetodoPost;

    $.fileDownload(ObterPath() + relativeUrl, {
        httpMethod: isGet ? 'GET' : 'POST',
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

                    if ((/^<pre/i).test(html)) //hack para quando vem a tag <pre> (em alguns casos retorna <pre>{...}</pre>, acredito que devido ao server)
                        html = $(html).text();

                    var retorno = JSON.parse(html.replace("(", "").replace(");", ""));

                    if (retorno.Success) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                    }
                } catch (ex) {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.NaoPossivelRealizarDownloadArquivoAtualizePaginaTenteNovamente);
                }
            }
        }
    });
    return false;
}

function executarDownloadArquivo(relativeUrl, dados) {
    executarReST(relativeUrl, dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                BuscarProcessamentosPendentes(function () {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.VoceSolicitouDownloadArquivoAssimTerminarVoceSeraNotificado, 5000);
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
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
