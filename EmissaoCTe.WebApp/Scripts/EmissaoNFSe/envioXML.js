
$(document).ready(function () {

    InicializarPlUploadXML();
});

function InicializarPlUploadXML() {
    documentos = new Array();
    erros = "";
    uploaderXML = new plupload.Uploader({
        runtimes: 'gears,html5,flash,silverlight,browserplus',
        browse_button: 'btnImportarXMLNFSe',
        max_file_size: '10000kb',
        multi_selection: true,
        filters: [{ title: "Arquivos XML", extensions: "xml" }],
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap'
    });

    uploaderXML.init();

    uploaderXML.bind('FilesAdded', function (up, files) {

        $.each(uploaderXML.files, function (i, file) {
            up.setOption('url', 'NotaFiscalDeServicosEletronica/ImportarNFSeXMLAutorizado?callback=?');
            uploaderXML.start();

            //$.fancybox.close();
        });

        up.refresh();
    });

    uploaderXML.bind('UploadProgress', function (up, file) {
        $('#' + file.id + " b").html("   (" + file.percent + "%)");
    });

    uploaderXML.bind('FileUploaded', function (up, file, response) {
        $('#' + file.id + " b").html("   (100%)");

        var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
        if (!retorno.Sucesso)
            jAlert(retorno.Erro);
        else {
            ConsultarNFSes();
        }
    });
}