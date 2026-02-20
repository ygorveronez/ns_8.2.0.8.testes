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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumClasseTarifa.js" />
/// <reference path="CTe.js" />

var ModalAereo = function (cte) {

    var instancia = this;

    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataPrevisaoEntrega.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroMinuta = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroMinuta.getFieldDescription(), getType: typesKnockout.int, maxlength: 11, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroOCA = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroOCA.getFieldDescription(), getType: typesKnockout.int, maxlength: 13, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.Dimensao = PropertyEntity({ text: Localization.Resources.CTes.CTe.Dimensao.getFieldDescription(), maxlength: 14, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoTarifa = PropertyEntity({ text: Localization.Resources.CTes.CTe.CodigoTarifa.getFieldDescription(), maxlength: 4, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorTarifa = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorTarifa.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });
    this.ClasseTarifa = PropertyEntity({ val: ko.observable(""), options: EnumClasseTarifa.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.ClasseTarifa.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutModalAereo);
    };

    this.Validar = function () {
        if (cte.CTe.TipoModal.val() === EnumTipoModal.Aereo) {
            var valido = ValidarCamposObrigatorios(instancia);

            if (!valido) {
                $('a[href="#divModal_' + cte.IdModal + '"]').tab("show");
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.CTes.CTe.VerifiqueOsCamposObrigatorios);
            }

            return valido;
        }
        else
            return true;
    };

    this.DestivarModalAereo = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};