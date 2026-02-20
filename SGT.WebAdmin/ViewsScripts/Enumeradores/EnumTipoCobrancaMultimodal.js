var EnumTipoCobrancaMultimodalHelper = function () {
    this.Todos = -1;
    this.Nenhum = 0;
    this.BLLongoCurso = 1;
    this.NotaFiscalServico = 2;
    this.CTeRodoviario = 3;
    this.CTeMultimodal = 4;
    this.CTEAquaviario = 5;
}

EnumTipoCobrancaMultimodalHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return Localization.Resources.Enumeradores.TipoCobrancaMultimodal.Nenhum;
            case this.BLLongoCurso: return Localization.Resources.Enumeradores.TipoCobrancaMultimodal.UmBLDeLongoCurso;
            case this.NotaFiscalServico: return Localization.Resources.Enumeradores.TipoCobrancaMultimodal.DoisNotaFiscalDeServico;
            case this.CTeRodoviario: return Localization.Resources.Enumeradores.TipoCobrancaMultimodal.TresCTeRodoviario;
            case this.CTeMultimodal: return Localization.Resources.Enumeradores.TipoCobrancaMultimodal.QuatroCTeMultimodal;
            case this.CTEAquaviario: return Localization.Resources.Enumeradores.TipoCobrancaMultimodal.CincoCTeAquaviario;
            case this.Todos: return Localization.Resources.Enumeradores.TipoCobrancaMultimodal.Todos;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.UmBLDeLongoCurso, value: this.BLLongoCurso },
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.DoisNotaFiscalDeServico, value: this.NotaFiscalServico },
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.TresCTeRodoviario, value: this.CTeRodoviario },
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.QuatroCTeMultimodal, value: this.CTeMultimodal },
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.CincoCTeAquaviario, value: this.CTEAquaviario }
        ];
    },
    obterOpcoesMultimodal: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.Nenhum, value: this.Nenhum },
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.Multimodal, value: this.CTeMultimodal },
            { text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.CincoCTeAquaviario, value: this.CTEAquaviario }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoCobrancaMultimodal.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumTipoCobrancaMultimodal = Object.freeze(new EnumTipoCobrancaMultimodalHelper());