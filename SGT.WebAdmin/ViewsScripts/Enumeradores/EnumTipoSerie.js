var EnumTipoSerieHelper = function () {
    this.CTe = 0;
    this.MDFe = 1;
    this.NFSe = 2;
    this.NFe = 3;
    this.OutrosDocumentos = 4;
    this.NFCe = 5;
    this.CTeRec = 9;
};

EnumTipoSerieHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.CTe: return Localization.Resources.Enumeradores.TipoSerie.CTe;
            case this.MDFe: return Localization.Resources.Enumeradores.TipoSerie.MDFe;
            case this.NFSe: return Localization.Resources.Enumeradores.TipoSerie.NFSe;
            case this.NFe: return Localization.Resources.Enumeradores.TipoSerie.NFe;
            case this.NFCe: return Localization.Resources.Enumeradores.TipoSerie.NFCe;
            case this.OutrosDocumentos: return Localization.Resources.Enumeradores.TipoSerie.OutrosDocumentos;
            case this.CTeRec: return Localization.Resources.Enumeradores.TipoSerie.CTeRecebido;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoSerie.CTe, value: this.CTe },
            { text: Localization.Resources.Enumeradores.TipoSerie.MDFe, value: this.MDFe },
            { text: Localization.Resources.Enumeradores.TipoSerie.NFSe, value: this.NFSe },
            { text: Localization.Resources.Enumeradores.TipoSerie.NFe, value: this.NFe },
            { text: Localization.Resources.Enumeradores.TipoSerie.NFCe, value: this.NFCe },
            { text: Localization.Resources.Enumeradores.TipoSerie.OutrosDocumentos, value: this.OutrosDocumentos },
            { text: Localization.Resources.Enumeradores.TipoSerie.CTeRecebido, value: this.CTeRec }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoSerie.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoSerie = Object.freeze(new EnumTipoSerieHelper());