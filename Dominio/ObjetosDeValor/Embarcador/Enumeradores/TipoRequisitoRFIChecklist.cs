namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRequisitoRFIChecklist
    {
        Desejavel = 0,
        Indispensavel = 1
    }

    public static class TipoRequisitoRFIChecklistHelper
    {
        public static string ObterDescricao(this TipoRequisitoRFIChecklist requisito)
        {
            switch (requisito)
            {
                case TipoRequisitoRFIChecklist.Desejavel: return "Desejável";
                case TipoRequisitoRFIChecklist.Indispensavel: return "Indispensável";
                default: return string.Empty;
            }
        }
    }
}
