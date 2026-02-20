using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaAtendimentoPedidoCliente
    {
        public int codigoPedido { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }

        public int NotaFiscal { get; set; }

        public double CNPJDestinatario { get; set; }

        public double CodigoCliente { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }
    }
}
