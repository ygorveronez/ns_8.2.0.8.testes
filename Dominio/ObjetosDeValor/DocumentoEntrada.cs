namespace Dominio.ObjetosDeValor
{
    public class DocumentoEntrada
    {
        public string BaseCalculoICMS { get; set; }

        public string BaseCalculoICMSST { get; set; }

        public int Codigo { get; set; }

        public string DataEmissao { get; set; }

        public string DataEntrada { get; set; }

        public string SiglaEspecie { get; set; }

        public string DescricaoEspecie { get; set; }

        public string CPFCNPJFornecedor { get; set; }

        public string NomeFornecedor { get; set; }

        public int CodigoModelo { get; set; }

        public string DescricaoModelo { get; set; }

        public int Numero { get; set; }

        public int NumeroLancamento { get; set; }

        public string Chave { get; set; }

        public int CodigoPlanoConta { get; set; }

        public string DescricaoPlanoConta { get; set; }

        public string Serie { get; set; }

        public Dominio.Enumeradores.StatusDocumentoEntrada Status { get; set; }

        public Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada IndicadorPagamento { get; set; }

        public string ValorProdutos { get; set; }

        public string ValorTotal { get; set; }

        public string ValorTotalCOFINS { get; set; }

        public string ValorTotalDesconto { get; set; }

        public string ValorTotalFrete { get; set; }

        public string ValorTotalICMS { get; set; }

        public string ValorTotalICMSST { get; set; }

        public string ValorTotalIPI { get; set; }

        public string ValorTotalOutrasDespesas { get; set; }

        public string ValorTotalPIS { get; set; }

        public string PlacaVeiculo { get; set; }

        public int CodigoVeiculo { get; set; }

        public object Itens { get; set; }

        public object Cobrancas { get; set; }
    }
}
