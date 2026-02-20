const EnumTipoDestinoGhostHelper = function () {
    this.FilaH = 0;
    this.MuleSoft = 1;
};

EnumTipoDestinoGhostHelper.prototype = {
    obterOpcoes: function (exibirOpcaoCancelada) {
        const opcoes = [];

        opcoes.push({ text: "Mule -> FilaH", value: this.FilaH });
        opcoes.push({ text: "FilaH -> Mule", value: this.MuleSoft });

        return opcoes;
    }
};

const EnumTipoDestinoGhost = Object.freeze(new EnumTipoDestinoGhostHelper());
