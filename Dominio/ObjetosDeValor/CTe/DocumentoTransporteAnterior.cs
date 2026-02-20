namespace Dominio.ObjetosDeValor.CTe
{
    public class DocumentoTransporteAnterior
    {

        public Dominio.Enumeradores.TipoDocumentoAnteriorCTe TipoDocumento { get; set; }

        public Cliente Emissor { get; set; }

        public string Tipo { get; set; }

        public string Serie { get; set; }

        public string Numero { get; set; }

        public string DataEmissao { get; set; }

        public string Chave { get; set; }

    }
}
