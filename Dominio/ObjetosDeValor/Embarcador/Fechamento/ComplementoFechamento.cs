namespace Dominio.ObjetosDeValor.Embarcador.Fechamento
{
    public class ComplementoFechamento
    {
        public Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        public Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManual { get; set; }

        public decimal ValorComplemento { get; set; }

        public decimal ValorPago { get; set; }

        public string Observacao { get; set; }
    }
}
