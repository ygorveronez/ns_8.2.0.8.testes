namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoComponenteFrete
    {
        TODOS = 0,
        ICMS = 1,
        PEDAGIO = 2,
        DESCARGA = 3,
        FRETE = 4,
        ADVALOREM = 5,
        ISS = 6,
        OUTROS = 9,
        ValorKM = 10,
        ValorKMExcedente = 11,
        CLIENTE = 12,
        PISCONFIS = 13,
        GRIS = 14,
        ENTREGA = 15,
        PERNOITE = 16,
        INCONSISTENTE = 99
    }

    public static class TipoComponenteFreteHelper
    {
        public static string ObterDescricao(this TipoComponenteFrete tipoComponenteFrete)
        {
            switch (tipoComponenteFrete)
            {
                case TipoComponenteFrete.DESCARGA: return "DESCARGA";
                case TipoComponenteFrete.ICMS: return "ICMS";
                case TipoComponenteFrete.ISS: return "ISS";
                case TipoComponenteFrete.PISCONFIS: return "PIS e COFINS";
                case TipoComponenteFrete.ADVALOREM: return "AD VALOREM";
                case TipoComponenteFrete.OUTROS: return "OUTROS";
                case TipoComponenteFrete.PEDAGIO: return "PEDAGIO";
                case TipoComponenteFrete.FRETE: return "FRETE";
                case TipoComponenteFrete.GRIS: return "GRIS";
                case TipoComponenteFrete.ENTREGA: return "ENTREGA";
                case TipoComponenteFrete.INCONSISTENTE: return "INCONSISTENTE";
                case TipoComponenteFrete.PERNOITE: return "PERNOITE";
                default: return string.Empty;
            }
        }
    }
}
