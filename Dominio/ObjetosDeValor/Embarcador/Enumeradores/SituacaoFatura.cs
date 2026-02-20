namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFatura
    {
        EmAntamento = 1,
        Fechado = 2,
        Cancelado = 3,
        Liquidado = 4,
        EmFechamento = 5,
        EmCancelamento = 6,
        SemRegraAprovacao = 7,
        AguardandoAprovacao = 8,
        AprovacaoRejeitada = 9,
        ProblemaIntegracao = 10
    }

    public static class SituacaoFaturaHelper
    {
        public static string ObterDescricao(this SituacaoFatura situacao)
        {
            switch (situacao)
            {
                case SituacaoFatura.EmAntamento: return "Em Andamento";
                case SituacaoFatura.Fechado: return "Fechado";
                case SituacaoFatura.Cancelado: return "Cancelado";
                case SituacaoFatura.Liquidado: return "Liquidado";
                case SituacaoFatura.EmFechamento: return "Em Fechamento";
                case SituacaoFatura.EmCancelamento: return "Em Cancelamento";
                case SituacaoFatura.SemRegraAprovacao: return "Sem Regra Aprovação";
                case SituacaoFatura.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoFatura.AprovacaoRejeitada: return "Aprovação Rejeitada";
                case SituacaoFatura.ProblemaIntegracao: return "Problema com a Integração";
                default: return null;
            }
        }
    }
}
