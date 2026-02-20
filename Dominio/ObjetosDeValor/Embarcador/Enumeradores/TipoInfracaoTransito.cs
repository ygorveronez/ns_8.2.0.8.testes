namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoInfracaoTransito
    {
        Multa = 0,
        Outro = 1,
        Sinistro = 2,
        Advertencia = 3,
    }

    public static class TipoInfracaoTransitoHelper
    {
        public static string ObterDescricao(this TipoInfracaoTransito tipo)
        {
            switch (tipo)
            {
                case TipoInfracaoTransito.Multa: return "Multa";
                case TipoInfracaoTransito.Outro: return "Outro";
                case TipoInfracaoTransito.Sinistro: return "Sinistro";
                case TipoInfracaoTransito.Advertencia: return "Advertência";
                default: return string.Empty;
            }
        }
    }
}
