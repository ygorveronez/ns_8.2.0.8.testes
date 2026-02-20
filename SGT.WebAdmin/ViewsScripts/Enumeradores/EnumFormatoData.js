var EnumFormatoDataHelper = function () {
    this.NaoDefinido = "";
    this.Padrao = 1;
    this.AnoMesDiaJunto = 2;
};

EnumFormatoDataHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "dd/MM/yyyy", value: this.Padrao },
            { text: "yyyyMMdd", value: this.AnoMesDiaJunto }
        ];
    }
}

var EnumFormatoData = Object.freeze(new EnumFormatoDataHelper());
