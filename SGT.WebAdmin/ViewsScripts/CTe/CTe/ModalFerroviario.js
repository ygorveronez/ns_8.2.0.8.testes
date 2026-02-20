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
/// <reference path="../../Enumeradores/EnumTipoTrafego.js" />
/// <reference path="../../Enumeradores/EnumFerroviaResponsavel.js" />
/// <reference path="CTe.js" />

var ModalFerroviario = function (cte) {

    var instancia = this;

    this.NumeroFluxoFerroviario = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroFluxoFerroviario.getRequiredFieldDescription(), maxlength: 10, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoTrafego = PropertyEntity({ val: ko.observable(""), options: EnumTipoTrafego.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.TipoTrafego.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    //Tráfego Mútuo
    this.ValorFrete = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorFrete.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 16, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });
    this.ChaveCTeFerroviaOrigem = PropertyEntity({ text: Localization.Resources.CTes.CTe.ChaveCTeFerroviaOrigem.getFieldDescription(), maxlength: 44, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.ResponsavelFaturamento = PropertyEntity({ val: ko.observable(""), options: EnumFerroviaResponsavel.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.ResponsavelFaturamento.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.FerroviaEmitente = PropertyEntity({ val: ko.observable(""), options: EnumFerroviaResponsavel.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.FerroviaEmitente.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutModalFerroviario);
        $("#" + instancia.ChaveCTeFerroviaOrigem.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
    };

    this.Validar = function () {
        if (cte.CTe.TipoModal.val() === EnumTipoModal.Ferroviario) {
            var valido = ValidarCamposObrigatorios(instancia);

            if (!valido) {
                $('a[href="#divModal_' + cte.IdModal + '"]').tab("show");
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatarios, Localization.Resources.CTes.CTe.VerifiqueOsCamposObrigatorios);
            }

            return valido;
        }
        else
            return true;
    };

    this.DestivarModalFerroviario = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};