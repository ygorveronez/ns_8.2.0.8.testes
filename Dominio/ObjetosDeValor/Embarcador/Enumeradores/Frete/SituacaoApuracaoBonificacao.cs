namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete
{
    public enum SituacaoApuracaoBonificacao
    {
        AguardandoGeracaoOcorrencia = 1,
        Cancelado = 2,
        Finalizado = 3
    }

    public static class SituacaoApuracaoBonificacaoHelper
    {
        public static string ObterDescricao(this SituacaoApuracaoBonificacao situacao)
        {
            switch (situacao)
            {
                case SituacaoApuracaoBonificacao.AguardandoGeracaoOcorrencia: return "Aguardando Geração de Ocorrência";
                case SituacaoApuracaoBonificacao.Cancelado: return "Cancelado";
                case SituacaoApuracaoBonificacao.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
