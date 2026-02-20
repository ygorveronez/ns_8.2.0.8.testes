namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class DocumentoSaidaRemetente
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public int Sequencia { get; set; }
        public Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaDestinatario Destinatario { get; set; }
        public Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaDocumento Documento { get; set; }
    }
}
