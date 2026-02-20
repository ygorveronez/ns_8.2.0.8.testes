$(document).ready(function () {
    $("#btnDownloadEspelho").click(function () {
        DownloadEspelho();
    });
});

function DownloadEspelho() {
    executarDownload("/Coleta/DownloadEspelho", { CodigoColeta: $("body").data("codigo") });
}
