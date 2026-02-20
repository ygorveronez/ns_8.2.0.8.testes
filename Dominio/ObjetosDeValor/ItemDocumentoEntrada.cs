namespace Dominio.ObjetosDeValor
{
    public class ItemDocumentoEntrada
    {
        public int Codigo { get; set; }

        public string CodigoProdutoFornecedor { get; set; }

        public int Sequencial { get; set; }

        public int CodigoProduto { get; set; }

        public string DescricaoProduto { get; set; }

        public int CodigoUnidadeMedida { get; set; }

        public string DescricaoUnidadeMedida { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal Desconto { get; set; }

        public decimal ValorTotal { get; set; }

        public string CST { get; set; }

        public string CSTIPI { get; set; }

        public string CSTPIS { get; set; }

        public string CSTCOFINS { get; set; }

        public int CodigoCFOP { get; set; }

        public string DescricaoCFOP { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal AliquotaICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal BaseCalculoIPI { get; set; }

        public decimal AliquotaIPI { get; set; }

        public decimal ValorIPI { get; set; }

        public decimal BaseCalculoICMSST { get; set; }

        public decimal ValorICMSST { get; set; }

        public decimal ValorOutrasDespesas { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorPIS { get; set; }

        public decimal ValorCOFINS { get; set; }

        public bool Excluir { get; set; }
    }
}
