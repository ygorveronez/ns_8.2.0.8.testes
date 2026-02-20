var EnumSexoHelper = function () {
    this.NaoInformado = 0;
    this.Masculino = 1;
    this.Feminino = 2;
};

EnumSexoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.Sexo.NaoInformado, value: this.NaoInformado },
            { text: Localization.Resources.Enumeradores.Sexo.Masculino, value: this.Masculino },
            { text: Localization.Resources.Enumeradores.Sexo.Feminino, value: this.Feminino }
        ];
    }
};

var EnumSexo = Object.freeze(new EnumSexoHelper());