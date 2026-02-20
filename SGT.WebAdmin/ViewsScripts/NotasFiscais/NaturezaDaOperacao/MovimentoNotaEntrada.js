/// <reference path="../../Consultas/TipoMovimento.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _movimentoNotaEntrada;

var MovimentoNotaEntrada = function () {
    this.GerarMovimentoAutomaticoEntrada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento financeiro automatizado para as duplicatas da entrada deste documento: ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoUsoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso da Duplicata:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão da Duplicata:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.GerarMovimentoAutomaticoEntrada.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoEntradaChange(novoValor);
    });
}

//*******EVENTOS*******

function LoadMovimentoNotaEntrada() {

    _movimentoNotaEntrada = new MovimentoNotaEntrada();
    KoBindings(_movimentoNotaEntrada, "knockoutMovimentoFinanceiroEntrada");

    new BuscarTipoMovimento(_movimentoNotaEntrada.TipoMovimentoUsoEntrada);
    new BuscarTipoMovimento(_movimentoNotaEntrada.TipoMovimentoReversaoEntrada);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liTabMovimentoFinanceiroEntrada").removeClass("hidden");
    }
}

function GerarMovimentoAutomaticoEntradaChange(novoValor) {
    _movimentoNotaEntrada.GerarMovimentoAutomaticoEntrada.visibleFade(novoValor);
    _movimentoNotaEntrada.TipoMovimentoUsoEntrada.required(novoValor);
    _movimentoNotaEntrada.TipoMovimentoReversaoEntrada.required(novoValor);
}

function LimparCamposMovimentoNotaEntrada() {
    LimparCampos(_movimentoNotaEntrada);
}