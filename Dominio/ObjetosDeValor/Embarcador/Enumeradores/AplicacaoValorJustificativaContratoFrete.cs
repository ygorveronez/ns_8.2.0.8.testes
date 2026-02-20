namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AplicacaoValorJustificativaContratoFrete
    {
        NoAdiantamento = 0,
        NoTotal = 1,
        NoSaldo = 2
    }

    public static class AplicacaoValorJustificativaContratoFreteHelper
    {
        public static string ObterDescricao(this AplicacaoValorJustificativaContratoFrete aplicacao)
        {
            switch (aplicacao)
            {
                case AplicacaoValorJustificativaContratoFrete.NoAdiantamento: return "No Adiantamento";
                case AplicacaoValorJustificativaContratoFrete.NoTotal: return "No Total";
                case AplicacaoValorJustificativaContratoFrete.NoSaldo: return "No Saldo";
                default: return string.Empty;
            }
        }
    }
}
