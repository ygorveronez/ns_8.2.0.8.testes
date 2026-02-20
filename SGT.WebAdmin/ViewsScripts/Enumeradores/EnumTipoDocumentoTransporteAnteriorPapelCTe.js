var EnumTipoDocumentoTransporteAnteriorPapelCTeHelper = function () {
    this.CTRC = 00;
    this.CTAC = 01;
    this.ACT = 02;
    this.NFModelo7 = 03;
    this.NFModelo27 = 04;
    this.ConhecimentoAereoNacional = 05;
    this.CTMC = 06;
    this.ATRE = 07;
    this.DTA = 08;
    this.ConhecimentoAereoInternacional = 09;
    this.ConhecimentoCartaPorteInternacional = 10;
    this.ConhecimentoAvulso =11;
    this.TIF = 12;
    this.BIL = 13;
    this.Outros = 99;
};

EnumTipoDocumentoTransporteAnteriorPapelCTeHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.CTRC, value: this.CTRC },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.CTAC, value: this.CTAC },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.ACT, value: this.ACT },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.NFModeloSete, value: this.NFModelo7 },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.NFModeloVinteSete, value: this.NFModelo27 },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.ConhecimentoAereoNacional, value: this.ConhecimentoAereoNacional },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.CTMC, value: this.CTMC },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.ATRE, value: this.ATRE },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.DTA, value: this.DTA },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.ConhecimentoAereoInternacional, value: this.ConhecimentoAereoInternacional },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.ConhecimentoCartaPorteInternacional, value: this.ConhecimentoCartaPorteInternacional },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.ConhecimentoAvulso, value: this.ConhecimentoAvulso },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.TIF, value: this.TIF },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.BIL, value: this.BIL },
            { text: Localization.Resources.Enumeradores.TipoDocumentoTransporteAnteriorPapelCTe.Outros, value: this.Outros }
        ];
    },
};

var EnumTipoDocumentoTransporteAnteriorPapelCTe = Object.freeze(new EnumTipoDocumentoTransporteAnteriorPapelCTeHelper());