namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGestaoDadosColeta
    {
        AguardandoAprovacao = 0,
        Aprovado = 1,
        Reprovado = 2
    }

    public static class SituacaoGestaoDadosColetaHelper
    {
        public static string ObterDescricao(this SituacaoGestaoDadosColeta origem)
        {
            switch (origem)
            {
                case SituacaoGestaoDadosColeta.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoGestaoDadosColeta.Aprovado: return "Aprovado";
                case SituacaoGestaoDadosColeta.Reprovado: return "Reprovado";
                default: return string.Empty;
            }
        }
    }
}
