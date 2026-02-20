//#region Variaiveis Globais
var _gridProdutoPalletizacao;
var _produtoPalletizacao;
//#endregion

//#region Funções constructoras
function ProdutoPalletizacao() {
    this.QuantidadeCaixaPorCamadaPallet = PropertyEntity({ text: "Qtd caixa por camada no Pallet:", getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, maxlength: 14, required: false });
    this.ConfiguracaoPalletizacao = PropertyEntity({ text: "Configuração Palletização:", codEntity: ko.observable(0), type: types.entity, idBtnSearch: guid(), val: ko.observable(""), required: false });
    this.QuantidadeCaixaPorPallet = PropertyEntity({ getType: typesKnockout.int, maxlength: 15, text: Localization.Resources.Produtos.ProdutoEmbarcador.QtdCaixaPorPallet.getFieldDescription(), required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
}

function LoadProdutosPalletizacao() {
    _produtoPalletizacao = new ProdutoPalletizacao();
    KoBindings(_produtoPalletizacao, "knockoutPalletizacao");

    new BuscarConfiguracaoPalletizacao(_produtoPalletizacao.ConfiguracaoPalletizacao);

}
//#endregion


function LimparCamposPalletizacao() {
    LimparCampo(_produtoPalletizacao.ConfiguracaoPalletizacao);
    LimparCampo(_produtoPalletizacao.QuantidadeCaixaPorCamadaPallet);
    LimparCampo(_produtoPalletizacao.QuantidadeCaixaPorPallet);
}

function LimparTodosOsCampos() {
    LimparCampos(_produtoPalletizacao);
}

function SalvarConfiguracaoPalletizacao() {
    _produtoEmbarcador.ConfiguracaoPalletizacao.codEntity(_produtoPalletizacao.ConfiguracaoPalletizacao.codEntity());
    _produtoEmbarcador.ConfiguracaoPalletizacao.val(_produtoPalletizacao.ConfiguracaoPalletizacao.val());
    _produtoEmbarcador.QuantidadeCaixaPorPallet.val(_produtoPalletizacao.QuantidadeCaixaPorPallet.val());
    _produtoEmbarcador.QuantidadeCaixaPorCamadaPallet.val(_produtoPalletizacao.QuantidadeCaixaPorCamadaPallet.val());
}
function EditarConfiguracaoPalletizacao() {
    _produtoPalletizacao.ConfiguracaoPalletizacao.codEntity(_produtoEmbarcador.ConfiguracaoPalletizacao.codEntity());
    _produtoPalletizacao.ConfiguracaoPalletizacao.val(_produtoEmbarcador.ConfiguracaoPalletizacao.val());
    _produtoPalletizacao.QuantidadeCaixaPorPallet.val(_produtoEmbarcador.QuantidadeCaixaPorPallet.val());
    _produtoPalletizacao.QuantidadeCaixaPorCamadaPallet.val(_produtoEmbarcador.QuantidadeCaixaPorCamadaPallet.val());

}
//#endregion