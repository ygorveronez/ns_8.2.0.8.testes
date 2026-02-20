var EnumTipoMontagemCargaHelper = function () {
    this.Todos = 0;
    this.NovaCarga = 1;
    this.AgruparCargas = 2;
};

EnumTipoMontagemCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoMontagemCarga.AgruparCargas, value: this.AgruparCargas },
            { text: Localization.Resources.Enumeradores.TipoMontagemCarga.NovaCarga, value: this.NovaCarga }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoMontagemCarga.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoMontagemCarga = Object.freeze(new EnumTipoMontagemCargaHelper());
