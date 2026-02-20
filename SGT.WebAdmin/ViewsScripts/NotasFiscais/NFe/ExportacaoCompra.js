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
/// <reference path="../../Enumeradores/EnumEstado.js" />
/// <reference path="NFe.js" />

var ExportacaoCompra = function (nfe) {

    var instancia = this;

    this.UFEmbarque = PropertyEntity({ val: ko.observable(""), options: EnumEstado.obterOpcoesCadastro(), def: "", enable: ko.observable(true), text: "UF de embarque: ", });
    this.LocalEmbarque = PropertyEntity({ text: "Local de embarque: ", required: false, maxlength: 1000 });
    this.LocalDespacho = PropertyEntity({ text: "Local de despacho: ", required: false, maxlength: 1000 });
    this.InformacaoCompraNotaEmpenho = PropertyEntity({ text: "Informação da nota de empenho de compras públicas: ", required: false, maxlength: 1000 });
    this.InformacaoCompraPedido = PropertyEntity({ text: "Informação do pedido: ", required: false, maxlength: 1000 });
    this.InformacaoCompraContrato = PropertyEntity({ text: "Informação do contrato: ", required: false, maxlength: 1000 });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutExportacaoCompra);
    };

    this.DestivarExportacaoCompra = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };
};