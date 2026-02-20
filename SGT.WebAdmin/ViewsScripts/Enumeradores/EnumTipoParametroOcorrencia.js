var EnumTipoParametroOcorrenciaHelper = function () {
    this.Todos = "";
    this.Periodo = 1;
    this.Booleano = 2;
    this.Inteiro = 3;
    this.Texto = 4;
    this.Data = 5;
}

EnumTipoParametroOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoParametroOcorrencia.Booleano, value: this.Booleano },
            { text: Localization.Resources.Enumeradores.TipoParametroOcorrencia.Data, value: this.Data },
            { text: Localization.Resources.Enumeradores.TipoParametroOcorrencia.Inteiro, value: this.Inteiro },
            { text: Localization.Resources.Enumeradores.TipoParametroOcorrencia.Periodo, value: this.Periodo },
            { text: Localization.Resources.Enumeradores.TipoParametroOcorrencia.Texto, value: this.Texto }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoParametroOcorrencia.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoParametroOcorrencia = Object.freeze(new EnumTipoParametroOcorrenciaHelper());