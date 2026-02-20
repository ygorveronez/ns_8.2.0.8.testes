namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PessoaClasse
    {
        Um = 1,
        Dois = 2,
        Tres = 3,
        Quatro = 4,
        Cinco = 5
    }

    public static class PessoaClasseHelper
    {
        public static string ObterDescricao(this PessoaClasse classe)
        {
            switch (classe)
            {
                case PessoaClasse.Um: return "Classe 1";
                case PessoaClasse.Dois: return "Classe 2";
                case PessoaClasse.Tres: return "Classe 3";
                case PessoaClasse.Quatro: return "Classe 4";
                case PessoaClasse.Cinco: return "Classe 5";
                default: return string.Empty;
            }
        }
    }
}
