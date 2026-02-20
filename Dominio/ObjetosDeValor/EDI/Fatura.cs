using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI
{
    public class Fatura
    {
        public int Numero { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorDocumentoCobranca { get; set; }
        public string TipoCobranca { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorJurosDiasAtrazo { get; set; }
        public DateTime DataLimitePagamento { get; set; }
        public decimal ValorDesconto { get; set; }
        public List<FaturaCTe> faturaCTes { get; set; }
    }
}
