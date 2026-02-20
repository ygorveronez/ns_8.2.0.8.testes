var EnumResponsavelChamadoHelper = function () {
    this.Todos = 0;
    this.Backhall = 1;
    this.Comercial = 2;
    this.GA = 3;
    this.ADM = 4;
    this.CD = 5;
    this.Cliente = 6;
    this.Transportador = 7;
};

EnumResponsavelChamadoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ResponsavelChamado.Backhaul, value: this.Backhall },
            { text: Localization.Resources.Enumeradores.ResponsavelChamado.Comercial, value: this.Comercial },
            { text: Localization.Resources.Enumeradores.ResponsavelChamado.GA, value: this.GA },
            { text: Localization.Resources.Enumeradores.ResponsavelChamado.ADM, value: this.ADM },
            { text: Localization.Resources.Enumeradores.ResponsavelChamado.CD, value: this.CD },
            { text: Localization.Resources.Enumeradores.ResponsavelChamado.Cliente, value: this.Cliente },
            { text: Localization.Resources.Enumeradores.ResponsavelChamado.Transportador, value: this.Transportador }
        ];
    },
    obterOpcoesPesquisa: function (defaultText, defaultValue) {
        return [{ text: defaultText || Localization.Resources.Enumeradores.ResponsavelChamado.Todos, value: defaultValue || "" }].concat(this.obterOpcoes());
    }
}

var EnumResponsavelChamado = Object.freeze(new EnumResponsavelChamadoHelper());