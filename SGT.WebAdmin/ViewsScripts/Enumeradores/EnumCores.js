var EnumCoresHelper = function () {
    this.Verde = 1;
    this.VerdeEscuro = 2;
    this.Vermelho = 3;
    this.Cinza = 4;
    this.Branco = 5;
    this.Azul = 6;
    this.Amarelo = 7;
    this.Laranja = 8;
};

EnumCoresHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Branco", value: this.Branco },
            { text: "Verde", value: this.Verde },
            { text: "Verde Escuro", value: this.VerdeEscuro },
            { text: "Vermelho", value: this.Vermelho },
            { text: "Cinza", value: this.Cinza },            
            { text: "Azul", value: this.Azul },
            { text: "Amarelo", value: this.Amarelo },
            { text: "Laranja", value: this.Laranja }
        ];
    },
};

var EnumCores = Object.freeze(new EnumCoresHelper());