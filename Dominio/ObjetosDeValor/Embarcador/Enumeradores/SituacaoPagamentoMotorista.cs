namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPagamentoMotorista
    {
        Todas = 0,
        AgAprovacao = 1,
        Finalizada = 2,
        Rejeitada = 3,
        Cancelada = 4,
        AgIntegracao = 5,
        FalhaIntegracao = 6,
        AutorizacaoPendente = 7,
        AgInformacoes = 8,
        SemRegraAprovacao = 9,
        FinalizadoPagamento = 10
    }

    public static class SituacaoPagamentoMotoristaHelper
    {
        public static string ObterDescricao(this SituacaoPagamentoMotorista situacao)
        {
            switch (situacao)
            {
                case SituacaoPagamentoMotorista.AgAprovacao: return "Ag. Aprovação";
                case SituacaoPagamentoMotorista.Finalizada: return "Finalizada";
                case SituacaoPagamentoMotorista.Rejeitada: return "Rejeitada";
                case SituacaoPagamentoMotorista.Cancelada: return "Cancelada";
                case SituacaoPagamentoMotorista.AgIntegracao: return "Ag. Integração";
                case SituacaoPagamentoMotorista.FalhaIntegracao: return "Falha na Integração";
                case SituacaoPagamentoMotorista.AutorizacaoPendente: return "Autorização Pendente";
                case SituacaoPagamentoMotorista.AgInformacoes: return "Ag. Informações";
                case SituacaoPagamentoMotorista.SemRegraAprovacao: return "Sem Regra de Aprovação";
                case SituacaoPagamentoMotorista.FinalizadoPagamento: return "Finalizado Pagamento";
                default: return string.Empty;
            }
        }
    }
}
