const EnumTipoEntidadeIntegracaoHelper = function () {
    this.Todas = null;
    this.Carga = 1;
    this.CargaOcorrencia = 2;
    this.CTe = 3;
};

EnumTipoEntidadeIntegracaoHelper.prototype = {
    obterOpcoes: function () {
        const opcoes = [];

        opcoes.push({ text: "Carga", value: this.Carga });
        opcoes.push({ text: "Ocorrência", value: this.CargaOcorrencia });
        opcoes.push({ text: "CTe", value: this.CTe });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todas }].concat(this.obterOpcoes());
    }
};

const EnumTipoEntidadeIntegracao = Object.freeze(new EnumTipoEntidadeIntegracaoHelper());
