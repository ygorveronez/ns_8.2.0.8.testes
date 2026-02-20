var EnumTipoRejeicaoPelaIA = function () {
    this.Todos = 0;
    this.Comprovante = 1;
    this.NumeroDoc = 2;
    this.Data = 3;
    this.Assinatura = 4;
};

EnumTipoRejeicaoPelaIA.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Comprovante", value: this.Comprovante },
            { text: "Número Doc.", value: this.NumeroDoc },
            { text: "Data", value: this.Data },
            { text: "Assinatura", value: this.Assinatura }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoRejeicaoPelaIA = Object.freeze(new EnumTipoRejeicaoPelaIA());