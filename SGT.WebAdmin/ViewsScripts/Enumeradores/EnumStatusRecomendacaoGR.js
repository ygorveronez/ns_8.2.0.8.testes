var EnumStatusRecomendacaoGRHelper = function () {
    this.Todas = "";
    this.AguardandoConsulta = 1;
    this.AguardandoValidacaoGR = 2;
    this.Recomendado = 3;
    this.NaoRecomendado = 4;
    this.Falha = 5;
};

EnumStatusRecomendacaoGRHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Consulta", value: this.AguardandoConsulta },
            { text: "Aguardando Validação GR", value: this.AguardandoValidacaoGR },
            { text: "Recomendado", value: this.Recomendado },
            { text: "Não Recomendado", value: this.NaoRecomendado },
            { text: "Falha", value: this.Falha }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
}

var EnumStatusRecomendacaoGR = Object.freeze(new EnumStatusRecomendacaoGRHelper());
