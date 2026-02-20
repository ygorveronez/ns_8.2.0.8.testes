var EnumTipoAreaHelper = function () {
    this.Raio = 1;
    this.Poligono = 2;
    this.Ponto = 3;
};


EnumTipoAreaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoArea.Raio, value: this.Raio },
            { text: Localization.Resources.Enumeradores.TipoArea.Poligono, value: this.Poligono },
            { text: Localization.Resources.Enumeradores.TipoArea.Ponto, value: this.Ponto }
        ];
    },
    obterPontoPoligono: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoArea.Raio, value: this.Raio },
            { text: Localization.Resources.Enumeradores.TipoArea.Poligono, value: this.Poligono },
        ];
    }
};


var EnumTipoArea = Object.freeze(new EnumTipoAreaHelper());
