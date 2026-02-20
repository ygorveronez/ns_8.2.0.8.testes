
var EnumAprovacaoProvisaoHelper = function () {
    this.Todos = "";
    this.AguardandoProvisao = 1;
    this.Aprovado = 2;
    this.Rejeitado = 3;
};

EnumAprovacaoProvisaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Provisão", value: this.AguardandoProvisao },
            { text: "Aprovado", value: this.Aprovado },
            { text: "Rejeitado", value: this.Rejeitado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },

};


var EnumAprovacaoProvisao = Object.freeze(new EnumAprovacaoProvisaoHelper());