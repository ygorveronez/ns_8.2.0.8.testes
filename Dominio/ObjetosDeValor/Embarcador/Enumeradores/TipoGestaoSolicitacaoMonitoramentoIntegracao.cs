namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    /// <summary>
    /// 1: Inserir como uma SM(Agendamento). A API aceita uma fila de SM por veículo
    /// 2: Inserir como uma viagem.A API aceita apenas uma por viagem por veículo.
    /// </summary>
    public enum TipoGestaoSolicitacaoMonitoramentoIntegracao
    {
        Agendamento = 1,
        Viagem = 2
    }

    public static class TipoGestaoSolicitacaoMonitoramentoIntegracaoHelper
    {
        public static string ObterDescricao(this TipoGestaoSolicitacaoMonitoramentoIntegracao o)
        {
            switch (o)
            {
                case TipoGestaoSolicitacaoMonitoramentoIntegracao.Agendamento: return "Agendamento";
                case TipoGestaoSolicitacaoMonitoramentoIntegracao.Viagem: return "Viagem";
                default: return string.Empty;
            }
        }
    }
}
