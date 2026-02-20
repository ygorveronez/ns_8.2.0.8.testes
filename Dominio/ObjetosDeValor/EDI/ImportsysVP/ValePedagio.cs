using System;

namespace Dominio.ObjetosDeValor.EDI.ImportsysVP
{
    public class ValePedagio
    {
        public string ProcImportacao { get; set; }
        public string Numero { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CnpjTransportador { get; set; }
        public string NumeroCarga { get; set; }
    }
}
