using System;

namespace Dominio.ObjetosDeValor.EDI.INTPFAR
{
    public class ItemFatura //075
    {
        public string TipoRegistro { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoEmpresa { get; set; }
        public string GrupoTomador { get; set; }
        public string CodigoTransportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string CodigoContaContabil { get; set; }
        public string CodigoCentroCusto { get; set; }
        public string CodigoItem { get; set; }
        public string DebitoOuCredito { get; set; }
        public string TipoDebitoOuCredito { get; set; }
        public decimal ValorLancamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public int NumeroNF { get; set; }
        public int SerieNF { get; set; }
        public int IdDocumento { get; set; }
        public string TipoLancamento { get; set; }
        public string CodigoImposto { get; set; }
        public string NumeroFatura { get; set; }

        public string TipoFatura { get; set; }
        public string TipoModeloDocFatura { get; set; }
        public string CNPJCPFTomador { get; set; }
        public int TipoMovimentoGrupoEmpresa { get; set; }
    }
}
