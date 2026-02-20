
namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class PedidoCTeParaSubContratacao
    {
        public int Codigo { get; set; }
        public CTe.CTeTerceiro CTeTerceiro { get; set; }
        public decimal PercentualAliquota { get; set; }
        public decimal PercentualAliquotaInternaDifal { get; set; }
        public Financeiro.CFOP CFOP { get; set; }
        public string CST { get; set; }
        public string ObservacaoRegraICMSCTe { get; set; }
        public decimal PercentualIncluirBaseCalculo { get; set; }
        public decimal PercentualReducaoBC { get; set; }
        public NotaFiscal.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }
        public bool IncluirICMSBaseCalculo { get; set; }
        public bool PossuiCTe { get; set; }
        public bool PossuiNFS { get; set; }
        public bool PossuiNFSManual { get; set; }
        public decimal BaseCalculoISS { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public decimal PercentualAliquotaISS { get; set; }
        public decimal PercentualRetencaoISS { get; set; }
        public bool IncluirISSBaseCalculo { get; set; }
        public decimal BaseCalculoIR { get; set; }
        public decimal AliquotaIR { get; set; }
        public bool ReterIR { get; set; }
        public decimal ValorIR { get; set; }
        public Dominio.ObjetosDeValor.CTe.IBSCBS IBSCBS { get; set; }
    }
}
