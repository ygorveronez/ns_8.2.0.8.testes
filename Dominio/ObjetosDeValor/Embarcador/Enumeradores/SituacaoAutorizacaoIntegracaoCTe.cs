namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAutorizacaoIntegracaoCTe
    {
        NaoInformada = 0,
        AguardandoAprovacao = 1,
        Aprovada = 2,
        Reprovada = 3
    }

    public static class SituacaoAutorizacaoIntegracaoCTeHelper
    {
        public static bool IsLiberada(this SituacaoAutorizacaoIntegracaoCTe situacao)
        {
            return (
                (situacao == SituacaoAutorizacaoIntegracaoCTe.Aprovada) ||
                (situacao == SituacaoAutorizacaoIntegracaoCTe.NaoInformada)
            );
        }

        public static string ObterDescricao(this SituacaoAutorizacaoIntegracaoCTe situacao)
        {
            switch (situacao)
            {
                case SituacaoAutorizacaoIntegracaoCTe.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoAutorizacaoIntegracaoCTe.Aprovada: return "Aprovada";
                case SituacaoAutorizacaoIntegracaoCTe.NaoInformada: return "Não Informada";
                case SituacaoAutorizacaoIntegracaoCTe.Reprovada: return "Reprovada";
                default: return string.Empty;
            }
        }
    }
}
