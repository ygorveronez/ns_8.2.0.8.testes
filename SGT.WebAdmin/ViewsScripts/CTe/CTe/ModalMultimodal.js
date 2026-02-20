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
/// <reference path="../../Enumeradores/EnumIndicadorNegociavel.js" />
/// <reference path="CTe.js" />

var ModalMultimodal = function (cte) {

    var instancia = this;

    this.NumeroCOTM = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroCOTM.getRequiredFieldDescription(), maxlength: 20, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.IndicadorNegociavel = PropertyEntity({ val: ko.observable(EnumIndicadorNegociavel.NaoNegociavel), options: EnumIndicadorNegociavel.obterOpcoes(), def: EnumIndicadorNegociavel.NaoNegociavel, text: Localization.Resources.CTes.CTe.IndicadorNegociavel.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutModalMultimodal);
    };

    this.Validar = function () {
        if (cte.CTe.TipoModal.val() === EnumTipoModal.Multimodal) {
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

    this.DestivarModalMultimodal = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};