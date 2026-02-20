var EnumListaTipoTempoDescargaHelper = function () {
    this.PorModeloVeicular = 1;
    this.PorPeso = 2;
};

EnumListaTipoTempoDescargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ListaTipoTempoDescarga.PorModeloVeicular, value: this.PorModeloVeicular },
            { text: Localization.Resources.Enumeradores.ListaTipoTempoDescarga.PorPeso, value: this.PorPeso }
        ];
    }
};

var EnumListaTipoTempoDescarga = Object.freeze(new EnumListaTipoTempoDescargaHelper());