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
/// <reference path="Liquidacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var GlobalPercentual = 0;
var dropzone;

//*******EVENTOS*******

function loadDropZone() {
    $("#" + _liquidacaoPallet.Dropzone.id).dropzone({
        init: function () {
            dropzone = this;
            LimparDropzoneLiquidacao();
        },
        dictDefaultMessage: '<span class="text-center"><span class="font-lg"><span class="font-md"><i class="fa fa-caret-right text-danger"></i> Arraste os arquivos <span class="font-xs">para o envio</span></span><span><br/><h5 class="display-inline" style="margin-top:-10px"> (ou clique e selecione)</h5></span>',
        dictResponseError: 'Falha ao enviar arquivos!',
        acceptedFiles: ".png,.jpg,.jpeg,.pdf,.PDF",
        dictInvalidFileType: 'A extensão do arquivo é inválida.',
        success: DropZoneSucess,
        uploadMultiple: true,
        clickable: true,
        processing: function () {
            this.options.url = `Devolucao/EnviarPallet`
        },
        queuecomplete: DropZoneComplete,
        totaluploadprogress: Totaluploadprogress,
        url: `Devolucao/EnviarPallet`
    });
}

function Totaluploadprogress(percentualProgresso) {
    if (GlobalPercentual < Math.round(percentualProgresso)) {
        GlobalPercentual = Math.round(percentualProgresso);
        $("#" + _liquidacaoPallet.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentual < 100) {
            $("#" + _liquidacaoPallet.Dropzone.idTab).css("width", GlobalPercentual + "%");
        } else {
            $("#" + _liquidacaoPallet.Dropzone.idTab).text("Finalizando Envio...");
            $("#" + _liquidacaoPallet.Dropzone.idTab).css("width", "100%");
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
            const palletsAtuais = _liquidacaoPallet.Pallets.val();
            const palletsNovos = arg.Data;

            palletsNovos.forEach((novoPallet) => {
                const [existePalet] = palletsAtuais.filter(pallet => pallet.NomeArquivo === novoPallet.NomeArquivo);
                if (existePalet == null && novoPallet.Processado && novoPallet.PDF)
                    palletsAtuais.push(novoPallet);
            })

            _liquidacaoPallet.Pallets.val(palletsAtuais);
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

    $("#" + _liquidacaoPallet.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _liquidacaoPallet.Dropzone.idTab).text("");
    $("#" + _liquidacaoPallet.Dropzone.idTab).css("width", "0%");

    GlobalPercentual = 0;
}

function LimparDropzoneLiquidacao() {
    dropzone.removeAllFiles(true);
};

function obterArquivosProcessados() {
    return dropzone.getAcceptedFiles();
}