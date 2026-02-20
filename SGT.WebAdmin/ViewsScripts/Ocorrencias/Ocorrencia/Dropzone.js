/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="NFSComplementar.js" />



//*******MAPEAMENTO*******


//*******MAPEAMENTO KNOUCKOUT*******

var GlobalPercentualPreCTeOcorrencia = 0;
var _dropzone = null;
//*******EVENTOS*******

function loadDropZonePreCTeOcorrencia() {
    if (_dropzone != null) return _dropzone.removeAllFiles();

    _dropzone = $("#" + _documentoComplementar.Dropzone.id).dropzone({
        dictDefaultMessage: '<span class="text-center"><span class="font-lg"><span class="font-md"><i class="fa fa-caret-right text-danger"></i> Arraste os arquivos <span class="font-xs">para o envio</span></span><span><br/><h5 class="display-inline" style="margin-top:-10px"> (ou clique e selecione)</h5></span>',
        dictResponseError: 'Falha ao enviar arquivos!',
        acceptedFiles: ".xml,.txt,.xlsx",
        dictInvalidFileType: 'A extensão do arquivo é inválida.',
        processing: function () {
            this.options.url = "OcorrenciaPreCTe/EnviarCTesParaPreCTe?Ocorrencia=" + _ocorrencia.Codigo.val();
        },
        success: DropZoneSucessPreCTeOcorrencia,
        uploadMultiple: true,
        queuecomplete: DropZoneCompletePreCTeOcorrencia,
        TotaluploadprogressPreCTeOcorrencia: TotaluploadprogressPreCTeOcorrencia,
        url: "OcorrenciaPreCTe/EnviarCTesParaPreCTe?Ocorrencia=" + _ocorrencia.Codigo.val()
    });

    _dropzone = Dropzone.forElement(document.getElementById(_documentoComplementar.Dropzone.id));
}

function TotaluploadprogressPreCTeOcorrencia(percentualProgresso) {
    if (GlobalPercentualPreCTeOcorrencia < Math.round(percentualProgresso)) {
        GlobalPercentualPreCTeOcorrencia = Math.round(percentualProgresso);
        $("#" + _documentoComplementar.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentualPreCTeOcorrencia < 100) {
            $("#" + _documentoComplementar.Dropzone.idTab).css("width", GlobalPercentualPreCTeOcorrencia + "%");
        } else {
            $("#" + _documentoComplementar.Dropzone.idTab).text(Localization.Resources.Ocorrencias.Ocorrencia.FinalizandoEnvio);
            $("#" + _documentoComplementar.Dropzone.idTab).css("width", "100%");
        }
    }
}


function preecherErroPreCTeOcorrencia(mensagem, file) {
    var node, _i, _len, _ref, _results;
    file.previewElement.classList.add("dz-error");
    _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
    _results = [];
    for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        node = _ref[_i];
        _results.push(node.textContent = mensagem);
    }
    return _results;
}

function DropZoneSucessPreCTeOcorrencia(file, response, i, b) {
    var arg = typeof response === 'object' ? response : JSON.parse(response);
    if (arg.Success) {
        if (arg.Data !== false) {
            $.each(arg.Data.Arquivos, function (i, arquivo) {
                if (file.name == arquivo.nome) {
                    if (arquivo.processada)
                        return file.previewElement.classList.add("dz-success");
                    else
                        return preecherErroPreCTeOcorrencia(arquivo.mensagem, file);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            return preecherErroPreCTeOcorrencia(arg.Msg, file);
        }
    } else {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        return preecherErroPreCTeOcorrencia(arg.Msg, file);
    }
}

function DropZoneCompletePreCTeOcorrencia(arg) {
    _gridCargaPreCTeOcorrencia.CarregarGrid();

    $("#" + _documentoComplementar.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _documentoComplementar.Dropzone.idTab).text("");
    $("#" + _documentoComplementar.Dropzone.idTab).css("width", "0%");

    GlobalPercentualPreCTeOcorrencia = 0;
}