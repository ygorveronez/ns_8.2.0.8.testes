var EnumAceiteMotoristaHelper = function () {
    this.Todos = "";
    this.Enviado = 0;
    this.Aceite = 1;
    this.Recusou = 2;
    this.Expirou = 3;
};

EnumAceiteMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Escala enviada", value: this.Enviado },
            { text: "Motorista confirmou escala", value: this.Aceite },
            { text: "Motorista recusou escala", value: this.Recusou },
            { text: "Expirou tempo confirmação escala", value: this.Expirou },
        ];
    },

    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumAceiteMotorista = Object.freeze(new EnumAceiteMotoristaHelper);