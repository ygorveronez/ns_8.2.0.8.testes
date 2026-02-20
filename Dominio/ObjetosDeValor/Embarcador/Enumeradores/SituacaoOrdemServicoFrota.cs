namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOrdemServicoFrota
    {
        EmDigitacao = 0,
        AgAutorizacao = 1,
        Rejeitada = 2,
        EmManutencao = 3,
        DivergenciaOrcadoRealizado = 4,
        Finalizada = 5,
        Cancelada = 6,
        AgNotaFiscal = 7,
        SemRegraAprovacao = 8,
        AguardandoAprovacao = 9,
        AprovacaoRejeitada = 10
    }

    public static class SituacaoOrdemServicoFrotaHelper
    {
        public static string ObterDescricao(this SituacaoOrdemServicoFrota situacao)
        {
            switch (situacao)
            {
                case SituacaoOrdemServicoFrota.EmDigitacao: return "Em Digitação";
                case SituacaoOrdemServicoFrota.AgAutorizacao: return "Ag. Autorização";
                case SituacaoOrdemServicoFrota.Rejeitada: return "Rejeitada";
                case SituacaoOrdemServicoFrota.EmManutencao: return "Em Manutenção";
                case SituacaoOrdemServicoFrota.DivergenciaOrcadoRealizado: return "Divergência Orçado X Realizado";
                case SituacaoOrdemServicoFrota.Finalizada: return "Finalizada";
                case SituacaoOrdemServicoFrota.Cancelada: return "Cancelada";
                case SituacaoOrdemServicoFrota.AgNotaFiscal: return "Aguardando Nota Fiscal";
                case SituacaoOrdemServicoFrota.SemRegraAprovacao: return "Sem Regra Aprovação";
                case SituacaoOrdemServicoFrota.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoOrdemServicoFrota.AprovacaoRejeitada: return "Aprovação Rejeitada";
                default: return string.Empty;
            }
        }
    }
}
