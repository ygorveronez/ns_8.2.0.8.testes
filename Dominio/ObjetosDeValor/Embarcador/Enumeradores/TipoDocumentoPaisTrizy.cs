namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDocumentoPaisTrizy
    {
        Brasil = 0,
        Argentina = 1,
        Paraguai = 2,
        Uruguai = 3,
        Bolivia = 4,
        Venezuela = 5,
        Chile = 6,
        Colombia = 7,
        CostaRica = 8,
        Cuba = 9,
        Equador = 10,
        ElSalvador = 11,
        Guatemala = 12,
        Haiti = 13,
        Honduras = 14,
        Mexico = 15,
        Nicaragua = 16,
        Panama = 17,
        Peru = 18,
        RepublicaDominicana = 19,
        EstadosUnidosDaAmerica = 20,
        Canada = 21,
        Estrangeiro = 999,
    }

    public static class TipoDocumentoPaisTrizyHelper
    {
        public static string ObterDescricao(this TipoDocumentoPaisTrizy data)
        {
            switch (data)
            {
                case TipoDocumentoPaisTrizy.Brasil: return "Brasil";
                case TipoDocumentoPaisTrizy.Argentina: return "Argentina";
                case TipoDocumentoPaisTrizy.Paraguai: return "Paraguai";
                case TipoDocumentoPaisTrizy.Uruguai: return "Uruguai";
                case TipoDocumentoPaisTrizy.Bolivia: return "Bolivia";
                case TipoDocumentoPaisTrizy.Venezuela: return "Venezuela";
                case TipoDocumentoPaisTrizy.Chile: return "Chile";
                case TipoDocumentoPaisTrizy.Colombia: return "Colombia";
                case TipoDocumentoPaisTrizy.CostaRica: return "Costa Rica";
                case TipoDocumentoPaisTrizy.Cuba: return "Cuba";
                case TipoDocumentoPaisTrizy.Equador: return "Equador";
                case TipoDocumentoPaisTrizy.ElSalvador: return "El Salvador";
                case TipoDocumentoPaisTrizy.Guatemala: return "Guatemala";
                case TipoDocumentoPaisTrizy.Haiti: return "Haití";
                case TipoDocumentoPaisTrizy.Honduras: return "Honduras";
                case TipoDocumentoPaisTrizy.Mexico: return "México";
                case TipoDocumentoPaisTrizy.Nicaragua: return "Nicarágua";
                case TipoDocumentoPaisTrizy.Panama: return "Panamá";
                case TipoDocumentoPaisTrizy.Peru: return "Perú";
                case TipoDocumentoPaisTrizy.RepublicaDominicana: return "Republica Dominicana";
                case TipoDocumentoPaisTrizy.EstadosUnidosDaAmerica: return "Estados Unidos da América";
                case TipoDocumentoPaisTrizy.Canada: return "Canadá";
                case TipoDocumentoPaisTrizy.Estrangeiro: return "Estrangeiro";
                default: return "";
            }
        }

        public static string ObterPersonDocumentType(this TipoDocumentoPaisTrizy data)
        {
            switch (data)
            {
                case TipoDocumentoPaisTrizy.Brasil: return "CPF";
                case TipoDocumentoPaisTrizy.Argentina: return "ARG_CUIL";
                case TipoDocumentoPaisTrizy.Paraguai: return "PRY_RUC";
                case TipoDocumentoPaisTrizy.Uruguai: return "URY_CI";
                case TipoDocumentoPaisTrizy.Bolivia: return "BOL_CI";
                case TipoDocumentoPaisTrizy.Venezuela: return "VEN_CI";
                case TipoDocumentoPaisTrizy.Chile: return "CHL_RUN";
                case TipoDocumentoPaisTrizy.Colombia: return "COL_CC";
                case TipoDocumentoPaisTrizy.CostaRica: return "CRI_CI";
                case TipoDocumentoPaisTrizy.Cuba: return "CUB_CI";
                case TipoDocumentoPaisTrizy.Equador: return "ECU_CC";
                case TipoDocumentoPaisTrizy.ElSalvador: return "SLV_DUI";
                case TipoDocumentoPaisTrizy.Guatemala: return "GTM_DPI";
                case TipoDocumentoPaisTrizy.Haiti: return "HTI_CIN";
                case TipoDocumentoPaisTrizy.Honduras: return "HND_CI";
                case TipoDocumentoPaisTrizy.Mexico: return "MEX_INE";
                case TipoDocumentoPaisTrizy.Nicaragua: return "NIC_CI";
                case TipoDocumentoPaisTrizy.Panama: return "PAN_CIP";
                case TipoDocumentoPaisTrizy.Peru: return "PER_DNI";
                case TipoDocumentoPaisTrizy.RepublicaDominicana: return "DOM_CIE";
                case TipoDocumentoPaisTrizy.EstadosUnidosDaAmerica: return "USA_SSN";
                case TipoDocumentoPaisTrizy.Canada: return "CAN_SIN";
                case TipoDocumentoPaisTrizy.Estrangeiro: return "FOREIGNER";
                default: return "";
            }
        }

        public static string ObterLegalPersonDocumentType(this TipoDocumentoPaisTrizy data)
        {
            switch (data)
            {
                case TipoDocumentoPaisTrizy.Brasil: return "CNPJ";
                case TipoDocumentoPaisTrizy.Argentina: return "ARG_CUIT";
                case TipoDocumentoPaisTrizy.Paraguai: return "PRY_RUC";
                case TipoDocumentoPaisTrizy.Uruguai: return "URY_RUT";
                case TipoDocumentoPaisTrizy.Bolivia: return "BOL_NIT";
                case TipoDocumentoPaisTrizy.Venezuela: return "VEN_RIF";
                case TipoDocumentoPaisTrizy.Colombia: return "COL_NIT";
                case TipoDocumentoPaisTrizy.Peru: return "FOREIGNER";
                case TipoDocumentoPaisTrizy.Estrangeiro: return "FOREIGNER";
                default: return "";
            }
        }
    }
}
