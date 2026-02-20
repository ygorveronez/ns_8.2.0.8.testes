namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContaPagamentoEletronico
    {
        ContaCorrenteIndividual = 1,
        TDSTransferencia = 3
    }

    public static class TipoContaPagamentoEletronicoHelper
    {
        public static string ObterDescricao(this TipoContaPagamentoEletronico tipoConta)
        {
            switch (tipoConta)
            {
                case TipoContaPagamentoEletronico.ContaCorrenteIndividual: return "01 - Crédito em conta corrente";
                case TipoContaPagamentoEletronico.TDSTransferencia: return "03 - TDS Transferência";
                default: return string.Empty;
            }
        }
    }
}
