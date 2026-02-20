var countArquivosCTe = 0;
$(document).ready(function () {
    $("#btnImportarXMLCTe").click(function () {
        AbrirDivUploadArquivosXMLCTe();
    });
});
function InicializarPlUploadCTe() {
    countArquivosCTe = 0;
    countArquivosCTeImportados = 0;
    codigoCTeGerado = 0;
    var urlBase = path + '/ConhecimentoDeTransporteEletronico/GerarPreCTe?callback=?';

    $("#divUploadArquivosBody").pluploadQueue({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        url: urlBase,
        max_file_size: '500kb',
        unique_names: true,
        filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        init: {
            FilesAdded: function (up, files) {
                countArquivosCTe += files.length;
                //if (countArquivosCTe > 1) {
                //    $(".plupload_start").css("display", "none");
                //    jAlert('O sistema só permite enviar um arquivo por vez. Remova os demais!', 'Atenção');
                //}
            },
            FilesRemoved: function (up, files) {
                countArquivosCTe -= files.length;
                if (countArquivosCTe <= 1) {
                    $(".plupload_start").css("display", "");
                }
            },
            FileUploaded: function (up, file, info) {
                var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                if (retorno.Sucesso && countArquivosCTe == 1) {
                    jAlert("O CT-e foi importado com sucesso! CT-e Número: <b>" + retorno.Objeto.Numero + "</b>.<br/><br/>Clique em OK para prosseguir com a edição do CT-e.", "Sucesso", function () {
                        AtualizarGridCTes();
                        EditarCTe(retorno.Objeto.Codigo);
                        FecharDivUploadArquivosXMLCTe();
                    });
                }
                else if (retorno.Sucesso && countArquivosCTe > 1) {
                    countArquivosCTeImportados += 1;               

                    if ($("#selAgrupamentoCTe").val() == "1")
                        codigoCTeGerado = retorno.Objeto.Codigo;                    

                    if (countArquivosCTeImportados >= countArquivosCTe) {
                        if ($("#selAgrupamentoCTe").val() == "1") {
                            jAlert("CT-es importados com sucesso! CT-e Número: <b>" + retorno.Objeto.Numero + "</b>", "Sucesso", function () {
                                AtualizarGridCTes();
                                FecharDivUploadArquivosXMLCTe();
                            });
                        }
                        else {
                            jAlert("CT-es importados com sucesso!", "Sucesso", function () {
                                AtualizarGridCTes();
                                FecharDivUploadArquivosXMLCTe();
                            });
                        }
                    }
                } else {
                    file.status = plupload.FAILED;
                    jAlert(retorno.Erro, "Falha no Envio", function () {
                        AtualizarGridCTes();
                        FecharDivUploadArquivosXMLCTe();
                    });
                    up.trigger('UploadProgress', file);
                }
            },
            BeforeUpload: function (up, file) {
                var novaUrl = urlBase + '&CodigoCTe=' + codigoCTeGerado;
                up.setOption('url', novaUrl);
            }
        }
    });
}
function AbrirDivUploadArquivosXMLCTe() {
    InicializarPlUploadCTe();
    document.getElementById('divAgrupamentoXML').style.visibility = 'hidden';
    document.getElementById('divImportacaoCTe').style.visibility = "visible";

    $("#tituloUploadArquivos").text("Envio de XML de Conhecimento de Transporte Eletrônico");
    $('#divUploadArquivos').modal("show");
}
function FecharDivUploadArquivosXMLCTe() {
    $('#divUploadArquivos').modal("hide");
}