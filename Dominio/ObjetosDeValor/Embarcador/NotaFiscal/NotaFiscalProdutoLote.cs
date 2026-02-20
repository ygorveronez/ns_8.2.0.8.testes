using System;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class NotaFiscalProdutoLote
    {
        public int Codigo { get; set; }
        public string NumeroLote { get; set; }
        public decimal QuantidadeLote { get; set; }
        public DateTime DataFabricacao { get; set; }
        public DateTime DataValidade { get; set; }
        public string CodigoAgregacao { get; set; }
    }
}