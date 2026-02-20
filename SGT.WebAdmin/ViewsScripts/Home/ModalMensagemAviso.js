/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _modalMensagemAviso;

var ModalMensagemAvisoModel = function () {
    this.Mensagem = PropertyEntity({ val: ko.observable("") });
    this.Titulo = PropertyEntity({ val: ko.observable("") });
}

//*******EVENTOS*******

function LoadModalMensagemAviso() {
    _modalMensagemAviso = new ModalMensagemAvisoModel();
    KoBindings(_modalMensagemAviso, "knockoutModalAviso");

    BuscarModalMensagemAviso();
}

//*******MÉTODOS*******

function BuscarModalMensagemAviso() {
    executarReST("ModalMensagemAviso/Buscar", {}, function (r) {
        if (r.Success) {
            if (r.Data != null && r.Data !== false) {
                PreencherObjetoKnout(_modalMensagemAviso, r);
                Global.abrirModal('modalMensagemAviso');
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, null, false);
}