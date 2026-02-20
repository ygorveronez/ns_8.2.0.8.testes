let EnumSituacaoCargaOfertaHelper = function () {
    this.Todos = 0;
    this.PendenteDeOferta = 1;
    this.EmOferta = 2;
    this.EmConfirmacao = 3;
    this.PrazoExpirado = 4;
    this.Confirmada = 5;
    this.Cancelada = 6
};

EnumSituacaoCargaOfertaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Pendente de oferta", value: this.PendenteDeOferta },
            { text: "Em oferta", value: this.EmOferta },
            { text: "Em confirmação", value: this.EmConfirmacao },
            { text: "Prazo expirado", value: this.PrazoExpirado },
            { text: "Confirmada", value: this.Confirmada },
            { text: "Cancelada", value: this.Cancelada },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDescricao: function (codigo) {
        let opcoes = this.obterOpcoes();
        let [descricao] = opcoes.filter(item => item.value === codigo);

        if (!descricao)
            return "";

        return descricao.text;
    }
};

let EnumSituacaoCargaOferta = Object.freeze(new EnumSituacaoCargaOfertaHelper());