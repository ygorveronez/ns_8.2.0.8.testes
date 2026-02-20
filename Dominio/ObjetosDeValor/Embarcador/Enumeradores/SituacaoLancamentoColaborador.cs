namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLancamentoColaborador
    {
        Todos = 0,
        Agendado = 1,
        Cancelado = 2,
        Execucao = 3,
        Finalizado = 4
    }

    public static class SituacaoLancamentoColaboradorHelper
    {
        public static string ObterDescricao(this SituacaoLancamentoColaborador situacao)
        {
            switch (situacao)
            {
                case SituacaoLancamentoColaborador.Agendado: return "Agendado";
                case SituacaoLancamentoColaborador.Cancelado: return "Cancelado";
                case SituacaoLancamentoColaborador.Execucao: return "Em Execução";
                case SituacaoLancamentoColaborador.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
