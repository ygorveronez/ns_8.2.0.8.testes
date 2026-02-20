var EnumTamanhoNumerodeCargaAlfanumericoHelper = function () {
    this.Nenhum = null;
    this.Dois = 2;
    this.Tres = 3;
    this.Quatro = 4;
    this.Cinco = 5;
    this.Seis = 6;
    this.Sete = 7;
    this.Oito = 8;
    this.Nove = 9;
    this.Dez = 10;
};

EnumTamanhoNumerodeCargaAlfanumericoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "", value: this.Nenhum },
            { text: this.Dois +" "+ Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Dois },
            { text: this.Tres + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Tres },
            { text: this.Quatro + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Quatro },
            { text: this.Cinco + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Cinco },
            { text: this.Seis + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Seis },
            { text: this.Sete + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Sete },
            { text: this.Oito + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Oito },
            { text: this.Nove + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Nove },
            { text: this.Dez + " " + Localization.Resources.Enumeradores.TamanhoNumerodeCargaAlfanumerico.Caracteres, value: this.Dez }
        ];
    }
};

var EnumTamanhoNumerodeCargaAlfanumerico = Object.freeze(new EnumTamanhoNumerodeCargaAlfanumericoHelper());
