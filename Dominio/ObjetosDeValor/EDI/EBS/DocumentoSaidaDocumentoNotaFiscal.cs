using System;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class DocumentoSaidaDocumentoNotaFiscal
    {
        public string NotaCarga { get; set; }
        public string ModeloCarga { get; set; }
        public decimal ValorCarga { get; set; }
        public string SerieCarga { get; set; }
        public DateTime DataEmissaoCarga { get; set; }
        public int NotaCarga2 { get; set; }
        public int Sequencia { get; set; }
    }
}
