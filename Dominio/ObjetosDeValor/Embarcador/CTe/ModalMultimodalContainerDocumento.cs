namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalMultimodalContainerDocumento
    {
        public Dominio.Enumeradores.TipoDocumentoCTe TipoDocumento { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string Chave { get; set; }
        public decimal UnidadeMedidaRateada { get; set; }
    }
}
