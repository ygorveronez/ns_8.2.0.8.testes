namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusDocumento
    {
        NAOOK = 0,
        OK = 1
    }

    public static class StatusDocumentoHelper
    {
        public static string ObterDescricao(this StatusDocumento aceite)
        {
            switch (aceite)
            {
                case StatusDocumento.NAOOK: return "NÃO OK";
                case StatusDocumento.OK: return "OK";
               
                default: return string.Empty;
            }
        }
    }
}
