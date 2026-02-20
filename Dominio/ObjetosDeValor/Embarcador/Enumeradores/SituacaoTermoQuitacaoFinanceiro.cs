namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTermoQuitacaoFinanceiro
    {
        Todas = 0,
        AguardandoAprovacaoTransportador = 1,
        AprovadoTransportador = 2,
        RejeitadoTransportador = 3,
        AguardandoAprovacaoProvisao = 4,
        AprovadoProvisao = 5,
        RejeitadoProvisao = 6,
        SemRegraProvisao = 7,
        Novo = 8,
        Finalizada = 9
    }

    public static class SituacaoTermoQuitacaoFinanceiroHelper
    {
        public static string ObterDescricao(this SituacaoTermoQuitacaoFinanceiro situacaoTermoQuitacao)
        {
            switch (situacaoTermoQuitacao)
            {
                case SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoTransportador: return "Aguardando Aprovação Transportador";
                case SituacaoTermoQuitacaoFinanceiro.AprovadoTransportador: return "Aprovado transportador";
                case SituacaoTermoQuitacaoFinanceiro.RejeitadoTransportador: return "Rejeitado Transportador";
                case SituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoProvisao: return "Aguardando Aprovação  Provisão";
                case SituacaoTermoQuitacaoFinanceiro.AprovadoProvisao: return "Aprovado Provisão";
                case SituacaoTermoQuitacaoFinanceiro.RejeitadoProvisao: return "Rejeitado Provisão";
                case SituacaoTermoQuitacaoFinanceiro.SemRegraProvisao: return "Sem Regra Provisão";
                case SituacaoTermoQuitacaoFinanceiro.Novo: return "Novo";
                case SituacaoTermoQuitacaoFinanceiro.Finalizada: return "Finalizada ";
                default: return string.Empty;
            }
        }
    }
}
