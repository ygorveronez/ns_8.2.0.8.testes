/// <reference path="ControleChamadoTMS.js" />
/// <reference path="AnexosDocumentoAnalise.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _documentoAnalise;

var DocumentoAnalise = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataDocumentoRecebido = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), def: "", text: "*Data Recebido:", required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorRecibo = PropertyEntity({ text: "*Valor Recibo:", def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });
    this.NumeroDocumento = PropertyEntity({ val: ko.observable(""), def: "", text: "Número Documento:", maxlength: 200, enable: ko.observable(true) });
    this.ObservacaoDocumento = PropertyEntity({ val: ko.observable(""), def: "", text: "Observação:", enable: ko.observable(true) });

    this.SalvarDocumento = PropertyEntity({ eventClick: SalvarDocumentoClick, type: types.event, text: "Salvar Documento", visible: ko.observable(true), enable: ko.observable(true) });
    this.AnexoDocumento = PropertyEntity({ eventClick: gerenciarAnexosDocumentoAnaliseClick, type: types.event, text: "Adicionar Anexos no Documento", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadDocumentoAnalise() {
    _documentoAnalise = new DocumentoAnalise();
    KoBindings(_documentoAnalise, "knockoutDocumentoAnalise");

    $("#divModalDocumentoAnalise").on('hidden.bs.modal', function () {
        limparCamposDocumentoAnalise();
    });

    loadAnexosDocumentoAnalise();
}

function SalvarDocumentoClick(e, sender) {
    Salvar(_documentoAnalise, "ChamadoTMSAnalise/SalvaDocumentoAnalise", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento da análise salvo com sucesso");

                EnviarArquivosAnexadosDocumentoAnalise(function () {
                    buscarControleChamadoTMS();
                });

                Global.fecharModal('divModalDocumentoAnalise');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function limparCamposDocumentoAnalise() {
    LimparCampos(_documentoAnalise);
    limparOcorrenciaAnexosDocumentoAnalise();
}