var EnumNumeroReboqueHelper = function () {
    this.SemReboque = 0;
    this.ReboqueUm = 1;
    this.ReboqueDois = 2;
}

EnumNumeroReboqueHelper.prototype = {
    obterDescricao: function (numeroReboque) {
        switch (numeroReboque) {
            case this.ReboqueUm: return "Reboque Um";
            case this.ReboqueDois: return "Reboque Dois";
            default: return "Sem Reboque";
        }
    },
    obterOpcoes: function () {
        return [
            { text: "Sem Reboque", value: this.SemReboque },
            { text: "Reboque Um", value: this.ReboqueUm },
            { text: "Reboque Dois", value: this.ReboqueDois }
        ];
    }
}

var EnumNumeroReboque = Object.freeze(new EnumNumeroReboqueHelper());