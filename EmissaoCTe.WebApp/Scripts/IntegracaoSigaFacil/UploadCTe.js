var path = "";
var countArquivos = 0;
var errosEnvioXMLCTe, ctesImportados;
$(document).ready(function () {
    if (document.location.pathname.split("/").length > 1) {
        var paths = document.location.pathname.split("/");
        for (var i = 0; (paths.length - 1) > i; i++) {
            if (paths[i] != "") {
                path += "/" + paths[i];
            }
        }
    }

    $("#btnImportarCTe").click(function () {
        InicializarPlUploadCTe();
        AbrirPlUpload();
    });
});
function InicializarPlUploadCTe() {
    errosEnvioXMLCTe = "";
    ctesImportados = new Array();
    $("#divUploadArquivos").pluploadQueue({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        url: path + '/IntegracaoSigaFacil/ImportarCTeCIOT?callback=?',
        max_file_size: '500kb',
        unique_names: true,
        filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        init: {
            FileUploaded: function (up, file, info) {
                var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                if (retorno.Sucesso) {
                    ctesImportados.push(retorno.Objeto);
                } else {
                    errosEnvioXMLCTe += retorno.Erro + "<br />";
                }
            },
            StateChanged: function (up) {
                if (up.state != plupload.STARTED) {
                    if (errosEnvioXMLCTe.trim() != "") {
                        jAlert("Ocorreram as seguintes falhas na importação dos arquivos: <br /><br />" + errosEnvioXMLCTe + "<br />", "Atenção");
                    } else {
                        jAlert("CTe importado com sucesso!", "Sucesso", function () { $("#modalUploadArquivos").modal("hide"); AtualizarGridCIOT(); });
                    }
                }
            }
        }
    });
}

function AbrirPlUpload() {
    $("#modalUploadArquivos").modal("show");
}
