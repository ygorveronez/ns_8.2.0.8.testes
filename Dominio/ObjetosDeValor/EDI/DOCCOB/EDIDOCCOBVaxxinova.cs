using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class EDIDOCCOBVaxxinova
    {
        public List<EDIDOCCOBVaxxinovaDocumentos> Documentos { get; set; }
    }

    public class EDIDOCCOBVaxxinovaDocumentos
    {
        public int Fatura { get; set; }
        public DateTime DataEmissaoFatura { get; set; }
        public DateTime DataVencimentoFatura { get; set; }
        public decimal ValorFatura { get; set; }
        public int CTe { get; set; }
        public DateTime DataEmissaoCTe { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal BaseICMSFrete { get; set; }
        public decimal ValorICMSFrete { get; set; }
        public int NotaFiscal { get; set; }
        public string SerieNota { get; set; }
        public DateTime DataEmissaoNota { get; set; }
        public string Cliente { get; set; }
        public decimal ValorNota { get; set; }
        public decimal AliquotaNota { get; set; }
        public decimal ValorFreteNota { get; set; }
        public string TipoFrete { get; set; }
        public string TipoModal { get; set; }
        public int Transportador { get; set; }
        public string CFOPNota { get; set; }
    }
}
