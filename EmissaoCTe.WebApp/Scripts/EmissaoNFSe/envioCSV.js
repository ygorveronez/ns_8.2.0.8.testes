
$(document).ready(function () {

    InicializarPlUploadCSV();
});

function InicializarPlUploadCSV() {
    documentos = new Array();
    erros = "";
    uploader = new plupload.Uploader({
        runtimes: 'gears,html5,flash,silverlight,browserplus',
        browse_button: 'btnImportarCSV',
        max_file_size: '10000kb',
        multi_selection: true,
        filters: [{ title: "Arquivos CSV", extensions: "csv" }],
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
    });

    uploader.init();

    uploader.bind('FilesAdded', function (up, files) {

        $.each(uploader.files, function (i, file) {
            up.setOption('url', 'NotaFiscalDeServicosEletronica/ImportarNFSeCSV?callback=?');
            uploader.start();

            //$.fancybox.close();
        });

        up.refresh();
    });

    uploader.bind('UploadProgress', function (up, file) {
        $('#' + file.id + " b").html("   (" + file.percent + "%)");
    });

    uploader.bind('FileUploaded', function (up, file, response) {
        $('#' + file.id + " b").html("   (100%)");

        var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
        if (!retorno.Sucesso)
            jAlert(retorno.Erro);
        else {
            ConsultarNFSes();
        }
    });
}