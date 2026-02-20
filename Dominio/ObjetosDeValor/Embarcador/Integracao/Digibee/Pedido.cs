using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class Pedido
    {
        public string site { get; set; }
        public string codigoDepositante { get; set; }
        public int numeroPedido { get; set; }
        public int protocoloIntegracaoCarga { get; set; }
        public string numeroCarga { get; set; }
        public string codigoTransportadora { get; set; }
        public string dataDoPedidoPrevisto { get; set; }
        public string codigoCliente { get; set; }
        public int sequenciaDeEntrega { get; set; }
        public string tipoDePedido { get; set; }
        public int prioridade { get; set; }
        public List<Digibee.Produto> itens { get; set; }
    }
}
