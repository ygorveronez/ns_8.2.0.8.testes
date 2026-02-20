namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAtualPedidoRetirada
    {
        PedidoCriado = 1,
        LiberacaoComercial = 2,
        LiberacaoFinanceira = 3,
        Agendamento = 4,
        Remessa = 5,
        Carregamento = 6,
        Faturamento = 7,
    }

    public static class SituacaoAtualPedidoRetiradaHelper
    {
        public static string ObterDescricao(this SituacaoAtualPedidoRetirada situacaoPedido)
        {
            switch (situacaoPedido)
            {
                case SituacaoAtualPedidoRetirada.PedidoCriado:
                    return "PedidoCriado";
                case SituacaoAtualPedidoRetirada.LiberacaoComercial:
                    return "LiberacaoComercial";
                case SituacaoAtualPedidoRetirada.LiberacaoFinanceira:
                    return "LiberacaoFinanceira";
                case SituacaoAtualPedidoRetirada.Agendamento:
                    return "Agendamento";
                case SituacaoAtualPedidoRetirada.Remessa:
                    return "Remessa";
                case SituacaoAtualPedidoRetirada.Carregamento:
                    return "Carregamento";
                case SituacaoAtualPedidoRetirada.Faturamento:
                    return "Faturamento";
                default:
                    return string.Empty;
            }
        }
    }
}
