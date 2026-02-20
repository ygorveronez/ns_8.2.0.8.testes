namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLoteLiberacaoComercialPedido
    {
        Todos = 0,
        EmIntegracao = 1,
        Finalizado = 2,
        FalhaNaIntegracao = 3
    }

    public static class SituacaoLoteLiberacaoComercialPedidoHelper
    {
        public static string ObterDescricao(this SituacaoLoteLiberacaoComercialPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoLoteLiberacaoComercialPedido.EmIntegracao: return "Em Integração";
                case SituacaoLoteLiberacaoComercialPedido.Finalizado: return "Finalizado";
                case SituacaoLoteLiberacaoComercialPedido.FalhaNaIntegracao: return "Falha na Integração";
                default: return string.Empty;
            }
        }
    }
}
