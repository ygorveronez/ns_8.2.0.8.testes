using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Monitoramento
{
    public sealed class Pedido
    {
        
        public string DataChegadaCliente { get; set; }

        public string DataInicioColeta { get; set; }

        public string DataPrevisaoEntrega { get; set; }

        public Pessoa Cliente { get; set; }

        public Pessoa DonoContainer { get; set; }

        public string NumeroOrdemEmbarque { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public Porto PortoDestino { get; set; }

        public Porto PortoOrigem { get; set; }

        public int Protocolo { get; set; }

        public CentroDescarregamento CentroDescarregamento { get; set; }

        public List<Produto> Produtos { get; set; }
        public decimal KmRestante { get; set; }

    }
}
