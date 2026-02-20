namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalAquaviarioContainerDocumento
    {
        public Dominio.Enumeradores.TipoDocumentoCTe TipoDocumento { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string Chave { get; set; }
        public decimal UnidadeMedidaRateada { get; set; }
    }
}
