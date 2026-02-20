var EnumModalPropostaCabotagemHelper = function () {
    this.Todos = 0;
    this.PortoPorto = 1;
};

EnumModalPropostaCabotagemHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ModalProposta.PortoPorto, value: this.PortoPorto },
            { text: Localization.Resources.Enumeradores.ModalProposta.Todos, value: this.Todos }
        ];
    },
};

var EnumModalPropostaCabotagem = Object.freeze(new EnumModalPropostaCabotagemHelper());