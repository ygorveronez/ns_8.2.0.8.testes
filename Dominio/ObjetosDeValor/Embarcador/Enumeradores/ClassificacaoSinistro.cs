namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ClassificacaoSinistro
    {
        Sl = 0,
        Spt = 1,
        Cpt = 2
    }

    public static class ClassificacaoSinistroHelper
    {
        public static string ObterDescricao(this ClassificacaoSinistro tipo)
        {
            switch (tipo)
            {
                case ClassificacaoSinistro.Sl: return "SL";
                case ClassificacaoSinistro.Spt: return "SPT";
                case ClassificacaoSinistro.Cpt: return "CPT";
                default: return string.Empty;
            }
        }
    }
}
