using System;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class Lote
    {
        public string NumeroLote { get; set; }
        public decimal QuantidadeLote { get; set; }
        public DateTime DataFabricacao { get; set; }
        public DateTime DataValidade { get; set; }
    }
}
