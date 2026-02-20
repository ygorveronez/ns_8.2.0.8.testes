namespace Dominio.ObjetosDeValor.EDI.OCOREN
{
    public class CabecalhoDocumento
    {
        public string IdentificacaoDocumento { get; set; }
        public string IdentificacaoDocumento50 { get; set; }
        public string Filler { get; set; }
        public Transportador Transportador { get; set; }
    }
}
