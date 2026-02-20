var EnumCausadorSinistroHelper = function () {
    this.Todos = 0;
    this.VeiculoProprio = 1;
    this.VeiculoTerceiro = 2;
    this.NaoIdentificado = 3;
    this.Outros = 4;
};

EnumCausadorSinistroHelper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.VeiculoProprio,
                text: "Veículo Próprio"
            },
            {
                value: this.VeiculoTerceiro,
                text: "Veículo Terceiro"
            },
            {
                value: this.NaoIdentificado,
                text: "Não Identificado"
            },
            {
                value: this.Outros,
                text: "Outros"
            }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumCausadorSinistro = Object.freeze(new EnumCausadorSinistroHelper());
