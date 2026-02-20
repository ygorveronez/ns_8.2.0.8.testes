function executarRest(relativeUrl, dados, callback, exibirLoading) {
    if (exibirLoading == null)
        exibirLoading = true;

    return $.ajax({
        type: "POST",
        url: document.location.protocol + "//" + document.location.host + ObterPath() + relativeUrl,
        data: dados,
        dataType: 'jsonp',
        beforeSend: function () {
            if ($.active <= 1 && exibirLoading)
                iniciarRequisicao();
        },
        success: function (retorno) {
            if (retorno) {
                if (retorno.SessaoExpirada) {
                    location.href = "Logout.aspx";
                }
            }
            callback(retorno);
        },
        error: function (x) {
            //erroCallback(x);
            location.href = 'Logon.aspx?ReturnUrl=' + document.location.pathname;
        }
    }).always(function (data) {
        if ($.active <= 1 && exibirLoading)
            finalizarRequisicao();
    });
}

function executarDownload(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading, placeholder) {
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
                        if (html.substr(0, 4) == "<pre")
                        {
                            var $html = $(html); // quando a string de retorno tiver uma tag html, jquery cria um elemento <pre>
                            html = $html.html();
                        }

                        var retorno = JSON.parse(html.replace("(", "").replace(");", ""));
                        ExibirMensagemErro(retorno.Erro, "Atenção!", placeholder);
                    } catch (ex) {
                        ExibirMensagemErro("Não foi possível realizar o download do arquivo. Atualize a página e tente novamente.", "Atenção!", placeholder);
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
    $.unblockUI();
}