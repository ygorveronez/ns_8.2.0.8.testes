var EnumTipoFavorecidoCIOTHelper = function () {
    this.Transportador = 1;
    this.Motorista = 2;
};

EnumTipoFavorecidoCIOTHelper.prototype.ObterOpcoes = function () {
    return [
        { value: "", text: Localization.Resources.Enumeradores.TipoFavorecidoCIOT.NaoSelecionado },
        { value: this.Transportador, text: Localization.Resources.Enumeradores.TipoFavorecidoCIOT.Transportador },
        { value: this.Motorista, text: Localization.Resources.Enumeradores.TipoFavorecidoCIOT.Motorista }
    ];
};

EnumTipoFavorecidoCIOTHelper.prototype.ObterOpcoesPesquisa = function () {
    return [
        { value: "", text: Localization.Resources.Enumeradores.TipoFavorecidoCIOT.Todos },
        { value: this.Transportador, text: Localization.Resources.Enumeradores.TipoFavorecidoCIOT.Transportador },
        { value: this.Motorista, text: Localization.Resources.Enumeradores.TipoFavorecidoCIOT.Motorista }
    ];
};

var EnumTipoFavorecidoCIOT = Object.freeze(new EnumTipoFavorecidoCIOTHelper());