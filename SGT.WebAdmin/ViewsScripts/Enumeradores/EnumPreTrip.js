var EnumPreTripHelper = function () {
    this.Todas = "";
    this.SemPreTrip = 0;
    this.ComPreTrip = 1;
};

EnumPreTripHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todas", value: this.Todas },
            { text: "Sem Pré Trip", value: this.SemPreTrip },
            { text: "Com Pré Trip", value: this.ComPreTrip },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumPreTrip = Object.freeze(new EnumPreTripHelper());