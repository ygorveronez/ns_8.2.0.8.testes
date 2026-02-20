using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedidoDadosParaCanhotoIntegracao
    {
        public int CodigoCarga { get; set; }
        public int CodigoPedido { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public DateTime? DataDeEntregaMaisAntiga { get; set; }
    }
}
