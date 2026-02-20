namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAlteracaoRegraPagamentoProvedor
    {
        NaoInformada = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoAlteracaoRegraPagamentoProvedorHelper
    {
        public static string ObterDescricao(this SituacaoAlteracaoRegraPagamentoProvedor situacao)
        {
            switch (situacao)
            {
                case SituacaoAlteracaoRegraPagamentoProvedor.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAlteracaoRegraPagamentoProvedor.Aprovada: return "Aprovada";
                case SituacaoAlteracaoRegraPagamentoProvedor.Reprovada: return "Reprovada";
                case SituacaoAlteracaoRegraPagamentoProvedor.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}
