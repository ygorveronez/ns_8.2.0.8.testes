var EnumCategoriaOpcaoCheckListHelper = function () {
    this.Todas = "";
    this.Tracao = 1;
    this.Reboque = 2;
    this.Motorista = 3;
    this.Manutencao = 4;
    this.Outro = 5;
}

EnumCategoriaOpcaoCheckListHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Tração", value: this.Tracao },
            { text: "Reboque", value: this.Reboque },
            { text: "Motorista", value: this.Motorista },
            { text: "Manutenção", value: this.Manutencao },
            { text: "Outro", value: this.Outro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumCategoriaOpcaoCheckList = Object.freeze(new EnumCategoriaOpcaoCheckListHelper());
