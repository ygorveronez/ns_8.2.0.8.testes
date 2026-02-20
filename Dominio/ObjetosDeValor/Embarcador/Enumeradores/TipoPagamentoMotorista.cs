namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPagamentoMotorista
    {
        Todos = -1,
        Nenhum = 0,
        Diaria = 1,
        Adiantamento = 2,
        Terceiro = 3
    }
    public static class TipoPagamentoMotoristaHelper
    {
        public static string ObterDescricao(this TipoPagamentoMotorista tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case TipoPagamentoMotorista.Nenhum: return "Nenhum";
                case TipoPagamentoMotorista.Diaria: return "Diária";
                case TipoPagamentoMotorista.Adiantamento: return "Adiantamento";
                case TipoPagamentoMotorista.Terceiro: return "Terceiro";
                default: return string.Empty;
            }
        }
    }
}
