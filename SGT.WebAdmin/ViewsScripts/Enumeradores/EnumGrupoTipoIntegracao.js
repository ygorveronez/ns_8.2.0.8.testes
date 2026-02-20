var EnumGrupoTipoIntegracaoHelper = function () {
    this.Todos = null;
    this.ValePedagio = 1;
    this.GerenciadoraDeRisco = 2;
    this.Rastreadora = 3;
    this.OcorrenciaEntrega = 4;
};

EnumGrupoTipoIntegracaoHelper.prototype = {
    obterOpcoes: function () {
        var options = [
            { text: "Vale pedágio", value: this.ValePedagio },
            { text: "Gerenciadora de risco", value: this.GerenciadoraDeRisco },
            { text: "Rastreadora", value: this.Rastreadora },
            { text: "Ocorrência de entrega", value: this.OcorrenciaEntrega },
        ];
        return options;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumGrupoTipoIntegracao = Object.freeze(new EnumGrupoTipoIntegracaoHelper());