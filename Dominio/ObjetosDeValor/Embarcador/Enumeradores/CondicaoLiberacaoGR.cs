namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CondicaoLiberacaoGR
    {
        E = 1,
        Ou = 2
    }

    public static class CondicaoLiberacaoGRHelper
    {
        public static string ObterDescricao(this CondicaoLiberacaoGR situacao)
        {
            switch (situacao)
            {
                case CondicaoLiberacaoGR.E: return "E";
                case CondicaoLiberacaoGR.Ou: return "Ou";
                default: return null;
            }
        }
    }
}