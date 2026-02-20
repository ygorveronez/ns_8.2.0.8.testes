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
/// <reference path="../../Consultas/Navio.js" />
/// <reference path="../../Enumeradores/EnumDirecao.js" />
/// <reference path="CTe.js" />

var ModalAquaviario = function (cte) {

    var instancia = this;

    this.NumeroViagem = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroViagem.getFieldDescription(), maxlength: 10, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorPrestacaoAfrmm = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorPrestacao.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });
    this.ValorAdicionalAfrmm = PropertyEntity({ text: Localization.Resources.CTes.CTe.AdicionalDeFrete.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });

    this.Direcao = PropertyEntity({ val: ko.observable(""), options: EnumDirecao.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.Direcao.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Navio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Navio.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutModalAquaviario);

        new BuscarNavios(instancia.Navio);
    };

    this.Validar = function () {
        if (cte.CTe.TipoModal.val() === EnumTipoModal.Aquaviario) {
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

    this.DestivarModalAquaviario = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};