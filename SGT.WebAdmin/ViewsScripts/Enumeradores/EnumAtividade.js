var EnumAtividadeHelper = function () {
    this.ServicosMesma = 1;
    this.Industrial = 2;
    this.Comercial = 3;
    this.PrestadoraServico = 4;
    this.DistribuidoraEnergia = 5;
    this.ProdutorRural = 6;
    this.NaoContribuinte = 7;
};

EnumAtividadeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "1 - " + Localization.Resources.Enumeradores.Atividade.ServicosMesma, value: this.ServicosMesma },
            { text: "2 - " + Localization.Resources.Enumeradores.Atividade.Industrial, value: this.Industrial },
            { text: "3 - " + Localization.Resources.Enumeradores.Atividade.Comercial, value: this.Comercial },
            { text: "4 - " + Localization.Resources.Enumeradores.Atividade.PrestadoraServico, value: this.PrestadoraServico },
            { text: "5 - " + Localization.Resources.Enumeradores.Atividade.DistribuidoraEnergia, value: this.DistribuidoraEnergia },
            { text: "6 - " + Localization.Resources.Enumeradores.Atividade.ProdutorRural, value: this.ProdutorRural },
            { text: "7 - " + Localization.Resources.Enumeradores.Atividade.NaoContribuinte, value: this.NaoContribuinte }
        ];
    },
};

var EnumAtividade = Object.freeze(new EnumAtividadeHelper());