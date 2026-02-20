var EnumTipoNotasGestaoDevolucaoHelper = function () {
    this.Mercadoria = 1;
    this.Pallet = 2;
    this.Mista = 3;
};

EnumTipoNotasGestaoDevolucaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Mercadoria", value: this.Mercadoria },
            { text: "Pallet", value: this.Pallet },
            { text: "Mista", value: this.Mista }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Mercadoria", value: this.Mercadoria },
            { text: "Pallet", value: this.Pallet },
            { text: "Mista", value: this.Mista }
        ];
    },
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Pallet: return "Pallet";
            case this.Mercadoria: return "Mercadoria";
            case this.Mista: return "Mista";
            default: return "";
        }
    }
};

var EnumTipoNotasGestaoDevolucao = Object.freeze(new EnumTipoNotasGestaoDevolucaoHelper());