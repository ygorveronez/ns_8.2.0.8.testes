namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoConexaoEmail
    {
        Padrao = 0,
        Gmail = 1,
        Exchange = 2
    }

    public static class TipoConexaoEmailHelper
    {
        public static string ObterDescricao(this TipoConexaoEmail status)
        {
            switch (status)
            {
                case TipoConexaoEmail.Padrao: return "Padrão";
                case TipoConexaoEmail.Gmail: return "Gmail";
                case TipoConexaoEmail.Exchange: return "Exchange";
                default: return "Padrão";
            }
        }
    }
}
