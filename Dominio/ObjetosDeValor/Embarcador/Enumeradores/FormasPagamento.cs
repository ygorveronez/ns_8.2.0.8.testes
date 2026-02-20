namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormasPagamento
    {
        Avista = 0,
        Prazo = 1
    }

    public static class FormasPagamentoCIOTHelper
    {
        public static string ObterDescricao(this FormasPagamento formaPagamento)
        {
            switch (formaPagamento)
            {
                case FormasPagamento.Avista: return "A vista";
                case FormasPagamento.Prazo: return "A prazo";
                default: return "";
            }
        }
    }
}
