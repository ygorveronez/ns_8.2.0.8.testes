/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoDigitalCom;

var IntegracaoDigitalCom = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });
    this.NotificarTransportadorPorEmail = PropertyEntity({ text: "Notificar transportador por e-mail", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.EnviarNumeroCargaNoCampoDocumentoTransporte = PropertyEntity({ text: "Enviar número da carga no campo idDocumentoTransporte", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadConfiguracaoDigitalCom() {
    _integracaoDigitalCom = new IntegracaoDigitalCom();
    KoBindings(_integracaoDigitalCom, "knockoutIntegracaoDigitalCom");

    BuscarClientes(_integracaoDigitalCom.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposDigitalCom() {
    LimparCampos(_integracaoDigitalCom);
}