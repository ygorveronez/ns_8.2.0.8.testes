var EnumPrioridadeOfertaCargaHelper = function () {
    this.Zero = 0;
    this.Um = 1;
    this.Dois = 2;
    
};


EnumPrioridadeOfertaCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "0", value: this.Zero },
            { text: "1", value: this.Um },
            { text: "2", value: this.Dois }
        ];
    },

};


var EnumPrioridadeOfertaCarga = Object.freeze(new EnumPrioridadeOfertaCargaHelper());