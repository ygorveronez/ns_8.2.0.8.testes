var EnumVidaPneuHelper = function () {
    this.Todas = "";
    this.PneuNovo = 1;
    this.PrimeiroRecape = 2;
    this.SegundoRecape = 3;
    this.TerceiroRecape = 4;
    this.QuartoRecape = 5;
    this.QuintoRecape = 6;
    this.SextoRecape = 7;
};

EnumVidaPneuHelper.prototype = {
    obterDescricao: function (vida) {
        switch (vida) {
            case this.PneuNovo: return "Pneu Novo (1° Vida)";
            case this.PrimeiroRecape: return "1º Recape (2ª Vida)";
            case this.SegundoRecape: return "2º Recape (3ª Vida)";
            case this.TerceiroRecape: return "3º Recape (4ª Vida)";
            case this.QuartoRecape: return "4º Recape (5ª Vida)";
            case this.QuintoRecape: return "5º Recape (6ª Vida)";
            case this.SextoRecape: return "6º Recape (7ª Vida)";
            default: return undefined;
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Pneu Novo (1° Vida)", value: this.PneuNovo },
            { text: "1º Recape (2ª Vida)", value: this.PrimeiroRecape },
            { text: "2º Recape (3ª Vida)", value: this.SegundoRecape },
            { text: "3º Recape (4ª Vida)", value: this.TerceiroRecape },
            { text: "4º Recape (5ª Vida)", value: this.QuartoRecape },
            { text: "5º Recape (6ª Vida)", value: this.QuintoRecape },
            { text: "6º Recape (7ª Vida)", value: this.SextoRecape }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    },
    obterValorPorDescricao: function (descricao) {
        switch (descricao) {
            case "Pneu Novo (1° Vida)": return this.PneuNovo;
            case "1º Recape (2ª Vida)": return this.PrimeiroRecape;
            case "2º Recape (3ª Vida)": return this.SegundoRecape;
            case "3º Recape (4ª Vida)": return this.TerceiroRecape;
            case "4º Recape (5ª Vida)": return this.QuartoRecape;
            case "5º Recape (6ª Vida)": return this.QuintoRecape;
            case "6º Recape (7ª Vida)": return this.SextoRecape;
            default: return undefined;
        }
    },
    obterOpcoesCadastro: function () {
        return [{ text: "Selecione", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumVidaPneu = Object.freeze(new EnumVidaPneuHelper());