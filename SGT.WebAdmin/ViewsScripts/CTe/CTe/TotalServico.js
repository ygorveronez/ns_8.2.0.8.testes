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
/// <reference path="EntregaSimplificado.js" />
/// <reference path="../../Enumeradores/EnumICMSCTe.js" />

var _tipoAliquotaICMSCTe = [];

var TotalServico = function (cte) {

    var instancia = this;
    var obrigatorioInformarAliquotaImpostoSuspenso =
        (typeof _cargaAtual !== "undefined" &&
            _cargaAtual?.TipoOperacao?.ObrigatorioInformarAliquotaImpostoSuspensoeValor) || false;

    this.ValorFrete = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ValorDoFrete.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorPrestacaoServico = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ValorDaPrestacao.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorReceber = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ValorReceber.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.IncluirICMSFrete = PropertyEntity({ text: Localization.Resources.CTes.CTe.IncluirICMSNoFrete, val: ko.observable(false), def: false, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.PercentualICMSIncluir = PropertyEntity({ def: "100,00", val: ko.observable("100,00"), text: Localization.Resources.CTes.CTe.PercentualIncluir.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 6, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ICMS = PropertyEntity({ val: ko.observable(EnumICMSCTe.Normal_00), def: EnumICMSCTe.Normal_00, options: EnumICMSCTe.ObterOpcoes(), text: Localization.Resources.CTes.CTe.ICMS.getRequiredFieldDescription(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.BaseCalculoICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.BaseDeCalculo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaICMS = PropertyEntity({ val: ko.observable(_tipoAliquotaICMSCTe[0].value), def: _tipoAliquotaICMSCTe[0].value, options: _tipoAliquotaICMSCTe, text: Localization.Resources.CTes.CTe.Aliquota.getFieldDescription(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.Valor.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualReducaoBaseCalculoICMS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ReducaoBaseCalculo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 6, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorCredito = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: Localization.Resources.CTes.CTe.ValorCredito.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ExibirNaDACTE = PropertyEntity({ text: Localization.Resources.CTes.CTe.ExibirNaDACTE, val: ko.observable(true), def: true, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });

    this.AliquotaImpostoSuspenso = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.CTes.CTe.AliquotaImpostoSuspenso.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, required: ko.observable(obrigatorioInformarAliquotaImpostoSuspenso), visible: ko.observable(obrigatorioInformarAliquotaImpostoSuspenso), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorImpostoSuspenso = PropertyEntity({ val: ko.observable(""), enable: ko.observable(obrigatorioInformarAliquotaImpostoSuspenso), text: Localization.Resources.CTes.CTe.ValorImpostoSuspenso.getFieldDescription(), required: ko.observable(obrigatorioInformarAliquotaImpostoSuspenso), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(obrigatorioInformarAliquotaImpostoSuspenso) });
    this.ValorCommodities = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.CTes.CTe.ValorCommodities.getFieldDescription(), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(obrigatorioInformarAliquotaImpostoSuspenso) });

    this.CodigoClassificacaoTributaria = PropertyEntity({ val: ko.observable(""), text: "Código de Class. Tributária", def: "", getType: typesKnockout.string, maxlength: 15, enable: ko.observable(false) });

    this.BaseCalculoIBSUF = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base de Cálculo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaIBSUF = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorBrutoTributoIBSUF = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Bruto do Tributo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualReducaoIBSUF = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Percentual de Redução", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaEfetivaIBSUF = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota Efetiva", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTributoIBSUF = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor do Tributo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.BaseCalculoIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base de Cálculo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorBrutoTributoIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Bruto do Tributo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualReducaoIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Percentual de Redução", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaEfetivaIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota Efetiva", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTributoIBSMunicipal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor do Tributo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.BaseCalculoCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Base de Cálculo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorBrutoTributoCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Bruto do Tributo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PercentualReducaoCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Percentual de Redução", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaEfetivaCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "% Alíquota Efetiva", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTributoCBS = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor do Tributo", getType: typesKnockout.decimal, maxlength: 8, required: true, visible: ko.observable(false), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutTotalServico);

        $('#' + instancia.IncluirICMSFrete.id).bind('change', function () { instancia.ChangeIncluirICMSFrete(); });
        $('#' + instancia.ICMS.id).bind('change', function () { instancia.ChangeICMS(); });
        $('#' + instancia.ValorFrete.id).bind('blur', function () { instancia.AtualizarTotaisCTe(); });
        //$('#' + instancia.BaseCalculoICMS.id).bind('blur', function () { instancia.CalcularICMS(); });
        $('#' + instancia.AliquotaICMS.id).bind('change', function () { instancia.CalcularICMS(); });
        $('#' + instancia.PercentualReducaoBaseCalculoICMS.id).bind('blur', function () { instancia.AtualizarTotaisCTe(); });
        $('#' + instancia.PercentualICMSIncluir.id).bind('blur', function () { instancia.AtualizarTotaisCTe(); });
        $('#' + instancia.ValorCredito.id).bind('blur', function () { instancia.AtualizarTotaisCTe(); });
        //instancia.ChangeIncluirICMSFrete();
    }

    this.ChangeIncluirICMSFrete = function () {
        var elemento = $('#' + instancia.IncluirICMSFrete.id);
        if (elemento.prop('checked'))
            elemento.closest("label").addClass("input-margin-top-25-md");
        else
            elemento.closest("label").removeClass("input-margin-top-25-md");

        instancia.PercentualICMSIncluir.val("100,00");
        instancia.AtualizarTotaisCTe();

        if (cte.CTe.Tipo.val() === EnumTipoCTe.Simplificado) {
            cte.EntregaSimplificado.AtualizarValoresFreteEntregaGrid();
            AlterarEstadoEntregaSimplificado(cte);
        }
    }

    this.DestivarTotalServico = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    }

    this.ChangeICMS = function () {
        TrocarICMS(instancia);
        instancia.AtualizarTotaisCTe();
    }

    this.AtualizarTotaisCTe = function () {

        var totalComponenteIncluirBC = 0;
        $.each(cte.Componentes, function (i, componente) {
            if (componente.IncluirBaseCalculoICMS === true)
                totalComponenteIncluirBC += Globalize.parseFloat(componente.Valor);
        });

        var valorBaseCalculoICMS = totalComponenteIncluirBC + Globalize.parseFloat(instancia.ValorFrete.val());

        instancia.BaseCalculoICMS.val(Globalize.format(valorBaseCalculoICMS, "n2"));
        instancia.CalcularICMS();
    }

    this.SetarValores = function () {
        var totalCompomente = 0, totalComponenteDescontarTotalReceber = 0, totalComponenteIncluirBCICMS = 0;
        $.each(cte.Componentes, function (i, componente) {

            var valorComponente = Globalize.parseFloat(componente.Valor);

            if (componente.DescontarTotalReceber === true)
                totalComponenteDescontarTotalReceber += valorComponente;
            else {
                totalCompomente += valorComponente;

                if (componente.IncluirBaseCalculoICMS === true)
                    totalComponenteIncluirBCICMS += valorComponente;
            }
        });

        var elemento = $('#' + instancia.IncluirICMSFrete.id);

        var cstICMS = instancia.ICMS.val();
        var valorFrete = Globalize.parseFloat(instancia.ValorFrete.val());
        var valorAReceber = totalCompomente + valorFrete;
        var valorPrestacaoServico = valorAReceber;

        valorAReceber += totalComponenteDescontarTotalReceber;

        var valorBaseCalculoICMS = valorFrete + totalComponenteIncluirBCICMS;

        var aliquota = cstICMS != EnumICMSCTe.Isencao_40 && cstICMS != EnumICMSCTe.NaoTributado_41 && cstICMS != EnumICMSCTe.Diferido_51 && cstICMS != EnumICMSCTe.SimplesNacional ? Globalize.parseFloat(instancia.AliquotaICMS.val()) : 0;
        var percentualReducaoBaseCalculoICMS = Globalize.parseFloat(instancia.PercentualReducaoBaseCalculoICMS.val());
        valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);

        var valorICMS = valorBaseCalculoICMS * (aliquota / 100);
        var valorCreditoICMS = Globalize.parseFloat(instancia.ValorCredito.val());

        if (elemento.prop('checked')) {

            var percentualICMSRecolhido = Globalize.parseFloat(instancia.PercentualICMSIncluir.val());

            valorBaseCalculoICMS += (aliquota > 0 ? Global.roundNumber(valorBaseCalculoICMS / ((100 - aliquota) / 100), 2) - valorBaseCalculoICMS : 0);
            valorICMS = valorBaseCalculoICMS * (aliquota / 100);
            var valorRecolhido = valorICMS * (percentualICMSRecolhido / 100);

            if (cstICMS == EnumICMSCTe.CobradoPorSubstituicaoTributaria_60) {
                instancia.ValorReceber.val(Globalize.format((valorAReceber + valorCreditoICMS), "n2"));
                instancia.ValorPrestacaoServico.val(Globalize.format((valorPrestacaoServico + valorRecolhido), "n2"));
            } else {
                instancia.ValorReceber.val(Globalize.format((valorAReceber + valorRecolhido + valorCreditoICMS), "n2"));
                instancia.ValorPrestacaoServico.val(Globalize.format((valorPrestacaoServico + valorRecolhido + valorCreditoICMS), "n2"));
            }
        } else {
            instancia.ValorReceber.val(Globalize.format((valorAReceber + valorCreditoICMS), "n2"));
            instancia.ValorPrestacaoServico.val(Globalize.format((valorPrestacaoServico + valorCreditoICMS), "n2"));
        }

        instancia.ValorICMS.val(Globalize.format(valorICMS, "n2"));
        instancia.BaseCalculoICMS.val(Globalize.format(valorBaseCalculoICMS, "n2"));
    }

    this.CalcularICMS = function () {
        var icms = instancia.ICMS.val();
        if (icms != EnumICMSCTe.Isencao_40 && icms != EnumICMSCTe.NaoTributado_41 && icms != EnumICMSCTe.Diferido_51 && icms != EnumICMSCTe.SimplesNacional) {
            var valor = Globalize.parseFloat(instancia.BaseCalculoICMS.val());
            var aliquota = Globalize.parseFloat(instancia.AliquotaICMS.val());
            var valorICMS = valor * (aliquota / 100);

            if (isNaN(valorICMS))
                valorICMS = 0;
            //instancia.ValorICMS.val(Globalize.format(valorICMS, "n2"));

        } else {
            ZerarICMS(instancia);
        }

        instancia.SetarValores();
    }
}

function ZerarICMS(instancia) {
    instancia.PercentualReducaoBaseCalculoICMS.val("0,00");
    instancia.BaseCalculoICMS.val("0,00");
    instancia.ValorICMS.val("0,00");
    instancia.ValorCredito.val("0,00");
}

function TrocarICMS(instancia) {
    switch (instancia.ICMS.val()) {
        case EnumICMSCTe.Normal_00:
            AlterarEstadoCamposICMS(instancia, false, true, true, true, false, false);
            break;
        case EnumICMSCTe.ReducaoBaseCalculo_20:
            AlterarEstadoCamposICMS(instancia, true, true, true, true, false, false);
            break;
        case EnumICMSCTe.Isencao_40:
            AlterarEstadoCamposICMS(instancia, false, false, false, false, false, false);
            break;
        case EnumICMSCTe.NaoTributado_41:
            AlterarEstadoCamposICMS(instancia, false, false, false, false, false, false);
            break;
        case EnumICMSCTe.Diferido_51:
            AlterarEstadoCamposICMS(instancia, false, false, false, false, false, false);
            break;
        case EnumICMSCTe.CobradoPorSubstituicaoTributaria_60:
            AlterarEstadoCamposICMS(instancia, true, true, true, true, true, false);
            break;
        case EnumICMSCTe.DevidoAUFOrigemPrestacaoQuandoDiferenteUFEmitente_90:
            AlterarEstadoCamposICMS(instancia, true, true, true, true, false, true);
            $("#" + instancia.ExibirNaDACTE.id).closest("label").addClass("input-margin-top-25-md input-margin-top-25-lg");
            break;
        case EnumICMSCTe.OutrasSituacoes_90:
            AlterarEstadoCamposICMS(instancia, true, true, true, true, true, true);
            $("#" + instancia.ExibirNaDACTE.id).closest("label").removeClass("input-margin-top-25-md");
            break;
        case EnumICMSCTe.SimplesNacional:
            AlterarEstadoCamposICMS(instancia, false, false, false, false, false, false);
            break;
        default:
            AlterarEstadoCamposICMS(instancia, false, false, false, false, false, false);
            break;
    }
}

function AlterarEstadoCamposICMS(instancia, possuiReducaoBC, possuiBC, possuiAliquota, possuiValorICMS, possuiValorCredito, possuiExibeICMSDACTE) {
    if (possuiAliquota) {
        instancia.AliquotaICMS.visible(true);
    } else {
        instancia.AliquotaICMS.visible(false);
        instancia.AliquotaICMS.val(_tipoAliquotaICMSCTe[0].value);
    }

    if (possuiBC) {
        instancia.BaseCalculoICMS.visible(true);
    } else {
        instancia.BaseCalculoICMS.visible(false);
        instancia.BaseCalculoICMS.val("0,00");
    }

    if (possuiExibeICMSDACTE) {
        instancia.ExibirNaDACTE.visible(true);
    } else {
        instancia.ExibirNaDACTE.visible(false);
        instancia.ExibirNaDACTE.val(true);
    }

    if (possuiReducaoBC) {
        instancia.PercentualReducaoBaseCalculoICMS.visible(true);
    } else {
        instancia.PercentualReducaoBaseCalculoICMS.visible(false);
        instancia.PercentualReducaoBaseCalculoICMS.val("0,00");
    }

    if (possuiValorCredito) {
        instancia.ValorCredito.visible(true);
    } else {
        instancia.ValorCredito.visible(false);
        instancia.ValorCredito.val("0,00");
    }

    if (possuiValorICMS) {
        instancia.ValorICMS.visible(true);
    } else {
        instancia.ValorICMS.visible(false);
        instancia.ValorICMS.val("0,00");
    }
}

function ObterAliquotasICMS() {

    var p = new promise.Promise();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
        executarReST("Aliquota/ObterTodos", {}, function (r) {
            if (r.Success) {
                if (r.Data != null && r.Data.length > 0) {
                    _tipoAliquotaICMSCTe = r.Data;
                } else {
                    _tipoAliquotaICMSCTe = [{ value: "0", text: "0,00%" }];
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
            p.done();
        });
    } else {
        _tipoAliquotaICMSCTe = [
            { value: "0,00", text: "0,00%" },
            { value: "5,00", text: "5,00%" },
            { value: "7,00", text: "7,00%" },
            { value: "12,00", text: "12,00%" },
            { value: "17,00", text: "17,00%" },
            { value: "18,00", text: "18,00%" },
            { value: "19,00", text: "19,00%" },
            { value: "20,00", text: "20,00%" },
            { value: "22,00", text: "22,00%" }
        ];
        p.done();
    }

    return p;
}