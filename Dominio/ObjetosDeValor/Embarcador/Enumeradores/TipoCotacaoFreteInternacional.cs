namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCotacaoFreteInternacional
    {
        CotacaoCorrente = 0,
        CotacaoMinima = 1,
        CotacaoFixa = 2
    }

    public static class TipoCotacaoFreteInternacionalHelper
    {
        public static string ObterDescricao(this TipoCotacaoFreteInternacional tipoCotacaoFreteInternacional)
        {
            switch (tipoCotacaoFreteInternacional)
            {
                case TipoCotacaoFreteInternacional.CotacaoCorrente: return "Cotação Corrente";
                case TipoCotacaoFreteInternacional.CotacaoMinima: return "Cotação Mínima";
                case TipoCotacaoFreteInternacional.CotacaoFixa: return "Cotação Fixa";
                default: return string.Empty;
            }
        }
    }
}
