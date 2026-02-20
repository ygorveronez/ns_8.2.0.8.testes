var EnumTipoPalletizacaoHelper = function () {
    this.Todos = "";
    this.Pallet = 0;
    this.Araras = 1;
    this.Racks =  2;
};

EnumTipoPalletizacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pallet", value: this.Pallet },
            { text: "Araras", value: this.Araras },
            { text: "Racks", value: this.Racks }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoPalletizacao = Object.freeze(new EnumTipoPalletizacaoHelper());
