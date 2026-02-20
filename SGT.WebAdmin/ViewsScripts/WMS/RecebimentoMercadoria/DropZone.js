/// <autosync enabled="true" />
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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../js/plugin/dropzone/dropzone.js" />
/// <reference path="../../../js/plugin/dropzone/dropzone-amd-module.min.js" />
/// <reference path="RecebimentoMercadoria.js" />
/// <reference path="Mercadoria.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var GlobalPercentual = 0;


//*******EVENTOS*******

function loadDropZone() {

    $("#" + _recebimentoMercadoria.Dropzone.id).dropzone({
        init: function () {
            var dropzone = this;
            //clearDropzone = function () {
            LimparDropzone = function () {
                dropzone.removeAllFiles(true);
            };
        },
        dictDefaultMessage: '<span class="text-center"><span class="font-lg"><span class="font-md"><i class="fal fa-caret-right text-danger"></i> <h1>Arraste os arquivos <span class="font-xs">para o envio</h1></span></span><spzan><br/><h5 class="display-inline" style="margin-top:-10px"> (ou clique e selecione)</h5></span>',
        dictResponseError: 'Falha ao enviar arquivos!',
        acceptedFiles: ".xml,.txt,.xlsx",
        dictInvalidFileType: 'A extensão do arquivo é inválida.',
        processing: function () {
            this.options.url = "RecebimentoMercadoria/EnviarArquivo?Recebimento=" + _recebimentoMercadoria.Codigo.val() + "&TipoRecebimento=" + _recebimentoMercadoria.TipoRecebimento.val() + "&ProdutoEmbarcador=" + _recebimentoMercadoria.ProdutoEmbarcador.codEntity()
        },
        success: DropZoneSucess,
        uploadMultiple: true,
        clickable: true,
        queuecomplete: DropZoneComplete,
        totaluploadprogress: Totaluploadprogress,
        url: "RecebimentoMercadoria/EnviarArquivo?Recebimento=" + _recebimentoMercadoria.Codigo.val() + "&TipoRecebimento=" + _recebimentoMercadoria.TipoRecebimento.val() + "&ProdutoEmbarcador=" + _recebimentoMercadoria.ProdutoEmbarcador.codEntity()
    });
}

function Totaluploadprogress(percentualProgresso) {
    if (GlobalPercentual < Math.round(percentualProgresso)) {
        GlobalPercentual = Math.round(percentualProgresso);
        $("#" + _recebimentoMercadoria.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentual < 100) {
            $("#" + _recebimentoMercadoria.Dropzone.idTab).css("width", GlobalPercentual + "%");
        } else {
            $("#" + _recebimentoMercadoria.Dropzone.idTab).text("Finalizando Envio...");
            $("#" + _recebimentoMercadoria.Dropzone.idTab).css("width", "100%");
        }
    }
}


function preecherErro(mensagem, file) {
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

function DropZoneSucess(file, response, i, b) {
    var arg = typeof response === 'object' ? response : JSON.parse(response);
    if (arg.Success) {
        if (arg.Data !== false) {
            console.log("codigo merc DropZoneSucess ", _recebimentoMercadoria.Codigo.val());
            _recebimentoMercadoria.Codigo.val(arg.Data.CodigoRecebimento);

            if (_recebimentoMercadoria.Codigo.val() > 0) {
                _gridMercadoria.CarregarGrid();
                _gridVolume.CarregarGrid();
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            return preecherErro(arg.Msg, file);
        }
    } else {
        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        return preecherErro(arg.Msg, file);
    }
}

function DropZoneComplete(arg) {
    console.log("codigo merc DropZoneComplete ", _recebimentoMercadoria.Codigo.val());
    if (_recebimentoMercadoria.Codigo.val() > 0) {
        _gridMercadoria.CarregarGrid();
        _gridVolume.CarregarGrid();
    }

    $("#" + _recebimentoMercadoria.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _recebimentoMercadoria.Dropzone.idTab).text("");
    $("#" + _recebimentoMercadoria.Dropzone.idTab).css("width", "0%");

    GlobalPercentual = 0;
}