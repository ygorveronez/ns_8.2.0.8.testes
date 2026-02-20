namespace Dominio.ObjetosDeValor
{
    public class DocumentoSigaFacil
    {
        public int Codigo { get; set; }

        public int CodigoCTe { get; set; }

        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public int QuantidadeMercadoria { get; set; }

        public string EspecieMercadoria { get; set; }

        public Enumeradores.TipoPeso TipoPeso { get; set; }

        public decimal PesoBruto { get; set; }

        public decimal PesoLotacao { get; set; }

        public decimal ValorMercadoriaKG { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public decimal ValorTarifaFrete { get; set; }

        public decimal ValorFrete { get; set; }

        public /*Enumeradores.RecalculoFrete*/ int RecalculoFrete { get; set; }

        public /*Enumeradores.ExigePesoChegada*/ int ExigePesoChegada { get; set; }

        public /*Enumeradores.TipoQuebra*/ int TipoQuebra { get; set; }

        public /*Enumeradores.TipoTolerancia*/ int TipoTolerancia { get; set; }

        public decimal Tolerancia { get; set; }

        public decimal ToleranciaSuperior { get; set; }

        public decimal ValorAdiantamento { get; set; }

        public decimal ValorSeguro { get; set; }

        public decimal ValorTarifaEmissaoCartao { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorAbastecimento { get; set; }

        public decimal ValorCartaoPedagio { get; set; }

        public decimal ValorIRRF { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorSENAT { get; set; }

        public decimal ValorSEST { get; set; }

        public decimal ValorOutrosDescontos { get; set; }

        public bool Excluir { get; set; }
    }
}
