var EnumSistemaEmissorHelper = function () {
    this.Todos = 0;
    this.Multiembarcador = 1;
    this.ForaMultiembarcador = 2;
};

EnumSistemaEmissorHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SistemaEmissor.Todos, value: this.Todos },
            { text: Localization.Resources.Enumeradores.SistemaEmissor.Multiembarcador, value: this.Multiembarcador },
            { text: Localization.Resources.Enumeradores.SistemaEmissor.ForaDoMultiembarcador, value: this.ForaMultiembarcador }
        ];
    }
};

var EnumSistemaEmissor = Object.freeze(new EnumSistemaEmissorHelper());