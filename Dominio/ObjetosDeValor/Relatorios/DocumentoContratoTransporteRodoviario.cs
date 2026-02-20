namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DocumentoContratoTransporteRodoviario
    {
        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public string Mercadoria { get; set; }

        public int NumeroNotaFiscal { get; set; }

        public decimal QuantidadeMercadoria { get; set; }

        public string EspecieMercadoria { get; set; }

        public decimal PesoBruto { get; set; }

        public decimal PesoLotacao { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public decimal ValorMercadoriaKG { get; set; }

        public decimal ValorTarifaFrete { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorAdiantamento { get; set; }

        public decimal ValorSeguro { get; set; }

        public decimal ValorTarifaEmissaoCartao { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorAbastecimento { get; set; }

        public decimal ValorCartaoPedagio { get; set; }

        public decimal ValorIRRF { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorOutrosDescontos { get; set; }

        public decimal ValorSEST { get; set; }

        public decimal ValorSENAT { get; set; }
    }
}
