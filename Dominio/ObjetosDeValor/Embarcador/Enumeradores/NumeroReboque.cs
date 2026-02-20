namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum NumeroReboque
    {
        SemReboque = 0,
        ReboqueUm = 1,
        ReboqueDois = 2
    }

    public static class NumeroReboqueHelper
    {
        public static string ObterDescricao(this NumeroReboque numeroReboque)
        {
            switch (numeroReboque)
            {
                case NumeroReboque.ReboqueDois: return "Reboque Dois";
                case NumeroReboque.ReboqueUm: return "Reboque Um";
                default: return "Sem Reboque";
            }
        }
    }
}
