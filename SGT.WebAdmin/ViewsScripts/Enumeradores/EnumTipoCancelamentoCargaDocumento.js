var EnumTipoCancelamentoCargaDocumentoHelper = function () {
    this.Todos = "";
    this.Carga = 0;
    this.Documentos = 1;
    this.TodosDocumentos = 2;
};

EnumTipoCancelamentoCargaDocumentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCancelamentoCargaDocumento.Carga, value: this.Carga },
            { text: Localization.Resources.Enumeradores.TipoCancelamentoCargaDocumento.Documentos, value: this.Documentos },
            { text: "Todos Documentos", value: this.TodosDocumentos }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCancelamentoCargaDocumento = Object.freeze(new EnumTipoCancelamentoCargaDocumentoHelper());