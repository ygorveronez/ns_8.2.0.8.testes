var EnumSituacaoTrajetoHelper = function () {
    this.Todos = 0;
    this.Origem = 1;
    this.TransitoOrigem = 2;
    this.TransitoDestino = 3;
    this.Destino = 4;
};

EnumSituacaoTrajetoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Origem", value: this.Origem },
            { text: "Trânsito Origem", value: this.TransitoOrigem },
            { text: "Trânsito Destino", value: this.TransitoDestino },
            { text: "Destino", value: this.Destino }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoTrajeto = Object.freeze(new EnumSituacaoTrajetoHelper());