namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAjusteValor
    {
        Desconto = 1,
        Acrescimo = 2
    }

    public static class TipoAjusteValorHelper
    {
        public static string ObterDescricao(this TipoAjusteValor status)
        {
            switch (status)
            {
                case TipoAjusteValor.Acrescimo: return "Acréscimo";
                case TipoAjusteValor.Desconto: return "Desconto";
                default: return string.Empty;
            }
        }
    }
}
