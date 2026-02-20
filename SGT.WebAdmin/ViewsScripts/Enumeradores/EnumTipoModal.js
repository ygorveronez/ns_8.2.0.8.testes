var EnumTipoModalHelper = function () {
    this.Todos = 0;
    this.Rodoviario = 1;
    this.Aereo = 2;
    this.Aquaviario = 3;
    this.Ferroviario = 4;
    this.Dutoviario = 5;
    this.Multimodal = 6;
};

EnumTipoModalHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoModal.Rodoviario, value: this.Rodoviario },
            { text: Localization.Resources.Enumeradores.TipoModal.Aereo, value: this.Aereo },
            { text: Localization.Resources.Enumeradores.TipoModal.Aquaviario, value: this.Aquaviario },
            { text: Localization.Resources.Enumeradores.TipoModal.Ferroviario, value: this.Ferroviario },
            { text: Localization.Resources.Enumeradores.TipoModal.Dutoviario, value: this.Dutoviario },
            { text: Localization.Resources.Enumeradores.TipoModal.Multimodal, value: this.Multimodal }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoModal.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoModal = Object.freeze(new EnumTipoModalHelper());