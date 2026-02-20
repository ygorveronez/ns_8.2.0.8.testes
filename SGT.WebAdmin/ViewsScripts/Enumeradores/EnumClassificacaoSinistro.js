var EnumClassificacaoSinistroHelper = function () {
    this.Sl = 0;
    this.Spt = 1;
    this.Cpt = 2;
}

EnumClassificacaoSinistroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "SL", value: this.Multa },
            { text: "SPT", value: this.Outro },
            { text: "CPT", value: this.Sinistro },
        ];
    },   
}

var EnumClassificacaoSinistro = Object.freeze(new EnumClassificacaoSinistroHelper());