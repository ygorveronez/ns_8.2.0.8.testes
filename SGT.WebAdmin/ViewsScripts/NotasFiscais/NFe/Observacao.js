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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="NFe.js" />

var Observacao = function (nfe) {

    var instancia = this;

    this.ObservacaoTributaria = PropertyEntity({ text: "Observação Tributária: ", required: false, maxlength: 2000 });
    this.ObservacaoNFe = PropertyEntity({ text: "Observação da Nota: ", required: false, maxlength: 5000 });

    this.LancarObservacaoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Lançar Observação Fiscal", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutObservacao);

        new BuscarObservacaoFiscal(instancia.LancarObservacaoFiscal, function (data) {
            instancia.ObservacaoNFe.val(instancia.ObservacaoNFe.val() + " - " + data.Observacao);
        });
    }

    this.DestivarObservacao = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    }
}