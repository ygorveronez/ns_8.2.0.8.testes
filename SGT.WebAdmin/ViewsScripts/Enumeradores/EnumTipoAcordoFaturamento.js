var EnumTipoAcordoFaturamentoHelper = function () {
    this.Todos = 0;
    this.NaoInformado = 1;
    this.FreteLongoCurso = 2;
    this.CustoExtra = 3;
};

EnumTipoAcordoFaturamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Informado", value: this.NaoInformado },
            { text: "Frete Longo Curso", value: this.FreteLongoCurso },
            { text: "Custo Extra", value: this.CustoExtra },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesProjetoNFTP: function () {
        return [
            { text: "Não Informado", value: this.NaoInformado },
            { text: "Custo Extra", value: this.CustoExtra },
        ];
    }
};

var EnumTipoAcordoFaturamento = Object.freeze(new EnumTipoAcordoFaturamentoHelper());
