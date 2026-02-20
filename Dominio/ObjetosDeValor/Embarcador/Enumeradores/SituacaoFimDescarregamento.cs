namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFimDescarregamento
    {
        AguardandoFimDescarregamento = 1,
        DescarregamentoFinalizado = 2
    }

    public static class SituacaoFimDescarregamentoHelper
    {
        public static string ObterDescricao(this SituacaoFimDescarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoFimDescarregamento.AguardandoFimDescarregamento: return "Aguardando Fim do Descarregamento";
                case SituacaoFimDescarregamento.DescarregamentoFinalizado: return "Descarregamento Finalizado";
                default: return string.Empty;
            }
        }
    }
}
