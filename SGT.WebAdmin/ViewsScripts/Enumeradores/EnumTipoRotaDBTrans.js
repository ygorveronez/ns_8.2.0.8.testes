var EnumTipoRotaDBTransHelper = function () {
    this.Todos = "";
    this.RotaFixa = 0;
    this.RotaTemporaria = 1;
};

EnumTipoRotaDBTransHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Rota Fixa", value: this.RotaFixa },
            { text: "Rota Temporária", value: this.RotaTemporaria }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRotaDBTrans = Object.freeze(new EnumTipoRotaDBTransHelper());