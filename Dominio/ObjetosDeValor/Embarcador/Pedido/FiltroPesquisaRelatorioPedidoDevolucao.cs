using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaRelatorioPedidoDevolucao
    {
        public int CodigoPedido { get; set; }
        public int CodigoCarga { get; set; }
        public TipoColetaEntregaDevolucao? TipoDevolucao { get; set; }
        public int NumeroNF { get; set; }
        public DateTime DataEmissaoNFInicial { get; set; }
        public DateTime DataEmissaoNFFinal { get; set; }
        public int CodigoTransportador { get; set; }
        public double CodigoCliente { get; set; }
        public int CodigoMotivo { get; set; }
    }
}
