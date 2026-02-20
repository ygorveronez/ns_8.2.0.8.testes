var EnumChamadoResponsavelOcorrenciaHelper = function () {
    this.Logistica = 1;
    this.Fabrica = 2;
    this.Comercial = 3;
    this.Representante = 4;
    this.Cliente = 5;
    this.CD = 6;
    this.Qualidade = 7;
    this.Fiscal = 8;
    this.Transportador = 9;
};

EnumChamadoResponsavelOcorrenciaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Comercial, value: this.Comercial },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Fabrica, value: this.Fabrica },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Logistica, value: this.Logistica },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Representante, value: this.Representante },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Cliente, value: this.Cliente },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.CD, value: this.CD },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Qualidade, value: this.Qualidade },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Fiscal, value: this.Fiscal },
            { text: Localization.Resources.Enumeradores.ChamadoResponsavelOcorrencia.Transportador, value: this.Transportador }
        ];
    },
};

var EnumChamadoResponsavelOcorrencia = Object.freeze(new EnumChamadoResponsavelOcorrenciaHelper());