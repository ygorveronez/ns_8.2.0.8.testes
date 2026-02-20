using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.INTPFAR
{
    public class Fatura //073
    {
        public string TipoRegistro { get; set; }
        public string CNPJTransportadora { get; set; }
        public string CNPJCompanhia { get; set; }
        public string NumeroFatura { get; set; }
        public string NumeroFaturaCodTransportador { get; set; }
        public string SerieFatura { get; set; }
        public DateTime DataFatura { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime? DataEmissaoNCMesContabil { get; set; }
        public DateTime DataLancamento { get; set; }

        public string CNPJTransportadorCTe { get; set; }
        public string CFOP { get; set; }
        public string CodigoTipoFatura { get; set; }
        public string TipoModeloDocFatura { get; set; }
        public decimal ValorFatura { get; set; }
        public string STFatura { get; set; }
        public string CodigoFatura { get; set; }
        public int CodigoImposto { get; set; }
        public decimal BaseCalculoImposto { get; set; }
        public decimal AliquotaImposto { get; set; }
        public decimal ValorImposto { get; set; }
        public string CNPJEmitente { get; set; }
        public int TipoDespacho { get; set; }
        public List<ItemFatura> ItemFatura { get; set; }
    }
}
