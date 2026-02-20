var EnumTipoProbeHelper = function () {
    this.Todos = -1;
    this.NaoDefinido = 0;
    this.Probe = 1;
    this.Termografo = 2;
};

EnumTipoProbeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Definido", value: this.NaoDefinido },
            { text: "Probe", value: this.Probe },
            { text: "Termógrafo", value: this.Termografo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoProbe = Object.freeze(new EnumTipoProbeHelper());