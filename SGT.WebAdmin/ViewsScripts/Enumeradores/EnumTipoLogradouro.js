var EnumTipoLogradouroHelper = function () {
    this.Todos = "";
    this.Rua = 1;
    this.Avenida = 2;
    this.Rodovia = 3;
    this.Estrada = 4;
    this.Praca = 5;
    this.Travessa = 6;
    this.Outros = 99;
};

EnumTipoLogradouroHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoLogradouro.Rua, value: this.Rua },
            { text: Localization.Resources.Enumeradores.TipoLogradouro.Avenida, value: this.Avenida },
            { text: Localization.Resources.Enumeradores.TipoLogradouro.Rodovia, value: this.Rodovia },
            { text: Localization.Resources.Enumeradores.TipoLogradouro.Estrada, value: this.Estrada },
            { text: Localization.Resources.Enumeradores.TipoLogradouro.Praca, value: this.Praca },
            { text: Localization.Resources.Enumeradores.TipoLogradouro.Travessa, value: this.Travessa },
            { text: Localization.Resources.Enumeradores.TipoLogradouro.Outros, value: this.Outros }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoLogradouro = Object.freeze(new EnumTipoLogradouroHelper());