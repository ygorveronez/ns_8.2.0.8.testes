let EnumVersaoA52Helper = function () {
    this.Versao10 = 0;
    this.Versao17 = 1;
};

EnumVersaoA52Helper.prototype = {
    obterOpcoes: function () {
        return [
            {
                value: this.Versao10,
                text: "Versão 1.0"
            }, {
                value: this.Versao17,
                text: "Versão 1.7"
            }
        ];
    }
}

let EnumVersaoA52 = Object.freeze(new EnumVersaoA52Helper());