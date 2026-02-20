var EnumSituacaoEnvioEmailHelper = function () {
    this.Todas = "";
    this.AguardandoEnvio = 1;
    this.Enviado = 2;
};

EnumSituacaoEnvioEmailHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Envio", value: this.AguardandoEnvio },
            { text: "Enviado", value: this.Enviado },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoEnvioEmail = Object.freeze(new EnumSituacaoEnvioEmailHelper());