var EnumTipoPesCarregamentoNavioHelper = function () {
    this.Todos = "";
    this.NaoInformado = 0;
    this.vintepes = 1;
    this.quarentapes = 2;



};




EnumTipoPesCarregamentoNavioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Informado", value: this.NaoInformado },
            { text: "20 Pés", value: this.vintepes },
            { text: "40 Pés", value: this.quarentapes }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },



};




var EnumTipoPesCarregamentoNavio = Object.freeze(new EnumTipoPesCarregamentoNavioHelper ());