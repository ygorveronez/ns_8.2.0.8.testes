var EnumTipoResidenciaHelper = function () {
    this.Nenhum = 0;
    this.Proprio = 1;
    this.ProprioFinanciado = 2;
    this.Alugado = 3;
    this.Familiar = 4;
    this.Cedido = 5;
};

EnumTipoResidenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoResidencia.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoResidencia.Proprio, value: this.Proprio },
            { text: Localization.Resources.Enumeradores.TipoResidencia.ProprioFinanciado, value: this.ProprioFinanciado },
            { text: Localization.Resources.Enumeradores.TipoResidencia.Alugado, value: this.Alugado },
            { text: Localization.Resources.Enumeradores.TipoResidencia.Familiar, value: this.Familiar },
            { text: Localization.Resources.Enumeradores.TipoResidencia.Cedido, value: this.Cedido }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoResidencia = Object.freeze(new EnumTipoResidenciaHelper());