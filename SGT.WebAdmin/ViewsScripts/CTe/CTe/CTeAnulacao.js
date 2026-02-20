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
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="CTe.js" />

var CTeAnulacao = function (cte) {

    var instancia = this;

    this.ChaveCTeAnulado = PropertyEntity({ text: Localization.Resources.CTes.CTe.ChaveDoCTeSerAnulado.getRequiredFieldDescription(), maxlength: 44, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(false) });
    this.DataAnulacao = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataAnulacao.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutCTeAnulacao);

        $("#" + instancia.ChaveCTeAnulado.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
    };

    this.Validar = function () {
        if (cte.CTe.Tipo.val() === EnumTipoCTe.Anulacao) {
            var valido = ValidarCamposObrigatorios(instancia);

            if (!valido) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.CTes.CTe.VerifiqueOsCamposObrigatorios);
            } else if (instancia.ChaveCTeAnulado.val().trim().replace(/\s/g, "").length !== 44) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.CTeAnulacao, Localization.Resources.CTes.CTe.FavorVerificarChaveDoCTeSerAnuladoElaDevePossuirQuarentaQuatroDigitos);
            }

            if (!valido)
                $('a[href="#divCTeOutros_' + cte.IdModal + '"]').tab("show");

            return valido;
        }
        else
            return true;
    };

    this.DestivarCTeAnulacao = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };
};