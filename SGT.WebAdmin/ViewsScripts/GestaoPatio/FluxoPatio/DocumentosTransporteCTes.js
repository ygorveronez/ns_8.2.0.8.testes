/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="FluxoPatio.js" />

// #region Objetos Globais do Arquivo

var _gridDocumentosTransporteCTes;
var _pesquisaDocumentosTransporteCTes;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaDocumentosTransporteCTes = function () {

    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: _documentosTransporteFluxoPatio.Carga.val, def: 0 });
    this.Status = PropertyEntity({ text: "traduzir", val: ko.observable(EnumStatusCTe.TODOS), enable: ko.observable(true), visible: ko.observable(true), options: EnumStatusCTe.obterOpcoes(), def: EnumStatusCTe.TODOS });
    this.NumeroNF = PropertyEntity({ text: "traduzir", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroDocumento = PropertyEntity({ text: "traduzir", val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosTransporteCTes.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDocumentosTransporteFluxoPatioCTes() {
    _pesquisaDocumentosTransporteCTes = new PesquisaDocumentosTransporteCTes();
    KoBindings(_pesquisaDocumentosTransporteCTes, "knoutDocumentosTransporteFluxoPatioCTes");

    _gridDocumentosTransporteCTes = new GridView(_pesquisaDocumentosTransporteCTes.Pesquisar.idGrid, "CargaCTe/ConsultarCargaCTe", _pesquisaDocumentosTransporteCTes);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos



// #endregion Funções Associadas a Eventos

// #region Funções Públicas



// #endregion Funções Públicas

// #region Funções Privadas



// #endregion Funções Privadas
