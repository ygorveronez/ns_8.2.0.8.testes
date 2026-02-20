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
/// <reference path="CTe.js" />

var InformacaoCarga = function (cte) {

    var instancia = this;

    this.ValorTotalCarga = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorTotalDaCarga.getRequiredFieldDescription(), getType: typesKnockout.decimal, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorCargaAverbacao = PropertyEntity({ text: Localization.Resources.CTes.CTe.ValorDaCargaParaAverbacao.getFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.ProdutoPredominante = PropertyEntity({ text: Localization.Resources.CTes.CTe.ProdutoPredominante.getRequiredFieldDescription(), maxlength: 60, required: true, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.Container = PropertyEntity({ text: Localization.Resources.CTes.CTe.Container.getFieldDescription(), maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataEntregaContainer = PropertyEntity({ text: Localization.Resources.CTes.CTe.EntregaContainer.getFieldDescription(), getType: typesKnockout.date, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroLacreContainer = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroLacreContainer.getFieldDescription(), maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.OutrasCaracteristicas = PropertyEntity({ text: Localization.Resources.CTes.CTe.OutrasCaracteristicas.getFieldDescription(), maxlength: 30, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.CaracteristicaAdicionalTransporte = PropertyEntity({ text: Localization.Resources.CTes.CTe.CaracteristicaDoTransporte.getFieldDescription(), maxlength: 15, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.CaracteristicaAdicionalServico = PropertyEntity({ text: Localization.Resources.CTes.CTe.CaracteristicaDoServico.getFieldDescription(), maxlength: 30, required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, cte.IdKnockoutInformacaoCarga);

    }

    this.AtivarPermissoesEspecificas = function (emissaoCTe) {

        if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.AlterarProdutoPredominante))
            instancia.ProdutoPredominante.enable(true);

        if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.AlterarCaracteristicaAdicionalTransporte))
            instancia.CaracteristicaAdicionalTransporte.enable(true);

        if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.AlterarValorTotalCarga))
            instancia.ValorTotalCarga.enable(true);

    }

    this.Validar = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (!valido) {
            $('a[href="#divInformacoes_' + cte.IdModal + '"]').tab("show");
            $('a[href="#divInformacoesCarga_' + cte.IdModal + '"]').tab("show");
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.CTes.CTe.VerifiqueOsCamposObrigatorios);
        }

        return valido;
    }

    this.DestivarInformacaoCarga = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    }
}