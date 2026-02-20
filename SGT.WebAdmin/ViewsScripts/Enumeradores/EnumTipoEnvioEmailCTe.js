var EnumTipoEnvioEmailCTeHelper = function () {
    this.Normal = 0;
    this.SomenteXMLSemCorpoAssuntoChave = 1;
};

EnumTipoEnvioEmailCTeHelper.prototype.ObterOpcoes = function () {
    return [
        { text: Localization.Resources.Enumeradores.TipoEnvioEmailCTe.NormalPDFXML, value: this.Normal },
        { text: Localization.Resources.Enumeradores.TipoEnvioEmailCTe.SomenteXMLSemCorpoDeEmail, value: this.SomenteXMLSemCorpoAssuntoChave }
    ];
};

var EnumTipoEnvioEmailCTe = Object.freeze(new EnumTipoEnvioEmailCTeHelper());