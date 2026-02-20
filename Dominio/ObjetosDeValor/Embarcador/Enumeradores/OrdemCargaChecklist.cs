namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrdemCargaChecklist
    {
        UltimaCarga = 1,
        PenultimaCarga = 2,
        AntepenultimaCarga = 3
    }

    public static class EnumOrdemCargaChecklistHelper
    {
        public static string ObterDescricao(this OrdemCargaChecklist regime)
        {
            switch (regime)
            {
                case OrdemCargaChecklist.UltimaCarga: return "Última Carga";
                case OrdemCargaChecklist.PenultimaCarga: return "Penúltima Carga";
                case OrdemCargaChecklist.AntepenultimaCarga: return "Antepenúltima Carga";
                default: return string.Empty;
            }
        }
    }
}