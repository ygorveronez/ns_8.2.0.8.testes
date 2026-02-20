var EnumTipoLocalPrestacaoHelper = function () {
    this.todos = 0;
    this.interMunicipal = 1;
    this.intraMunicipal = 2;
};

EnumTipoLocalPrestacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoLocalPrestacao.Municipal, value: this.intraMunicipal },
            { text: Localization.Resources.Enumeradores.TipoLocalPrestacao.Intermunicipal, value: this.interMunicipal }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todas, value: this.todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoLocalPrestacao = Object.freeze(new EnumTipoLocalPrestacaoHelper());