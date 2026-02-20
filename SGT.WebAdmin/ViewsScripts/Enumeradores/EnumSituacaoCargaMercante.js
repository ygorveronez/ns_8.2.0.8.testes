var EnumSituacaoCargaMercanteHelper = function () {
    this.Todas = 0;
    this.Cancelada = 1;
    this.AguardandoEmissao = 2;
    this.PendenteEmissaoCTe = 3;
    this.PendenteMDFe = 4;
    this.PendenteMercante = 5;
    this.PendenteFaturamento = 6;
    this.PendenteIntegracaoCTe = 7;
    this.PendenteIntegracaoFatura = 8;
    this.ComErro = 9;
    this.Finalizada = 10;
    this.Anulada = 11;
    this.PendenteSVM = 12;
};

EnumSituacaoCargaMercanteHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Canceladas", value: this.Cancelada },
            { text: "Aguardando Emissão", value: this.AguardandoEmissao },
            { text: "Pendente Emissão CT-e", value: this.PendenteEmissaoCTe },
            { text: "Pendente MDF-e", value: this.PendenteMDFe },
            { text: "Pendente Mercante", value: this.PendenteMercante },
            { text: "Pendente SVM", value: this.PendenteSVM },
            { text: "Pendente Faturamento", value: this.PendenteFaturamento },
            { text: "Pendente Integração CT-e", value: this.PendenteIntegracaoCTe },
            { text: "Pendente Integração Fatura", value: this.PendenteIntegracaoFatura },
            { text: "Com Erro", value: this.ComErro },
            { text: "Anuladas", value: this.Anulada },
            { text: "Finalizada", value: this.Finalizada }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todas", value: this.Todas }].concat(this.obterOpcoes());
    }
};

var EnumSituacaoCargaMercante = Object.freeze(new EnumSituacaoCargaMercanteHelper());