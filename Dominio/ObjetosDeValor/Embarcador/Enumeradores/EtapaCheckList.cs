namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaCheckList
    {
         Checklist = 0,
         AvaliacaoDescarga = 1
    }

    public static class EtapaCheckListtHelper
    {
        public static string ObterDescricao(this EtapaCheckList etapa)
        {
            switch (etapa)
            {
                case EtapaCheckList.Checklist: return "Checklist";
                case EtapaCheckList.AvaliacaoDescarga: return "Avaliação Descarga";
                default: return string.Empty;
            }
        }
    }
}
