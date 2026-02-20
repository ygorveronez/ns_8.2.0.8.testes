var EnumTipoObrigacaoUsoTerminalHelper = function () {
    this.Todos = 0;
    this.Nenhum = 1;
    this.OrigemDestino = 2;
    this.Origem  = 3;
    this.Destino  = 4;
}

EnumTipoObrigacaoUsoTerminalHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Nenhum;
            case this.OrigemDestino: return Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.OrigemDestino;
            case this.Origem: return Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Origem;
            case this.Destino: return Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Destino;
            case this.Todos: return Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Todos;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.OrigemDestino, value: this.OrigemDestino },
            { text: Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Origem, value: this.Origem },
            { text: Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Destino, value: this.Destino }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoObrigacaoUsoTerminal.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoObrigacaoUsoTerminal = Object.freeze(new EnumTipoObrigacaoUsoTerminalHelper());