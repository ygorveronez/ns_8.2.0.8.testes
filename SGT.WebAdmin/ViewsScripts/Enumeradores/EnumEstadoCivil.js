var EnumEstadoCivilHelper = function () {
    this.Outros = 0;
    this.Solteiro = 1;
    this.Casado = 2;
    this.Divorciado = 3;
    this.Desquitado = 4;
    this.Viuvo = 5;
};

EnumEstadoCivilHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.EstadoCivil.Outros, value: this.Outros },
            { text: Localization.Resources.Enumeradores.EstadoCivil.Solteiro, value: this.Solteiro },
            { text: Localization.Resources.Enumeradores.EstadoCivil.Casado, value: this.Casado },
            { text: Localization.Resources.Enumeradores.EstadoCivil.Divorciado, value: this.Divorciado },
            { text: Localization.Resources.Enumeradores.EstadoCivil.Desquitado, value: this.Desquitado },
            { text: Localization.Resources.Enumeradores.EstadoCivil.Viuvo, value: this.Viuvo },
        ];
    }
};

var EnumEstadoCivil = Object.freeze(new EnumEstadoCivilHelper());