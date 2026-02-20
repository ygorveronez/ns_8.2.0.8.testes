var EnumTipoNotificacaoMotoristaSMSHelper = function () {
    this.Todos = "";
    this.FilaCarregamentoEnvioManual = 0;
    this.GestaoDePatioEnvioManual = 1;
};

EnumTipoNotificacaoMotoristaSMSHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Fila Carregamento - Envio Manual", value: this.FilaCarregamentoEnvioManual },
            { text: "Gestão de Pátio - Envio Manual", value: this.GestaoDePatioEnvioManual },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoNotificacaoMotoristaSMS = Object.freeze(new EnumTipoNotificacaoMotoristaSMSHelper());
