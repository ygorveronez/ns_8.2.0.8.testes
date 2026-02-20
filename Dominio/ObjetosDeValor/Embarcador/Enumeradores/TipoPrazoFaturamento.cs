namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPrazoFaturamento
    {
        DataFatura = 1,
        DataDocumento = 2,
        DataPrevisaoEncerramento = 3,
        DataPrevisaoInicioViagem = 4,
        ApartirAprovacaoFatura = 5,
        ApartirDataGeracaoFatura = 6
    }

    public static class TipoPrazoFaturamentoHelper
    {
        public static string ObterDescricao(this TipoPrazoFaturamento tipoPrazoFaturamento)
        {
            switch (tipoPrazoFaturamento)
            {
                case TipoPrazoFaturamento.DataFatura: return "Data Fatura";
                case TipoPrazoFaturamento.DataDocumento: return "Data Documento";
                case TipoPrazoFaturamento.DataPrevisaoEncerramento: return "Data Previsão Encerramento";
                case TipoPrazoFaturamento.DataPrevisaoInicioViagem: return "Data Previsão Início Viagem";
                case TipoPrazoFaturamento.ApartirAprovacaoFatura: return "A partir da aprovação da fatura";
                case TipoPrazoFaturamento.ApartirDataGeracaoFatura: return "A partir da geração da fatura";
                default: return string.Empty;
            }
        }
    }
}
