namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public enum enumTipoWS
    {
        POST = 1,
        GET = 2,
        PATCH = 3,
        PUT = 4
    }

    public enum enumAcaoContratoFrete
    {
        IncluirContratoFrete = 1,
        ConsultarStatusIncluirContratoFrete = 2,
        ConsultarStatusCIOTANTT = 3
    }

    public enum enumStatusCIOT
    {
        ERROR = 0,
        INTRANSIT = 1,
        ACCOUNTABILITY = 2,
        CLEARED = 3,
        AUTHORIZED = 4,
        PAID = 5,
        INTERRUPTED = 6,
        CANCELLED = 7,
        PENDINGISSUE = 8,
        PENDINGCANCELLATION = 9,
        LOCKED = 10,
        AWAITINGSETTLEMENTINPUT = 11
    }

    public static class enumRepomFrete
    {
        public static enumStatusCIOT ObterEnumStatusCIOT(string status)
        {
            switch (status.ToLower())
            {
                case "intransit": return enumStatusCIOT.INTRANSIT;
                case "accountability": return enumStatusCIOT.ACCOUNTABILITY;
                case "cleared": return enumStatusCIOT.CLEARED;
                case "authorized": return enumStatusCIOT.AUTHORIZED;
                case "paid": return enumStatusCIOT.PAID;
                case "interrupted": return enumStatusCIOT.INTERRUPTED;
                case "cancelled": return enumStatusCIOT.CANCELLED;
                case "pendingissue": return enumStatusCIOT.PENDINGISSUE;
                case "pendingcancellation": return enumStatusCIOT.PENDINGCANCELLATION;
                case "locked": return enumStatusCIOT.LOCKED;
                case "awaitingsettlementinput": return enumStatusCIOT.AWAITINGSETTLEMENTINPUT;
                default: return enumStatusCIOT.ERROR;
            }
        }
    }
}
