namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CategoriaHabilitacao
    {
        CategoriaA = 1,
        CategoriaB = 2,
        CategoriaC = 3,
        CategoriaD = 4,
        CategoriaE = 5,
        CategoriaAB = 6,
        CategoriaAC = 7,
        CategoriaAD = 8,
        CategoriaAE = 9
    }

    public static class CategoriaHabilitacaoHelper
    {
        public static string ObterDescricao(this CategoriaHabilitacao situacao)
        {
            switch (situacao)
            {
                case CategoriaHabilitacao.CategoriaA: return "A";
                case CategoriaHabilitacao.CategoriaB: return "B";
                case CategoriaHabilitacao.CategoriaC: return "C";
                case CategoriaHabilitacao.CategoriaD: return "D";
                case CategoriaHabilitacao.CategoriaE: return "E";
                case CategoriaHabilitacao.CategoriaAB: return "AB";
                case CategoriaHabilitacao.CategoriaAC: return "AC";
                case CategoriaHabilitacao.CategoriaAD: return "AD";
                case CategoriaHabilitacao.CategoriaAE: return "AE";
                default: return null;
            }
        }
    }
}