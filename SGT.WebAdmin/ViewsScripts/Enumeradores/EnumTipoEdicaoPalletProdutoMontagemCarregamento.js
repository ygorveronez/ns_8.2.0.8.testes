var EnumTipoEdicaoPalletProdutoMontagemCarregamentoHelper = function () {
    this.ControlePalletAbertoFechado = 0;    // Controle Pallet (Fechado Não, Aberto Sim) (Valor Padrão)
    this.EdicaoPermitida = 1;                // Permite editar quantidade de pallet em ambos (Pallet aberto ou fechado)
    this.EdicaoNaoPermitida = 2;             // Não permite editar quantidade pallet
}

EnumTipoEdicaoPalletProdutoMontagemCarregamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado, value: this.ControlePalletAbertoFechado },
            { text: Localization.Resources.Enumeradores.TipoEdicaoPalletProdutoMontagemCarregamento.EdicaoPermitida, value: this.EdicaoPermitida },
            { text: Localization.Resources.Enumeradores.TipoEdicaoPalletProdutoMontagemCarregamento.EdicaoNaoPermitida, value: this.EdicaoNaoPermitida }
        ];
    }
}

var EnumTipoEdicaoPalletProdutoMontagemCarregamento = Object.freeze(new EnumTipoEdicaoPalletProdutoMontagemCarregamentoHelper());