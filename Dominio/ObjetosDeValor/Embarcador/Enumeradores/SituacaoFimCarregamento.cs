namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFimCarregamento
    {
        AguardandoFimCarregamento = 1,
        CarregamentoFinalizado = 2
    }

    public static class SituacaoFimCarregamentoHelper
    {
        public static string ObterDescricao(this SituacaoFimCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoFimCarregamento.AguardandoFimCarregamento: return "Aguardando Fim do Carregamento";
                case SituacaoFimCarregamento.CarregamentoFinalizado: return "Carregamento Finalizado";
                default: return string.Empty;
            }
        }
    }
}
