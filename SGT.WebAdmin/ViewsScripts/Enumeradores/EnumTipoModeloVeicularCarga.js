var EnumTipoModeloVeicularCargaHelper = function () {
    this.SemModelo = -2;
    this.Todos = -1;
    this.Geral = 1;
    this.Reboque = 2;
    this.Tracao = 3;
};

EnumTipoModeloVeicularCargaHelper.prototype = {
    obterTodos: function () {
        return [
            this.Geral,
            this.Reboque,
            this.Tracao
        ];
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoModeloVeicularCarga.TracaoComCarroceria, value: this.Geral },
            { text: Localization.Resources.Enumeradores.TipoModeloVeicularCarga.Reboque, value: this.Reboque },
            { text: Localization.Resources.Enumeradores.TipoModeloVeicularCarga.Tracao, value: this.Tracao }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoModeloVeicularCarga.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaSemModelo: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoModeloVeicularCarga.SemModeloVeicular, value: this.SemModelo }].concat(this.obterOpcoes());
    },
}

var EnumTipoModeloVeicularCarga = Object.freeze(new EnumTipoModeloVeicularCargaHelper());