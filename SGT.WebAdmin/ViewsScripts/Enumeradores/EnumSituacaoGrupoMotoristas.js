var EnumSituacaoGrupoMotoristasHelper = function () {
    this.AguardandoIntegracoes = 0;
    this.FalhaNasIntegracoes = 1;
    this.Finalizado = 2;
};

EnumSituacaoGrupoMotoristasHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Aguardando Integrações", value: this.AguardandoIntegracoes },
            { text: "Falha nas Integrações", value: this.FalhaNasIntegracoes },
            { text: "Finalizado", value: this.Finalizado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterDescricao: function (valor) {
        const opcao = this.obterOpcoes().find(op => op.value === valor);
        return opcao ? opcao.text : null;
    }
};

var EnumSituacaoGrupoMotoristas = Object.freeze(new EnumSituacaoGrupoMotoristasHelper());