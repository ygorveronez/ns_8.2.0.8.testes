namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFimHigienizacao
    {
        AguardandoFimHigienizacao = 1,
        HigienizacaoFinalizada = 2
    }

    public static class SituacaoFimHigienizacaoHelper
    {
        public static string ObterDescricao(this SituacaoFimHigienizacao situacao)
        {
            switch (situacao)
            {
                case SituacaoFimHigienizacao.AguardandoFimHigienizacao: return "Aguardando Fim da Higienização";
                case SituacaoFimHigienizacao.HigienizacaoFinalizada: return "Higienização Finalizada";
                default: return string.Empty;
            }
        }
    }
}
