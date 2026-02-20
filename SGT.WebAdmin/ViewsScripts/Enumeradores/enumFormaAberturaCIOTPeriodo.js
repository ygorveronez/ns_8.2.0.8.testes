var EnumFormaAberturaCIOTPeriodoHelper = function () {
    this.Manual = 0;
    this.Automatico = 1;
};

EnumFormaAberturaCIOTPeriodoHelper.prototype = {
    ObterDescricao: function (formaAberturaCIOTPeriodo) {
        switch (formaAberturaCIOTPeriodo) {
            case this.Manual: return "Manual";
            case this.Automatico: return "Automatico";
            default: return "";
        }
    },
    ObterOpcoes: function () {
        return [
            { text: "Manual", value: this.Manual },
            { text: "Automatico", value: this.Automatico }
        ];
    }
};

var EnumFormaAberturaCIOTPeriodo = Object.freeze(new EnumFormaAberturaCIOTPeriodoHelper());