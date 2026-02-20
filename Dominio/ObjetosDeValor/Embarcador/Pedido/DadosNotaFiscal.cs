namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class DadosNotaFiscal
    {
        public int Codigo { get; set; }
        public string Chave { get; set; }
        public int Numero { get; set; }
        public string Serie { get; set; }
        public string Modelo { get; set; }
        public decimal Valor { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal BaseCalculoST { get; set; }
        public decimal ValorST { get; set; }
        public decimal ValorTotalProdutos { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorImpostoImportacao { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public decimal ValorOutros { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoLiquido { get; set; }
        public int Volumes { get; set; }
        public string DataEmissao { get; set; }
        public string NaturezaOP { get; set; }
        public int codigoCanhotoNF { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal TipoNotaFiscal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento TipoDocumento { get; set; }
        public string Descricao { get; set; }

    }
}
