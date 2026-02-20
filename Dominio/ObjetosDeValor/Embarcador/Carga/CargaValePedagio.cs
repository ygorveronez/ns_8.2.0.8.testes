using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaValePedagio
    {
        public int CodigoCargaValePedagio { get; set; }
        public string NumeroValePedagio { get; set; }
        public decimal ValorValePedagio { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CNPJEmpresa { get; set; }
        public int CodigoCarga { get; set; }
        public string NumeroCarga { get; set; }
    }
}
