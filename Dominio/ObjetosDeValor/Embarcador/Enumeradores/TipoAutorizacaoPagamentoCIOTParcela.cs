namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAutorizacaoPagamentoCIOTParcela
    {
        Adiantamento = 1,
        Abastecimento = 2,
        Saldo = 3
    }

    public static class TipoAutorizacaoPagamentoCIOTParcelaHelper
    {
        public static string ObterDescricao(this TipoAutorizacaoPagamentoCIOTParcela? situacao)
        {
            if (!situacao.HasValue)
                return string.Empty;

            return situacao.Value.ObterDescricao();
        }

        public static string ObterDescricao(this TipoAutorizacaoPagamentoCIOTParcela situacao)
        {
            switch (situacao)
            {
                case TipoAutorizacaoPagamentoCIOTParcela.Adiantamento: return "Adiantamento";
                case TipoAutorizacaoPagamentoCIOTParcela.Abastecimento: return "Abastecimento";
                case TipoAutorizacaoPagamentoCIOTParcela.Saldo: return "Saldo";
                default: return string.Empty;
            }
        }
    }
}
