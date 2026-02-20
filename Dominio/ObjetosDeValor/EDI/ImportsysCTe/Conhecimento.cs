using System;

namespace Dominio.ObjetosDeValor.EDI.ImportsysCTe
{
    public class Conhecimento
    {
        public string ProcImportacao { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoOcorrencia { get; set; }
        public int NumeroDocumento { get; set; }
        public int Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorAReceber { get; set; }
        public string CnpjTransportador { get; set; }
        public string NumeroCarga { get; set; }
    }
}
