var EnumGeoLocalizacaoTipoHelper = function () {
    this.Todos = -1;
    this.Endereco = 0;
    this.Cidade = 1;
};

EnumGeoLocalizacaoTipoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.GeoLocalizacaoTipo.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.GeoLocalizacaoTipo.Endereco, value: this.Endereco },
            { text: Localization.Resources.Enumeradores.GeoLocalizacaoTipo.Cidade, value: this.Cidade }
        ];
    }
};

var EnumGeoLocalizacaoTipo = Object.freeze(new EnumGeoLocalizacaoTipoHelper());