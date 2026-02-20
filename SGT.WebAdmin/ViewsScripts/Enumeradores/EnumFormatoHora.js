var EnumFormatoHoraHelper = function () {
    this.NaoDefinido = "";
    this.Padrao = 1;
    this.HoraMinutoJunto = 2;
};

EnumFormatoHoraHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "HH:mm:ss", value: this.Padrao },
            { text: "HHmmss", value: this.HoraMinutoJunto }
        ];
    }
}

var EnumFormatoHora = Object.freeze(new EnumFormatoHoraHelper());
