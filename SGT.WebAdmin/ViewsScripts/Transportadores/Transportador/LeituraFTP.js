/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLeituraFTP;
var _LeituraFTP;

var LeituraFTP = function () {
    this.EnderecoFTP = PropertyEntity({ text: Localization.Resources.Transportadores.FTP.EnderecoFTP.getFieldDescription(), val: ko.observable(""), def: "", required: ko.observable(false), maxlength: 150 });
    this.Porta = PropertyEntity({ text: Localization.Resources.Transportadores.FTP.Porta.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 10, getType: typesKnockout.int });
    this.DiretorioInput = PropertyEntity({ text: Localization.Resources.Transportadores.FTP.DiretorioInput.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 400 });
    this.DiretorioOutput = PropertyEntity({ text: Localization.Resources.Transportadores.FTP.DiretorioOutput.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 400 });
    this.DiretorioXML = PropertyEntity({ text: Localization.Resources.Transportadores.FTP.DiretorioXML.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 400 });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Transportadores.FTP.Usuario.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 50 });
    this.Senha = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.FTP.Senha.getFieldDescription(), required: false, maxlength: 50 });
    this.Passivo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Transportadores.FTP.Passivo, required: false });
    this.UtilizarSFTP = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Transportadores.FTP.UtilizarSFTP, required: false });
    this.SSL = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Transportadores.FTP.SSL, required: false });
};

//*******EVENTOS*******

function loadLeituraFTP() {
    _LeituraFTP = new LeituraFTP();
    KoBindings(_LeituraFTP, "knockoutLeituraFTP");
}

function limparCamposLeituraFTP() {
    LimparCampos(_LeituraFTP);
}