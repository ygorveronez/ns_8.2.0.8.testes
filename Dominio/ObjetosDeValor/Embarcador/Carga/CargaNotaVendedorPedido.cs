using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaNotaVendedorPedido
    {
        public int Codigo { get; set; }
        public int CodigoVendedor { get; set; }
        public string Vendedor { get; set; }
        public string Pedido { get; set; }
        public DateTime? PrevisaoEntrega { get; set; }
        public int CodigoNota { get; set; }
        public string NotaFiscal { get; set; }
        public decimal ValorNF { get; set; }
    }
}
