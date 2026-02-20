var EnumTipoEnvioIntegracaoNeokohmHelper = function () {
    this.NaoSelecionado = "";
    this.InicioViagem = 1;
    this.FimViagem = 2;
};

EnumTipoEnvioIntegracaoNeokohmHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Selecionado", value: this.NaoSelecionado },
            { text: "Início de Viagem", value: this.InicioViagem },
            { text: "Fim de Viagem", value: this.FimViagem }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: "Todos", value: this.NaoSelecionado },
            { text: "Início de Viagem", value: this.InicioViagem },
            { text: "Fim de Viagem", value: this.FimViagem }
        ];
    }
};

var EnumTipoEnvioIntegracaoNeokohm = Object.freeze(new EnumTipoEnvioIntegracaoNeokohmHelper());