var EnumOcorrenciaGeracaoRelatorioHelper = function () {
    this.Diario = 1;
    this.Semanal = 2;
    this.Mensal = 3;
}

EnumOcorrenciaGeracaoRelatorioHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Diário", value: this.Diario },
            { text: "Semanal", value: this.Semanal },
            { text: "Mensal", value: this.Mensal }
        ];
    }
}

var EnumOcorrenciaGeracaoRelatorio = Object.freeze(new EnumOcorrenciaGeracaoRelatorioHelper());
