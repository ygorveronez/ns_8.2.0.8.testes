namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class PedidoAdicional
    {
        public bool AjudanteCarga { get; set; }

        public bool AjudanteDescarga { get; set; }

        public int QtdAjudantesCarga { get; set; }

        public int QtdAjudantesDescarga { get; set; }

        public SituacaoEstoquePedido SituacaoEstoquePedido { get; set; }
    }
}
