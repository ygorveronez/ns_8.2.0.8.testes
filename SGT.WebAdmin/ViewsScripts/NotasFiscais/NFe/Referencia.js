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
/// <reference path="../../Enumeradores/EnumTipoDocumentoReferenciaNFe.js" />
/// <reference path="../../Enumeradores/EnumEstado.js" />
/// <reference path="NFe.js" />

var _tipoDocumentoReferencia = [
    { text: "Nenhum", value: 0 },
    { text: "NF-e/NFC-e", value: EnumTipoDocumentoReferenciaNFe.NF },
    { text: "NF Modelo 1", value: EnumTipoDocumentoReferenciaNFe.NFModelo1 },
    { text: "NF Produtor Rural", value: EnumTipoDocumentoReferenciaNFe.NFProdutorRural },
    { text: "CT-e", value: EnumTipoDocumentoReferenciaNFe.CTe },
    { text: "Cupom Fiscal", value: EnumTipoDocumentoReferenciaNFe.CupomFiscal }
];

var _modelDocumento = [
    { text: "Selecione", value: "" },
    { text: "Modelo 01", value: "01" },
    { text: "Modelo 02", value: "02" },
    { text: "NF de Produtor 04", value: "04" },
    { text: "NF 01", value: "01" },
    { text: "Cupom Fiscal PDV 2C", value: "2C" },
    { text: "Cupom Fiscal Emitido por ECF", value: "2D" }
];

var Referencia = function (nfe) {

    var instancia = this;

    this.TipoDocumento = PropertyEntity({ val: ko.observable(0), def: 0, options: _tipoDocumentoReferencia, text: "Tipo Documento:", required: true, enable: ko.observable(true), eventChange: function () { instancia.TipoDocumentoChange(true) } });
    this.Estado = PropertyEntity({ val: ko.observable(""), options: EnumEstado.obterOpcoesCadastro(), def: "", text: "*Estado: ", visible: ko.observable(false) });

    this.Chave = PropertyEntity({ text: "*Chave: ", required: false, maxlength: 44, visible: ko.observable(false) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: "*Data Emissão:", required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.CNPJCPFEmitente = PropertyEntity({ text: "*CNPJ/CPF Emitente: ", required: false, getType: typesKnockout.string, visible: ko.observable(false), maxlength: 14 });
    this.IEEmitente = PropertyEntity({ text: "*IE Emitente: ", required: false, maxlength: 14, visible: ko.observable(false) });
    this.Serie = PropertyEntity({ text: "*Série: ", required: false, maxlength: 3, visible: ko.observable(false), getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "*Número: ", required: false, maxlength: 9, visible: ko.observable(false), getType: typesKnockout.int });
    this.NumeroECF = PropertyEntity({ text: "*Número ECF: ", required: false, maxlength: 3, visible: ko.observable(false), getType: typesKnockout.int });
    this.NumeroCOO = PropertyEntity({ text: "*Número COO: ", required: false, maxlength: 6, visible: ko.observable(false), getType: typesKnockout.int });

    this.Modelo = PropertyEntity({ val: ko.observable(""), def: "", options: _modelDocumento, text: "Modelo Documento:", required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.NotaFiscalSaida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false), text: "NF Própria" });
    this.NotaFiscalEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false), text: "NF Compra" });
    this.ConhecimentoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false), text: "CT-e" });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutReferencia);
        $("#" + instancia.Chave.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + instancia.CNPJCPFEmitente.id).mask("00000000000999", { selectOnFocus: true, clearIfNotMatch: true });

        new BuscarNotaFiscal(instancia.NotaFiscalSaida, -1, function (data) {
            instancia.Chave.val(data.Chave);
            $("#" + instancia.Chave.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
        });
        new BuscarDocumentoEntrada(instancia.NotaFiscalEntrada, function (data) {
            instancia.Chave.val(data.Chave);
            $("#" + instancia.Chave.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
        });

        new BuscarConhecimentoNotaReferencia(instancia.ConhecimentoFrete, function (data) {
            instancia.Chave.val(data.Chave);
            $("#" + instancia.Chave.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
        });
    };

    this.TipoDocumentoChange = function (v) {
        var tipoDocumento = instancia.TipoDocumento.val();
        if (v)
            LimparCampos(instancia);
        instancia.Estado.visible(false);
        instancia.Chave.visible(false);
        instancia.DataEmissao.visible(false);
        instancia.CNPJCPFEmitente.visible(false);
        instancia.IEEmitente.visible(false);
        instancia.Serie.visible(false);
        instancia.Numero.visible(false);
        instancia.NumeroECF.visible(false);
        instancia.NumeroCOO.visible(false);
        instancia.Modelo.visible(false);
        instancia.NotaFiscalSaida.visible(false);
        instancia.NotaFiscalEntrada.visible(false);
        instancia.ConhecimentoFrete.visible(false);

        instancia.Estado.required = false;
        instancia.Chave.required = false;
        instancia.DataEmissao.required = false;
        instancia.CNPJCPFEmitente.required = false;
        instancia.IEEmitente.required = false;
        instancia.Serie.required = false;
        instancia.Numero.required = false;
        instancia.NumeroECF.required = false;
        instancia.NumeroCOO.required = false;
        instancia.Modelo.required = false;

        instancia.TipoDocumento.val(tipoDocumento);

        if (tipoDocumento == EnumTipoDocumentoReferenciaNFe.NF) {
            instancia.Chave.visible(true);
            instancia.NotaFiscalSaida.visible(true);
            instancia.NotaFiscalEntrada.visible(true);
            instancia.Chave.required = true;
        }
        else if (tipoDocumento == EnumTipoDocumentoReferenciaNFe.NFModelo1) {
            instancia.Estado.visible(true);
            instancia.DataEmissao.visible(true);
            instancia.CNPJCPFEmitente.visible(true);
            instancia.Modelo.visible(true);
            instancia.Serie.visible(true);
            instancia.Numero.visible(true);
            instancia.Estado.required = true;
            instancia.DataEmissao.required = true;
            instancia.CNPJCPFEmitente.required = true;
            instancia.Serie.required = true;
            instancia.Numero.required = true;
            instancia.Modelo.required = true;
        }
        else if (tipoDocumento == EnumTipoDocumentoReferenciaNFe.NFProdutorRural) {
            instancia.Estado.visible(true);
            instancia.DataEmissao.visible(true);
            instancia.CNPJCPFEmitente.visible(true);
            instancia.IEEmitente.visible(true);
            instancia.Modelo.visible(true);
            instancia.Serie.visible(true);
            instancia.Numero.visible(true);
            instancia.Estado.required = true;
            instancia.DataEmissao.required = true;
            instancia.CNPJCPFEmitente.required = true;
            instancia.IEEmitente.required = true;
            instancia.Serie.required = true;
            instancia.Numero.required = true;
            instancia.Modelo.required = true;
        }
        else if (tipoDocumento == EnumTipoDocumentoReferenciaNFe.CTe) {
            instancia.Chave.visible(true);
            instancia.ConhecimentoFrete.visible(true);
            instancia.Chave.required = true;
        }
        else if (tipoDocumento == EnumTipoDocumentoReferenciaNFe.CupomFiscal) {
            instancia.Modelo.visible(true);
            instancia.NumeroECF.visible(true);
            instancia.NumeroCOO.visible(true);
            instancia.Modelo.required = true;
            instancia.NumeroECF.required = true;
            instancia.NumeroCOO.required = true;
        }
    };

    this.DestivarReferencia = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };
};

var ValidarCPFCNPJReferencia = function (instancia) {
    var cpfCnpj = instancia.CNPJCPFEmitente.val();
    if (cpfCnpj.length == 11) {
        if (!ValidarCPF(cpfCnpj)) {
            exibirAlertaNotificacao("CPF informado é inválido");
            $("#" + instancia.CNPJCPFEmitente.id).focus();
        }
    } else if (cpfCnpj.length == 14) {
        if (!ValidarCNPJ(cpfCnpj)) {
            exibirAlertaNotificacao("CNPJ informado é inválido");
            $("#" + instancia.CNPJCPFEmitente.id).focus();
        }
    } else if (cpfCnpj.length > 0) {
        exibirAlertaNotificacao("CNPJ/CPF informado é inválido");
        $("#" + instancia.CNPJCPFEmitente.id).focus();
    }
};