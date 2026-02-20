using System;

namespace Dominio.ObjetosDeValor.EDI.INTNC
{
    public class NotaFiscal
    {
        public int IdNotaCobranca { get; set; }
        public long SequencialUnico { get; set; }
        public int NumeroNotaCobranca { get; set; }
        public string CNPJTomador { get; set; }
        public string CodNotaFiscal { get; set; }
        public string CodSerieNotaFiscal { get; set; }
        public string TipoPessoaEmitente { get; set; }
        public string TipoPessoaEmitentePF { get; set; }
        public string CNPJEmitente { get; set; }
        public string IEEmitente { get; set; }
        public string UFEmitente { get; set; }
        public string CNPJDestinatario { get; set; }
        public string CodNaturezaOperacao { get; set; }
        public int CFOP { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal ValorTotalNotas { get; set; }
        public DateTime DataEmissaoNotaFiscal { get; set; }
        public string StatusCreditoICMS { get; set; }      

    }
}
