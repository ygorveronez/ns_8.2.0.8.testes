var EnumRegimeTributarioCTeHelper = function () {
    this.NaoSelecionado = "";
    this.SimplesNacional = 1;
    this.SimplesNacionalExcessoReceita = 2;
    this.RegimeNormal = 3;
    this.SimplesNacionalMEI = 4;
};

EnumRegimeTributarioCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.RegimeTributarioCTe.SimplesNacional, value: this.SimplesNacional },
            { text: Localization.Resources.Enumeradores.RegimeTributarioCTe.SimplesNacionalExcessoReceita, value: this.SimplesNacionalExcessoReceita },
            { text: Localization.Resources.Enumeradores.RegimeTributarioCTe.RegimeNormal, value: this.RegimeNormal },
            { text: Localization.Resources.Enumeradores.RegimeTributarioCTe.SimplesNacionalMEI, value: this.SimplesNacionalMEI }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.NaoSelecionado }].concat(this.obterOpcoes());
    },
    obterOpcoesNaoSelecionado: function () {
        return [{ text: Localization.Resources.Gerais.Geral.NaoSelecionado, value: this.NaoSelecionado }].concat(this.obterOpcoes());
    }
};

var EnumRegimeTributarioCTe = Object.freeze(new EnumRegimeTributarioCTeHelper());