namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContaContabil
    {
        Todos = 0,
        FreteLiquido = 1,
        TotalReceber = 2,
        ICMS = 3,
        ICMSST = 4,
        PIS = 5,
        COFINS = 6,
        FreteLiquido9 = 9,
        FreteLiquido2 = 10,
        ISS = 11,
        ISSRetido = 12,
        FreteValor = 13,
        AdValorem = 14,
        GRIS = 15,
        Pedagio = 16,
        TaxaDescarga = 17,
        TaxaEntrega = 18,
        ImpostoValorAgregado = 19,
        CustoFixo = 20,
        FreteCaixa = 21,
        FreteKM = 22,
        FretePeso = 23,
        FreteViagem = 24,
        TaxaTotal = 25,
        Pernoite = 26,
        TotalReceberSemISS = 27,
        FreteLiquidoSemComponentesFrete = 28,
        Criacao = 30,
        FreteLiquidoTotal = 31,
        Estadia = 32,
        DevolucaoPorPeso = 33,
        DevolucaoPercentual = 34,
        Reentrega = 35,
        Infrutifera = 36,
        FreetimeCavaloAtrelado = 37,
        FreetimeCavaloDesatrelado = 38,
        FreetimePorto = 39,
        FreetimeCarreta = 40,
        Demurrage = 41,
        PagamentoDesova = 42,
        ViagemFrustradaNoDestino = 43,
        PacoteDeVistoriaSuframa = 44,
        Recusa = 45,
        ValorManual = 46,
        CBS = 47,
        IBSMunicipal = 48,
        IBSEstadual = 49,
    }

    public static class TipoContaContabilHelper
    {
        public static string ObterDescricao(this TipoContaContabil tipo)
        {
            switch (tipo)
            {
                case TipoContaContabil.FreteLiquido: return "Frete Líquido";
                case TipoContaContabil.TotalReceber: return "Total a Receber";
                case TipoContaContabil.ICMS: return "ICMS";
                case TipoContaContabil.ICMSST: return "ICMS ST";
                case TipoContaContabil.COFINS: return "COFINS";
                case TipoContaContabil.FreteLiquido9: return "Frete Líquido";
                case TipoContaContabil.FreteLiquido2: return "Frete Líquido";
                case TipoContaContabil.ISS: return "ISS";
                case TipoContaContabil.ISSRetido: return "ISS Retido";
                case TipoContaContabil.FreteValor: return "Frete Valor";
                case TipoContaContabil.AdValorem: return "Ad Valorem";
                case TipoContaContabil.GRIS: return "GRIS";
                case TipoContaContabil.Pedagio: return "Pedágio";
                case TipoContaContabil.TaxaDescarga: return "Taxa de Descarga";
                case TipoContaContabil.TaxaEntrega: return "Taxa de Entrega";
                case TipoContaContabil.ImpostoValorAgregado: return "Imposto sobre Valor Agregado";
                case TipoContaContabil.CustoFixo: return "Custo Fixo";
                case TipoContaContabil.FreteCaixa: return "Frete Caixa";
                case TipoContaContabil.FreteKM: return "Frete KM";
                case TipoContaContabil.FretePeso: return "Frete Peso";
                case TipoContaContabil.FreteViagem: return "Frete Viagem";
                case TipoContaContabil.TaxaTotal: return "Taxa Total";
                case TipoContaContabil.Pernoite: return "Pernoite";
                case TipoContaContabil.TotalReceberSemISS: return "Total a Receber sem ISS";
                case TipoContaContabil.FreteLiquidoSemComponentesFrete: return "Frete Líquido sem Componentes de Frete";
                default: return string.Empty;
            }
        }

        public static string ObterCodigo(this TipoContaContabil tipo)
        {
            switch (tipo)
            {
                case TipoContaContabil.FreteLiquido:
                case TipoContaContabil.FreteLiquido9: 
                case TipoContaContabil.FreteLiquido2:
                case TipoContaContabil.FreteLiquidoSemComponentesFrete:
                case TipoContaContabil.FreteValor:
                case TipoContaContabil.TaxaDescarga:
                    return "10";

                case TipoContaContabil.ICMS: return "03";
                case TipoContaContabil.COFINS:
                case TipoContaContabil.PIS: 
                    return "11";

                case TipoContaContabil.TotalReceber:
                case TipoContaContabil.TotalReceberSemISS:
                    return "01";

                default:
                    return "";
            }
        }
    }
}
