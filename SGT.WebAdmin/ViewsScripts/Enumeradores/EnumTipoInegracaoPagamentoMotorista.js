var EnumTipoIntegracaoPagamentoMotoristaHelper = function () {
    this.SemIntegracao = 1;
    this.Pamcard = 2;
    this.PagBem = 3;
    this.PamcardCorporativo = 4;
    this.Email = 5;
    this.Target = 6;
    this.Extratta = 7;
    this.RepomFrete = 8;
    this.KMM = 9;
};

EnumTipoIntegracaoPagamentoMotoristaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Sem Integração", value: this.SemIntegracao },
            { text: "Pamcard", value: this.Pamcard },
            { text: "Pagbem", value: this.PagBem },
            { text: "Pamcard Corporativo", value: this.PamcardCorporativo },
            { text: "E-mail", value: this.Email },
            { text: "Target", value: this.Target },
            { text: "Extratta", value: this.Extratta },
            { text: "Repom Frete", value: this.RepomFrete }
        ];
    }
};

var EnumTipoIntegracaoPagamentoMotorista = Object.freeze(new EnumTipoIntegracaoPagamentoMotoristaHelper());