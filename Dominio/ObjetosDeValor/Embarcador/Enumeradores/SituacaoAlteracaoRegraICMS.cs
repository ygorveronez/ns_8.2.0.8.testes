namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAlteracaoRegraICMS
    {
        NaoInformada = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3,
        SemRegraAprovacao = 4
    }

    public static class SituacaoAlteracaoRegraICMSHelper
    {
        public static string ObterDescricao(this SituacaoAlteracaoRegraICMS situacao)
        {
            switch (situacao)
            {
                case SituacaoAlteracaoRegraICMS.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAlteracaoRegraICMS.Aprovada: return "Aprovada";
                case SituacaoAlteracaoRegraICMS.Reprovada: return "Reprovada";
                case SituacaoAlteracaoRegraICMS.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}
