$(document).ready(function () {
    $("#btnImportarNotaFiscal").on('click', function () {
        AbrirUploadPadrao({
            title: "Importação de NF-e",
            url: '/DocumentoEntrada/ObterDetalhesPorNFe?callback=?',
            filter: [{ title: "Arquivos XML", extensions: "xml" }],
            max_file_size: '2000kb',
            multiple: false,
            onFinish: function (arquivo, erros) {
                BuscarCFOPs();
                BuscarUnidadesDeMedida();
                if (erros.length > 0)
                    ExibirMensagemErro(erros.join("<br>"), "Atenção!<br>");
                else {
                    CarregandoDados = true;
                    RenderizarDocumentoEntrada(arquivo[0]);
                    CarregandoDados = false;
                } 
            }
        });
    });
    
});
/* CODIGO ANTIGO, REMOVIDO 11/04/2017
var uploader = null;
function InicializarPlUpload() {
    var path = "";
    if (document.location.pathname.split("/").length > 1) {
        var paths = document.location.pathname.split("/");
        for (var i = 0; (paths.length - 1) > i; i++) {
            if (paths[i] != "") {
                path += "/" + paths[i];
            }
        }
    }

    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
        browse_button: 'btnImportarNotaFiscal',
        container: 'xmlUpload',
        url: path + '/DocumentoEntrada/ObterDetalhesPorNFe?callback=?',
        filters: [{ title: "Arquivos XML", extensions: "xml" }],
        max_file_size: '500kb',
        multi_selection: false
    });

    uploader.bind('QueueChanged', function (up) {
        if (uploader.files.length > 1) {
            uploader.splice(0, uploader.files.length);
            ExibirMensagemAlerta("Somente um arquivo deve ser selecionado.");
        } else {
            iniciarRequisicao();
            uploader.start();
        }
    });

    uploader.bind('FileUploaded', function (up, file, response) {
        finalizarRequisicao();

        var retorno = JSON.parse(response.response.replace("});", "}").replace("?({", "{"));

        if (retorno.Sucesso) {
            RenderizarDocumentoEntrada(retorno.Objeto);
        } else {
            ExibirMensagemErro(retorno.Erro, "Atenção!");
        }

        uploader.destroy();

        InicializarPlUpload();
    });

    uploader.bind('Error', function (up, err) {
        ExibirMensagemErro(err.message + (err.file ? " Arquivo: " + err.file.name : ""), "Atenção!");

        finalizarRequisicao();
        uploader.destroy();

        InicializarPlUpload();
    });

    uploader.init();
}*/