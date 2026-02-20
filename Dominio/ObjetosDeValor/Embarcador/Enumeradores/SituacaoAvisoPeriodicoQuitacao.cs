namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAvisoPeriodicoQuitacao
    {
        AguardandoConfirmacao = 0,
        Confirmado = 1,
        Rejeitado = 2
    }

    public static class SituacaoAvisoPeriodicoQuitacaoHelper
    {
        public static string ObterDescricao(this SituacaoAvisoPeriodicoQuitacao situacaoAviso)
        {
            switch (situacaoAviso)
            {
                case SituacaoAvisoPeriodicoQuitacao.AguardandoConfirmacao: return "Aguardando Confirmação";
                case SituacaoAvisoPeriodicoQuitacao.Confirmado: return "Confirmado";
                case SituacaoAvisoPeriodicoQuitacao.Rejeitado: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }
}