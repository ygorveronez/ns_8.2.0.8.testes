namespace Dominio.ObjetosDeValor.EDI.INTDNE
{
    public class Itens
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NotaFiscal { get; set; }
        public string TipoOperacao { get; set; }
        public string ParceiroComercial { get; set; }
        public string ContaContabil { get; set; }
        public string CentroCusto { get; set; }
        public string DescricaoUnidadeItem { get; set; }
        public string ValorFretePagoCliente { get; set; }
        public string CreditoICMS { get; set; }
        public string CreditoImposto1 { get; set; }
        public string CreditoImposto2 { get; set; }
        public string CreditoImposto3 { get; set; }
        public string CodigoNaturezaOperacao { get; set; }
        public string QuantidadeEmbaladaTransportador { get; set; }
    }
}
