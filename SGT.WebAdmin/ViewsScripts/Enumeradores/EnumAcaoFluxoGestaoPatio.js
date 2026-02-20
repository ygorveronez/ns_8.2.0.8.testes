var EnumAcaoFluxoGestaoPatioHelper = function () {
    this.Confirmar = 1;
    this.Voltar = 2;
};

EnumAcaoFluxoGestaoPatioHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Confirmar: return Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarEtapa;
            case this.Voltar: return Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Confirmar), value: this.Confirmar },
            { text: this.obterDescricao(this.Voltar), value: this.Voltar},
        ];
    },
    obterOpcoesSubareaCliente: function () {
        return [
            { text: this.obterDescricao(this.Confirmar), value: this.Confirmar },
        ];
    }
};

var EnumAcaoFluxoGestaoPatio = Object.freeze(new EnumAcaoFluxoGestaoPatioHelper());
