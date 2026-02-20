var EnumOrigemMercadoriaHelper = function () {
    this.Todos = -1;
    this.Origem0 = 0;
    this.Origem1 = 1;
    this.Origem2 = 2;
    this.Origem3 = 3;
    this.Origem4 = 4;
    this.Origem5 = 5;
    this.Origem6 = 6;
    this.Origem7 = 7;
    this.Origem8 = 8;
};

EnumOrigemMercadoriaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "0 - Nacional, exceto as indicadas nos códigos 3, 4, 5 e 8", value: this.Origem0 },
            { text: "1 - Estrangeira - Importação direta, exceto a indicada no código 6", value: this.Origem1 },
            { text: "2 - Estrangeira - Adquirida no mercado interno, exceto a indicada no código 7", value: this.Origem2 },
            { text: "3 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a 40% e inferior ou igual a 70%", value: this.Origem3 },
            { text: "4 - Nacional, cuja produção tenha sido feita em conformidade com os processos produtivos básicos de que tratam as legislações citadas nos Ajustes", value: this.Origem4 },
            { text: "5 - Nacional, mercadoria ou bem com Conteúdo de Importação inferior ou igual a 40%", value: this.Origem5 },
            { text: "6 - Estrangeira - Importação direta, sem similar nacional, constante em lista da CAMEX e gás natural", value: this.Origem6 },
            { text: "7 - Estrangeira - Adquirida no mercado interno, sem similar nacional, constante lista CAMEX e gás natural", value: this.Origem7 },
            { text: "8 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a 70%", value: this.Origem8 }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesCadastro: function () {
        return [{ text: "Selecione", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumOrigemMercadoria = Object.freeze(new EnumOrigemMercadoriaHelper());