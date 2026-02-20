var EnumUnidadeMedidaFornecedorHelper = function () {
    this.Quilograma = 1;
    this.MetroCubico = 2;
    this.Tonelada = 3;
    this.Unidade = 4;
    this.Litros = 5;
    this.MMBTU = 6;
};

EnumUnidadeMedidaFornecedorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Quilograma, value: this.Quilograma },
            { text: Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.MetroCubico, value: this.MetroCubico },
            { text: Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Tonelada, value: this.Tonelada },
            { text: Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Unidade, value: this.Unidade },
            { text: Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Litros, value: this.Litros },
            { text: Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.MMBTU, value: this.MMBTU }
        ];
    },
    obterDescricao: function (tipoUnidadeMedida) {
        switch (tipoUnidadeMedida) {
            case this.Quilograma:
                return Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Quilograma;
            case this.MetroCubico:
                return Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.MetroCubico;
            case this.Tonelada:
                return Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Tonelada;
            case this.Unidade:
                return Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Unidade;
            case this.Litros:
                return Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.Litros;
            case this.MMBTU:
                return Localization.Resources.Enumeradores.UnidadeMedidaFornecedor.MMBTU;
            default:
                return "";
        }
    }
}

var EnumUnidadeMedidaFornecedor = Object.freeze(new EnumUnidadeMedidaFornecedorHelper());