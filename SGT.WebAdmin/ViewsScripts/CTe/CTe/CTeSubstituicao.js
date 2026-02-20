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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoAnulacao.js" />
/// <reference path="CTe.js" />

var CTeSubstituicao = function (cte) {

    var instancia = this;

    this.ChaveCTeSubstituido = PropertyEntity({ text: Localization.Resources.CTes.CTe.ChaveDoCTeSerSubstituido.getRequiredFieldDescription(), maxlength: 44, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(false) });
    this.Chave = PropertyEntity({ text: Localization.Resources.CTes.CTe.ChaveDeAcessoDoDocumentoDeAnulacao.getRequiredFieldDescription(), maxlength: 44, required: ko.observable(true), visible: ko.observable(false), enable: ko.observable(false) });
    this.Numero = PropertyEntity({ text: Localization.Resources.CTes.CTe.Numero.getRequiredFieldDescription(), maxlength: 20, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Serie = PropertyEntity({ text: Localization.Resources.CTes.CTe.Serie.getRequiredFieldDescription(), maxlength: 10, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Subserie = PropertyEntity({ text: Localization.Resources.CTes.CTe.SubSerie.getFieldDescription(), maxlength: 10, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataEmissao.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: Localization.Resources.CTes.CTe.Valor.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });

    this.ContribuinteICMS = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Nao), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Nao, text: Localization.Resources.CTes.CTe.TomadorContribuinteDeICMS, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoDocumentoAnulacao.CTe), options: EnumTipoDocumentoAnulacao.obterOpcoes(), def: EnumTipoDocumentoAnulacao.CTe, text: Localization.Resources.CTes.CTe.TipoDeDocumento.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(false), enable: ko.observable(true) });

    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Emitente.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Modelo.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutCTeSubstituicao);

        $("#" + instancia.ChaveCTeSubstituido.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + instancia.Chave.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });

        new BuscarModeloDocumentoFiscal(instancia.ModeloDocumentoFiscal);
        new BuscarClientes(instancia.Emitente);
    };

    this.Validar = function () {
        if (cte.CTe.Tipo.val() === EnumTipoCTe.Substituicao) {
            var valido = ValidarCamposObrigatorios(instancia);

            if (!valido) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.CTes.CTe.VerifiqueOsCamposObrigatorios);
            } else if (instancia.ChaveCTeSubstituido.val().trim().replace(/\s/g, "").length !== 44) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.CTeSubstituicao, Localization.Resources.CTes.CTe.FavorVerificarChaveDoCTeSerSubstituidoElaDevePossuirQuarentaQuatroDigitos);
            }

            if (!valido)
                $('a[href="#divCTeOutros_' + cte.IdModal + '"]').tab("show");

            return valido;
        }
        else
            return true;
    };

    this.DestivarCTeSubstituicao = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };

    this.ContribuinteICMS.val.subscribe(function (novoValor) {
        if (novoValor === EnumSimNaoPesquisa.Nao) {
            instancia.Tipo.visible(false);
            instancia.Tipo.val(EnumTipoDocumentoAnulacao.CTe);
        } else {
            instancia.Tipo.visible(true);
        }
    });

    this.Tipo.val.subscribe(function (novoValor) {
        instancia.Chave.required(false);
        instancia.Numero.required(false);
        instancia.Serie.required(false);
        instancia.Subserie.required(false);
        instancia.DataEmissao.required(false);
        instancia.Valor.required(false);
        instancia.ModeloDocumentoFiscal.required(false);
        instancia.Emitente.required(false);

        instancia.Chave.visible(false);
        instancia.Numero.visible(false);
        instancia.Serie.visible(false);
        instancia.Subserie.visible(false);
        instancia.DataEmissao.visible(false);
        instancia.Valor.visible(false);
        instancia.ModeloDocumentoFiscal.visible(false);
        instancia.Emitente.visible(false);

        if (novoValor === EnumTipoDocumentoAnulacao.CTouNF) {
            instancia.Numero.required(true);
            instancia.Serie.required(true);
            instancia.DataEmissao.required(true);
            instancia.Valor.required(true);
            instancia.ModeloDocumentoFiscal.required(true);
            instancia.Emitente.required(true);

            instancia.Numero.visible(true);
            instancia.Serie.visible(true);
            instancia.Subserie.visible(true);
            instancia.DataEmissao.visible(true);
            instancia.Valor.visible(true);
            instancia.ModeloDocumentoFiscal.visible(true);
            instancia.Emitente.visible(true);
        } else {
            instancia.Chave.visible(false);
            instancia.Chave.required(false);
        }
    });
};