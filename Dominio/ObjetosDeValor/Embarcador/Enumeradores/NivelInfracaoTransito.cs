namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum NivelInfracaoTransito
    {
        Leve = 0,
        Media = 1,
        Grave = 2,
        Gravissima = 3,
    }

    public static class NivelInfracaoTransitoHelper
    {
        public static string ObterDescricao(this NivelInfracaoTransito nivel)
        {
            switch (nivel)
            {
                case NivelInfracaoTransito.Grave: return "Grave";
                case NivelInfracaoTransito.Gravissima: return "Gravíssima";
                case NivelInfracaoTransito.Leve: return "Leve";
                case NivelInfracaoTransito.Media: return "Média";
                default: return string.Empty;
            }
        }
    }
}
