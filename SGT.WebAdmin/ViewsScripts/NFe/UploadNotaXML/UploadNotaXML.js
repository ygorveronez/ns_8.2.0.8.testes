/// <reference path="DropZone.js" />

//#region Variaveis Globais
var _uploadNotaXML;
var dropzone;
var GlobalPercentual = 0;
var corpoDropZone = '<span class="text-center"><span class="font-lg"><span class="font-md"><i class="fa fa-caret-right text-danger"></i> Arraste os arquivos <span class="font-xs">para o envio</span></span><span><br/><h5 class="display-inline" style="margin-top:-10px"> (ou clique e selecione)</h5></span>';
//#endregion

//#region Constructores

function UploadNotaXML() {
    this.Dropzone = PropertyEntity({ type: types.local, id: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

function LoadDropZoneEnvioXml() {
    _uploadNotaXML = new UploadNotaXML();
    KoBindings(_uploadNotaXML, "knockoutEnvioXMLNota");


    $("#" + _uploadNotaXML.Dropzone.id).dropzone({
        init: function () {
            dropzone = this;
            LimparDropzonexML();
        },
        dictDefaultMessage: corpoDropZone,
        dictResponseError: 'Falha ao enviar arquivos!',
        acceptedFiles: ".xml",
        dictInvalidFileType: 'A extensão do arquivo é inválida.',
        success: DropZoneSucessNota,
        uploadMultiple: true,
        clickable: true,
        processing: function () {
            this.options.url = `UploadNotaXML/ProcessarXmlParaVinculacao`
        },
        queuecomplete: DropZoneCompleteNota,
        totaluploadprogress: TotaluploadprogressNota,
        url: `UploadNotaXML/ProcessarXmlParaVinculacao`
    });

}

//#endregion


//#region Funções auxilizares

function DropZoneSucessNota(file, response, i, b) {

    var arg = typeof response === 'object' ? response : JSON.parse(response);

    if (!arg.Success)
        return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

    return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
}


function DropZoneCompleteNota(arg) {
    $("#" + _uploadNotaXML.Dropzone.id).html('<div class="dz-default dz-message"><button class="dz-button" type="button"><span class="text-center"><span class="font-lg"><span class="font-md"><i class="fa fa-caret-right text-danger"></i> Arraste os arquivos <span class="font-xs">para o envio</span></span><span><br><h5 class="display-inline" style="margin-top:-10px"> (ou clique e selecione)</h5></span></span></span></button></div>');
    LimparDropzonexML();
    GlobalPercentual = 0;
}

function TotaluploadprogressNota(percentualProgresso) {
    if (GlobalPercentual < Math.round(percentualProgresso)) {
        GlobalPercentual = Math.round(percentualProgresso);
        $("#" + _uploadNotaXML.Dropzone.id).parent().css("visibility", "visible");
        if (GlobalPercentual < 100) {
            $("#" + _uploadNotaXML.Dropzone.id).css("width", GlobalPercentual + "%");
        } else {
            $("#" + _uploadNotaXML.Dropzone.id).text("Finalizando Envio...");
            $("#" + _uploadNotaXML.Dropzone.id).css("width", "100%");
        }
    }
}

function LimparDropzonexML() {
    dropzone.removeAllFiles(true);
};
//#endregion
