var EnumTipoCargaMDFeHelper = function () {
    this.NaoDefinido = 0;
    this.GranelSolido = 1;
    this.GranelLiquido = 2;
    this.Frigorificada = 3;
    this.Conteinerizada = 4;
    this.CargaGeral = 5;
    this.Neogranel = 6;
    this.PerigosaGranelSolido = 7;
    this.PerigosaGranelLiquido = 8;
    this.PerigosaFrigorificada = 9;
    this.PerigosaConteinerizada = 10;
    this.PerigosaCargaGeral = 11;
    this.GranelPressurizada = 12;
};

EnumTipoCargaMDFeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.NaoDefinido, value: this.NaoDefinido },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.GranelSolido, value: this.GranelSolido },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.GranelLiquido, value: this.GranelLiquido },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.Frigorificada, value: this.Frigorificada },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.Conteinerizada, value: this.Conteinerizada },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.CargaGeral, value: this.CargaGeral },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.Neogranel, value: this.Neogranel },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.PerigosaGranelSolido, value: this.PerigosaGranelSolido },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.PerigosaGranelLiquido, value: this.PerigosaGranelLiquido },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.PerigosaFrigorificada, value: this.PerigosaFrigorificada },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.PerigosaConteinerizada, value: this.PerigosaConteinerizada },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.PerigosaCargaGeral, value: this.PerigosaCargaGeral },
            { text: Localization.Resources.Enumeradores.TipoCargaMDFe.GranelPressurizada, value: this.GranelPressurizada }
        ];
    }
};

var EnumTipoCargaMDFe = Object.freeze(new EnumTipoCargaMDFeHelper());