namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAutorizacaoPagamentoEletronico
    {
        Todos = 0,
        Iniciada = 1,
        Finalizada = 2,
        SemRegraAprovacao = 3,
        AguardandoAprovacao = 4,
        AprovacaoRejeitada = 5
    }

    public static class SituacaoAutorizacaoPagamentoEletronicoHelper
    {
        public static string ObterDescricao(this SituacaoAutorizacaoPagamentoEletronico situacao)
        {
            switch (situacao)
            {
                case SituacaoAutorizacaoPagamentoEletronico.Iniciada: return "Em Digitação";                
                case SituacaoAutorizacaoPagamentoEletronico.Finalizada: return "Finalizada";
                case SituacaoAutorizacaoPagamentoEletronico.SemRegraAprovacao: return "Sem Regra Aprovação";
                case SituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAutorizacaoPagamentoEletronico.AprovacaoRejeitada: return "Aprovação Rejeitada";
                default: return string.Empty;
            }
        }
    }
}
