var _documentoEmitidoEmbarcador;

//*******MAPEAMENTO KNOUCKOUT*******

var DocumentoEmitidoEmbarcador = function () {
    this.LerNumeroPedidoObservacaoMDFe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerNumeroPedidoEmbarcadorMDFe, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.LerNumeroPedidoObservacaoCTe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerNumeroPedidoEmbarcador, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.VincularMDFePeloNumeroPedido = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.VincularMDFeCargaPeloPedido, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.VincularCTePeloNumeroPedido = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.VincularCTeCargaPeloPedido, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.SetarNumeroPedidoEmbarcador, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.RegexNumeroPedidoObservacaoMDFe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExpressaoRegularMDFe.getFieldDescription(), issue: 0, required: false, maxLength: 150, enable: ko.observable(true) });
    this.RegexNumeroPedidoObservacaoCTe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExpressaoRegularCTe.getFieldDescription(), issue: 0, required: false, maxLength: 150, enable: ko.observable(true) });
}

//*******EVENTOS*******

function LoadDocumentoEmitidoEmbarcador() {

    _documentoEmitidoEmbarcador = new DocumentoEmitidoEmbarcador();
    KoBindings(_documentoEmitidoEmbarcador, "knockoutDocumentoEmitidoEmbarcador");

    _grupoPessoas.LerNumeroPedidoObservacaoMDFe = _documentoEmitidoEmbarcador.LerNumeroPedidoObservacaoMDFe;
    _grupoPessoas.LerNumeroPedidoObservacaoCTe = _documentoEmitidoEmbarcador.LerNumeroPedidoObservacaoCTe;
    _grupoPessoas.VincularMDFePeloNumeroPedido = _documentoEmitidoEmbarcador.VincularMDFePeloNumeroPedido;
    _grupoPessoas.VincularCTePeloNumeroPedido = _documentoEmitidoEmbarcador.VincularCTePeloNumeroPedido;
    _grupoPessoas.RegexNumeroPedidoObservacaoMDFe = _documentoEmitidoEmbarcador.RegexNumeroPedidoObservacaoMDFe;
    _grupoPessoas.RegexNumeroPedidoObservacaoCTe = _documentoEmitidoEmbarcador.RegexNumeroPedidoObservacaoCTe;
    _grupoPessoas.SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe = _documentoEmitidoEmbarcador.SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe;
}