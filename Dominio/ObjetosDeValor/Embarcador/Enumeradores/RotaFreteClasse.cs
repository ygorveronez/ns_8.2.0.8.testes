namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RotaFreteClasse
    {
        Um = 1,
        Dois = 2,
        Tres = 3,
        Quatro = 4,
        Cinco = 5
    }

    public static class RotaFreteClasseHelper
    {
        public static string ObterDescricao(this RotaFreteClasse classe)
        {
            switch (classe)
            {
                case RotaFreteClasse.Um: return "Classe 1";
                case RotaFreteClasse.Dois: return "Classe 2";
                case RotaFreteClasse.Tres: return "Classe 3";
                case RotaFreteClasse.Quatro: return "Classe 4";
                case RotaFreteClasse.Cinco: return "Classe 5";
                default: return string.Empty;
            }
        }
    }
}
