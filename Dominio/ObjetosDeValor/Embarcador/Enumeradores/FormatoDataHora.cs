namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormatoData
    {
        Padrao = 1,
        AnoMesDiaJunto = 2
    }
    
    public static class FormatoDataHelper
    {
        public static string ObterDescricao(this FormatoData formato)
        {
            switch (formato)
            {
                case FormatoData.Padrao: return "dd/MM/yyyy";
                case FormatoData.AnoMesDiaJunto: return "yyyyMMdd";
                default: return string.Empty;
            }
        }
    }
    
    public enum FormatoHora
    {
        Padrao = 1,
        HoraMinutoJunto = 2
    }
    
    public static class FormatoHoraHelper
    {
        public static string ObterDescricao(this FormatoHora formato)
        {
            switch (formato)
            {
                case FormatoHora.Padrao: return "HH:mm:ss";
                case FormatoHora.HoraMinutoJunto: return "HHmmss";
                default: return string.Empty;
            }
        }
    }
}
