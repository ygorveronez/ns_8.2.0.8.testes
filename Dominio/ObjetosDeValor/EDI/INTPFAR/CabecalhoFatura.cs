using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.INTPFAR
{
    public class CabecalhoFatura //070
    {
        public string TipoRegistro { get; set; }
        public int IdDocumento { get; set; }
        public string TipoLancamento { get; set; }
        public string CodigoTransportadora { get; set; }
        public string CNPJTomador { get; set; }
        public string CodigoReferencia { get; set; }
        public string CodigoCompanhia { get; set; }
        public DateTime DataLancamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataEmissao { get; set; }
        public List<Fatura> Fatura { get; set; }
        public List<ItemFatura> ItemFatura { get; set; }
    }
}
