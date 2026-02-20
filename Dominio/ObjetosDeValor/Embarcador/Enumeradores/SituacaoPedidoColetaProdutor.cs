namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPedidoColetaProdutor
    {
        Todas = 0,
        AgFechamento = 1,
        EmFechamento = 2,
        Fechada = 3,
        Cancelada = 4
    }

    public static class SituacaoPedidoColetaProdutorHelper
    {
        public static string ObterDescricao(this SituacaoPedidoColetaProdutor sitaucao)
        {
            switch (sitaucao)
            {
                case SituacaoPedidoColetaProdutor.Todas: return "";
                case SituacaoPedidoColetaProdutor.AgFechamento: return "Ag. Fechamento";
                case SituacaoPedidoColetaProdutor.EmFechamento: return "Em Fechamento";
                case SituacaoPedidoColetaProdutor.Fechada: return "Fechada";
                case SituacaoPedidoColetaProdutor.Cancelada: return "Cancelada";
                default: return "";
            }
        }
    }
}
