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
/// <reference path="NFe.js" />

var ExportacaoProdutoServico = function (nfe) {

    var instancia = this;

    this.NumeroDrawback = PropertyEntity({ text: "Número Drawback: ", maxlength: 11, required: ko.observable(false), enable: ko.observable(true) });
    this.NumeroRegistroExportacao = PropertyEntity({ text: "Número Registro Exportação: ", maxlength: 12, required: ko.observable(false), enable: ko.observable(true) });
    this.ChaveAcessoExportacao = PropertyEntity({ text: "Chave Acesso NF-e Exportação: ", maxlength: 44, required: ko.observable(false), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutExportacaoProdutoServico);
        $("#" + instancia.NumeroDrawback.id).mask("00000000099", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + instancia.NumeroRegistroExportacao.id).mask("000000000000", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + instancia.ChaveAcessoExportacao.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });
    };

    this.DesativarExportacaoProdutoServico = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarExportacaoProdutoServico = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };
};