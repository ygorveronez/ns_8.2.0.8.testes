namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CorRaca
    {
        SemInformacao = 0,
        Branca = 1,
        Preta = 2,
        Parda = 3,
        Amarela = 4,
        Indigena = 5
    }

    public static class CorRacaHelper
    {
        public static string ObterDescricao(this CorRaca situacao)
        {
            switch (situacao)
            {
                case CorRaca.Branca: return "Branca";
                case CorRaca.Preta: return "Preta";
                case CorRaca.Parda: return "Parda";
                case CorRaca.Amarela: return "Amarela";
                case CorRaca.Indigena: return "Indígena";
                default: return string.Empty;
            }
        }
    }
}
