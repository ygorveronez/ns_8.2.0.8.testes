namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PosicaoReboque
    {
        ReboqueUm = 1,
        ReboqueDois = 2,
        ReboqueTres = 3
    }

    public static class PosicaoReboqueHelper
    {
        public static string ObterDescricao(this PosicaoReboque posicaoReboque)
        {
            switch (posicaoReboque)
            {
                case PosicaoReboque.ReboqueUm: return "1";
                case PosicaoReboque.ReboqueDois: return "2";
                case PosicaoReboque.ReboqueTres: return "3";
                default: return string.Empty;
            }
        }
    }
}
