using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class PedidoDetalhesAdicionais
    {
        public int CodigoPedido { get; set; }

        public DateTime? DataDigitalizacaoCanhoto { get; set; }

        public DateTime? DataEntregaNotaCanhoto { get; set; }

        public string CodigoCargaEmbarcador { get; set; }
    }
}
