/// <reference path="Produto.js" />

var _custoProduto;

var CustoProduto = function () {

    this.CalculoCustoProduto = PropertyEntity({ text: "Fórmula para o custo do produto: *Informe os complementos para o cálculo: ((Valor Total do Item) + ......... )", required: false, maxlength: 300, visible: ko.observable(true), val: ko.observable(""), def: "", enable: ko.observable(true) });

    this.TagValorUnitario = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorUnitario"); }, type: types.event, text: "Valor Unitário" });
    this.TagQuantidade = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#Quantidade"); }, type: types.event, text: "Quantidade" });
    this.TagValorICMS = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorICMS"); }, type: types.event, text: "Valor ICMS" });
    this.TagValorDiferencial = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorDiferencial"); }, type: types.event, text: "Valor Diferencial" });
    this.TagValorICMSST = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorICMSST"); }, type: types.event, text: "Valor ICMS ST" });
    this.TagValorIPI = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorIPI"); }, type: types.event, text: "Valor IPI" });
    this.TagValorFrete = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorFrete"); }, type: types.event, text: "Valor Frete" });
    this.TagValorSeguro = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorSeguro"); }, type: types.event, text: "Valor Seguro" });
    this.TagValorOutras = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorOutras"); }, type: types.event, text: "Valor Outras Despesas" });
    this.TagValorDesconto = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorDesconto"); }, type: types.event, text: "Valor Desconto" });
    this.TagValorDescontoFora = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorDescontoFora"); }, type: types.event, text: "Valor Desconto Fora" });
    this.TagValorImpostoFora = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorImpostoFora"); }, type: types.event, text: "Valor Impostos Fora" });
    this.TagValorOutrasFora = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorOutrasFora"); }, type: types.event, text: "Valor Outras Despesas Fora" });
    this.TagValorFreteFora = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorFreteFora"); }, type: types.event, text: "Valor Frete Fora" });
    this.TagValorICMSFreteFora = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorICMSFreteFora"); }, type: types.event, text: "Valor ICMS Frete Fora" });
    this.TagValorDiferencialFreteFora = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorDiferencialFreteFora"); }, type: types.event, text: "Valor Diferencial do Frete Fora" });
    this.TagValorPIS = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorPIS"); }, type: types.event, text: "Valor PIS" });
    this.TagValorCOFINS = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorCOFINS"); }, type: types.event, text: "Valor COFINS" });
    this.TagValorCreditoPresumido = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#ValorCreditoPresumido"); }, type: types.event, text: "Valor Crédito Presumido" });
    this.TagAbreParenteses = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#("); }, type: types.event, text: "(", visible: ko.observable(false) });
    this.TagFechaParenteses = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#)"); }, type: types.event, text: ")", visible: ko.observable(false) });
    this.TagMenos = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#-"); }, type: types.event, text: "Subtrair (-)" });
    this.TagMais = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#+"); }, type: types.event, text: "Somar (+)" });
    this.TagVezes = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#*"); }, type: types.event, text: "Multiplicar (*)", visible: ko.observable(false) });
    this.TagDivisao = PropertyEntity({ eventClick: function (e) { InserirTag(_custoProduto.CalculoCustoProduto.id, "#/"); }, type: types.event, text: "Dividir (/)", visible: ko.observable(false) });

    this.AdicionarCalculoPadrao = PropertyEntity({ eventClick: AdicionarCalculoPadraoClick, type: types.event, text: "Adicionar Formula Padrão", visible: ko.observable(true) });

    //this.CalculoCustoProduto.val.subscribe(function (novoValor) {
    //    _custoProduto.CalculoCustoProduto.val(_custoProduto.CalculoCustoProduto.val());
    //});
}


function LoadCustoProduto() {
    _custoProduto = new CustoProduto();
    KoBindings(_custoProduto, "knockoutCustoProduto");
}