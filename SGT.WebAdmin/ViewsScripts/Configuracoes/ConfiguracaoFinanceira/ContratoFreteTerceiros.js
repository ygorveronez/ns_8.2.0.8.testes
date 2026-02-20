//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoContratoFreteTerceiros, _CRUDConfiguracaoContratoFreteTerceiros;

var ConfiguracaoContratoFreteTerceiros = function () {
    this.GerarMovimentoAutomaticoNaGeracaoContratoFrete = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar movimento financeiro automático na geração do contrato de frete:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TipoMovimentoValorPagoTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Pago ao Terceiro:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoValorPagoTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Valor Pago ao Terceiro:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoValorPagoTerceiroCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Valor Pago ao Terceiro via CIOT:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoValorPagoTerceiroCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Valor Pago ao Terceiro via CIOT:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.GerarMovimentoAutomaticoPorTipoOperacao = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar movimento financeiro automático por tipo de operação ao aprovar o contrato de frete", val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.Visible = this.ExibirFiltros = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.ListaConfiguracoesTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.ConfiguracoesTipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.GerarMovimentoAutomaticoNaGeracaoContratoFrete.val.subscribe(function (novoValor) {
        if (novoValor == true)
            _configuracaoContratoFreteTerceiros.GerarMovimentoAutomaticoPorTipoOperacao.val(false);

        GerarMovimentoAutomaticoNaGeracaoContratoFreteChange(novoValor);
    });

    this.GerarMovimentoAutomaticoPorTipoOperacao.val.subscribe(function (novoValor) {
        if (novoValor == true) {
            _configuracaoContratoFreteTerceiros.GerarMovimentoAutomaticoNaGeracaoContratoFrete.val(false);
            $("#knockoutConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao").removeClass("d-none");
        } else {
            $("#knockoutConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao").addClass("d-none");
        }
    });

};

var CRUDConfiguracaoContratoFreteTerceiros = function () {
    this.Salvar = PropertyEntity({ eventClick: SalvarConfiguracaoContratoFreteTerceirosClick, type: types.event, text: "Salvar", icon: "fal fa-save", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadConfiguracaoContratoFreteTerceiros() {

    _configuracaoContratoFreteTerceiros = new ConfiguracaoContratoFreteTerceiros();
    KoBindings(_configuracaoContratoFreteTerceiros, "knockoutConfiguracaoFinanceiraContratoFreteTerceiros");

    _CRUDConfiguracaoContratoFreteTerceiros = new CRUDConfiguracaoContratoFreteTerceiros();
    KoBindings(_CRUDConfiguracaoContratoFreteTerceiros, "knockoutCRUDConfiguracaoFinanceiraContratoFreteTerceiros");

    new BuscarTipoMovimento(_configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiro);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiro);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT);
    new BuscarTipoMovimento(_configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT);

    LoadConfiguracaoContratoFreteTerceirosTipoOperacao();
}

function SalvarConfiguracaoContratoFreteTerceirosClick(e, sender) {
    _configuracaoContratoFreteTerceiros.ListaConfiguracoesTipoOperacao.val(JSON.stringify(_configuracaoContratoFreteTerceiros.ConfiguracoesTipoOperacao.val()));

    Salvar(_configuracaoContratoFreteTerceiros, "ConfiguracaoFinanceira/SalvarConfiguracaoContratoFreteTerceiros", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function GerarMovimentoAutomaticoNaGeracaoContratoFreteChange(novoValor) {
    _configuracaoContratoFreteTerceiros.Visible.visibleFade(novoValor);
    _configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiro.required(novoValor);
    _configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiro.required(novoValor);
    _configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT.required(novoValor);
    _configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT.required(novoValor);

    if (novoValor == true)
        _configuracaoContratoFreteTerceiros.GerarMovimentoAutomaticoPorTipoOperacao.val(!novoValor);
}

//*******MÉTODOS*******

