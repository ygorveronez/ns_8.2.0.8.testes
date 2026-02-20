//*******MAPEAMENTO OPN*******

var _downloadDocumentosCargaExterno;

var DownloadDocumentosCargaExterno = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DownloadLoteXMLMDFe = PropertyEntity({ eventClick: DownloadLoteXMLMDFeClick, type: types.event });
    this.DownloadLoteDAMDFE = PropertyEntity({ eventClick: DownloadLoteDAMDFEClick, type: types.event });
    this.DownloadLoteDocumentosMDFe = PropertyEntity({ eventClick: DownloadLoteDocumentosMDFeClick, type: types.event });
    this.DownloadLoteXMLCTe = PropertyEntity({ eventClick: DownloadLoteXMLCTeClick, type: types.event });
    this.DownloadLoteDACTE = PropertyEntity({ eventClick: DownloadLoteDACTEClick, type: types.event });
    this.DownloadLoteDocumentosCte = PropertyEntity({ eventClick: DownloadLoteDocumentosCTeClick, type: types.event });
}

//*******MAPEAMENTO END*******

//*******EVENTOS OPEN*******

var LoadDownloadDocumentosCargaExterno = function (codigoCarga) {
    _downloadDocumentosCargaExterno = new DownloadDocumentosCargaExterno();

    _downloadDocumentosCargaExterno.Carga.val(codigoCarga);

    KoBindings(_downloadDocumentosCargaExterno, "knockoutDownloadDocumentosCargaExterno");
}

//*******EVENTOS END*******

//*******MÉTODOS OPEN*******

function DownloadLoteXMLMDFeClick(e) {
    executarDownloadEspecificoDownloadTelaExternaDoSistemaSemUsarOComponente("/downloads-documentos-carga/DownloadLoteXMLMDFe", { Carga: e.Carga.val() });
}

function DownloadLoteDAMDFEClick(e) {
    executarDownloadEspecificoDownloadTelaExternaDoSistemaSemUsarOComponente("/downloads-documentos-carga/DownloadLoteDAMDFE", { Carga: e.Carga.val() });
}

function DownloadLoteDocumentosMDFeClick(e) {
    executarDownloadEspecificoDownloadTelaExternaDoSistemaSemUsarOComponente("/downloads-documentos-carga/DownloadLoteDocumentosMDFe", { Carga: e.Carga.val() });
}

function DownloadLoteXMLCTeClick(e) {
    executarDownloadEspecificoDownloadTelaExternaDoSistemaSemUsarOComponente("/downloads-documentos-carga/DownloadLoteXMLCTe", { Carga: e.Carga.val() });
}

function DownloadLoteDACTEClick(e) {
    executarDownloadEspecificoDownloadTelaExternaDoSistemaSemUsarOComponente("/downloads-documentos-carga/DownloadLoteDACTE", { Carga: e.Carga.val() });
}

function DownloadLoteDocumentosCTeClick(e) {
    executarDownloadEspecificoDownloadTelaExternaDoSistemaSemUsarOComponente("/downloads-documentos-carga/DownloadLoteDocumentosCTe", { Carga: e.Carga.val() });
}

//*******MÉTODOS END*******

function executarDownloadEspecificoDownloadTelaExternaDoSistemaSemUsarOComponente(relativeUrl, dados, sucessCallback, errorCallback, exibirLoading, executarComoMetodoPost) {
    if (!$.fileDownload) return;

    if (exibirLoading == null)
        exibirLoading = true;

    var isGet = !executarComoMetodoPost;

    $.fileDownload(relativeUrl, {
        httpMethod: isGet ? 'GET' : 'POST',
        data: dados,
        prepareCallback: function (url) {
            //iniciarRequisicao();
        },
        successCallback: function (url) {
            //finalizarRequisicao();
            if (sucessCallback) {
                sucessCallback(url);
            }
        },
        failCallback: function (html, url) {
            //finalizarRequisicao();
            if (errorCallback) {
                errorCallback(html, url);
            } else {
                try {
                    if ((/^<pre/i).test(html)) //hack para quando vem a tag <pre> (em alguns casos retorna <pre>{...}</pre>, acredito que devido ao server)
                        html = $(html).text();

                    var retorno = JSON.parse(html.replace("(", "").replace(");", ""));

                    if (retorno.Success) {
                        exibirMensagem(TipoMensagem.Warning, "Atenção", retorno.Msg);
                    } else {
                        exibirMensagem(TipoMensagem.Danger, "Falha", retorno.Msg);
                    }
                } catch (ex) {
                    exibirMensagem(null, "Atenção", ex.message);
                }
            }
        }
    });

    return false;
}