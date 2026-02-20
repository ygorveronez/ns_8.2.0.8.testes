//*******MAPEAMENTO KNOUCKOUT*******

var _negociacaoBaixaTituloReceberDocumentoRateioValorPago;

var NegociacaoBaixaTituloReceberDocumentoRateioValorPago = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true, maxlength: 15, enable: ko.observable(true) });
    this.ValorMoeda = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor em Moeda:", val: ko.observable(""), def: "", required: false, maxlength: 15, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarRateioValorPagoDocumentosClick, type: types.event, text: "Aplicar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarRateioValorPagoDocumentosClick, type: types.event, text: "Limpar", icon: "fa fa-rotate-left", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharNegociacaoBaixaTituloReceberDocumentoRateioValorPagoClick, type: types.event, text: "Fechar", icon: "fa fa-window-close", visible: ko.observable(true) });

    this.ValorMoeda.val.subscribe(function (novoValor) {
        _negociacaoBaixaTituloReceberDocumentoRateioValorPago.Valor.val(Globalize.format(ObterValorEmRealBaixaTituloReceber(novoValor), "n2"));
    });
}

//*******EVENTOS*******

function LoadNegociacaoBaixaTituloReceberDocumentoRateioValorPago() {

    _negociacaoBaixaTituloReceberDocumentoRateioValorPago = new NegociacaoBaixaTituloReceberDocumentoRateioValorPago();
    KoBindings(_negociacaoBaixaTituloReceberDocumentoRateioValorPago, "knockoutRateioValorPagoDocumentos");

}

function RatearValorPagoEntreDocumentosClick(e, sender) {
    LimparCamposNegociacaoBaixaTituloReceberDocumentoRateioValorPago();

    if (_negociacaoBaixa.Moeda.val() != EnumMoedaCotacaoBancoCentral.Real) {
        _negociacaoBaixaTituloReceberDocumentoRateioValorPago.ValorMoeda.visible(true);
        _negociacaoBaixaTituloReceberDocumentoRateioValorPago.ValorMoeda.required = true;

        _negociacaoBaixaTituloReceberDocumentoRateioValorPago.Valor.enable(false);
    } else {
        _negociacaoBaixaTituloReceberDocumentoRateioValorPago.ValorMoeda.visible(false);
        _negociacaoBaixaTituloReceberDocumentoRateioValorPago.ValorMoeda.required = false;

        _negociacaoBaixaTituloReceberDocumentoRateioValorPago.Valor.enable(true);
    }

    Global.abrirModal('knockoutRateioValorPagoDocumentos');
}

function AdicionarRateioValorPagoDocumentosClick(e, sender) {

    _negociacaoBaixaTituloReceberDocumentoRateioValorPago.Codigo.val(_baixaTituloReceber.Codigo.val());

    Salvar(_negociacaoBaixaTituloReceberDocumentoRateioValorPago, "BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDesconto/RatearValorPagoEntreDocumentos", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor rateado sucesso!");

                _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();

                PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });

                Global.fecharModal('knockoutRateioValorPagoDocumentos');
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CancelarRateioValorPagoDocumentosClick(e, sender) {
    LimparCamposNegociacaoBaixaTituloReceberDocumentoRateioValorPago();
}

function FecharNegociacaoBaixaTituloReceberDocumentoRateioValorPagoClick(e, sender) {
    Global.fecharModal('knockoutRateioValorPagoDocumentos');
    LimparCamposNegociacaoBaixaTituloReceberDocumentoRateioValorPago();
}

function LimparCamposNegociacaoBaixaTituloReceberDocumentoRateioValorPago() {
    LimparCampos(_negociacaoBaixaTituloReceberDocumentoRateioValorPago);
}