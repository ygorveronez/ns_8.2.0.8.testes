var EnumModalidadeFreteHelper = function () {
    this.Todos = "";
    this.Emitente = 0;
    this.Destinatario = 1;
    this.Terceiros = 2;
    this.ProprioRemetente = 3;
    this.ProprioDestinatario = 4;
    this.SemFrete = 9;
};

EnumModalidadeFreteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "0 - Contratação do Frete por conta do Remetente (CIF)", value: this.Emitente },
            { text: "1 - Contratação do Frete por conta do Destinatário (FOB)", value: this.Destinatario },
            { text: "2 - Contratação do Frete por conta de Terceiros", value: this.Terceiros },
            { text: "3 - Transporte Próprio por conta do Remetente", value: this.ProprioRemetente },
            { text: "4 - Transporte Próprio por conta do Destinatário", value: this.ProprioDestinatario },
            { text: "9 - Sem Ocorrência de Transporte", value: this.SemFrete }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumModalidadeFrete = Object.freeze(new EnumModalidadeFreteHelper());