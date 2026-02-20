namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Inconterm
    {
        CIF = 1,
        FOBDirigido = 2,
        CIFFOBDirigido = 3,
        FOB = 4
    }
    public static class IncontermHelper
    {
        public static string ObterDescricao(this Inconterm tipo)
        {
            switch (tipo)
            {
                case Inconterm.CIF: return "CIF";
                case Inconterm.FOBDirigido: return "FOB Dirigido";
                case Inconterm.CIFFOBDirigido: return "CIF e FOB Dirigido";
                case Inconterm.FOB: return "FOB";
                default: return string.Empty;
            }
        }
    }
}
