/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../../js/plugin/dropzone/dropzone.js" />
/// <reference path="../../../../js/plugin/dropzone/dropzone-amd-module.min.js" />
/// <reference path="DropZone.js" />
/// <reference path="TituloFinanceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var GlobalPercentual = 0;


//*******EVENTOS*******

function loadDropZoneTitulo() {

    $("#" + _tituloFinanceiro.Dropzone.id).dropzone({
        init: function () {
            let dropzone = this;
            //clearDropzone = function () {
            LimparDropzone = function () {
                dropzone.removeAllFiles(true);
            };
        },
        dictDefaultMessage: '<i class="fal fa-upload"></i><div class="text-center">Arraste o arquivo de CT-e para o envio <br/> (ou clique e selecione) </div> ',
        dictResponseError: 'Falha ao enviar o arquivo!',
        acceptedFiles: ".xml",
        dictInvalidFileType: 'A extensão do arquivo é inválida.',
        processing: function () {
            this.options.url = "TituloFinanceiro/EnviarArquivoParaEmissao?Titulo=" + _tituloFinanceiro.Codigo.val();
        },
        success: DropZoneSucessTitulo,
        uploadMultiple: false,
        clickable: true,
        queuecomplete: DropZoneCompleteTitulo,
        totaluploadprogress: TotaluploadprogressTitulo,
        url: "TituloFinanceiro/EnviarArquivoParaEmissao?Titulo=" + _tituloFinanceiro.Codigo.val()
    });

    //$("#" + _tituloFinanceiro.Dropzone.id).dropzone({
    //    init: function () {
    //        var dropzone = this;
    //        //clearDropzone = function () {
    //        LimparDropzone = function () {
    //            dropzone.removeAllFiles(true);
    //        };
    //    },
    //    dictDefaultMessage: '<span class="text-center"><span class="font-lg"><span class="font-md"><i class="fa fa-caret-right text-danger"></i> Arraste o arquivo de CT-e <span class="font-xs">para o envio</span></span><span><br/><h5 class="display-inline" style="margin-top:-10px"> (ou clique e selecione)</h5></span>',
    //    dictResponseError: 'Falha ao enviar o arquivo!',
    //    acceptedFiles: ".xml",
    //    dictInvalidFileType: 'A extensão do arquivo é inválida.',
    //    processing: function () {
    //        this.options.url = "TituloFinanceiro/EnviarArquivoParaEmissao?Titulo=" + _tituloFinanceiro.Codigo.val();
    //    },
    //    success: DropZoneSucessTitulo,
    //    uploadMultiple: false,
    //    clickable: true,
    //    queuecomplete: DropZoneCompleteTitulo,
    //    totaluploadprogress: TotaluploadprogressTitulo,
    //    url: "TituloFinanceiro/EnviarArquivoParaEmissao?Titulo=" + _tituloFinanceiro.Codigo.val()
    //});
}

function TotaluploadprogressTitulo(percentualProgresso) {
    if (GlobalPercentual < Math.round(percentualProgresso)) {
        GlobalPercentual = Math.round(percentualProgresso);
        $("#" + _tituloFinanceiro.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentual < 100) {
            $("#" + _tituloFinanceiro.Dropzone.idTab).css("width", GlobalPercentual + "%");
        } else {
            $("#" + _tituloFinanceiro.Dropzone.idTab).text("Finalizando Envio...");
            $("#" + _tituloFinanceiro.Dropzone.idTab).css("width", "100%");
        }
    }
}


function preecherErroTitulo(mensagem, file) {
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

function DropZoneSucessTitulo(file, response, i, b) {
    var arg = typeof response === 'object' ? response : JSON.parse(response);

    if (arg.Success) {
        if (arg.Data !== false) {
            $.each(arg.Data.Arquivos, function (i, arquivo) {
                if (file.name == arquivo.nome) {
                    if (arquivo.processada) {

                        var data =
                        {
                            Codigo: arquivo.codigo
                        }

                        executarReST("CTe/BuscarPorCodigo", data, function (e) {
                            if (!e.Success) {
                                exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
                            } else {
                                LimparCampoEntity(_tituloFinanceiro.ConhecimentoEletronico)
                                _tituloFinanceiro.ConhecimentoEletronico.val(e.Data.CTe.Chave);
                                _tituloFinanceiro.ConhecimentoEletronico.codEntity(e.Data.CTe.Codigo);
                            }
                        });

                        return file.previewElement.classList.add("dz-success");
                    }
                    else
                        return preecherErroTitulo(arquivo.mensagem, file);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            return preecherErroTitulo(arg.Msg, file);
        }
    } else {
        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        return preecherErroTitulo(arg.Msg, file);
    }
}

function DropZoneCompleteTitulo(arg) {
    //fazer aqui a seleção automática ou talvez aqui

    $("#" + _tituloFinanceiro.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _tituloFinanceiro.Dropzone.idTab).text("");
    $("#" + _tituloFinanceiro.Dropzone.idTab).css("width", "0%");

    GlobalPercentual = 0;
}