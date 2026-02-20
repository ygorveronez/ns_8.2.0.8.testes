var EnumSituacaoAceiteContratoHelper = function () {
    this.Todos = "";
    this.Aceito = 1;
    this.Pendente = 2;
};

EnumSituacaoAceiteContratoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aceito", value: this.Aceito },
            { text: "Pendente", value: this.Pendente }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoAceiteContrato = Object.freeze(new EnumSituacaoAceiteContratoHelper());