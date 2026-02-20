
//*******MAPEAMENTO KNOUCKOUT*******
var _NFeAgendamento;
var _CRUDEtapaNFe;
var _gridNFeAgendamento;
var LimparDropzone;
var GlobalPercentual = 0;

var NFeAgendamento = function () {
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });

    this.NFes = PropertyEntity({ type: types.event });
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

var CRUDEtapaNFe = function () {
    this.Encaminhar = PropertyEntity({ eventClick: EncaminharParaTransporteClick, type: types.event, text: "Encaminhar para Transporte", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false) });
}

//*******EVENTOS*******
function LoadEtapaNFe() {
    _NFeAgendamento = new NFeAgendamento();
    KoBindings(_NFeAgendamento, "knockoutNFeAgendamento");

    _CRUDEtapaNFe = new CRUDEtapaNFe();
    KoBindings(_CRUDEtapaNFe, "knockoutCRUDNFeAgendamentoColeta");

    LoadDropZoneNFe();
    LoadEdicaoNFe();
    LoadGridNFeAgendamento();
}

function LoadGridNFeAgendamento() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirNFeClick, tamanho: "10", icone: "", visibilidade: function () { return _agendamentoColeta.Etapa.val() == EnumEtapaAgendamentoColeta.NFe; } };
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarNFeClick, tamanho: "10", icone: "", visibilidade: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [] };

    menuOpcoes.opcoes.push(opcaoExcluir);
    menuOpcoes.opcoes.push(opcaoEditar);

    _gridNFeAgendamento = new GridView(_NFeAgendamento.NFes.id, "AgendamentoColeta/PesquisaNFe", { Codigo: _agendamentoColeta.CodigoAgendamento }, menuOpcoes);
    _gridNFeAgendamento.CarregarGrid();
}

function LoadDropZoneNFe() {
    $("#" + _NFeAgendamento.Dropzone.id).dropzone({
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
            this.options.url = "AgendamentoColeta/EnviarNFe?Codigo=" + _agendamentoColeta.CodigoAgendamento.val();
        },
        success: DropZoneSuccessNFe,
        uploadMultiple: false,
        clickable: true,
        queuecomplete: DropZoneSuccessNFe,
        totaluploadprogress: TotaluploadprogressNFe,
        //url: "AgendamentoColeta/EnviarNFe?Titulo=" + _agendamentoColeta.CodigoAgendamento.val()
    });
}

function ExcluirNFeClick(nfe) {
    exibirConfirmacao("Excluir NFe?", "Tem certeza que deseja excluir a nota?", function () {
        executarReST("AgendamentoColeta/ExcluirNFe", {
            Agendamento: _agendamentoColeta.CodigoAgendamento.val(),
            Codigo: nfe.Codigo
        }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_gridNFeAgendamento)
                        _gridNFeAgendamento.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function EditarNFeClick(nfe) {
    PreencherObjetoKnout(_EdicaoNFe, { Data: nfe });
    _EdicaoNFe.ChaveVenda.visible(nfe.ExigeChaveVenda);
    Global.abrirModal('divEdicaoNotaFiscalAgendamentoColeta');
    $("#divEdicaoNotaFiscalAgendamentoColeta").one('hidden.bs.modal', function () {
        LimparCampos(_EdicaoNFe);
    });
}


//*******MÉTODOS*******
function EncaminharParaTransporteClick() {
    executarReST("AgendamentoColeta/EncaminharParaTransporte", {
        Codigo: _agendamentoColeta.CodigoAgendamento.val()
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                BuscarAgendamento({ Codigo: _agendamentoColeta.CodigoAgendamento.val() });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function TotaluploadprogressNFe(percentualProgresso) {
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

function DropZoneSuccessNFe(file, response, i, b) {
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

    if (_gridNFeAgendamento) 
        _gridNFeAgendamento.CarregarGrid();
    
    LimparBarraProguessoNFeAgendamento();
}

function preecherErroNFe(mensagem, file) {
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

function LimparBarraProguessoNFeAgendamento() {
    $("#" + _NFeAgendamento.Dropzone.idTab).parent().css("visibility", "hidden");
    $("#" + _NFeAgendamento.Dropzone.idTab).text("");
    $("#" + _NFeAgendamento.Dropzone.idTab).css("width", "0%");

    GlobalPercentual = 0;
}

function LimparUploadNFe() {
    if (LimparDropzone) LimparDropzone();
    if (_gridNFeAgendamento) {
        _gridNFeAgendamento.CarregarGrid();
    }

    _NFeAgendamento.Dropzone.visible(false);
    LimparBarraProguessoNFeAgendamento();
}