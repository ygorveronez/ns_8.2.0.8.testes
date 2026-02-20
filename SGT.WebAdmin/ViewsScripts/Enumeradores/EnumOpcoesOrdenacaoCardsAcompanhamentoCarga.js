var EnumOpcoesOrdenacaoCardsAcompanhamentoCargaHelper = function () {
    this.DataCriacaoCarga = 1;
    this.DataCarregamentoCarga = 2;
};

EnumOpcoesOrdenacaoCardsAcompanhamentoCargaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Data de criação da carga", value: this.DataCriacaoCarga },
            { text: "Data de carregamento da carga", value: this.DataCarregamentoCarga },
        ];    
    }
}

var EnumOpcoesOrdenacaoCardsAcompanhamentoCarga = Object.freeze(new EnumOpcoesOrdenacaoCardsAcompanhamentoCargaHelper());