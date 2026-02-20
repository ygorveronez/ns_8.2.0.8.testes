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

var Rodoviario = function (cte) {

    var instancia = this;

    this.RNTRC = PropertyEntity({ text: Localization.Resources.CTes.CTe.RNTRC.getRequiredFieldDescription(), maxlength: 8, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.PrevisaoEntrega = PropertyEntity({ text: Localization.Resources.CTes.CTe.PrevisaoDeEntrega.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.CIOT = PropertyEntity({ text: Localization.Resources.CTes.CTe.CIOT.getFieldDescription(), maxlength: 12, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.IndicadorLotacao = PropertyEntity({ text: Localization.Resources.CTes.CTe.IndicadorDeLotacao, val: ko.observable(false), def: false, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, cte.IdKnockoutRodoviario);

        $("#" + instancia.RNTRC.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });
    };

    this.Validar = function () {
        if (cte.CTe.TipoModal.val() === EnumTipoModal.Rodoviario) {
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

    this.DestivarRodoviario = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};