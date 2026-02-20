var EnumTipoComercialHelper = function () {
    this.Selecione = "";
    this.Vendedor = 1;
    this.Gerente = 2;
    this.Supervisor = 3;
    this.GerenteNacional = 42;
    this.GerenteRede = 89;
    this.GerenteArea = 6;
    this.Promotor = 46;
    this.SupervisorDanone = 5;
};

EnumTipoComercialHelper.prototype = {
    obterOpcoes: function (opcaoSelecione) {
        var arrayOpcoes = [
            { text: Localization.Resources.Enumeradores.TipoComercial.Vendedor, value: this.Vendedor },
            { text: Localization.Resources.Enumeradores.TipoComercial.Gerente, value: this.Gerente },
            { text: Localization.Resources.Enumeradores.TipoComercial.Supervisor, value: this.Supervisor },
            { text: Localization.Resources.Enumeradores.TipoComercial.GerenteNacional, value: this.GerenteNacional },
            { text: Localization.Resources.Enumeradores.TipoComercial.GerenteDeRede, value: this.GerenteRede },
            { text: Localization.Resources.Enumeradores.TipoComercial.GerenteDeArea, value: this.GerenteArea },
            { text: Localization.Resources.Enumeradores.TipoComercial.Promotor, value: this.Promotor },
            { text: Localization.Resources.Enumeradores.TipoComercial.Vendedor, value: this.SupervisorDanone },
        ];

        if (opcaoSelecione) {
            arrayOpcoes.push({ text: Localization.Resources.Enumeradores.TipoComercial.Selecione, value: this.Selecione });
        }

        return arrayOpcoes;
    }
};

var EnumTipoComercial = Object.freeze(new EnumTipoComercialHelper());