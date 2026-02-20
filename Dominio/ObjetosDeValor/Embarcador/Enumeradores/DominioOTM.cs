namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DominioOTM
    {
        SAO = 1,
    }

    public static class DominioOTMHelper
    {
        public static string ObterDescricao(this DominioOTM dominioOTM)
        {
            switch (dominioOTM)
            {
                case DominioOTM.SAO: return "SAO";
                default: return string.Empty;
            }
        }
    }
}
