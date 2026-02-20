namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class RetornoConsultaStatusSefaz
    {
        public bool HouveConsula { get; set; }
        public string Mensagem { get; set; }
        public bool DocumentoCancelado { get; set; }
        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa Documento { get; set; }
    }
}
