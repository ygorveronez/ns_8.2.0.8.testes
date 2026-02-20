var EnumTipoNotasFiscaisHelper = function () {
    this.Faturamento = 1;
    this.RemessaPallet = 2;
    this.OrdemVenda = 3;
    this.RemessaVenda = 4;
};

EnumTipoNotasFiscaisHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Faturamento), value: this.Faturamento },
            { text: this.obterDescricao(this.RemessaPallet), value: this.RemessaPallet },
            { text: this.obterDescricao(this.OrdemVenda), value: this.OrdemVenda },
            { text: this.obterDescricao(this.RemessaVenda), value: this.RemessaVenda }
        ];
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Faturamento: return "Faturamento";
            case this.RemessaPallet: return "Remessa de Pallet";
            case this.OrdemVenda: return "Ordem de Venda";
            case this.RemessaVenda: return "Remessa de Venda";
            default: return "";
        }
    }
};

var EnumTipoNotasFiscais = Object.freeze(new EnumTipoNotasFiscaisHelper());