//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoBaixaTituloReceber, _CRUDConfiguracaoBaixaTituloReceber;

var ConfiguracaoBaixaTituloReceber = function () {
    this.GerarMovimentoAutomaticoDiferencaCotacaoMoeda = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar movimento financeiro automático de diferenças na cotação de moedas estrangeiras:", val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.ListaConfiguracoesMoedas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.ConfiguracoesMoedas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.GerarMovimentoAutomaticoDiferencaCotacaoMoeda.val.subscribe(function (novoValor) {
        if (novoValor === true)
            $("#knockoutConfiguracaoFinanceiraBaixaTituloReceberMoeda").removeClass("d-none");
        else
            $("#knockoutConfiguracaoFinanceiraBaixaTituloReceberMoeda").addClass("d-none");
    });
};

var CRUDConfiguracaoBaixaTituloReceber = function () {
    this.Salvar = PropertyEntity({ eventClick: SalvarConfiguracaoBaixaTituloReceberClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadConfiguracaoBaixaTituloReceber() {

    _configuracaoBaixaTituloReceber = new ConfiguracaoBaixaTituloReceber();
    KoBindings(_configuracaoBaixaTituloReceber, "knockoutConfiguracaoFinanceiraBaixaTituloReceber");

    _CRUDConfiguracaoBaixaTituloReceber = new CRUDConfiguracaoBaixaTituloReceber();
    KoBindings(_CRUDConfiguracaoBaixaTituloReceber, "knockoutCRUDConfiguracaoFinanceiraBaixaTituloReceber");

    LoadConfiguracaoBaixaTituloReceberMoeda();
}

function SalvarConfiguracaoBaixaTituloReceberClick(e, sender) {
    _configuracaoBaixaTituloReceber.ListaConfiguracoesMoedas.val(JSON.stringify(_configuracaoBaixaTituloReceber.ConfiguracoesMoedas.val()));

    Salvar(_configuracaoBaixaTituloReceber, "ConfiguracaoFinanceira/SalvarConfiguracaoBaixaTituloReceber", function (r) {
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