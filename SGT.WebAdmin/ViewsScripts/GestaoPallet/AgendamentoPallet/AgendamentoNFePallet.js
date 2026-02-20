var _NFeAgendamentoPallet;
var _botoesNFePallet;
var _gridNFeAgendamentoPallet;
var LimparDropzone;
var GlobalPercentual = 0;

var NFeAgendamentoPallet = function () {
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });

    this.NFes = PropertyEntity({ type: types.event });
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

var NFeBotoesNFePallet = function () {
    this.Imprimir = PropertyEntity({ eventClick: imprimirAgendamentoClick, type: types.event, text: "Imprimir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAgendamentoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparAgendamentoClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
}

function carregarEtapaNFePallet() {
    _NFeAgendamentoPallet = new NFeAgendamentoPallet();
    KoBindings(_NFeAgendamentoPallet, "knockoutNFeAgendamentoPallet");

    _botoesNFePallet = new NFeBotoesNFePallet();
    KoBindings(_botoesNFePallet, "knockoutBotoesNFePallet");

    LoadDropZoneNFePallet();
    LoadGridNFeAgendamentoPallet();
}

function LoadDropZoneNFePallet() {
    $("#" + _NFeAgendamentoPallet.Dropzone.id).dropzone({
        init: function () {
            var dropzone = this;

            LimparDropzone = function () {
                dropzone.removeAllFiles(true);
            };
        },
        dictDefaultMessage: '<div class="row"><i class="fal fa-arrow-alt-right text-danger"></i><label class="text-center"><span class="font-lg"><span class="font-md">  Arraste os arquivos para o envio</span><span><br/><h5 > (ou clique e selecione)</h5></label></div>',
        dictResponseError: 'Falha ao enviar o arquivo!',
        acceptedFiles: ".xml",
        dictInvalidFileType: 'A extensão do arquivo é inválida.',
        processing: function () {
            this.options.url = "AgendamentoPallet/EnviarNFe?Codigo=" + _etapaAgendamentoPallet.Codigo.val();
        },
        success: DropZoneSuccessNFePallet,
        uploadMultiple: false,
        clickable: true,
        queuecomplete: DropZoneCompleteNFeAgendamentoPallet,
        totaluploadprogress: TotaluploadprogressNFePallet,
    });
}

function LoadGridNFeAgendamentoPallet() {
    var opcaoExcluir = {
        descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirNFeClick, tamanho: "10", icone: "", visibilidade: function () {
            return (_etapaAgendamentoPallet.EtapaAgendamentoPallet.val() === EnumEtapaAgendamentoPallet.NFe) && (_etapaAgendamentoPallet.Situacao.val() !== EnumSituacaoAgendamentoPallet.Cancelado);
        }
    };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoExcluir] };

    _gridNFeAgendamentoPallet = new GridView(_NFeAgendamentoPallet.NFes.id, "AgendamentoPallet/PesquisaNFe", { Codigo: _etapaAgendamentoPallet.Codigo }, menuOpcoes);
    _gridNFeAgendamentoPallet.CarregarGrid();
}

function DropZoneSuccessNFePallet(file, response, i, b) {
    if (!file) return;

    var arg = typeof response === 'object' ? response : JSON.parse(response);

    if (!arg.Success) {
        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        return preecherErroNFe(arg.Msg, file);
    }

    if (arg.Data == false) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        return preecherErroNFe(arg.Msg, file);
    }

    if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Data.MensagemRetorno);

    $.each(arg.Data.Arquivos, function (i, arquivo) {
        if (file.name == arquivo.nome) {
            if (arquivo.processada)
                return file.previewElement.classList.add("dz-success");
            else
                return preecherErroNFe(arquivo.mensagem, file);
        }
    });
}

function LimparBarraProguessoNFeAgendamentoPallet() {
    $("#" + _NFeAgendamentoPallet.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _NFeAgendamentoPallet.Dropzone.idTab).text("");
    $("#" + _NFeAgendamentoPallet.Dropzone.idTab).css("width", "0%");

    GlobalPercentual = 0;
}

function DropZoneCompleteNFeAgendamentoPallet() {
    LimparBarraProguessoNFeAgendamentoPallet();

    if (_gridNFeAgendamentoPallet)
        _gridNFeAgendamentoPallet.CarregarGrid();
}

function TotaluploadprogressNFePallet(percentualProgresso) {
    if (GlobalPercentual < Math.round(percentualProgresso)) {
        GlobalPercentual = Math.round(percentualProgresso);
        $("#" + _NFeAgendamentoPallet.Dropzone.idTab).parent().css("visibility", "visible");
        if (GlobalPercentual < 100) {
            $("#" + _NFeAgendamentoPallet.Dropzone.idTab).css("width", GlobalPercentual + "%");
        } else {
            $("#" + _NFeAgendamentoPallet.Dropzone.idTab).text("Finalizando Envio...");
            $("#" + _NFeAgendamentoPallet.Dropzone.idTab).css("width", "100%");
        }
    }
}

function excluirNFeClick(nfe) {
    exibirConfirmacao("Excluir NFe?", "Tem certeza que deseja excluir a nota?", function () {
        executarReST("AgendamentoPallet/ExcluirNFe", {
            Agendamento: _etapaAgendamentoPallet.Codigo.val(),
            NFe: nfe.Codigo
        }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_gridNFeAgendamentoPallet)
                        _gridNFeAgendamentoPallet.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function LimparUploadNFePallet() {
    if (LimparDropzone) {
        LimparDropzone();
    }

    if (_gridNFeAgendamentoPallet) {
        _gridNFeAgendamentoPallet.CarregarGrid();
    }

    _NFeAgendamentoPallet.Dropzone.visible(_etapaAgendamentoPallet.EtapaAgendamentoPallet.val() == EnumEtapaAgendamentoPallet.NFe);
    LimparBarraProguessoNFeAgendamentoPallet();
}

function imprimirAgendamentoClick() {
    var dados = { Codigo: _etapaAgendamentoPallet.Codigo.val() }

    executarDownload("AgendamentoPallet/Imprimir", dados);
}