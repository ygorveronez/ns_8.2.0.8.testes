namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGestaoDevolucao
    {
        Ativa = 0,
        AnaliseCancelamento = 1,
        Cancelada = 2,
        Finalizada = 3,
    }

    public static class SituacaosGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this SituacaoGestaoDevolucao situacaoGestaoDevolucao)
        {
            switch (situacaoGestaoDevolucao)
            {
                case SituacaoGestaoDevolucao.Ativa: return "Ativa";
                case SituacaoGestaoDevolucao.AnaliseCancelamento: return "Análise de Cancelamento";
                case SituacaoGestaoDevolucao.Cancelada: return "Cancelada";
                case SituacaoGestaoDevolucao.Finalizada: return "Finalizada";
                default: return string.Empty;
            }
        }
    }
}
