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


var _openTech;

var OpenTech = function () {
    this.CodigoIntegracao = PropertyEntity({ type: types.map, text: Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(false) });
    this.CodigoCliente = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoCliente.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), required: false, val: _transportador.CodigoClienteOpenTech.val });
    this.CodigoPAS = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoPAS.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), required: false, val: _transportador.CodigoPASOpenTech.val });
    
    this.PossuiIntegracaoOpenTech = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
}


//*******EVENTOS*******
function LoadOpenTech() {
    _openTech = new OpenTech();
    KoBindings(_openTech, "knockoutOpenTech");


    _transportador.OpenTech = _openTech.CodigoIntegracao;
}

function LimparOpenTech() {
    LimparCampos(_openTech);
}

//*******EVENTOS*******
function IntegrarComGerenciadoraDeRiscoChange() {
    _openTech.CodigoPAS.visible(_openTech.PossuiIntegracaoOpenTech.val());
    _openTech.CodigoCliente.visible(_openTech.PossuiIntegracaoOpenTech.val());
    _openTech.CodigoIntegracao.visible(_transportador.IntegrarComGerenciadoraDeRisco.val());
}