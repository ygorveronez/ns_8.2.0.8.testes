namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFreteDocumentosViagem
    {
        public string tipoDocumentoViagem { get; set; }
        public string chaveAcessoDocumentoViagem { get; set; }
        public ContratoFreteDocumentosViagemDocumentoViagem documentoViagem { get; set; }
        public string filialDocumentoViagem { get; set; }
        public ContratoFreteDocumentosViagemCarga carga { get; set; }
        public ContratoFreteDocumentosViagemNFes[] NFes { get; set; }
        public decimal valorDocumentoViagem { get; set; }
    }
}