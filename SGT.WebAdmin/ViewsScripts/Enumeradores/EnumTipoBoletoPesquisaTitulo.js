var EnumTipoBoletoPesquisaTituloHelper = function () {
    this.Todos = 0;
    this.ComBoleto = 1;
    this.SemBoleto = 2;
    this.ComRemessa = 3;
    this.SemRemessa = 4;
};

EnumTipoBoletoPesquisaTituloHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Com Boleto", value: this.ComBoleto },
            { text: "Sem Boleto", value: this.SemBoleto },
            { text: "Com Remessa", value: this.ComRemessa },
            { text: "Sem Remessa", value: this.SemRemessa }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoBoletoPesquisaTitulo = Object.freeze(new EnumTipoBoletoPesquisaTituloHelper());