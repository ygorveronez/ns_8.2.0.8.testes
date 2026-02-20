var EnumTipoTaxaTerceiroHelper = function () {
    this.Todos = 0;
    this.PorKM = 1;
    this.PorVeiculo = 2;
    this.PorTerceiro = 3;
};

EnumTipoTaxaTerceiroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Por KM", value: this.PorKM },
            { text: "Por Veículo", value: this.PorVeiculo },
            { text: "Por Terceiro", value: this.PorTerceiro }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoTaxaTerceiro = Object.freeze(new EnumTipoTaxaTerceiroHelper());