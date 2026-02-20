var EnumSituacaoTrajetoCargaHelper = function () {
    this.Todos = -1;
    this.AguardandoInicio = 0;
    this.EmTransporte = 1;
    this.Finalizada = 2;
};

EnumSituacaoTrajetoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Início", value: this.AguardandoInicio },
            { text: "Em transporte", value: this.EmTransporte },
            { text: "Finalizada", value: this.Finalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoTrajetoCarga = Object.freeze(new EnumSituacaoTrajetoCargaHelper());