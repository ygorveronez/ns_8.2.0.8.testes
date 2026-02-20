namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CargaTrechoSumarizada
    {
        Agrupadora = 1,
        SubCarga = 2,
    }

    public static class CargaTrechoSumarizadaHelper
    {
        public static string ObterDescricao(this CargaTrechoSumarizada cargaTrecho)
        {
            switch (cargaTrecho)
            {
                case CargaTrechoSumarizada.Agrupadora: return Localization.Resources.Enumeradores.CargaTrechoSumarizada.Agrupadora;
                case CargaTrechoSumarizada.SubCarga: return Localization.Resources.Enumeradores.CargaTrechoSumarizada.SubCarga;
                default: return string.Empty;
            }
        }
    }
}
