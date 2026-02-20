namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEntregaPedido
    {
        Entregue = 1,
        NaoEntregue = 2,
        Rejeitado = 3
    }

    public static class SituacaoEntregaPedidoHelper
    {
        public static string ObterDescricao(this SituacaoEntregaPedido situacaoEntregaPedido)
        {
            switch (situacaoEntregaPedido)
            {
                case SituacaoEntregaPedido.Entregue: return "Entregue";
                case SituacaoEntregaPedido.NaoEntregue: return "Não Entregue";
                case SituacaoEntregaPedido.Rejeitado: return "Rejeitado";
                default: return "";
            }
        }
    }
}
