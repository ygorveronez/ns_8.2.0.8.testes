var EnumRetornoCargaTipoHelper = function () {
    this.Todos = "";
    this.Carregado = 1;
    this.Vazio = 2;
    this.Devolucao = 3;
}

EnumRetornoCargaTipoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.RetornoCargaTipo.Carregado, value: this.Carregado },
            { text: Localization.Resources.Enumeradores.RetornoCargaTipo.Vazio, value: this.Vazio },
            { text: Localization.Resources.Enumeradores.RetornoCargaTipo.Devolucao, value: this.Devolucao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.RetornoCargaTipo.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
}

var EnumRetornoCargaTipo = Object.freeze(new EnumRetornoCargaTipoHelper());