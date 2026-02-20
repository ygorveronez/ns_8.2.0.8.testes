var EnumTipoClienteVendaDiretaHelper = function () {
    this.Web = 0;
    this.CallCenter = 1;
    this.NaoSeAplica = 2;
};

EnumTipoClienteVendaDiretaHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Web: return "Web";
            case this.CallCenter: return "Call Center";            
            case this.NaoSeAplica: return "Não se aplica";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Web), value: this.Web },
            { text: this.obterDescricao(this.CallCenter), value: this.CallCenter },
            { text: this.obterDescricao(this.NaoSeAplica), value: this.NaoSeAplica }
        ];
    }
};

var EnumTipoClienteVendaDireta = Object.freeze(new EnumTipoClienteVendaDiretaHelper());