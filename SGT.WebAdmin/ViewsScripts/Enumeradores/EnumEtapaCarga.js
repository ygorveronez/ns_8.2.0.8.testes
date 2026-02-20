var EnumEtapaCargaHelper = function () {
    this.InicioEmbarcador = 1;
    this.InicioTMS = 2;
    this.FreteEmbarcador = 3;
    this.FreteTMS = 4;
    this.DadosTransportador = 5;
    this.NotaFiscal = 6;
    this.CTeNFs = 7;
    this.MDFe = 8;
    this.CTeFilialEmissora = 9;
    this.MDFeFilialEmissora = 10;
    this.Integracao = 11;
    this.Impressao = 12;
    this.IntegracaoFilialEmissora = 13;
    this.SubContratacao = 14;
    this.Transbordo = 15;
};

EnumEtapaCargaHelper.prototype = {
    obterNomeEtapa: function (etapa) {
        switch (etapa) {
            case this.CTeFilialEmissora: return "EtapaCTeFilialEmissora";
            case this.CTeNFs: return "EtapaCTeNFs";
            case this.DadosTransportador: return "EtapaDadosTransportador";
            case this.FreteEmbarcador: return "EtapaFreteEmbarcador";
            case this.FreteTMS: return "EtapaFreteTMS";
            case this.Impressao: return "EtapaImpressao";
            case this.InicioEmbarcador: return "EtapaInicioEmbarcador";
            case this.InicioTMS: return "EtapaInicioTMS";
            case this.Integracao: return "EtapaIntegracao";
            case this.IntegracaoFilialEmissora: return "EtapaIntegracaoFilialEmissora";
            case this.MDFe: return "EtapaMDFe";
            case this.MDFeFilialEmissora: return "EtapaMDFeFilialEmissora";
            case this.NotaFiscal: return "EtapaNotaFiscal";
            case this.SubContratacao: return "EtapaSubContratacao";
            case this.Transbordo: return "EtapaTransbordo";
            default: return "";
        }
    }
};

var EnumEtapaCarga = Object.freeze(new EnumEtapaCargaHelper());