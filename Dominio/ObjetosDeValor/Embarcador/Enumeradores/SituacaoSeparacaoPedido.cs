namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoSeparacaoPedido
    {
        Aberto = 1,
        AguardandoIntegracao = 2,
        IntegracaoRejeitada = 3,
        Finalizada = 4,
        Cancelada = 5
    }

    public static class SituacaoSeparacaoPedidoHelper
    {
        public static string ObterDescricao(this SituacaoSeparacaoPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoSeparacaoPedido.Aberto: return "Aberto";
                case SituacaoSeparacaoPedido.AguardandoIntegracao: return "Aguardando Integração";
                case SituacaoSeparacaoPedido.IntegracaoRejeitada: return "Integração Rejeitada";
                case SituacaoSeparacaoPedido.Finalizada: return "Finalizada";
                case SituacaoSeparacaoPedido.Cancelada: return "Cancelada";
                default: return string.Empty;
            }
        }
    }
}
