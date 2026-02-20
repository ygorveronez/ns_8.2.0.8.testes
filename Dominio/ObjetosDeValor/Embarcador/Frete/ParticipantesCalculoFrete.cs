namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ParticipantesCalculoFrete
    {
        public double Remetente { get; set; }
        public double Destinatario { get; set; }
        public double? Expedidor { get; set; }
        public double? Recebedor { get; set; }
        public double Tomador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? Modalidade { get; set; }
    }
}
