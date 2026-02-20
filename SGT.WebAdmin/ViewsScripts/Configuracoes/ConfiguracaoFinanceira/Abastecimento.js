//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoAbastecimento;

var ConfiguracaoAbastecimento = function () {
    this.GerarMovimentoAutomaticoNoLancamentoAbastecimento = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar movimento financeiro automático no lançamento do Abastecimento:", val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.TipoMovimentoLancamentoAbastecimentoPosto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Lançamento Abastecimento (Posto):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoLancamentoAbastecimentoPosto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Reversão do Lançamento Abastecimento (Posto):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoLancamentoAbastecimentoBombaPropria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Lançamento Abastecimento (Bomba Própria):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão do Lançamento Abastecimento (Bomba Própria):", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.Visible = this.ExibirFiltros = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Salvar = PropertyEntity({ eventClick: SalvarConfiguracaoAbastecimentoClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });

    this.GerarMovimentoAutomaticoNoLancamentoAbastecimento.val.subscribe(function (novoValor) {
        GerarMovimentoAutomaticoNoLancamentoAbastecimento(novoValor);
    });
}

//*******EVENTOS*******

function LoadConfiguracaoAbastecimento() {

    _configuracaoAbastecimento = new ConfiguracaoAbastecimento();
    KoBindings(_configuracaoAbastecimento, "divConfiguracaoFinanceiraAbastecimento");

    new BuscarTipoMovimento(_configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto);
    new BuscarTipoMovimento(_configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoPosto);
    new BuscarTipoMovimento(_configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria);
    new BuscarTipoMovimento(_configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria);
}

function SalvarConfiguracaoAbastecimentoClick(e, sender) {
    Salvar(_configuracaoAbastecimento, "ConfiguracaoFinanceira/SalvarConfiguracaoAbastecimento", function (r) {
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

function GerarMovimentoAutomaticoNoLancamentoAbastecimento(novoValor) {
    if (novoValor) {
        _configuracaoAbastecimento.Visible.visibleFade(true);
        _configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto.required(false);
        _configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoPosto.required(false);
        _configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria.required(true);
        _configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria.required(true);
    } else {
        _configuracaoAbastecimento.Visible.visibleFade(false);
        _configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto.required(false);
        _configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoPosto.required(false);
        _configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria.required(false);
        _configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria.required(false);
    }
}

//*******MÉTODOS*******

