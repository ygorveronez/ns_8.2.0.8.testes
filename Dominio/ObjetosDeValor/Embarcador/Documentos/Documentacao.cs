namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class Documentacao
    {
        public virtual long Protocolo { get; set; }
        public string DataGeracao { get; set; }
        public bool CargaIMO { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao TipoTrackingDocumentacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao SituacaoTrackingDocumentacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO TipoIMO { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Porto PortoOrigem { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Porto PortoDestino { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Viagem Viagem { get; set; }
        public string CPFOperadorCarga { get; set; }
        public string NomeOperadorCarga { get; set; }
        public string EmailOperadorCarga { get; set; }
        public string EmailOperadorTrakingDocumentacao { get; set; }
    }
}
