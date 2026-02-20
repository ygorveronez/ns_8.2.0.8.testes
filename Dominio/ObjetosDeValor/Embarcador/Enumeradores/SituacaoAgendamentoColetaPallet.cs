namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAgendamentoColetaPallet
    {
        EmAndamento = 0,
        Finalizado = 1,
        Cancelado = 2
    }

    public static class SituacaoAgendamentoColetaPalletHelper
    {
        public static string ObterDescricao(this SituacaoAgendamentoColetaPallet situacao)
        {
            switch (situacao)
            {
                case SituacaoAgendamentoColetaPallet.EmAndamento: return "Em Andamento";
                case SituacaoAgendamentoColetaPallet.Finalizado: return "Finalizado";
                case SituacaoAgendamentoColetaPallet.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
