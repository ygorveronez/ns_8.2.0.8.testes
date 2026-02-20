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
/// <reference path="Cargas.js" />
/// <reference path="CTes.js" />
/// <reference path="Etapas.js" />
/// <reference path="MDFe.js" />
/// <reference path="SignalR.js" />
/// <reference path="Terminais.js" />
/// <reference path="CargaMDFeAquaviarioManual.js" />

//*******EVENTOS*******

var _impressaoMDFeManual;

var ImpressaoMDFeManual = function () {
    this.CargaMDFeAquaviario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnviarParaImpressao = PropertyEntity({
        eventClick: enviarParaImpressaoClick, type: types.event,
        text: "Enviar documentos para Impressão", visible: ko.observable(false), enable: ko.observable(true)
    });
    this.DownloadLotePDF = PropertyEntity({
        eventClick: DownloadLotePDFClick, type: types.event, text: "PDF dos Documentos", idGrid: guid(), visible: ko.observable(false)
    });
}

function LoadImpressaoMDFeManual() {
    _impressaoMDFeManual = new ImpressaoMDFeManual();
    KoBindings(_impressaoMDFeManual, "knockoutImpressao");

}

function verificarEtapaImpressaoClick(e) {
    _cargaMDFeAquaviarioMDFe.CargaMDFeAquaviario.val(_cargaMDFeAquaviario.Codigo.val());
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        _impressaoMDFeManual.EnviarParaImpressao.visible(true);
}

function DownloadLotePDFClick(e, sender) {
    executarDownload("CargaMDFeManualImpressao/DownloadLotePDF", { CargaMDFeManual: _cargaMDFeAquaviario.Codigo.val() });
}

function enviarParaImpressaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Você realmente deseja enviar para impressão?", function () {
        executarReST("CargaMDFeManualImpressao/EnviarDocumentosParaImpressao", { CargaMDFeManual: _cargaMDFeAquaviario.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    })
}
