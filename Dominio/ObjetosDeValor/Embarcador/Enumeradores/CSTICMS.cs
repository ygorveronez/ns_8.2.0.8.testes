namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CSTICMS
    {
        CST00 = 1,
        CST10 = 2,
        CST20 = 3,
        CST30 = 4,
        CST40 = 5,
        CST41 = 6,
        CST50 = 7,
        CST51 = 8,
        CST60 = 9,
        CST70 = 10,
        CST90 = 11,
        CSOSN101 = 12,
        CSOSN102 = 13,
        CSOSN103 = 14,
        CSOSN201 = 15,
        CSOSN202 = 16,
        CSOSN203 = 17,
        CSOSN300 = 18,
        CSOSN400 = 19,
        CSOSN500 = 20,
        CSOSN900 = 21,
        CST61 = 22
    }

    public static class CSTICMSHelper
    {
        public static string ObterDescricao(this CSTICMS csticms)
        {
            switch (csticms)
            {
                case CSTICMS.CST00: return "CST00";
                case CSTICMS.CST10: return "CST10";
                case CSTICMS.CST20: return "CST20";
                case CSTICMS.CST30: return "CST30";
                case CSTICMS.CST40: return "CST40";
                case CSTICMS.CST41: return "CST41";
                case CSTICMS.CST50: return "CST50";
                case CSTICMS.CST51: return "CST51";
                case CSTICMS.CST60: return "CST60";
                case CSTICMS.CST61: return "CST61";
                case CSTICMS.CST70: return "CST70";
                case CSTICMS.CST90: return "CST90";
                case CSTICMS.CSOSN101: return "CSOSN101";
                case CSTICMS.CSOSN102: return "CSOSN102";
                case CSTICMS.CSOSN103: return "CSOSN103";
                case CSTICMS.CSOSN201: return "CSOSN201";
                case CSTICMS.CSOSN202: return "CSOSN202";
                case CSTICMS.CSOSN203: return "CSOSN203";
                case CSTICMS.CSOSN300: return "CSOSN300";
                case CSTICMS.CSOSN400: return "CSOSN400";
                case CSTICMS.CSOSN500: return "CSOSN500";
                case CSTICMS.CSOSN900: return "CSOSN900";
                default: return string.Empty;
            }
        }
    }
}
