using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaEntregaPedido
    {
        public int Codigo { get; set; }
        public string Vendedor { get; set; }
        public string Pedido { get; set; }
        public DateTime? PrevisaoEntrega { get; set; }
        public DateTime? DataAbate { get; set; }
        public string NotasFiscais { get; set; }
        public decimal ValorNF { get; set; }
        public string Reentrega { get; set; }
        public int CodigoCargaEntrega { get; set; }
        public int CodigoCargaEntregaPedido { get; set; }
        public string CodigosCargasPedido { get; set; }
        public int QuantidadeVolumes { get; set; }
        public string CodigoPedidoCliente { get; set; }
        public string NumeroOrdem { get; set; }
        public string CanalEntrega { get; set; }
    }
}
