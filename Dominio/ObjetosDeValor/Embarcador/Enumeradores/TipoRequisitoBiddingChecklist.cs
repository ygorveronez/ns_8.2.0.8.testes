namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRequisitoBiddingChecklist
    {
        Desejavel = 0,
        Indispensavel = 1
    }

    public static class TipoRequisitoBiddingChecklistHelper
    {
        public static string ObterDescricao(this TipoRequisitoBiddingChecklist requisito)
        {
            switch (requisito)
            {
                case TipoRequisitoBiddingChecklist.Desejavel: return "Desejável";
                case TipoRequisitoBiddingChecklist.Indispensavel: return "Indispensável";
                default: return string.Empty;
            }
        }
    }
}
