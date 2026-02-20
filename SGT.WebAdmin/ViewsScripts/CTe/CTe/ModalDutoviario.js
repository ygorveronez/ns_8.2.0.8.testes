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
/// <reference path="CTe.js" />

var ModalDutoviario = function (cte) {

    var instancia = this;

    this.ValorTarifa = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorTarifa.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 16, val: ko.observable("0,000000"), def: "0,000000", configDecimal: { precision: 6, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });
    this.DataInicioPrestacaoServico = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataInicioPrestacaoDeServico.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataFimPrestacaoServico = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataFimPrestacaoDeServico.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutModalDutoviario);
    };

    this.Validar = function () {
        if (cte.CTe.TipoModal.val() === EnumTipoModal.Dutoviario) {
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

    this.DestivarModalDutoviario = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};