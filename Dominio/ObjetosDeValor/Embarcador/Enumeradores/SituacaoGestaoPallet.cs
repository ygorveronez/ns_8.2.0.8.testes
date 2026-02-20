namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGestaoPallet
    {
        Pendente = 1,
        Concluido = 2,
        Cancelada = 3,
        Reserva = 4,
        AguardandoAvaliacao = 5
    }

    public static class SituacaoGestaoPalletHelper
    {
        public static string ObterDescricao(this SituacaoGestaoPallet situacaoGestaoPallet)
        {
            switch (situacaoGestaoPallet)
            {
                case SituacaoGestaoPallet.Pendente: return "Pendente";
                case SituacaoGestaoPallet.Concluido: return "Concluído";
                case SituacaoGestaoPallet.Cancelada: return "Cancelada";
                case SituacaoGestaoPallet.Reserva: return "Reserva";
                case SituacaoGestaoPallet.AguardandoAvaliacao: return "Aguardando Avaliação";
                default: return string.Empty;
            }
        }
    }
}
