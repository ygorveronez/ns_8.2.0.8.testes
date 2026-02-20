//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoPedagio;

var ConfiguracaoPedagio = function () {
    this.GerarMovimentoAutomaticoNoLancamentoPedagio = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar movimento financeiro automático no lançamento do pedágio:", val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.TipoMovimentoLancamentoPedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Lançamento Pedágio:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoLancamentoPedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Lançamento Pedágio:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.Visible = this.ExibirFiltros = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Salvar = PropertyEntity({ eventClick: SalvarConfiguracaoPedagioClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });

    this.GerarMovimentoAutomaticoNoLancamentoPedagio.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoNoLancamentoPedagio(novoValor);
    });
}

//*******EVENTOS*******

function LoadConfiguracaoPedagio() {

    _configuracaoPedagio = new ConfiguracaoPedagio();
    KoBindings(_configuracaoPedagio, "divConfiguracaoFinanceiraPedagio");

    new BuscarTipoMovimento(_configuracaoPedagio.TipoMovimentoLancamentoPedagio);
    new BuscarTipoMovimento(_configuracaoPedagio.TipoMovimentoReversaoLancamentoPedagio);

}

function SalvarConfiguracaoPedagioClick(e, sender) {
    Salvar(_configuracaoPedagio, "ConfiguracaoFinanceira/SalvarConfiguracaoPedagio", function (r) {
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

function GerarMovimentoAutomaticoNoLancamentoPedagio(novoValor) {
    if (novoValor) {
        _configuracaoPedagio.Visible.visibleFade(true);
        _configuracaoPedagio.TipoMovimentoLancamentoPedagio.required(true);
        _configuracaoPedagio.TipoMovimentoReversaoLancamentoPedagio.required(true);
    } else {
        _configuracaoPedagio.Visible.visibleFade(false);
        _configuracaoPedagio.TipoMovimentoLancamentoPedagio.required(false);
        _configuracaoPedagio.TipoMovimentoReversaoLancamentoPedagio.required(false);
    }
}

//*******MÉTODOS*******

