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
/// <reference path="../../Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="../../Enumeradores/EnumIndicadorEscalaRelevante.js" />
/// <reference path="../../Enumeradores/EnumUnidadeMedida.js" />
/// <reference path="NFe.js" />

var _indicadorEscalaRelevante = [
    { text: "Nenhum", value: EnumIndicadorEscalaRelevante.Nenhum },
    { text: "N - Produzido em Escala NÃO Relevante", value: EnumIndicadorEscalaRelevante.ProduzidoEscalaNaoRelevante },
    { text: "S - Produzido em Escala Relevante", value: EnumIndicadorEscalaRelevante.ProduzidoEscalaRelevante }
];

var InformacoesAdicionaisProdutoServico = function (nfe) {

    var instancia = this;

    this.InformacoesAdicionaisItem = PropertyEntity({ text: "Informações Adicionais: ", required: false, maxlength: 500 });
    this.IndicadorEscalaRelevante = PropertyEntity({ val: ko.observable(EnumIndicadorEscalaRelevante.Nenhum), options: _indicadorEscalaRelevante, def: EnumIndicadorEscalaRelevante.Nenhum, text: "Indicador Escala Relevante: ", required: false, enable: ko.observable(true) });
    this.CNPJFabricante = PropertyEntity({ text: "CNPJ Fabricante: ", required: false, maxlength: 20, getType: typesKnockout.cnpj, enable: ko.observable(true) });
    this.CodigoBeneficioFiscal = PropertyEntity({ text: "Código Beneficio Fiscal: ", required: false, maxlength: 50, enable: ko.observable(true) });
    this.CodigoNFCI = PropertyEntity({ text: "nFCI:", maxlength: 500, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local de Armazenamento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) });

    this.QuantidadeTributavel = PropertyEntity({ def: "0,0000", val: ko.observable("0,0000"), text: "Quantidade Tributável:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: nfe.NFe.CasasQuantidadeProdutoNFe.val(), allowZero: true } });
    this.ValorUnitarioTributavel = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), text: "Valor Unitário Tributável:", getType: typesKnockout.decimal, maxlength: 22, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: nfe.NFe.CasasValorProdutoNFe.val(), allowZero: true } });
    this.CodigoEANTributavel = PropertyEntity({ text: "Código EAN Tributável: ", required: false, maxlength: 50 });
    this.UnidadeDeMedidaTributavel = PropertyEntity({ val: ko.observable(""), options: EnumUnidadeMedida.obterOpcoes(), text: "Unidade de Medida Tributável:", def: "", issue: 88, required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutInformacoesAdicionaisProdutoServico);

        new BuscarLocalArmazenamentoProduto(instancia.LocalArmazenamento);
    };

    this.DestivarInformacoesAdicionaisProdutoServico = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };

    this.HabilitarInformacoesAdicionaisProdutoServico = function () {
        HabilitarCamposInstanciasNFe(instancia);
    };
};