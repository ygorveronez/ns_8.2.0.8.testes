var _cteSubcontratacao;

//*******MAPEAMENTO KNOUCKOUT*******

var CTeSubcontratacao = function () {
    this.LerNumeroPedidoObservacaoCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerNumeroPedidoEmbarcador, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.VincularCTeSubcontratacaoPeloNumeroPedido = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.VincularCTeCargaNumeroPedidoEmbarcador, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.RegexNumeroPedidoObservacaoCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExpressaoRegularCTe.getFieldDescription(), issue: 0, required: false, maxLength: 150, enable: ko.observable(true) });
}

//*******EVENTOS*******

function LoadCTeSubcontratacao() {
    _cteSubcontratacao = new CTeSubcontratacao();
    KoBindings(_cteSubcontratacao, "knockoutCTeSubcontratacao");

    _grupoPessoas.LerNumeroPedidoObservacaoCTeSubcontratacao = _cteSubcontratacao.LerNumeroPedidoObservacaoCTeSubcontratacao;
    _grupoPessoas.VincularCTeSubcontratacaoPeloNumeroPedido = _cteSubcontratacao.VincularCTeSubcontratacaoPeloNumeroPedido;
    _grupoPessoas.RegexNumeroPedidoObservacaoCTeSubcontratacao = _cteSubcontratacao.RegexNumeroPedidoObservacaoCTeSubcontratacao;
}