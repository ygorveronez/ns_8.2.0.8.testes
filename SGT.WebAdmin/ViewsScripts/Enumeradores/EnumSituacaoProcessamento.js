var EnumSituacaoProcessamentoHelper = function () {
    this.Todos = "";
    this.Processado = 1;
    //this.RegistroNaoEncontrado = 2;
    this.PendenteProcessamento = 0;
};

EnumSituacaoProcessamentoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Todos", value: this.Todos },
            { text: "Processados", value: this.Processado },
            { text: "Pendente Processamento", value: this.PendenteProcessamento }
            //{ text: "Registro Não Encontrado", value: this.RegistroNaoEncontrado }
        ];
    },
}

var EnumSituacaoProcessamento = Object.freeze(new EnumSituacaoProcessamentoHelper());