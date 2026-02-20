namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFechamentoPontuacao
    {
        AguardandoFinalizacao = 1,
        Cancelado = 2,
        Finalizado = 3
    }

    public static class SituacaoFechamentoPontuacaoHelper
    {
        public static string ObterDescricao(this SituacaoFechamentoPontuacao situacao)
        {
            switch (situacao)
            {
                case SituacaoFechamentoPontuacao.AguardandoFinalizacao: return "Aguardando Finalização";
                case SituacaoFechamentoPontuacao.Cancelado: return "Cancelado";
                case SituacaoFechamentoPontuacao.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
