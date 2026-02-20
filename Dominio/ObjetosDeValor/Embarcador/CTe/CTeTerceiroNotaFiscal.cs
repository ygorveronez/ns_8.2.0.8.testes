using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class CTeTerceiroNotaFiscal
    {
        public CTeTerceiro CTeTerceiro { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Peso { get; set; }
        public string CFOP { get; set; }
    }
}
