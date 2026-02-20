using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioDocumentosEntrada
    {
        public int CodigoDocumentoEntrada { get; set; }

        public int Numero { get; set; }

        public string Serie { get; set; }

        public Dominio.Enumeradores.StatusDocumentoEntrada Status { get; set; }

        public string DescricaoStatus
        {
            get
            {
                return this.Status.ToString("G");
            }
        }

        public DateTime DataEmissao { get; set; }

        public DateTime DataEntrada { get; set; }

        public string NomeFornecedor { get; set; }

        public double CPFCNPJFornecedor { get; set; }

        public string Veiculo { get; set; }

        public decimal ValorTotalDocumentoEntrada { get; set; }

        public decimal ValorTotalProdutos { get; set; }

        public decimal ValorTotalDesconto { get; set; }

        public decimal ValorTotalOutrasDespesas { get; set; }

        public decimal ValorTotalFrete { get; set; }

        public decimal ValorTotalBCICMS { get; set; }

        public decimal ValorTotalICMS { get; set; }

        public decimal ValorTotalBCICMSST { get; set; }

        public decimal ValorTotalICMSST { get; set; }

        public decimal ValorTotalIPI { get; set; }

        public decimal ValorTotalPIS { get; set; }

        public decimal ValorTotalCOFINS { get; set; }

        public string EspercieDocumentoFiscal { get; set; }

        public string ModeloDocumentoFiscal { get; set; }



        public int SequencialItem { get; set; }

        public string ProdutoItem { get; set; }

        public int CFOPItem { get; set; }

        public decimal QuantidadeItem { get; set; }

        public string CSTItem { get; set; }

        public decimal ValorUnitarioItem { get; set; }

        public decimal ValorTotalItem { get; set; }

        public decimal ValorDescontoItem { get; set; }

        public decimal ValorBCICMSItem { get; set; }

        public decimal AliquotaICMSItem { get; set; }

        public decimal ValorICMSItem { get; set; }

        public string CSTPISItem { get; set; }

        public decimal ValorPISItem { get; set; }

        public string CSTCOFINSItem { get; set; }

        public decimal ValorCOFINSItem { get; set; }

        public string CSTIPIItem { get; set; }

        public decimal ValorBCIPIItem { get; set; }

        public decimal AliquotaIPIItem { get; set; }

        public decimal ValorIPIItem { get; set; }

        public decimal ValorBCICMSSTItem { get; set; }

        public decimal ValorICMSSTItem { get; set; }

        public decimal ValorOutrasDespesasItem { get; set; }

        public decimal ValorFreteItem { get; set; }

    }
}
