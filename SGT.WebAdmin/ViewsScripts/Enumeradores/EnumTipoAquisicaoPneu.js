var EnumTipoAquisicaoPneuHelper = function () {
    this.Todos = "";
    this.Carcaca = 1;
    this.PneuEmUso = 2;
    this.PneuNovoReposicao = 3;
    this.PneuNovoVeiculoNovo = 4;
    this.PneuRecapado = 5;
    this.PneuUsado = 6;
};

EnumTipoAquisicaoPneuHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Carcaças", value: this.Carcaca },
            { text: "Pneus em Uso", value: this.PneuEmUso },
            { text: "Pneus Novos - Reposição", value: this.PneuNovoReposicao },
            { text: "Pneus Novos - Veículos Novos", value: this.PneuNovoVeiculoNovo },
            { text: "Pneus Recapados", value: this.PneuRecapado },
            { text: "Pneus Usados", value: this.PneuUsado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastro: function () {
        return [{ text: "Selecione", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoAquisicaoPneu = Object.freeze(new EnumTipoAquisicaoPneuHelper());