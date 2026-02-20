var EnumTipoTomaodorCabotagemHelper = function () {
    this.Todos = 0;
    this.Destinatario = 1;
    this.Remetente = 2;
    this.Terceiro = 3;
};

EnumTipoTomaodorCabotagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoTomador.Destinatario, value: this.Destinatario },
            { text: Localization.Resources.Enumeradores.TipoTomador.Remetente, value: this.Remetente },
            { text: Localization.Resources.Enumeradores.TipoTomador.Terceiro, value: this.Terceiro },
            { text: Localization.Resources.Enumeradores.TipoTomador.Todos, value: this.Todos }
        ];
    },
};

var EnumTipoTomaodorCabotagem = Object.freeze(new EnumTipoTomaodorCabotagemHelper());
