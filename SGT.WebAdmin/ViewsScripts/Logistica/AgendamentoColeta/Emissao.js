//#region Variaveis globais

var _gridNFe;
var _gridMDFe;
var _emissao;

//#endregion

//#region Mapeamento Knockout 

var Emissao = function () {
    this.CTes = PropertyEntity({ type: types.local });
    this.MDFes = PropertyEntity({ type: types.local });
    this.Download = PropertyEntity({ eventClick: DownloadLotePDFAgendamentoClick, type: types.event, text: "Download dos Documentos", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false) });
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });

    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

function LoadEmissao() {
    _emissao = new Emissao();
    KoBindings(_emissao, "knockoutEmissao");

    LoadGridNFe();
    LoadGridMDFe();
    loadDropZoneCTe();
    
    _emissao.Download.visible(_configuracaoAgendamentoColeta.PermiteDownloadDocumentos);
}

function loadDropZoneCTe() {
    $("#" + _emissao.Dropzone.id).dropzone({
        init: function () {
            var dropzone = this;
            //clearDropzone = function () {
            LimparDropzone = function () {
                dropzone.removeAllFiles(true);
            };
        },
        dictDefaultMessage: '<div class="row"><i class="fal fa-arrow-alt-right text-danger"></i><label class="text-center"><span class="font-lg"><span class="font-md">  Arraste os arquivos para o envio</span><span><br/><h5 > (ou clique e selecione)</h5></label></div>',
        dictResponseError: 'Falha ao enviar o arquivo!',
        acceptedFiles: ".xml",
        dictInvalidFileType: 'A extensão do arquivo é inválida.',
        processing: function () {
            this.options.url = "AgendamentoColeta/AdicionarCTe?Codigo=" + _agendamentoColeta.CodigoAgendamento.val();
        },
        success: dropZoneSuccessCTe,
        uploadMultiple: false,
        clickable: true,
        queuecomplete: dropZoneSuccessCTe,
        totaluploadprogress: totalUploadProgressCTe
    });
}

function LoadGridNFe() {
    _gridNFe = new GridViewExportacao(_emissao.CTes.id, "AgendamentoColeta/ConsultarCargaCTe", _agendamentoColeta, null);
    _gridNFe.CarregarGrid();
}

function LoadGridMDFe() {
    _gridMDFe = new GridViewExportacao(_emissao.MDFes.id, "AgendamentoColeta/ConsultarCargaMDFe", _agendamentoColeta, null);
    _gridMDFe.CarregarGrid();
}

//#endregion

//#region Métodos Globais

function DownloadLotePDFAgendamentoClick(e, sender) {
    executarDownload("AgendamentoColeta/DownloadLotePDF", { Codigo: _agendamentoColeta.CodigoCarga.val() });
}

//#endregion

//#region Métodos Privados

function preecherErroCTe(mensagem, file) {
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

function totalUploadProgressCTe(percentualProgresso) {
    if (GlobalPercentual < Math.round(percentualProgresso)) {
        GlobalPercentual = Math.round(percentualProgresso);
        $("#" + _NFeAgendamento.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentual < 100) {
            $("#" + _NFeAgendamento.Dropzone.idTab).css("width", GlobalPercentual + "%");
        } else {
            $("#" + _NFeAgendamento.Dropzone.idTab).text("Finalizando Envio...");
            $("#" + _NFeAgendamento.Dropzone.idTab).css("width", "100%");
        }
    }
}

function dropZoneSuccessCTe(file, response, i, b) {
    if (!file) return;

    var arg = typeof response === 'object' ? response : JSON.parse(response);

    if (!arg.Success) {
        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        return preecherErroCTe(arg.Msg, file);
    }
    
    if (arg.Data == false) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        return preecherErroCTe(arg.Msg, file);
    }

    $.each(arg.Data.Arquivos, function (i, arquivo) {
        if (file.name == arquivo.nome) {
            if (arquivo.processada)
                return file.previewElement.classList.add("dz-success");
            else
                return preecherErroCTe(arquivo.mensagem, file);
        }
    });

    _gridNFe.CarregarGrid();
}

//#region