var EnumSimNaoNAHelper = function () {
    this.Nao = 0;
    this.Sim = 1;
    this.NaoAplicavel = 2;
};

EnumSimNaoNAHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nao: return Localization.Resources.Enumeradores.SimNao.Nao;
            case this.Sim: return Localization.Resources.Enumeradores.SimNao.Sim;
            case this.NaoAplicavel: return Localization.Resources.Enumeradores.SimNao.NaoAplicavel;
            default: return "";
        }
    },
    obterOpcoesNA: function () {
        return [
            { text: this.obterDescricao(this.Nao), value: this.Nao },
            { text: this.obterDescricao(this.Sim), value: this.Sim },
            { text: this.obterDescricao(this.NaoAplicavel), value: this.NaoAplicavel },
        ];
    }
};

var EnumSimNaoNA = Object.freeze(new EnumSimNaoNAHelper());