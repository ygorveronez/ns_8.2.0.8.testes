var EnumEtapaSinistroHelper = function () {
    this.Todos = 0;
    this.Dados = 1;
    this.Documentacao = 2;
    this.Manutencao = 3;
    this.IndicacaoPagador = 4;
    this.Acompanhamento = 5;
};

EnumEtapaSinistroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Dados", value: this.Dados },
            { text: "Documentação", value: this.Documentacao },
            { text: "Manutenção", value: this.Manutencao },
            { text: "Indicação Pagador", value: this.IndicacaoPagador },
            { text: "Acompanhamento", value: this.Acompanhamento }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumEtapaSinistro = Object.freeze(new EnumEtapaSinistroHelper());
